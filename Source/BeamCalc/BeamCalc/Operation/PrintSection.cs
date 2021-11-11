using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Project;

namespace BeamCalc.Operation
{
    class PrintSection : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            if (!MandatoryArgumentPresense(args, "beam name")) return true;
            string beamName = args.TakeArg();

            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double sectionLocalCoordinate, "section local coordinate")) return true;

            if (Program.TryGetActiveSolutionResult(out SolutionResultData solutionResult))
            {
                foreach (SolutionResultData.SolutionBeam beam in solutionResult.beams)
                {
                    if (beam.key == beamName)
                    {
                        if (sectionLocalCoordinate >= 0 && sectionLocalCoordinate <= beam.length)
                        {
                            double reaction = beam.inverted ? -beam.reaction[sectionLocalCoordinate] : beam.reaction[sectionLocalCoordinate];
                            double normalLoad = reaction / beam.crossSection;

                            double offset = beam.inverted ? -beam.offset[sectionLocalCoordinate] : beam.offset[sectionLocalCoordinate];

                            TableOutput tableOutput = new TableOutput();

                            tableOutput.AddColumn(new string[]
                            {
                                "Beam Name",
                                "",
                                beam.key
                            }, 5);

                            tableOutput.AddColumn(new string[]
                            {
                                "Local Coordinate",
                                "",
                                StringLib.DisplayedString(sectionLocalCoordinate)
                            }, 5);

                            tableOutput.AddColumn(new string[]
                            {
                                "Material",
                                "",
                                beam.materialName
                            }, 5);

                            tableOutput.AddColumn(new string[]
                            {
                                "Cross Section",
                                "",
                                StringLib.DisplayedString(beam.crossSection)
                            }, 5);

                            tableOutput.AddColumn(new string[]
                            {
                                "Reaction",
                                "",
                                StringLib.DisplayedString(reaction)
                            }, 5);

                            tableOutput.AddColumn(new string[]
                            {
                                "Normal Stress",
                                "",
                                StringLib.DisplayedString(normalLoad)
                            }, 5);

                            tableOutput.AddColumn(new string[]
                            {
                                "Offset",
                                "",
                                StringLib.DisplayedString(offset)
                            });

                            Console.WriteLine();
                            tableOutput.Print();
                            return true;
                        }
                        else
                        {
                            Program.AddError($"Coordinate {sectionLocalCoordinate} is out of range [{0} ; {beam.length}] allowed for beam \"{beam.key}\".");
                            return true;
                        }
                    }
                }

                Program.AddError($"Beam {beamName} does not exist. Make sure you typed beam name correctly and beam exists.");
                return true;
            }
            else
            {
                Program.AddError("No result data opened.");
                return true;
            }
        }

        public override string BasicHelpResponse =>
            $"Prints data about a section of chosen beam. Uses and returns data according to beam's frame of reference.\n" +
            $"\n" +
            $"Usage:\n" +
            $"PrintSection BeamName Coordinate.\n";
    }
}
