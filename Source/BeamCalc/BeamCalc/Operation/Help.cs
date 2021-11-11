using System;
using System.Collections.Generic;

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
                    "\n" +
                    "Help [Command Name]\n" +
                    "\n" +
                    "Prints help data for specified command.\n" +
                    "Or this text if no command specified.\n" +
                    "\n" +
                    "Use CommandList to get the full list of available commands.");
            }


            return true;
        }

        public override string BasicHelpResponse => "Huh, seriously?";
    }
}
