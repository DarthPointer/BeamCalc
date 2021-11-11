using Newtonsoft.Json;

namespace BeamCalc.Project
{
    [JsonObject]
    class BeamData
    {
        public string materialName;
        public double crossSection;

        public string node1Name;
        public string node2Name;

        public double xLoad;
    }
}
