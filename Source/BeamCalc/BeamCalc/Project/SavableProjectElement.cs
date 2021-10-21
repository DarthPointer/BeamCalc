using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BeamCalc.Project
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    abstract class SavableProjectElement
    {
        protected abstract string SavableProjectElementTypeKey { get; }

        [JsonProperty]
        private string SerializedSavableProjectElementTypeKey
        {
            get => SavableProjectElementTypeKey;

            set
            {
                loadedSavableProjectElementTypeKey = value;
            }
        }

        protected string loadedSavableProjectElementTypeKey;

        protected bool ValidateSavableProjectElementType()
        {
            return loadedSavableProjectElementTypeKey == SavableProjectElementTypeKey;
        }
    }
}
