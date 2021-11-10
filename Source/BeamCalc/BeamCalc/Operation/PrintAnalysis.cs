using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Project;

namespace BeamCalc.Operation
{
    class PrintAnalysis : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            if (Program.TryGetActiveSolutionResult(out SolutionResultData solutionResult))
            {
                List<string> beamNames = new List<string>() { "Beam Name", "" };
                List<string> materialNames = new List<string>() { "Material Name", "" };
                List<string> stressLimits = new List<string>() { "Stress Limit", "" };
                List<string> maxStresses = new List<string>() { "Max Normal Stress", "" };
                List<string> minStresses = new List<string>() { "Min Normal Stress", "" };
                List<string> overStresses = new List<string>() { "Is Overstressed", "" };

                foreach (SolutionResultData.SolutionBeam beam in solutionResult.beams)
                {
                    MaterialData beamMaterial = solutionResult.materials[beam.materialName];
                    float maxStress = beam.reaction.Max(0, beam.length) / beam.crossSection;
                    float minStress = beam.reaction.Min(0, beam.length) / beam.crossSection;
                    bool isOverStressed = Math.Abs(maxStress) > beamMaterial.stressLimit || Math.Abs(minStress) > beamMaterial.stressLimit;

                    beamNames.Add(beam.key);
                    materialNames.Add(beam.materialName);
                    stressLimits.Add(StringLib.DisplayedString(beamMaterial.stressLimit));
                    maxStresses.Add(StringLib.DisplayedString(maxStress));
                    minStresses.Add(StringLib.DisplayedString(minStress));
                    overStresses.Add(isOverStressed.ToString());

                    if (maxStress > beamMaterial.stressLimit)
                    {
                        Program.AddWarning($"Beam {beam.key} is overstressed. Max stress is {StringLib.DisplayedString(maxStress)} and exceeds stress limit {StringLib.DisplayedString(beamMaterial.stressLimit)} of {beam.materialName}");
                    }

                    if (-minStress > beamMaterial.stressLimit)
                    {
                        Program.AddWarning($"Beam {beam.key} is overstressed. Min stress is {StringLib.DisplayedString(minStress)} and exceeds stress limit {StringLib.DisplayedString(beamMaterial.stressLimit)} of {beam.materialName}");
                    }
                }

                TableOutput tableOutput = new TableOutput();
                tableOutput.AddColumn(beamNames, 5);
                tableOutput.AddColumn(materialNames, 5);
                tableOutput.AddColumn(stressLimits, 5);
                tableOutput.AddColumn(maxStresses, 5);
                tableOutput.AddColumn(minStresses, 5);
                tableOutput.AddColumn(overStresses);

                Console.WriteLine();
                tableOutput.Print();
                Console.WriteLine();

                return true;
            }
            else
            {
                Program.AddError("No result data opened.");
                return true;
            }
        }

        public override string BasicHelpResponse => throw new NotImplementedException();
    }
}
