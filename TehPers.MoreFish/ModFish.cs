using System;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.FishingOverhaul.Api;
using SObject = StardewValley.Object;

namespace TehPers.MoreFish {
    public class ModFish : Mod {
        public static ModFish Instance { get; private set; }

        public override void Entry(IModHelper helper) {
            ModFish.Instance = this;

            // TODO: Proper credits file or something
            // CheesySteak + Noises - seahorse (pink)
            // Emissary of Infinity - seahorse (orange)

            GameEvents.FirstUpdateTick += this.FirstUpdateTick;
            ControlEvents.KeyPressed += (sender, e) => {
                if (e.KeyPressed == Keys.NumPad6) {
                    Game1.player.addItemToInventory(new SObject(AddedFish.Seahorse.ParentSheetIndex, 1, false, AddedFish.Seahorse.FishTraits.Price));
                }
            };
        }

        private void FirstUpdateTick(object sender, EventArgs e) {
            IFishingApi api = this.Helper.ModRegistry.GetApi<IFishingApi>("TehPers.FishingOverhaul");
            if (api == null) {
                // Failed to load
                this.Monitor.Log("Failed to load API for Teh's Fishing Overhaul. This mod will be disabled.", LogLevel.Error);
            } else {
                AddedFish.LoadFish(api);
            }
        }

        #region Static
        public static string Translate(string key, params object[] formatArgs) {
            return string.Format(ModFish.Instance.Helper.Translation.Get(key), formatArgs);
        }
        #endregion
    }
}
