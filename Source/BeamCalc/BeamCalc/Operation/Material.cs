using System;
using System.Collections.Generic;

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

        public Material()
        {
            modes = new Dictionary<string, Action<MaterialDataStorage, List<string>>>()
            {
                { create, Create },
                { change, Change },
                { delete, Delete }
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

                Console.WriteLine($"Successfully added a new {materialName} material");
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


            }
            else
            {
                Program.AddError($"Material {existingMaterialName} does not exist. Make sure you typed material name correctly and material exists.");
                return;
            }
        }

        void Delete(MaterialDataStorage storage, List<string> args)
        {
        }
        #endregion
    }
}
