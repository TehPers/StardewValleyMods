using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Conflux.Collections;
using TehPers.Logistics.Items;
using SObject = StardewValley.Object;

namespace TehPers.Logistics {
    public class ModEntry : Mod {
        public override void Entry(IModHelper helper) {
            // Register an event for the first update tick to handle all core API calls
            GameEvents.FirstUpdateTick += (sender, e) => {
                if (helper.ModRegistry.GetApi("TehPers.CoreMod") is Func<IMod, ICoreApi> coreApiFactory) {
                    // Create core API
                    ICoreApi coreApi = coreApiFactory(this);

                    // Register custom machines
                    this.RegisterMachines(coreApi);
                }
            };
        }

        private void RegisterMachines(ICoreApi coreApi) {
            this.Monitor.Log("Registering machines...", LogLevel.Info);
            IItemApi itemApi = coreApi.Items;

            // Stone converter
            TextureInformation textureInfo = new TextureInformation(coreApi.Drawing.WhitePixel, null, Color.Blue);
            StoneConverterMachine stoneConverter = new StoneConverterMachine(this, "stoneConverter", textureInfo);
            itemApi.Register("stoneConverter", stoneConverter);

            // TODO: debug
            ControlEvents.KeyPressed += (sender, pressed) => {
                if (pressed.KeyPressed == Keys.NumPad3 && itemApi.TryGetIndex("stoneConverter", out int index)) {
                    SObject machine = new SObject(Vector2.Zero, index, false);
                    Game1.player.addItemToInventory(machine);
                }
            };

            GraphicsEvents.OnPreRenderHudEvent += (sender, args) => {
                if (Game1.currentLocation != null && Game1.player?.ActiveObject != null) {
                    Game1.drawPlayerHeldObject(Game1.player);
                }
            };

            this.Monitor.Log("Done");
        }
    }
}
