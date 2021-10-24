using BeamCalc.Project;
using System.Collections.Generic;
using System.Globalization;


namespace BeamCalc.Operation
{
    class PrintBeams : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            if (Program.TryGetActiveProject(out ProjectData project))
            {
                List<string> beamNames = new List<string>() { "Beam Name", "" };
                List<string> materialNames = new List<string>() { "Material Name", "" };
                List<string> crossSections = new List<string>() { "Cross Section", "" };
                List<string> startNodes = new List<string>() { "Start Node", "" };
                List<string> endNodes = new List<string>() { "End Node", "" };
                List<string> loads = new List<string>() { "Load", "" };

                foreach (var beam in project.beams)
                {
                    beamNames.Add(beam.Key);
                    materialNames.Add(beam.Value.materialName.ToString(CultureInfo.InvariantCulture));
                    crossSections.Add(beam.Value.crossSection.ToString("E", CultureInfo.InvariantCulture));
                    startNodes.Add(beam.Value.node1Name.ToString(CultureInfo.InvariantCulture));
                    endNodes.Add(beam.Value.node2Name.ToString(CultureInfo.InvariantCulture));
                    loads.Add(beam.Value.xLoad.ToString("E", CultureInfo.InvariantCulture));
                }

                TableOutput tableOutput = new TableOutput();

                tableOutput.AddColumn(beamNames, 5);
                tableOutput.AddColumn(materialNames, 5);
                tableOutput.AddColumn(crossSections, 5);
                tableOutput.AddColumn(startNodes, 5);
                tableOutput.AddColumn(endNodes, 5);
                tableOutput.AddColumn(loads);

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
            $"Prints a table of beams from currently opened project file\n" +
            $"\n" +
            $"Usage:\n" +
            $"PrintBeams";
    }
}
