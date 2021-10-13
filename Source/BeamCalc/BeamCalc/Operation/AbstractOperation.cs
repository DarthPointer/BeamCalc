﻿using System;
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
                Program.AddError($"Command lacks a mandatory \"{argumentName}\" argument.");
                return false;
            }
        }

        protected static bool TakeMandatoryFloatFromArgs(List<string> args, out float result, string argumentName)
        {
            result = -1;

            if (!MandatoryArgumentPresense(args, argumentName)) return false;

            string arg0 = args.TakeArg();

            if (float.TryParse(arg0, out result))
            {
                return true;
            }
            else
            {
                Program.AddError($"Can not parse \"{arg0}\" as a float for argument \"{argumentName}\".");
                return false;
            }
        }

        protected static void ReportBadArgument(string argument)
        {
            Program.runData.operationReports.AddError($"Bad Argument \"{argument}\". Abandoned.");
        }
    }
}
