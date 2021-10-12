using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalc.Operation
{
    abstract class AbstractParametrisedOperation : AbstractOperation
    {
        protected Dictionary<string, Action> paramDelegates;

        protected bool ProcessParams(List<string> args)         // Returns true if finished successfully
        {
            while (args.Count > 0)
            {
                string param = args.TakeArg();

                if (paramDelegates.ContainsKey(param))
                {
                    paramDelegates[param]();
                }
                else
                {
                    ReportBadArgument(param);
                    return false;
                }
            }

            return true;
        }
    }
}
