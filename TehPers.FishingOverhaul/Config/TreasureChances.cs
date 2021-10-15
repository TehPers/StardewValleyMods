using System.ComponentModel;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// Configuration for the chances of catching treasure while fishing.
    /// </summary>
    /// <inheritdoc cref="FishingChances"/>
    [JsonDescribe]
    public class TreasureChances : FishingChances
    {
        /// <summary>
        /// The effect that the magnet bait has.
        /// </summary>
        [DefaultValue(0.15)]
        public double MagnetFactor { get; set; } = 0.15;

        /// <summary>
        /// The effect that the treasure hunter tackle has.
        /// </summary>
        [DefaultValue(0.05)]
        public double TreasureHunterFactor { get; set; } = 0.05;

        /// <summary>
        /// The effect that the pirate profession has. This is multiplied by your base chance
        /// before being added.
        /// </summary>
        [DefaultValue(1d)]
        public double PirateFactor { get; set; } = 1d;

        public override void Reset()
        {
            base.Reset();

            this.MagnetFactor = 0.15d;
            this.TreasureHunterFactor = 0.05d;
            this.PirateFactor = 1d;
        }

        public override void RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.chances.treasure.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.chances.treasure.{key}.desc");

            base.RegisterOptions(configApi, manifest, translations);

            configApi.RegisterClampedOption(
                manifest,
                Name("magnetFactor"),
                Desc("magnetFactor"),
                () => (float)this.MagnetFactor,
                val => this.MagnetFactor = val,
                0f,
                1f
            );
            configApi.RegisterClampedOption(
                manifest,
                Name("treasureHunterFactor"),
                Desc("treasureHunterFactor"),
                () => (float)this.TreasureHunterFactor,
                val => this.TreasureHunterFactor = val,
                0f,
                1f
            );
        }

        protected override double GetUnclampedChance(Farmer farmer, int streak)
        {
            var chance = base.GetUnclampedChance(farmer, streak);

            // Check attachments
            if (farmer.CurrentItem is FishingRod rod)
            {
                // Magnet bait
                if (rod.attachments.Any(attachment => attachment?.ParentSheetIndex == 703))
                {
                    chance += this.MagnetFactor;
                }

                // Treasure hunter tackle
                if (rod.attachments.Any(attachment => attachment?.ParentSheetIndex == 693))
                {
                    chance += this.TreasureHunterFactor;
                }
            }

            // Pirate profession
            if (farmer.professions.Contains(9))
            {
                chance += this.BaseChance * this.PirateFactor;
            }

            return chance;
        }
    }
}
