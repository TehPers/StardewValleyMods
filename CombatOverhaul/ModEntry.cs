using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.CombatOverhaul {

    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public ModConfig config;

        public ModEntry() {
            INSTANCE = this;
        }

        public override void Entry(IModHelper helper) {
            this.config = helper.ReadConfig<ModConfig>();
            if (!config.ModEnabled) return;

            this.Monitor.Log("It is *HIGHLY* recommended you install a Health Bars mod for enemies!", LogLevel.Info);

            GameEvents.UpdateTick += UpdateTick;
        }

        #region Events
        private void UpdateTick(object sender, EventArgs e) {
            for (int i = 0; i < Game1.player.items.Count; i++) {
                Item cur = Game1.player.items[i];
                if (cur is MeleeWeapon && !(cur is ModWeapon)) {
                    Game1.player.items[i] = new ModWeapon(cur as MeleeWeapon);
                }
            }
        }
        #endregion
    }
}
