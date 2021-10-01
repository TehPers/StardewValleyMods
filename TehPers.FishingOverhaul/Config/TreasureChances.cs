using System.ComponentModel;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    [JsonDescribe]
    public class TreasureChances : FishingChances
    {
        [Description("The effect that the magnet bait has.")]
        public double MagnetFactor { get; set; } = 0.15;

        [Description("The effect that the treasure hunter tackle has.")]
        public double TreasureHunterFactor { get; set; } = 0.05;

        [Description(
            "The effect that the pirate profession has. This is multiplied by your base chance "
            + "before being added."
        )]
        public double PirateFactor { get; set; } = 1.0;

        public override void Reset()
        {
            base.Reset();

            this.MagnetFactor = 0.15d;
            this.TreasureHunterFactor = 0.05d;
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