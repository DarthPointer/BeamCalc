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
    class ProjectData : SavableProjectElement
    {
        public string filePath;
        public string folder;

        public MaterialDataStorage materialDataStorage;

        [JsonProperty]
        public string relativeMaterialDataStoragePath;

        protected override string SavableProjectElementTypeKey => "ProjectData";


        public ProjectData() { }

        public ProjectData(string filePath) : this()             // Only to create new storages. For filesaves, use LoadFromFile instead.
        {
            this.filePath = filePath;
        }


        public void Save()
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            }));
        }

        #region statics
        public static ProjectData LoadFromFile(string filePath)
        {
            ProjectData result = JsonConvert.DeserializeObject<ProjectData>(File.ReadAllText(filePath));

            result.filePath = filePath;
            result.folder = filePath.Substring(0, filePath.LastIndexOf('/') + 1);

            if (File.Exists(result.folder + result.relativeMaterialDataStoragePath))
            {
                result.materialDataStorage = MaterialDataStorage.LoadFromFile(result.folder + result.relativeMaterialDataStoragePath);
            }
            else
            {
                result.relativeMaterialDataStoragePath = "";
            }

            if (!result.ValidateSavableProjectElementType())
            {
                throw new Exception($"Unexpected savable project elemet type encountered: {result.loadedSavableProjectElementTypeKey}");
            }

            return result;
        }
        #endregion
    }
}
