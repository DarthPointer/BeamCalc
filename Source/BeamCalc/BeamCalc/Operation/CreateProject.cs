using System;
using System.Collections.Generic;
using System.IO;

namespace BeamCalc.Operation
{
    class CreateProject : AbstractParametrisedOperation
    {
        private bool ignoreSave;
        private bool ignoreOverwrite;


        public CreateProject()
        {
            paramDelegates = new Dictionary<string, Action>()
            {
                { OperationKeys.ignoreSave, () => ignoreSave = true },
                { OperationKeys.ignoreOverwrite, () => ignoreOverwrite = true }
            };
        }


        public override bool Execute(List<string> args)
        {
            args.TakeArg();
            ignoreSave = false;
            ignoreOverwrite = false;

            if (!MandatoryArgumentPresense(args, "new project path")) return true;

            string filePath = args.TakeArg();

            if (!ProcessParams(args)) return true;


            if (Program.runData.unsavedChanges && !ignoreSave)
            {
                Program.AddError($"Found unsaved changes, abandoned. Use Save command or add {OperationKeys.ignoreSave} parameter at the end.");
                return true;
            }
            if (File.Exists(filePath) && !ignoreOverwrite)
            {
                Program.AddError($"Specified filepath already exists, abandoned. Use a different file path or add {OperationKeys.ignoreOverwrite} at the end.");
                return true;
            }

            if (Program.runData.unsavedChanges)
            {
                Program.AddNote("Disregarded unsaved changes and switched to new file(s)");
            }
            if (File.Exists(filePath))
            {
                Program.AddNote($"Existing file {filePath} will be overwritten next time you save changes.");
            }

            Program.ToggleChanges();

            Program.OpenedProject = new Project.ProjectData(filePath);

            Console.WriteLine($"Successfully created project file {filePath}.");

            return true;
        }

        public override string BasicHelpResponse =>
            $"Creates new project file with specified file path and swithces to editing it.\n" +
            $"\n" +
            $"Usage:\n" +
            $"CreateProject filepath [params].\n" +
            $"\n" +
            $"Parameters:\n" +
            $"{OperationKeys.ignoreSave}: Disregard unsaved changes and unload current files without saving.\n" +
            $"{OperationKeys.ignoreOverwrite}: Create new file even if the specified file path is already used.";
    }
}
