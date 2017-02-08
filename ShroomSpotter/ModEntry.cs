using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace TehPers.Stardew.ShroomSpotter {

    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public ModConfig config;
        public List<UpdateEvent> updateEvents = new List<UpdateEvent>();
        public delegate bool UpdateEvent();

        public ModEntry() {
            INSTANCE = this;
        }

        public override void Entry(IModHelper helper) {
            this.config = helper.ReadConfig<ModConfig>();
            if (!config.ModEnabled) return;

            //this.Monitor.Log("It is *HIGHLY* recommended you install a Health Bars mod for enemies!", LogLevel.Info);

            ControlEvents.KeyPressed += KeyPressed;
        }

        #region Events
        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            Keys getShroomLevels;
            if (Enum.TryParse(config.GetShroomLevels, out getShroomLevels) && e.KeyPressed == getShroomLevels) {
                // Find all shroom levels
                int daysTilShroom = -1;
                List<int> shroomLevels = new List<int>();
                while (shroomLevels.Count == 0 && ++daysTilShroom < 50)
                for (int mineLevel = 1; mineLevel < 120; mineLevel++) {
                    Random random = new Random((int) Game1.stats.DaysPlayed + daysTilShroom + mineLevel + (int) Game1.uniqueIDForThisGame / 2);

                    // Simulate all the random values grabbed before the shrooms
                    if (random.NextDouble() < 0.3 && mineLevel > 2) random.NextDouble();
                    random.NextDouble();
                    if (random.NextDouble() < 0.035 && mineLevel >= 80 && mineLevel <= 120 && mineLevel % 5 != 0)
                        shroomLevels.Add(mineLevel);
                }

                if (shroomLevels.Count > 0) {
                    if (daysTilShroom == 0)
                        Game1.showGlobalMessage("Shroom layers will spawn on these mine levels: " + string.Join<int>(", ", shroomLevels));
                    else
                        Game1.showGlobalMessage("Shrooms will spawn in " + daysTilShroom + " day(s) on these mine levels: " + string.Join<int>(", ", shroomLevels));
                } else Game1.showGlobalMessage("No shroom layers will spawn in the next 50 days!");
            }
        }
        #endregion
    }
}
