using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BeamCalc.Project
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    class ProjectData : SavableProjectElement
    {
        public string filePath;
        public string folder;

        public MaterialDataStorage materialDataStorage;
        public SolutionResultData solutionResult;

        [JsonProperty]
        public string relativeMaterialDataStoragePath;

        [JsonProperty]
        public string relativeSolutionResultPath;

        [JsonProperty]
        public Dictionary<string, NodeData> nodes;

        [JsonProperty]
        public Dictionary<string, BeamData> beams;

        [JsonProperty]
        public bool valid;


        public override string UserFriendlyName => "project";

        protected override string SavableProjectElementTypeKey => "ProjectData";


        public ProjectData() 
        {
            nodes = new Dictionary<string, NodeData>();
            beams = new Dictionary<string, BeamData>();
        }

        public ProjectData(string filePath) : this()             // Only to create new storages. For filesaves, use LoadFromFile instead.
        {
            this.filePath = filePath;
            folder = filePath.Substring(0, filePath.LastIndexOf('/') + 1);
        }


        public void Save()
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            }));

            if (materialDataStorage != null) materialDataStorage.Save();
        }

        public void BindMaterialDataStorage(MaterialDataStorage storage, string relativePath)
        {
            materialDataStorage = storage;
            relativeMaterialDataStoragePath = relativePath;
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
                result.valid = false;
                result.relativeMaterialDataStoragePath = "";
            }

            result.ThrowIfInvalidSavalbeProjectElementType();

            return result;
        }
        #endregion
    }
}
