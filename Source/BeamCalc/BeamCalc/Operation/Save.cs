using System.Collections.Generic;

namespace BeamCalc.Operation
{
    class Save : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            bool saved = Program.Save();

            if (!saved)
            {
                Program.runData.operationReports.AddNote("Nothing was saved. Probably no file is opened to edit yet.");
            }

            return true;
        }

        public override string BasicHelpResponse => 
            "Usage:\n" +
            "Save\n" +
            "Saves changes for currently opened file(s).";
    }
}
