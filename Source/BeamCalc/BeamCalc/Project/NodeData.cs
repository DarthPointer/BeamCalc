using Newtonsoft.Json;

namespace BeamCalc.Project
{
    [JsonObject]
    class NodeData
    {
        public float location;

        public bool xFixed;

        public float xForce;
    }
}
