using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Project;
using BeamCalc.Solver;
using System.IO;

namespace BeamCalc.Operation
{
    class GenerateSolution : AbstractParametrisedOperation
    {
        bool ignoreOverwirte;

        public GenerateSolution()
        {
            paramDelegates = new Dictionary<string, Action>()
            {
                { OperationKeys.ignoreOverwrite, () => ignoreOverwirte = true }
            };
        }

        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            if (!MandatoryArgumentPresense(args, "solution result file")) return true;
            string solutionResultFilePath = args.TakeArg();

            ProcessParams(args);

            if (Program.TryGetActiveProject(out ProjectData project))
            {
                if (!project.valid)
                {
                    Validation.ValidateProject();

                    if (!project.valid) // Was actually invalid
                    {
                        Program.AddError("Opened project is not valid, can not solve.");
                        return true;
                    }
                }

                if (File.Exists(project.folder + solutionResultFilePath) && !ignoreOverwirte)
                {
                    Program.AddError($"Specified filepath already exists, abandoned. Use a different file path or add {OperationKeys.ignoreOverwrite} at the end.");
                    return true;
                }

                if (File.Exists(project.folder + solutionResultFilePath))
                {
                    Program.AddNote($"Existing file {project.folder + solutionResultFilePath} will be overwritten next time you save changes.");
                }

                SolutionResultData result = Solution.Solve(project, solutionResultFilePath);

                Program.ToggleChanges();

                result.filePath = project.folder + solutionResultFilePath;
                project.relativeSolutionResultPath = solutionResultFilePath;
                project.solutionResult = result;

                Console.WriteLine($"Project solution file {solutionResultFilePath} successfully generated. Use \"Save\" in order to write the file!");

                return true;
            }
            else
            {
                Program.AddError("No project opened.");
                return true;
            }
        }

        public override string BasicHelpResponse => 
            $"Solves opened project if it is valid and assigns specified file to save the results.\n" +
            $"\n" +
            $"Usage:\n" +
            $"GenerateSolution FilePath [{OperationKeys.ignoreOverwrite}]\n" +
            $"\n" +
            $"{OperationKeys.ignoreOverwrite}: Create new file even if the specified file path is already used.";
    }
}
