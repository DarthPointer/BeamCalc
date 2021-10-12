using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalc.Operation
{
    class Help : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            if (args.Count > 0)
            {
                string targetCommand = args.TakeArg();

                if (Program.commands.ContainsKey(targetCommand))
                {
                    Console.WriteLine();
                    Console.WriteLine(Program.commands[targetCommand].BasicHelpResponse);
                }
                else
                {
                    Program.AddError($"Command {targetCommand} not found.");
                }
            }
            else
            {
                Console.WriteLine(
                    "Usage:\n" +
                    "Help [Command Name]\n" +
                    "Prints help data for specified command.\n" +
                    "Or this text if no command specified.");
            }


            return true;
        }

        public override string BasicHelpResponse => "Huh, seriously?";
    }
}
