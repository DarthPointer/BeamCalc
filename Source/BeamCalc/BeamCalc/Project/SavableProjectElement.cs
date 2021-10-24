using Newtonsoft.Json;
using System;

namespace BeamCalc.Project
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    abstract class SavableProjectElement
    {
        public abstract string UserFriendlyName { get; }

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

        protected void ThrowIfInvalidSavalbeProjectElementType()
        {
            if (!ValidateSavableProjectElementType())
            {
                throw new Exception($"Unexpected savable project elemet type encountered: {loadedSavableProjectElementTypeKey}.");
            }
        }
    }
}
