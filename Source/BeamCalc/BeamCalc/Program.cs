using BeamCalc.Operation;
using BeamCalc.Project;
using System;
using System.Collections.Generic;

namespace BeamCalc
{
    class Program
    {
        public static RunData runData;

        public static Dictionary<string, AbstractOperation> commands = new Dictionary<string, AbstractOperation>()
        {
            { "Exit", new Exit() },
            { "Save", new Save() },
            { "Validate", new Validate() },

            { "Help", new Help() },

            { "CreateProject", new CreateProject() },
            { "LoadProject", new LoadProject() },

            { "Node", new Node() },
            { "Beam", new Beam() },

            { "CreateMaterialDataStorage", new CreateMaterialDataStorage() },
            { "LoadMaterialDataStorage", new LoadMaterialDataStorage() },

            { "BindMaterialDataStorage", new BindMaterialDataStorage() },

            { "Material", new Material() },

            { "PrintMaterials", new PrintMaterials() },
            { "PrintNodes", new PrintNodes() },
            { "PrintBeams", new PrintBeams() }
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
                            Console.WriteLine(e.ToString());
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

        public static bool TryGetActiveMaterialDataStorage(out MaterialDataStorage result)          // Returns true if succeeded.
        {
            if (runData.materialDataStorage != null)
            {
                result = runData.materialDataStorage;
                return true;
            }

            if (runData.project != null)
            {
                if (runData.project.materialDataStorage != null)
                {
                    result = runData.project.materialDataStorage;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public static bool TryGetActiveProject(out ProjectData result)
        {
            if (runData.project != null)
            {
                result = runData.project;
                return true;
            }

            result = null;
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
