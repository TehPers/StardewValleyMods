using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Configs;
using ModUtilities.Helpers;
using ModUtilities.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ModUtilities {
    public class ModUtilities : Mod {
        public static ModUtilities Instance { get; private set; }

        public ConfigMain Config { get; private set; }

        public ModUtilities() {
            ModUtilities.Instance = this;
        }

        public override void Entry(IModHelper helper) {
            this.Config = helper.ReadConfig<ConfigMain>();

            KeyboardInput.CharEntered += this.CharEntered;
            ControlEvents.KeyPressed += this.KeyPressed;
        }

        private void CharEntered(object sender, CharacterEventArgs e) {
            if (Game1.activeClickableMenu is ModMenu menu) {
                // Check if ctrl-v was pressed
                if (e.Character == '\u0016') {
                    // TODO: Check if System.Windows.Forms breaks cross-platform compatibilty
                    string clipboardText = "";
                    Thread clipboardThread = new Thread(() => clipboardText = Clipboard.GetText());
                    clipboardThread.SetApartmentState(ApartmentState.STA);
                    clipboardThread.Start();
                    clipboardThread.Join();
                    menu.EnterText(clipboardText);
                }

                // Check if character is printable (https://stackoverflow.com/a/45928048/8430206)
                if (e.Character.IsPrintable()) {
                    menu.EnterText(e.Character.ToString());
                }
            }
        }

        private void PasteThread() {

        }

        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            if (e.KeyPressed == this.Config.ModConfigKey && Game1.activeClickableMenu == null) {
                Game1.showGlobalMessage("Mod config key pressed");
                int x = (int) (Game1.viewport.Width * 0.25);
                int y = (int) (Game1.viewport.Height * 0.125);
                int width = (int) (Game1.viewport.Width * 0.5);
                int height = (int) (Game1.viewport.Height * 0.75);
                Game1.activeClickableMenu = new ModConfigMenu(x, y, width, height);
            }
        }
    }
}
