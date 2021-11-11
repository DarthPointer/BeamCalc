using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Project;

namespace BeamCalc.Operation
{
    class PrintSystem : AbstractOperation
    {
        const char nodeFree = 'o';
        const char nodeFixed = 'X';
        const char beamChar = '=';
        const string forceLine = "--";
        const string leftForce = "<";
        const string rightForce = ">";
        const char leftLoadChar = '<';
        const char rightLoadChar = '>';
        const char spaceChar = ' ';
        const char verticalChar = '|';

        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double leftPos, "left poistion")) return true;
            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double rightPos, "right position")) return true;
            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double metersPerChar, "meters per char")) return true;

            if (metersPerChar <= 0)
            {
                Program.AddError($"Meters per char has a non-positve value of {metersPerChar}");
                return true;
            }
            else
            {
                if (!Program.TryGetActiveProject(out ProjectData project))
                {
                    Program.AddError("No project opened.");
                    return true;
                }
                else
                {
                    List<BeamColumn> beamColumns = new List<BeamColumn>();
                    BeamColumn leftMost = null;

                    foreach (BeamData beam in project.beams.Values)
                    {
                        if (BeamLocation1(beam, project) > leftPos || BeamLocation1(beam, project) < rightPos ||
                            BeamLocation2(beam, project) > leftPos || BeamLocation2(beam, project) < rightPos)
                        {
                            BeamColumn beamColumn = new BeamColumn(beam, project);

                            BeamColumn leftBeam = beamColumns.Find(x => x.rightX == beamColumn.leftX);
                            if (leftBeam != null)
                            {
                                leftBeam.rightBeam = beamColumn;
                                beamColumn.leftBeam = leftBeam;
                            }

                            BeamColumn rightBeam = beamColumns.Find(x => x.leftX == beamColumn.rightX);
                            if (rightBeam != null)
                            {
                                rightBeam.leftBeam = beamColumn;
                                beamColumn.rightBeam = rightBeam;
                            }

                            beamColumns.Add(beamColumn);

                            if (leftMost == null)
                            {
                                leftMost = beamColumn;
                            }
                            else if (beamColumn.leftX < leftMost.leftX)
                            {
                                leftMost = beamColumn;
                            }
                        }
                    }

                    TableOutput tableOutput = new TableOutput();
                    ProcessLeftBorder(leftMost, leftPos, project.nodes[leftMost.leftName].xFixed, tableOutput);

                    for (BeamColumn current = leftMost; current != null; current = current.rightBeam)
                    {
                        foreach (List<string> subcolumn in current.ToStrings(project, leftPos, rightPos, metersPerChar))
                        {
                            tableOutput.AddColumn(subcolumn);
                        }
                    }

                    Console.WriteLine();
                    tableOutput.Print();

                    return true;
                }
            }
        }

        public override string BasicHelpResponse => 
            "Prints part of the system between specified borders with specified scale\n" +
            "\n" +
            "Usage:\n" +
            "PrintSystem LeftBorder RightBorder MetersPerChar";


        static double BeamLocation1(BeamData beam, ProjectData project)
        {
            return project.nodes[beam.node1Name].location;
        }

        static double BeamLocation2(BeamData beam, ProjectData project)
        {
            return project.nodes[beam.node2Name].location;
        }

        static void ProcessLeftBorder(BeamColumn leftMost, double leftPos, bool leftNodeFixed, TableOutput tableOutput)
        {
            if (leftMost.leftX >= leftPos)
            {
                List<string> leftNodeColumn = new List<string>();

                leftNodeColumn.Add(new string(verticalChar, 1));

                char nodeChar = leftNodeFixed ? nodeFixed : nodeFree;
                leftNodeColumn.Add(new string(nodeChar, 1));

                for (int i = 2; i < 12; i++)
                {
                    leftNodeColumn.Add(new string(verticalChar, 1));
                }

                tableOutput.AddColumn(leftNodeColumn);
            }
        }

        class BeamColumn
        {
            public BeamData beam;

            public BeamColumn leftBeam;
            public BeamColumn rightBeam;

            public double leftX;
            public string leftName;
            public double rightX;
            public string rightName;

            public BeamColumn(BeamData beam, ProjectData project)
            {
                this.beam = beam;

                if (BeamLocation1(beam, project) < BeamLocation2(beam, project))
                {
                    leftX = BeamLocation1(beam, project);
                    leftName = beam.node1Name;

                    rightX = BeamLocation2(beam, project);
                    rightName = beam.node2Name;
                }
                else
                {
                    rightX = BeamLocation1(beam, project);
                    rightName = beam.node1Name;

                    leftX = BeamLocation2(beam, project);
                    leftName = beam.node2Name;
                }
            }

            public List<List<string>> ToStrings(ProjectData project, double leftPos, double rightPos, double metersPerChar)
            {
                double left = Math.Max(leftX, leftPos);
                double right = Math.Min(rightX, rightPos);

                int toScaleCharLength = ((int)((right - left) / metersPerChar));

                int segmentWidth = 0;

                string crossSectionDisplay = StringLib.DisplayedString(beam.crossSection);
                segmentWidth = Math.Max(segmentWidth, crossSectionDisplay.Length);

                string loadDisplay = "";
                char loadLineChar = spaceChar;

                if (beam.xLoad != 0)
                {
                    loadDisplay = StringLib.DisplayedString(Math.Abs(beam.xLoad));

                    if (project.nodes[beam.node1Name].location < project.nodes[beam.node2Name].location ^ beam.xLoad < 0)
                    {
                        loadLineChar = rightLoadChar;
                    }
                    else
                    {
                        loadLineChar = leftLoadChar;
                    }
                }
                segmentWidth = Math.Max(segmentWidth, loadDisplay.Length);

                string leftNodeForceDisplay = "";
                string leftNodeForceArrow = "";

                if (project.nodes[leftName].xForce > 0 && leftX >= leftPos)
                {
                    leftNodeForceDisplay = StringLib.DisplayedString(project.nodes[leftName].xForce);
                    leftNodeForceArrow = forceLine + rightForce;
                }
                segmentWidth = Math.Max(segmentWidth, leftNodeForceDisplay.Length);
                segmentWidth = Math.Max(segmentWidth, leftNodeForceArrow.Length);

                string rightNodeForceDisplay = "";
                string rightNodeForceArrow = "";

                if (project.nodes[rightName].xForce < 0 && rightX <= rightPos)
                {
                    rightNodeForceDisplay = StringLib.DisplayedString(Math.Abs(project.nodes[rightName].xForce));
                    rightNodeForceArrow = leftForce + forceLine;
                }
                segmentWidth = Math.Max(segmentWidth, rightNodeForceDisplay.Length);
                segmentWidth = Math.Max(segmentWidth, rightNodeForceArrow.Length);

                string rightNodePosDisplay = "";

                if (rightX <= rightPos)
                {
                    rightNodePosDisplay = StringLib.DisplayedString(rightX);
                }
                segmentWidth = Math.Max(segmentWidth, rightNodePosDisplay.Length);

                segmentWidth = Math.Max(segmentWidth, toScaleCharLength);

                List<List<string>> result = new List<List<string>>();
                List<string> result0 = new List<string>();

                result0.Add(crossSectionDisplay.PadRight(segmentWidth));
                result0.Add(new string(beamChar, segmentWidth));

                result0.Add(new string(loadLineChar, segmentWidth));
                result0.Add(loadDisplay.PadRight(segmentWidth));
                result0.Add(new string(spaceChar, segmentWidth));

                result0.Add(rightNodeForceArrow.PadLeft(segmentWidth));
                result0.Add(rightNodeForceDisplay.PadLeft(segmentWidth));
                result0.Add(new string(spaceChar, segmentWidth));

                result0.Add(leftNodeForceArrow.PadRight(segmentWidth));
                result0.Add(leftNodeForceDisplay.PadRight(segmentWidth));
                result0.Add(new string(spaceChar, segmentWidth));

                result0.Add(rightNodePosDisplay.PadLeft(segmentWidth));

                result.Add(result0);

                if (rightX <= rightPos)
                {
                    List<string> result1 = new List<string>();

                    result1.Add(new string(verticalChar, 1));

                    char nodeChar = project.nodes[rightName].xFixed ? nodeFixed : nodeFree;
                    result1.Add(new string(nodeChar, 1));

                    for (int i = 2; i < result0.Count; i++)
                    {
                        result1.Add(new string(verticalChar, 1));
                    }

                    result.Add(result1);
                }

                return result;
            }
        }
    }
}
