using StardewModdingAPI;
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


        }

    }
}
