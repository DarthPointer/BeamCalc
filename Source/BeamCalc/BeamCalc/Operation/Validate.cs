using BeamCalc.Project;
using System;
using System.Collections.Generic;
using BeamCalc.Solver;

namespace BeamCalc.Operation
{
    class Validate : AbstractParametrisedOperation
    {
        private bool forced;

        public Validate()
        {
            paramDelegates = new Dictionary<string, Action>()
            {
                { OperationKeys.forceRedo, () => forced = true }
            };
        }

        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            forced = false;

            ProcessParams(args);

            if (Program.TryGetActiveProject(out ProjectData project))
            {
                if (project.valid && !forced)
                {
                    Program.AddError($"Opened project is already marked as valid. Use {OperationKeys.forceRedo} if you want to validate anyways.");
                    return true;
                }
            }

            Validation.ValidateProject();

            Console.WriteLine("Validation finished!");
            return true;
        }

        public override string BasicHelpResponse =>
            $"Validates currenlty opened files and reports results into stdout\n" +
            $"\n" +
            $"Usage:\n" +
            $"Validate [{OperationKeys.forceRedo}]\n" +
            $"\n" +
            $"{OperationKeys.forceRedo}: Validate even if the project is already marked as valid.";
    }
}
