using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BeamCalc.Project;

namespace BeamCalc.Operation
{
    class LoadSolution : AbstractParametrisedOperation
    {
        bool ignoreSave;

        public LoadSolution()
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

            if (!MandatoryArgumentPresense(args, "solution result path to load")) return true;

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
                Program.OpenedSolutionResult = SolutionResultData.LoadFromFile(filePath);
            }
            catch (Exception e)
            {
                Program.AddError("An exception arised in the process of loading solution results.");
                Program.AddError(e.Message);

                return true;
            }

            Program.runData.unsavedChanges = false;

            Console.WriteLine($"Successfully loaded solution results file {filePath}.");

            return true;
        }

        public override string BasicHelpResponse =>
            $"Loads soluiton results from specified file.\n" +
            $"\n" +
            $"Usage:\n" +
            $"LoadSolution filepath [params]\n" +
            $"\n" +
            $"Params:\n" +
            $"{OperationKeys.ignoreSave}: Disregard unsaved changes and unload current files without saving.";
    }
}
