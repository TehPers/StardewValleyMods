using Newtonsoft.Json;

namespace TehPers.FishingOverhaul.Config
{
    public abstract class JsonConfigRoot
    {
        /// <summary>
        /// For JSON schema. Adds an optional '$schema' attribute.
        /// </summary>
        [JsonProperty("$schema", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Schema { get; set; }
    }
}