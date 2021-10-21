using System;
using System.Collections.Generic;
using System.Linq;

using BeamCalc.Project;

namespace BeamCalc.Operation
{
    class Material : AbstractOperation
    {
        const string create = "Create";
        const string change = "Change";
        const string delete = "Delete";

        const string name = "Name";
        const string elasticModulus = "Elastic";
        const string stressLimit = "StessLim";

        Dictionary<string, Action<MaterialDataStorage, List<string>>> modes;
        Dictionary<string, Action<MaterialDataStorage, string, List<string>>> materialChangers;

        public Material()
        {
            modes = new Dictionary<string, Action<MaterialDataStorage, List<string>>>()
            {
                { create, Create },
                { change, Change },
                { delete, Delete }
            };

            materialChangers = new Dictionary<string, Action<MaterialDataStorage, string, List<string>>>()
            {
                { name, Rename },
                { elasticModulus, ChangeElasticModulus },
                { stressLimit, ChangeStressLimit }
            };
        }

        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            if (Program.TryGetActiveMaterialDataStorage(out MaterialDataStorage storage))
            {
                if (!MandatoryArgumentPresense(args, $"mode ({create}|{change}|{delete})")) return true;
                string mode = args.TakeArg();

                if (modes.ContainsKey(mode))
                {
                    modes[mode](storage ,args);
                }
                else
                {
                    Program.AddError($"Unknown mode \"{mode}\".");
                    return true;
                }
            }
            else
            {
                Program.AddError("No material data storage opened. Nothing was opened or a project has no storage linked.");
                return true;
            }

            return true;
        }

        public override string BasicHelpResponse => 
            $"Creates, changes or deletes a material definition in the opened file.\n" +
            $"\n" +
            $"Usage:\n" +
            $"Material {create} name {'{'}elastic modulus{'}'} {'{'}stress limit{'}'}: Creates new material definition.\n" +
            $"\n" +
            $"Material {change} name {name}|{elasticModulus}|{stressLimit} {'{'}new value{'}'}: Sets new name, elastic modulus or stress limit value.\n" +
            $"\n" +
            $"Material {delete} name: Deletes material with specified name.";

        #region Modes
        void Create(MaterialDataStorage storage, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "material name")) return;

            string materialName = args.TakeArg();

            if (!TakeMandatoryFloatFromArgs(args, out float elasticModulus, "elastic modulus")) return;

            if (!TakeMandatoryFloatFromArgs(args, out float stressLimit, "stress limit")) return;


            if (!storage.materials.ContainsKey(materialName))
            {
                Program.runData.unsavedChanges = true;

                storage.materials.Add(materialName, new MaterialData()
                {
                    elasticModulus = elasticModulus,
                    stressLimit = stressLimit
                });

                Console.WriteLine($"Successfully created a new {materialName} material");
                return;
            }
            else
            {
                Program.AddError($"Material \"{materialName}\" already exisits. Use a different material name or {change} mode instead.");
                return;
            }
        }

        void Change(MaterialDataStorage storage, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "existing material name")) return;

            string existingMaterialName = args.TakeArg();

            if (storage.materials.ContainsKey(existingMaterialName))
            {
                if (!MandatoryArgumentPresense(args, "parameter to change")) return;
                string parameterToChange = args.TakeArg();

                if (materialChangers.ContainsKey(parameterToChange))
                {
                    materialChangers[parameterToChange](storage, existingMaterialName, args);
                    return;
                }
                else
                {
                    Program.AddError($"Unknown parameter to change specified: {parameterToChange}. Need {name}|{elasticModulus}|{stressLimit}.");
                    return;
                }
            }
            else
            {
                Program.AddError($"Material {existingMaterialName} does not exist. Make sure you typed material name correctly and material exists.");
                return;
            }
        }

        void Delete(MaterialDataStorage storage, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "existing material name")) return;

            string existingMaterialName = args.TakeArg();

            if (storage.materials.ContainsKey(existingMaterialName))
            {
                storage.materials.Remove(existingMaterialName);

                Program.runData.unsavedChanges = true;

                Console.WriteLine($"Successfully deleted material {existingMaterialName}.");
            }
            else
            {
                Program.AddError($"Material {existingMaterialName} does not exist. Make sure you typed material name correctly and material exists.");
                return;
            }
        }
        #endregion

        #region Material Changers
        void Rename(MaterialDataStorage storage, string existingMaterialName, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "new material name")) return;

            string newMaterialName = args.TakeArg();

            if (storage.materials.ContainsKey(newMaterialName))
            {
                Program.AddError($"Material \"{newMaterialName}\" already exisits. Use a different new material name or {delete} {newMaterialName} first.");
                return;
            }
            else
            {
                storage.materials[newMaterialName] = storage.materials[existingMaterialName];
                storage.materials.Remove(existingMaterialName);

                Program.runData.unsavedChanges = true;

                Console.WriteLine($"Successfully renamed material to {newMaterialName}.");
            }
        }

        void ChangeElasticModulus(MaterialDataStorage storage, string existingMaterialName, List<string> args)
        {
            if (!TakeMandatoryFloatFromArgs(args, out float newElasticModulus, "new elastic modulus")) return;

            storage.materials[existingMaterialName].elasticModulus = newElasticModulus;

            Program.runData.unsavedChanges = true;

            Console.WriteLine($"Successfully set new elastic modulus for {existingMaterialName} material.");
        }


        void ChangeStressLimit(MaterialDataStorage storage, string existingMaterialName, List<string> args)
        {
            if (!TakeMandatoryFloatFromArgs(args, out float newStressLimit, "new stress limit")) return;

            storage.materials[existingMaterialName].stressLimit = newStressLimit;

            Program.runData.unsavedChanges = true;

            Console.WriteLine($"Successfully set new stress limit for {existingMaterialName} material.");
        }
        #endregion
    }
}
