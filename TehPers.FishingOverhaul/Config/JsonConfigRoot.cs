using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// A configuration.
    /// </summary>
    public abstract record JsonConfigRoot
    {
        /// <summary>
        /// Optional '$schema' URL. This is ignored and exists entirely for convenience.
        /// </summary>
        [JsonProperty("$schema", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [SuppressMessage(
            "CodeQuality",
            "IDE0051:Remove unused private members",
            Justification = "Used in JSON serialization."
        )]
        private string? Schema { get; init; }
    }
}