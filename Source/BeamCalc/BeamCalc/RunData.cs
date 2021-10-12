using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Project;

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
