using System.Collections.Generic;

namespace BeamCalc.Operation
{
    abstract class AbstractOperation
    {
        protected delegate bool SafeParser<T>(string str, out T result);

        public abstract string BasicHelpResponse { get; }

        public abstract bool Execute(List<string> args);        // true = continue execution

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

        protected static bool TakeMandatoryParsedArgument<T>(List<string> args, SafeParser<T> safeParser, out T result, string argumentName)
        {
            result = default;

            if (!MandatoryArgumentPresense(args, argumentName)) return false;

            string arg0 = args.TakeArg();

            if (safeParser(arg0, out result))
            {
                return true;
            }
            else
            {
                Program.AddError($"Can not parse \"{arg0}\" as a \"{typeof(T).Name}\" for argument \"{argumentName}\".");
                return false;
            }
        }

        protected static void ReportBadArgument(string argument)
        {
            Program.runData.operationReports.AddError($"Bad Argument \"{argument}\". Abandoned.");
        }
    }
}
