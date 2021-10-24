using BeamCalc.Project;
using System.Collections.Generic;
using System.Globalization;

namespace BeamCalc.Operation
{
    class PrintNodes : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            if (Program.TryGetActiveProject(out ProjectData project))
            {
                List<string> nodeNames = new List<string>() { "Node Name", "" };
                List<string> locations = new List<string>() { "Location", "" };
                List<string> fixations = new List<string>() { "Fixed", "" };
                List<string> forces = new List<string>() { "Force", "" };

                foreach (var node in project.nodes)
                {
                    nodeNames.Add(node.Key);
                    locations.Add(node.Value.location.ToString("E", CultureInfo.InvariantCulture));
                    fixations.Add(node.Value.xFixed.ToString(CultureInfo.InvariantCulture));
                    forces.Add(node.Value.xForce.ToString("E", CultureInfo.InvariantCulture));
                }

                TableOutput tableOutput = new TableOutput();

                tableOutput.AddColumn(nodeNames, 5);
                tableOutput.AddColumn(locations, 5);
                tableOutput.AddColumn(fixations, 5);
                tableOutput.AddColumn(forces);

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
            $"Prints a table of nodes from currently opened project file\n" +
            $"\n" +
            $"Usage:\n" +
            $"PrintNodes";
    }
}
