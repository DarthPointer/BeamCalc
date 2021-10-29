using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamCalc.Operation
{
    class CommandList : AbstractOperation
    {
        public override bool Execute(List<string> args)
        {
            Console.WriteLine("List of available commands:");
            Console.WriteLine();
            Console.WriteLine(string.Join('\n', Program.commands.Keys));

            return true;
        }

        public override string BasicHelpResponse => 
            "Dumps the list of available commands into stdout.\n" +
            "\n" +
            "Usage:\n" +
            "CommandList";
    }
}
