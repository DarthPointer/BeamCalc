using BeamCalc.Project;
using System;
using System.Collections.Generic;
using System.IO;

namespace BeamCalc.Operation
{
    class BindMaterialDataStorage : AbstractParametrisedOperation
    {
        private bool ignoreSave;
        private bool ignoreOverwrite;


        public BindMaterialDataStorage()
        {
            paramDelegates = new Dictionary<string, Action>()
            {
                { OperationKeys.ignoreOverwrite, () => ignoreOverwrite = true },
                { OperationKeys.ignoreSave, () => ignoreSave = true }
            };
        }


        public override bool Execute(List<string> args)
        {
            args.TakeArg();
            ignoreSave = false;
            ignoreOverwrite = false;

            if (Program.TryGetActiveProject(out ProjectData project))
            {
                if (!MandatoryArgumentPresense(args, "material data storage to bind")) return true;
                string relativeFilePath = args.TakeArg();
                string filePath = project.folder + relativeFilePath;

                if (!ProcessParams(args)) return true;


                if (Program.runData.unsavedChanges && !ignoreSave)
                {
                    Program.AddError($"Found unsaved changes, abandoned. Use Save command or add {OperationKeys.ignoreSave} parameter at the end.");
                    return true;
                }
                if (project.materialDataStorage != null && !ignoreOverwrite)
                {
                    Program.AddError($"Project already has a material data storage bound. Add {OperationKeys.ignoreOverwrite} parameter at the end if you are sure you want to replace it.");
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
                if (project.materialDataStorage != null)
                {
                    Program.AddNote("Unbinding current project's material data storage.");
                }

                try
                {
                    project.BindMaterialDataStorage(MaterialDataStorage.LoadFromFile(filePath), relativeFilePath);
                }
                catch (Exception e)
                {
                    Program.AddError("An exception arised in the process of loading a material data storage.");
                    Program.AddError(e.Message);

                    return true;
                }

                Program.ToggleChanges();

                Console.WriteLine($"Successfully bound {relativeFilePath} material data storage to current project");

                return true;
            }
            else
            {
                Program.AddError("No project loaded to bind material storage to.");
                return true;
            }
        }

        public override string BasicHelpResponse =>
            $"Binds specified material data storage to currently opened project.\n" +
            $"\n" +
            $"Usage:\n" +
            $"BindMaterialDataStorage filepath [params]\n" +
            $"\n" +
            $"Params:\n" +
            $"{OperationKeys.ignoreOverwrite}: Bind material data storage regardless project already having a storage bound.\n" +
            $"{OperationKeys.ignoreSave}: Disregard unsaved changes change/set bound material data storage file, losing all unsaved changes for previous bound storage.";
    }
}
