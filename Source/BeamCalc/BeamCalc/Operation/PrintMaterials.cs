using BeamCalc.Project;
using System.Collections.Generic;
using System.Globalization;

namespace BeamCalc.Operation
{
    class PrintMaterials : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            if (Program.TryGetActiveMaterialDataStorage(out MaterialDataStorage storage))
            {
                List<string> materialNames = new List<string>() { "Material Name", "" };
                List<string> elasticModuluses = new List<string>() { "Elastic Modulus", "" };
                List<string> stressLimits = new List<string>() { "Stress Limit", "" };

                foreach (var material in storage.materials)
                {
                    materialNames.Add(material.Key);
                    elasticModuluses.Add(material.Value.elasticModulus.ToString("E", CultureInfo.InvariantCulture));
                    stressLimits.Add(material.Value.stressLimit.ToString("E", CultureInfo.InvariantCulture));
                }

                TableOutput tableOutput = new TableOutput();

                tableOutput.AddColumn(materialNames, 5);
                tableOutput.AddColumn(elasticModuluses, 5);
                tableOutput.AddColumn(stressLimits);

                tableOutput.Print();

                return true;
            }
            else
            {
                Program.AddError("No material data storage opened. Nothing was opened or a project has no storage linked.");
                return true;
            }
        }

        public override string BasicHelpResponse => 
            $"Prints a table of materials from currently opened materials data storage file\n" +
            $"\n" +
            $"Usage:\n" +
            $"PrintMaterials";
    }
}
