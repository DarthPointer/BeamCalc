using Newtonsoft.Json;

namespace BeamCalc.Project
{
    [JsonObject]
    class NodeData
    {
        public double location;

        public bool xFixed;

        public double xForce;
    }
}
