using BeamCalc.Project;
using System;
using System.Collections.Generic;

namespace BeamCalc.Operation
{
    class Node : AbstractProjectElementOperation<NodeData>
    {
        const string name = "Name";
        const string location = "Location";
        const string fixation = "Fixation";
        const string force = "Force";

        public Node() : base()
        {
            changers = new Dictionary<string, Action<ProjectData, string, List<string>>>()
            {
                { name, Rename },
                { location, Relocate },
                { fixation, ChangeFixation },
                { force, ChangeForce }
            };
        }


        protected override string UserFreindlyElementName => "node";

        protected override Dictionary<string, NodeData> GetElementsDictFromHolder(ProjectData holder)
        {
            return holder.nodes;
        }

        #region Modes
        protected override void Create(ProjectData holder, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "node name")) return;

            string nodeName = args.TakeArg();

            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double location, "node location")) return;


            if (!holder.nodes.ContainsKey(nodeName))
            {
                Program.ToggleChanges();

                holder.nodes.Add(nodeName, new NodeData()
                {
                    location = location
                });

                Console.WriteLine($"Successfully created a new {nodeName} node");
                return;
            }
            else
            {
                Program.AddError($"Node \"{nodeName}\" already exisits. Use a different node name or {change} mode instead.");
                return;
            }
        }

        protected override void Delete(ProjectData holder, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "existing node name")) return;

            string existingNodeName = args.TakeArg();

            if (holder.nodes.ContainsKey(existingNodeName))
            {
                holder.nodes.Remove(existingNodeName);

                Program.ToggleChanges();

                Console.WriteLine($"Successfully deleted node {existingNodeName}.");
            }
            else
            {
                Program.AddError($"Node {existingNodeName} does not exist. Make sure you typed node name correctly and node exists.");
                return;
            }
        }
        #endregion

        #region Changers
        void Rename(ProjectData project, string existingNodeName, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "new node name")) return;

            string newNodeName = args.TakeArg();

            if (project.nodes.ContainsKey(newNodeName))
            {
                Program.AddError($"Node \"{newNodeName}\" already exisits. Use a different new node name or {delete} {newNodeName} first.");
                return;
            }
            else
            {
                project.nodes[newNodeName] = project.nodes[existingNodeName];
                project.nodes.Remove(existingNodeName);

                Program.ToggleChanges();

                Console.WriteLine($"Successfully renamed node to {newNodeName}.");
            }
        }

        void Relocate(ProjectData project, string existingNodeName, List<string> args)
        {
            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double newLocation, "new location")) return;

            project.nodes[existingNodeName].location = newLocation;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new location for {existingNodeName} node.");
        }

        void ChangeFixation(ProjectData project, string existingNodeName, List<string> args)
        {
            if (!TakeMandatoryParsedArgument(args, bool.TryParse, out bool newFixation, "new fixation")) return;

            project.nodes[existingNodeName].xFixed = newFixation;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new fixation for {existingNodeName} node.");
        }

        void ChangeForce(ProjectData project, string existingNodeName, List<string> args)
        {
            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double newForce, "new force")) return;

            project.nodes[existingNodeName].xForce = newForce;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new force for {existingNodeName} node.");
        }
        #endregion

        public override string BasicHelpResponse =>
            $"Creates, changes or deletes a node definition in the opened project file.\n" +
            $"\n" +
            $"Usage:\n" +
            $"Node {create} Name Location: Creates new node definition, with no fixation and zero force.\n" +
            $"\n" +
            $"Node {change} Name {name}|{location}|{fixation}|{force} NewValue: Sets new name, location, fixation or force value.\n" +
            $"\n" +
            $"Node {delete} Name: Deletes node with specified name.";
    }
}
