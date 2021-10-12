using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalc.Operation
{
    abstract class AbstractOperation
    {
        public abstract string BasicHelpResponse { get; }

        public abstract bool Execute(List<string> args);        // true = continue execution

        protected void Readparams()
        {
        }

        protected static bool MandatoryArgumentPresense(List<string> args, string argumentName)
        {
            if (args.Count > 0)
            {
                return true;
            }
            else
            {
                Program.runData.operationReports.AddError($"Command lacks a mandatory \"{argumentName}\" argument. Abandoned.");
                return false;
            }
        }

        protected static void ReportBadArgument(string argument)
        {
            Program.runData.operationReports.AddError($"Bad Argument \"{argument}\". Abandoned.");
        }
    }
}
