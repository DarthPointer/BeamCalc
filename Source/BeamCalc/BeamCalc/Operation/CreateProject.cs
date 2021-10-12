using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalc.Operation
{
    class CreateProject : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            if (args.Count == 0)
            {
                Program.runData.operationReports.Add("Project not created: no project name specified!");
            }
            else
            {
                string projectFileName = args.TakeArg();

                //Program.runData.project = 
            }

            return true;
        }

        public override string BasicHelpResponse => throw new NotImplementedException();
    }
}
