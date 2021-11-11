using BeamCalc.Project;
using System;
using System.Collections.Generic;

namespace BeamCalc.Operation
{
    class Material : AbstractElementOperation<MaterialDataStorage, MaterialData>
    {
        const string name = "Name";
        const string elasticModulus = "Elastic";
        const string stressLimit = "StessLim";

        public Material() : base()
        {
            changers = new Dictionary<string, Action<MaterialDataStorage, string, List<string>>>()
            {
                { name, Rename },
                { elasticModulus, ChangeElasticModulus },
                { stressLimit, ChangeStressLimit }
            };
        }


        protected override string UserFreindlyElementName => "material";

        protected override bool TryGetElementHolder(out MaterialDataStorage result)
        {
            return Program.TryGetActiveMaterialDataStorage(out result);
        }

        protected override Dictionary<string, MaterialData> GetElementsDictFromHolder(MaterialDataStorage holder)
        {
            return holder.materials;
        }

        #region Modes
        protected override void Create(MaterialDataStorage holder, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "material name")) return;

            string materialName = args.TakeArg();

            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double elasticModulus, "elastic modulus")) return;

            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double stressLimit, "stress limit")) return;


            if (!holder.materials.ContainsKey(materialName))
            {
                Program.ToggleChanges();

                holder.materials.Add(materialName, new MaterialData()
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

        protected override void Delete(MaterialDataStorage holder, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, "existing material name")) return;

            string existingMaterialName = args.TakeArg();

            if (holder.materials.ContainsKey(existingMaterialName))
            {
                holder.materials.Remove(existingMaterialName);

                Program.ToggleChanges();

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

                Program.ToggleChanges();

                Console.WriteLine($"Successfully renamed material to {newMaterialName}.");
            }
        }

        void ChangeElasticModulus(MaterialDataStorage storage, string existingMaterialName, List<string> args)
        {
            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double newElasticModulus, "new elastic modulus")) return;

            storage.materials[existingMaterialName].elasticModulus = newElasticModulus;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new elastic modulus for {existingMaterialName} material.");
        }


        void ChangeStressLimit(MaterialDataStorage storage, string existingMaterialName, List<string> args)
        {
            if (!TakeMandatoryParsedArgument(args, double.TryParse, out double newStressLimit, "new stress limit")) return;

            storage.materials[existingMaterialName].stressLimit = newStressLimit;

            Program.ToggleChanges();

            Console.WriteLine($"Successfully set new stress limit for {existingMaterialName} material.");
        }
        #endregion

        public override string BasicHelpResponse =>
            $"Creates, changes or deletes a material definition in the opened material data storage file.\n" +
            $"\n" +
            $"Usage:\n" +
            $"Material {create} name ElasticModulus StressLimit: Creates new material definition.\n" +
            $"\n" +
            $"Material {change} name {name}|{elasticModulus}|{stressLimit} NewValue: Sets new name, elastic modulus or stress limit value.\n" +
            $"\n" +
            $"Material {delete} name: Deletes material with specified name.";
    }
}
