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

                SolutionResultData result = Solution.Solve(project);
            }
            else
            {
                Program.AddError("No project opened.");
                return true;
            }
        }
    }
}
