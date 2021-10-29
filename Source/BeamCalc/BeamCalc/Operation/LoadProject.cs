using BeamCalc.Project;
using System;
using System.Collections.Generic;
using System.IO;


namespace BeamCalc.Operation
{
    class LoadProject : AbstractParametrisedOperation
    {
        private bool ignoreSave;

        public LoadProject()
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

            if (!MandatoryArgumentPresense(args, "project path to load")) return true;

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
                Program.OpenedProject = ProjectData.LoadFromFile(filePath);
            }
            catch (Exception e)
            {
                Program.AddError("An exception arised in the process of loading a project.");
                Program.AddError(e.Message);

                return true;
            }

            Program.runData.unsavedChanges = false;

            Console.WriteLine($"Successfully loaded project file {filePath}.");

            return true;
        }

        public override string BasicHelpResponse =>
            $"Loads project from specified file.\n" +
            $"\n" +
            $"Usage:\n" +
            $"LoadProject filepath [params]\n" +
            $"\n" +
            $"Params:\n" +
            $"{OperationKeys.ignoreSave}: Disregard unsaved changes and unload current files without saving.";
    }
}
