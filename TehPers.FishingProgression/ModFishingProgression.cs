using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.Core;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingProgression.Configs;

namespace TehPers.FishingProgression {
    public class ModFishingProgression : Mod {
        public static ModFishingProgression Instance { get; private set; }

        public IFishingApi FishingApi { get; private set; }
        internal TehCoreApi CoreApi { get; private set; }

        public MainConfig Config { get; set; }

        public override void Entry(IModHelper helper) {
            ModFishingProgression.Instance = this;
            this.CoreApi = TehCoreApi.Create(this);

            GameEvents.FirstUpdateTick += (sender, e) => this.Initialize();
        }

        private void Initialize() {
            this.FishingApi = this.Helper.ModRegistry.GetApi<IFishingApi>("TehPers.FishingOverhaul");
            if (this.FishingApi == null) {
                this.Monitor.Log("Teh's fishing overhaul is not installed. Please disable this mod.");
                return;
            }

            this.LoadConfig();

            GameEvents.OneSecondTick += (sender, e) => this.SetFishHidden();
        }

        private void LoadConfig() {
            this.Config = this.CoreApi.JsonHelper.ReadOrCreate<MainConfig>("config.json", this.Helper);
        }

        private void SetFishHidden() {

        }
    }
}

namespace TehPers.FishingProgression.Configs {
    public class MainConfig {
        [Description("Whether or not this mod should make changes to the game.")]
        public bool ModEnabled { get; set; } = true;
    }
}
