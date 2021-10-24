using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Project;

namespace BeamCalc.Operation
{
    class Validate : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            ValidateProjectState();

            ValidateMaterials();

            ValidateNodes();
            ValidateBeams();

            Console.WriteLine("Validation finished!");
            return true;
        }

        static void ValidateProjectState()
        {
            if (Program.TryGetActiveProject(out ProjectData project))
            {
                if (project.materialDataStorage == null)
                {
                    Program.AddWarning($"Project {project.filePath} has no material data storage linked.");
                }
            }
            else
            {
                Program.AddNote("No project loaded, skipping project state validation.");
            }
        }

        static void ValidateMaterials()
        {
            if (Program.TryGetActiveMaterialDataStorage(out MaterialDataStorage storage))
            {
                foreach (var material in storage.materials)
                {
                    if (material.Value.elasticModulus <= 0)
                    {
                        Program.AddWarning($"Material {material.Key} has non-positive elastic modulus value of {material.Value.elasticModulus}");
                    }
                    if (material.Value.stressLimit <= 0)
                    {
                        Program.AddWarning($"Material {material.Key} has non-positive stress limit value of {material.Value.stressLimit}");
                    }
                }
            }
            else
            {
                Program.AddNote("No material data storage loaded, skipping materials validation.");
            }
        }

        static void ValidateNodes()
        {
            if (Program.TryGetActiveProject(out ProjectData project))
            {
                bool hasFixedNode = false;

                foreach (var node in project.nodes.Values)
                {
                    if (node.xFixed) hasFixedNode = true;
                }

                if (!hasFixedNode)
                {
                    Program.AddNote("There are no fixed nodes declared");
                }
            }
            else
            {
                Program.AddNote("No project loaded, skipping nodes validation.");
            }
        }

        static void ValidateBeams()
        {
            if (Program.TryGetActiveProject(out ProjectData project))
            {
                List<string> definedMaterials;

                if (project.materialDataStorage == null)
                {
                    Program.AddNote("No material data storage found, beams materials can't be validated.");
                    definedMaterials = new List<string>();
                }
                else
                {
                    definedMaterials = project.materialDataStorage.materials.Keys.ToList();
                }

                List<List<BeamData>> jointedBeams = new List<List<BeamData>>();

                foreach (var beam in project.beams)
                {
                    if (!definedMaterials.Contains(beam.Value.materialName))
                    {
                        Program.AddWarning($"Beam {beam.Key} refers material {beam.Value.materialName} that is not defined.");
                    }

                    if (beam.Value.crossSection <= 0)
                    {
                        Program.AddWarning($"Beam {beam.Key} has a non-positive cross section value of {beam.Value.crossSection}.");
                    }

                    if (!project.nodes.ContainsKey(beam.Value.node1Name))
                    {
                        Program.AddWarning($"Beam {beam.Key} refers (start) node {beam.Value.node1Name} that is not defined.");
                    }

                    if (!project.nodes.ContainsKey(beam.Value.node2Name))
                    {
                        Program.AddWarning($"Beam {beam.Key} refers (end) node {beam.Value.node2Name} that is not defined.");
                    }

                    ProcessBeamJointing(jointedBeams, beam.Value);
                }

                if (jointedBeams.Count > 1)
                {
                    Program.AddWarning($"The beams system is not jointed, it contains isolated parts.");
                }
            }
            else
            {
                Program.AddNote("No project loaded, skipping nodes validation.");
            }
        }

        static void ProcessBeamJointing(List<List<BeamData>> jointedBeams, BeamData beamToProcess)
        {
            List<List<BeamData>> jointedBeamJointments = new List<List<BeamData>>();

            foreach (List<BeamData> beamJointment in jointedBeams)
            {
                IEnumerable<string> jointmentNodes = beamJointment.Select(x => x.node1Name).Concat(beamJointment.Select(x => x.node2Name)).Distinct();

                if (jointmentNodes.Contains(beamToProcess.node1Name) || jointmentNodes.Contains(beamToProcess.node2Name))
                {
                    jointedBeamJointments.Add(beamJointment);
                }
            }

            if (jointedBeamJointments.Count > 0)
            {
                jointedBeamJointments[0].Add(beamToProcess);

                if (jointedBeamJointments.Count == 2)
                {
                    List<BeamData> newBeamJointment = jointedBeamJointments[0].Concat(jointedBeamJointments[1]).ToList();

                    jointedBeams.Remove(jointedBeamJointments[0]);
                    jointedBeams.Remove(jointedBeamJointments[1]);

                    jointedBeams.Add(newBeamJointment);
                }
            }
        }

        public override string BasicHelpResponse =>
            $"Validates currenlty opened files and reports results into stdout\n" +
            $"\n" +
            $"Usage:\n" +
            $"Validate";
    }
}
