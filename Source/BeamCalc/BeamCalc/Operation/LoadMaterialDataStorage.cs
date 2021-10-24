using BeamCalc.Project;
using System;
using System.Collections.Generic;
using System.IO;

namespace BeamCalc.Operation
{
    class LoadMaterialDataStorage : AbstractParametrisedOperation
    {
        private bool ignoreSave;

        public LoadMaterialDataStorage()
        {
            paramDelegates = new Dictionary<string, Action>()
            {
                { OperationKeys.ignoreSave, () => ignoreSave = true }
            };
        }

        public override bool Execute(List<string> args)
        {
            args.TakeArg();
            ignoreSave = false;

            if (!MandatoryArgumentPresense(args, "material data storage path to load")) return true;

            string filePath = args.TakeArg();

            if (!ProcessParams(args)) return true;


            if (Program.runData.unsavedChanges && !ignoreSave)
            {
                Program.AddError($"Found unsaved changes, abandoned. Use Save command or add {OperationKeys.ignoreSave} parameter at the end.");
                return true;
            }
            if (!File.Exists(filePath))
            {
                Program.AddError($"File {filePath} does not exist.");
                return true;
            }

            if (Program.runData.unsavedChanges)
            {
                Program.AddNote("Disregarded unsaved changes and switched to new file(s).");
            }

            try
            {
                Program.runData.materialDataStorage = MaterialDataStorage.LoadFromFile(filePath);
            }
            catch (Exception e)
            {
                Program.AddError("An exception arised in the process of loading a material data storage.");
                Program.AddError(e.Message);

                return true;
            }

            Program.runData.unsavedChanges = false;

            Program.runData.project = null;

            Console.WriteLine($"Successfully loaded material data storage file {filePath}.");

            return true;
        }

        public override string BasicHelpResponse => 
            $"Loads material data storage from specified file.\n" +
            $"\n" +
            $"Usage:\n" +
            $"LoadMaterialDataStorage filepath [params]\n" +
            $"\n" +
            $"Params:\n" +
            $"{OperationKeys.ignoreSave}: Disregard unsaved changes and unload current files without saving.";
    }
}
