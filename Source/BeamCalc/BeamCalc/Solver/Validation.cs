using BeamCalc.Project;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeamCalc.Solver
{
    class Validation
    {
        public static void ValidateProject()
        {
            bool valid = true;

            valid = valid && ValidateProjectState();

            valid = valid && ValidateMaterials();

            valid = valid && ValidateNodes();
            valid = valid && ValidateBeams();

            if (Program.TryGetActiveProject(out ProjectData project))
            {
                project.valid = valid;
            }
        }

        static bool ValidateProjectState()
        {

            bool valid = true;

            if (Program.TryGetActiveProject(out ProjectData project))
            {
                if (project.materialDataStorage == null)
                {
                    valid = false;
                    Program.AddWarning($"Project {project.filePath} has no material data storage linked.");
                }
            }
            else
            {
                Program.AddNote("No project loaded, skipping project state validation.");
            }

            return valid;
        }

        static bool ValidateMaterials()
        {
            bool valid = true;

            if (Program.TryGetActiveMaterialDataStorage(out MaterialDataStorage storage))
            {
                foreach (var material in storage.materials)
                {
                    if (material.Value.elasticModulus <= 0)
                    {
                        valid = false;
                        Program.AddWarning($"Material {material.Key} has non-positive elastic modulus value of {material.Value.elasticModulus}");
                    }
                    if (material.Value.stressLimit <= 0)
                    {
                        valid = false;
                        Program.AddWarning($"Material {material.Key} has non-positive stress limit value of {material.Value.stressLimit}");
                    }
                }
            }
            else
            {
                valid = false;
                Program.AddNote("No material data storage loaded, skipping materials validation.");
            }

            return valid;
        }

        static bool ValidateNodes()
        {
            bool valid = true;

            if (Program.TryGetActiveProject(out ProjectData project))
            {
                bool hasFixedNode = false;

                foreach (var node in project.nodes.Values)
                {
                    if (node.xFixed) hasFixedNode = true;
                }

                if (!hasFixedNode)
                {
                    valid = false;
                    Program.AddNote("There are no fixed nodes declared");
                }
            }
            else
            {
                Program.AddNote("No project loaded, skipping nodes validation.");
            }

            return valid;
        }

        static bool ValidateBeams()
        {
            bool valid = true;

            if (Program.TryGetActiveProject(out ProjectData project))
            {
                List<string> definedMaterials;

                if (project.materialDataStorage == null)
                {
                    valid = false;
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
                    bool canTestIntersection = true;

                    if (!definedMaterials.Contains(beam.Value.materialName))
                    {
                        valid = false;
                        Program.AddWarning($"Beam {beam.Key} refers material {beam.Value.materialName} that is not defined.");
                    }

                    if (beam.Value.crossSection <= 0)
                    {
                        valid = false;
                        Program.AddWarning($"Beam {beam.Key} has a non-positive cross section value of {beam.Value.crossSection}.");
                    }

                    if (!project.nodes.ContainsKey(beam.Value.node1Name))
                    {
                        valid = false;
                        Program.AddWarning($"Beam {beam.Key} refers (start) node {beam.Value.node1Name} that is not defined.");

                        canTestIntersection = false;
                    }

                    if (!project.nodes.ContainsKey(beam.Value.node2Name))
                    {
                        valid = false;
                        Program.AddWarning($"Beam {beam.Key} refers (end) node {beam.Value.node2Name} that is not defined.");
                    
                        canTestIntersection = false;
                    }

                    if (canTestIntersection)
                    {
                        float left = Math.Min(project.nodes[beam.Value.node1Name].location, project.nodes[beam.Value.node2Name].location);
                        float right = Math.Max(project.nodes[beam.Value.node1Name].location, project.nodes[beam.Value.node2Name].location);

                        foreach (var otherBeam in project.beams)
                        {
                            if (project.nodes.ContainsKey(otherBeam.Value.node1Name))
                            {
                                if (project.nodes[otherBeam.Value.node1Name].location > left && project.nodes[otherBeam.Value.node1Name].location < right)
                                {
                                    valid = false;
                                    Program.AddWarning($"Beam {otherBeam.Key} has a start node {otherBeam.Value.node1Name} which is inside another beam ({beam.Key}).");
                                }

                                if (project.nodes[otherBeam.Value.node2Name].location > left && project.nodes[otherBeam.Value.node2Name].location < right)
                                {
                                    valid = false;
                                    Program.AddWarning($"Beam {otherBeam.Key} has an end node {otherBeam.Value.node2Name} which is inside another beam ({beam.Key}).");
                                }
                            }
                        }
                    }

                    ProcessBeamJointing(jointedBeams, beam.Value);
                }

                if (jointedBeams.Count > 1)
                {
                    valid = false;
                    Program.AddWarning($"The beams system is not jointed, it contains isolated parts.");
                }
            }
            else
            {
                Program.AddNote("No project loaded, skipping nodes validation.");
            }

            return valid;
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
    }
}
