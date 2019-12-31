using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Json;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework.Config
{
    [JsonDescribe]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "Private setters are called by the JSON deserializer through reflection.")]
    public class TreasureChances : FishingChances, ITreasureChances
    {
        [Description("The effect that the magnet bait has.")]
        public double MagnetFactor { get; private set; } = 0.15;

        [Description("The effect that the treasure hunter tackle has.")]
        public double TreasureHunterFactor { get; private set; } = 0.05;

        [Description("The effect that the pirate profession has.")]
        public double PirateFactor { get; private set; } = 0.15;
    }
}