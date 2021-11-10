using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Project;

namespace BeamCalc.Operation
{
    class PrintSections : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            if (!MandatoryArgumentPresense(args, "beam name")) return true;
            string beamName = args.TakeArg();

            if (!TakeMandatoryParsedArgument(args, float.TryParse, out float startAtLocalCoordinate, "start at local coordinate")) return true;
            if (!TakeMandatoryParsedArgument(args, float.TryParse, out float endAtLocalCoordinate, "end at local coordinate")) return true;
            if (!TakeMandatoryParsedArgument(args, int.TryParse, out int sectionCount, "sections count")) return true;

            if (Program.TryGetActiveSolutionResult(out SolutionResultData solutionResult))
            {
                foreach (SolutionResultData.SolutionBeam beam in solutionResult.beams)
                {
                    if (beam.key == beamName)
                    {
                        if (startAtLocalCoordinate < 0 || startAtLocalCoordinate >= beam.length)
                        {
                            Program.AddError($"Start coordinate {startAtLocalCoordinate} is out of range [{0} ; {beam.length}] allowed for beam \"{beam.key}\".");
                            return true;
                        }

                        if (endAtLocalCoordinate < 0 || endAtLocalCoordinate >= beam.length)
                        {
                            Program.AddError($"End coordinate {endAtLocalCoordinate} is out of range [{0} ; {beam.length}] allowed for beam \"{beam.key}\".");
                            return true;
                        }

                        if (sectionCount < 2)
                        {
                            Program.AddError($"Step count {sectionCount} is too low (has to be >= 2). For single section use PrintSection instead.");
                            return true;
                        }

                        float step = (endAtLocalCoordinate - startAtLocalCoordinate) / (sectionCount - 1);

                        List<string> coordinates = new List<string>() { "Local Cooridnate", "" };
                        List<string> reactions = new List<string>() { "Reaction", "" };
                        List<string> normalStresses = new List<string>() { "Normal Stress", "" };
                        List<string> offsets = new List<string>() { "Offset", "" };

                        for (int i = 0; i < sectionCount; i++)
                        {
                            float currentCoordinate = startAtLocalCoordinate + i * step;

                            coordinates.Add(StringLib.DisplayedString(currentCoordinate));
                            reactions.Add(StringLib.DisplayedString(beam.reaction[currentCoordinate]));
                            normalStresses.Add(StringLib.DisplayedString(beam.reaction[currentCoordinate] / beam.crossSection));
                            offsets.Add(StringLib.DisplayedString(beam.offset[currentCoordinate]));
                        }

                        TableOutput tableOutput = new TableOutput();
                        tableOutput.AddColumn(coordinates, 5);
                        tableOutput.AddColumn(reactions, 5);
                        tableOutput.AddColumn(normalStresses, 5);
                        tableOutput.AddColumn(offsets);

                        Console.WriteLine();
                        tableOutput.Print();
                        return true;
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
            $"Prints data about section of chosen beam. Uses and returns data according to beam's frame of reference.\n" +
            $"\n" +
            $"Usage:\n" +
            $"PrintSections BeamName StartCoordinate EndCoordinate SectionsCount.\n" +
            $"\n" +
            $"SectionsCount must be >= 2.";
    }
}
