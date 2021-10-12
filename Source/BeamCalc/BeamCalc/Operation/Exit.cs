using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalc.Operation
{
    class Exit : AbstractParametrisedOperation
    {
        private bool ignoreSave;

        public Exit()
        {
            paramDelegates = new Dictionary<string, Action>()
            {
                { OperationKeys.ignoreSave, () => ignoreSave = true}
            };
        }

        public override bool Execute(List<string> args)
        {
            args.TakeArg();
            ignoreSave = false;

            ProcessParams(args);

            if (Program.runData.unsavedChanges && !ignoreSave)
            {
                Program.AddError($"Found unsaved changes, abandoned. Use Save command or add {OperationKeys.ignoreSave} parameter at the end.");
                return true;
            }

            return false;
        }

        public override string BasicHelpResponse => 
            $"Closes the program.\n" +
            $"\n" +
            $"Usage:\n" +
            $"Exit [{OperationKeys.ignoreSave}]\n" +
            $"\n" +
            $"Abandons if used without {OperationKeys.ignoreSave} with unsaved changes.\n" +
            $"{OperationKeys.ignoreSave} forcingly closes the app, loosing all the unsaved changes.";
    }
}
