using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Operation;

namespace BeamCalc
{
    class Program
    {
        public static RunData runData;

        public static Dictionary<string, AbstractOperation> commands = new Dictionary<string, AbstractOperation>()
        {
            { "Exit", new Exit() },
            { "Save", new Save() },

            { "Help", new Help() },

            { "CreateMaterialDataStorage", new CreateMaterialDataStorage() },
            { "LoadMaterialDataStorage", new LoadMaterialDataStorage() }
        };

        static void Main(string[] args)
        {
            runData = new RunData();

            bool run = true;
            while (run)
            {
                string input = Console.ReadLine();
                List<string> inputArgs = input.Parse();

                if (inputArgs.Count > 0)
                {
                    if (commands.ContainsKey(inputArgs[0]))
                    {
                        runData.operationReports = new List<string>();
                        try
                        {
                            run = commands[inputArgs[0]].Execute(inputArgs);

                            foreach (string report in runData.operationReports)
                            {
                                Console.WriteLine(report);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.GetType());
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Command {inputArgs[0]} not found");
                    }
                }
            }
        }

        public static bool Save()       // Returns true if saved anything
        {
            if (runData.project != null)
            {
                runData.project.Save();
                runData.unsavedChanges = false;

                return true;
            }

            if (runData.materialDataStorage != null)
            {
                runData.materialDataStorage.Save();
                runData.unsavedChanges = false;

                return true;
            }

            return false;
        }

        public static void AddNote(string message)
        {
            runData.operationReports.AddNote(message);
        }

        public static void AddWarning(string message)
        {
            runData.operationReports.AddWarning(message);
        }

        public static void AddError(string message)
        {
            runData.operationReports.AddError(message);
        }
    }
}
