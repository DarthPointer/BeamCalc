using BeamCalc.Project;
using System.Collections.Generic;

namespace BeamCalc
{
    class RunData
    {
        public ProjectData project;
        public MaterialDataStorage materialDataStorage;

        public bool unsavedChanges = false;

        public List<string> operationReports;
    }
}
