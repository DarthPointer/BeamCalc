using Newtonsoft.Json;
using System.Collections.Generic;

namespace BeamCalc.Project
{
    [JsonObject(memberSerialization: MemberSerialization.OptIn)]
    class SolutionResultData : SavableProjectElement
    {
        public string filePath;

        [JsonProperty]
        public MaterialDataStorage materialDataStorage;

        [JsonProperty]
        public NestedProjectData nestedProjectData;

        [JsonProperty]
        public Dictionary<string, StressFunction> stressFunctions;


        protected override string SavableProjectElementTypeKey => "SolutionResultData";

        public override string UserFriendlyName => "solution result";


        public void Save()
        {
        }


        public class NestedProjectData : ProjectData
        {
            protected override string SavableProjectElementTypeKey => "NestedProjectData";
        }
    }

    class StressFunction
    {
    }
}
