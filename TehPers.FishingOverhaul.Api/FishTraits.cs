using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace TehPers.FishingOverhaul.Api
{
    public class FishTraits
    {
        [JsonRequired]
        [Description("How often the fish darts in the fishing minigame.")]
        public int DartFrequency { get; set; }

        [JsonRequired]
        [Description("How the fish darts during the fishing minigame.")]
        public string DartBehavior { get; set; }

        [JsonRequired]
        [Description("The minimum size the fish can be.")]
        public int MinSize { get; set; }

        [JsonRequired]
        [Description("The maximum size the fish can be.")]
        public int MaxSize { get; set; }

        [Description("Whether the fish is legendary.")]
        [DefaultValue(false)]
        public bool IsLegendary { get; set; }

        [JsonConstructor]
        public FishTraits(
            int dartFrequency,
            string? dartBehavior,
            int minSize,
            int maxSize,
            bool isLegendary = false
        )
        {
            this.DartFrequency = dartFrequency;
            this.DartBehavior =
                dartBehavior ?? throw new ArgumentNullException(nameof(dartBehavior));
            this.MinSize = minSize;
            this.MaxSize = maxSize;
            this.IsLegendary = isLegendary;
        }
    }
}