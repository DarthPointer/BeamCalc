using BeamCalc.Project;
using System;
using System.Collections.Generic;

namespace BeamCalc.Operation
{
    class Beam : AbstractProjectElementOperation<BeamData>
    {
        const string name = "Name";

        const string materialName = "MaterialName";
        const string crossSection = "CrossSection";

        const string node1Name = "StartNode";
        const string node2Name = "EndNode";

        const string load = "Load";


        public Beam() : base()
        {
            changers = new Dictionary<string, Action<ProjectData, string, List<string>>>()
            {
                { name, Rename },
                { materialName, ChangeMaterial },
                { crossSection, ChangeCrossSection },
                { node1Name, ChangeStartNode },
                { node2Name, ChangeEndNode },
                { load, ChangeLoad }
            };
        }


        protected override string UserFreindlyElementName => "beam";

        protected override Dictionary<string, BeamData> GetElementsDictFromHolder(ProjectData holder)
        {
            return holder.beams;
        }

        #region Modes
        protected override void Create(ProjectData holder, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "beam name")) return;
            string beamName = args.TakeArg();

            if (!MandatoryArgumentPresense(args, "material name")) return;
            string materialName = args.TakeArg();

            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double crossSection, "cross section")) return;

            if (!MandatoryArgumentPresense(args, "start node name")) return;
            string startNodeName = args.TakeArg();

            if (!MandatoryArgumentPresense(args, "end node name")) return;
            string endNodeName = args.TakeArg();

            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double load, "load")) return;


            if (!holder.beams.ContainsKey(beamName))
            {
                Program.ToggleChanges();

                holder.beams.Add(beamName, new BeamData()
                {
                    materialName = materialName,
                    crossSection = crossSection,
                    node1Name = startNodeName,
                    node2Name = endNodeName,
                    xLoad = load
                });

                Console.WriteLine($"Successfully created a new {beamName} beam");
                return;
            }
            else
            {
                Program.AddError($"Beam \"{beamName}\" already exisits. Use a different beam name or {change} mode instead.");
                return;
            }
        }

        protected override void Delete(ProjectData holder, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "existing beam name")) return;

            string existingBeamName = args.TakeArg();

            if (holder.beams.ContainsKey(existingBeamName))
            {
                holder.beams.Remove(existingBeamName);

                Program.ToggleChanges();

                Console.WriteLine($"Successfully deleted beam {existingBeamName}.");
            }
            else
            {
                Program.AddError($"Beam {existingBeamName} does not exist. Make sure you typed beam name correctly and beam exists.");
                return;
            }
        }
        #endregion

        #region Changers
        void Rename(ProjectData project, string existingBeamName, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "new beam name")) return;

            string newBeamName = args.TakeArg();

            if (project.beams.ContainsKey(newBeamName))
            {
                Program.AddError($"Beam \"{newBeamName}\" already exisits. Use a different new beam name or {delete} {newBeamName} first.");
                return;
            }
            else
            {
                project.beams[newBeamName] = project.beams[existingBeamName];
                project.beams.Remove(existingBeamName);

                Program.ToggleChanges();

                Console.WriteLine($"Successfully renamed beam to {newBeamName}.");
            }
        }

        void ChangeMaterial(ProjectData project, string existingBeamName, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "new material name")) return;
            string newMaterialName = args.TakeArg();

            project.beams[existingBeamName].materialName = newMaterialName;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new material name for {existingBeamName} beam.");
        }

        void ChangeCrossSection(ProjectData project, string existingBeamName, List<string> args)
        {
            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double newCrossSection, "new cross section")) return;

            project.beams[existingBeamName].crossSection = newCrossSection;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new cross section for {existingBeamName} beam.");
        }

        void ChangeStartNode(ProjectData project, string existingBeamName, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "new start node name")) return;
            string newStartNodeName = args.TakeArg();

            project.beams[existingBeamName].node1Name = newStartNodeName;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new start node name for {existingBeamName} beam.");
        }

        void ChangeEndNode(ProjectData project, string existingBeamName, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "new end node name")) return;
            string newEndNodeName = args.TakeArg();

            project.beams[existingBeamName].node2Name = newEndNodeName;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new end node name for {existingBeamName} beam.");
        }

        void ChangeLoad(ProjectData project, string existingBeamName, List<string> args)
        {
            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double newLoad, "new load")) return;

            project.beams[existingBeamName].xLoad = newLoad;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new load for {existingBeamName} beam.");
        }
        #endregion

        public override string BasicHelpResponse =>
            $"Creates, changes or deletes a beam definition in the opened project file.\n" +
            $"\n" +
            $"Usage:\n" +
            $"Beam {create} Name MaterialName CrossSection StartNode EndNode Load: Creates new beam definition.\n" +
            $"\n" +
            $"Beam {change} Name {name}|{materialName}|{crossSection}|{node1Name}|{node2Name}|{load} NewValue: Sets new name, material name, cross section, start/end node or load value.\n" +
            $"\n" +
            $"Beam {delete} Name: Deletes beam with specified name.";
    }
}
