using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace BeamCalc.Project
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    class ProjectData
    {
        public string filePath;
        public string folder;

        public MaterialDataStorage materialDataStorage;

        [JsonProperty]
        public string relativeMaterialDataStoragePath;

        public void Save()
        {
        }

        #region statics
        public static ProjectData LoadFromFile(string filePath)
        {
            ProjectData result = JsonConvert.DeserializeObject<ProjectData>(File.ReadAllText(filePath));

            result.filePath = filePath;
            result.folder = filePath.Substring(0, filePath.LastIndexOf('/') + 1);

            result.materialDataStorage = MaterialDataStorage.LoadFromFile(result.folder + result.relativeMaterialDataStoragePath);

            return result;
        }
        #endregion
    }
}
