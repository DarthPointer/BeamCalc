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
    class MaterialDataStorage
    {
        #region Fields
        public string filePath;

        [JsonProperty]
        public Dictionary<string, MaterialData> materials;
        #endregion


        #region Ctors
        private MaterialDataStorage()
        {
            materials = new Dictionary<string, MaterialData>();
        }

        public MaterialDataStorage(string filePath) : this()             // Only to create new storages. For filesaves, use LoadFromFile instead.
        {
            this.filePath = filePath;
        }
        #endregion


        #region Methods
        public void Save()
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            }));
        }
        #endregion


        #region Statics
        public static MaterialDataStorage LoadFromFile(string filePath)
        {
            MaterialDataStorage result = JsonConvert.DeserializeObject<MaterialDataStorage>(File.ReadAllText(filePath), new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });

            result.filePath = filePath;

            return result;
        }
        #endregion
    }


    [JsonObject]
    class MaterialData
    {
        public float elasticModulus;
        public float stressLimit;
    }
}
