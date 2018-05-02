using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FishingOverhaul.Configs;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using TehCore;

namespace FishingOverhaul {
    public class ModFishing : Mod {
        public static ModFishing Instance { get; private set; }

        public ConfigMain MainConfig { get; private set; }
        public ConfigFish FishConfig { get; private set; }
        public ConfigTreasure TreasureConfig { get; private set; }

        public override void Entry(IModHelper helper) {
            ModFishing.Instance = this;

            this.LoadConfigs();

            // Make sure this mod is enabled
            //if (!this.MainConfig.ModEnabled)
                //return;

            GameEvents.UpdateTick += this.UpdateTick;
        }

        private void LoadConfigs() {
            // Load configs
            this.MainConfig = this.Helper.TryReadJsonFile<ConfigMain>("config.json") ?? new ConfigMain();
            this.TreasureConfig = this.Helper.TryReadJsonFile<ConfigTreasure>("treasure.json") ?? new ConfigTreasure();
            this.FishConfig = this.Helper.TryReadJsonFile<ConfigFish>("fish.json");

            // Populate fish config if empty
            if (this.FishConfig == null) {
                this.FishConfig = new ConfigFish();
                this.FishConfig.PopulateData();
            }

            // Make sure the configs are generated
            ModCore.Instance.Json.WriteJson("config.json", this.MainConfig, this.Helper, this.MainConfig.MinifyConfigs);
            ModCore.Instance.Json.WriteJson("treasure.json", this.TreasureConfig, this.Helper, this.MainConfig.MinifyConfigs);
            ModCore.Instance.Json.WriteJson("fish.json", this.FishConfig, this.Helper, this.MainConfig.MinifyConfigs);
            //this.Helper.WriteJsonFile("treasure.json", this.TreasureConfig);
            //this.Helper.WriteJsonFile("fish.json", this.FishConfig);    
        }

        #region Events
        private void UpdateTick(object sender, EventArgs e) {

            // Replace the player's fishing rod with a custom rod
            if (Game1.player.CurrentTool is FishingRod rod && !(rod is CustomFishingRod)) {
                Console.WriteLine("Normal rod found, replacing...");

                CustomFishingRod newRod = new CustomFishingRod {
                    UpgradeLevel = rod.UpgradeLevel,
                    IndexOfMenuItemView = rod.IndexOfMenuItemView
                };
                newRod.numAttachmentSlots.Value = rod.numAttachmentSlots.Value;
                Game1.player.CurrentTool = newRod;
            }
        }
        #endregion
    }
}
