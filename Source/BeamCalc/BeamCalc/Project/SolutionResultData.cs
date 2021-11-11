using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace BeamCalc.Project
{
    [JsonObject(memberSerialization: MemberSerialization.OptIn)]
    class SolutionResultData : SavableProjectElement
    {
        public delegate double MathFunction(double arg);

        public string filePath;

        [JsonProperty]
        public Dictionary<string, MaterialData> materials;

        [JsonProperty]
        public Dictionary<string, NodeData> nodes;

        [JsonProperty]
        public SolutionBeam[] beams;


        protected override string SavableProjectElementTypeKey => "SolutionResultData";

        public override string UserFriendlyName => "solution result";


        public void Save()
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            }));
        }


        public static SolutionResultData LoadFromFile(string filePath)
        {
            SolutionResultData result = JsonConvert.DeserializeObject<SolutionResultData>(File.ReadAllText(filePath), new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });

            result.ThrowIfInvalidSavalbeProjectElementType();

            result.filePath = filePath;
            return result;
        }


        [JsonObject(memberSerialization: MemberSerialization.OptIn)]
        public class SolutionBeam
        {
            [JsonProperty]
            public string leftNode;
            [JsonProperty]
            public string rightNode;

            [JsonProperty]
            public string materialName;
            [JsonProperty]
            public double crossSection;

            [JsonProperty]
            public string key;

            [JsonProperty]
            public bool inverted;
            [JsonProperty]
            public double xLoad;
            [JsonProperty]
            public double length;

            [JsonProperty]
            public SqareFunction offset;
            [JsonProperty]
            public SqareFunction reaction;

            public double AbsoluteLoad => inverted ? -xLoad : xLoad;


            public SolutionBeam() { }

            public SolutionBeam(BeamData beam, string beamKey, ProjectData project)
            {
                if (project.nodes[beam.node1Name].location < project.nodes[beam.node2Name].location)
                {
                    leftNode = beam.node1Name;
                    rightNode = beam.node2Name;

                    inverted = false;
                }
                else
                {
                    rightNode = beam.node1Name;
                    leftNode = beam.node2Name;

                    inverted = true;
                }

                key = beamKey;
                materialName = beam.materialName;
                crossSection = beam.crossSection;
                xLoad = beam.xLoad;

                length = project.nodes[rightNode].location - project.nodes[leftNode].location;
            }
        }

        [JsonObject]
        public class SqareFunction
        {
            public double a2;
            public double a1;
            public double a0;

            public double this[double x] => x*x*a2 + x*a1 + a0;

            public double Peak => -a1 / (2 * a2);

            public double Max(double left, double right)
            {
                double max = Math.Max(this[left], this[right]);

                if (a2 != 0)
                {
                    max = Math.Max(max, this[Peak]);
                }

                return max;
            }

            public double Min(double left, double right)
            {
                double min = Math.Min(this[left], this[right]);

                if (a2 != 0)
                {
                    min = Math.Min(min, this[Peak]);
                }

                return min;
            }
        }
    }
}
