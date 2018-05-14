using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using TehPers.Core.Configs;
using TehPers.Core.Helpers;
using TehPers.Core.Helpers.EventHandlers;
using TehPers.Core.Menus;

namespace TehPers.Core {
    public class ModCore : Mod {
        public static ModCore Instance { get; private set; }

        internal ConfigMain MainConfig { get; private set; }

        public JsonHelper JsonHelper { get; } = new JsonHelper();
        public SaveHelper SaveHelper { get; } = new SaveHelper();
        public InputHelper InputHelper { get; } = new InputHelper();

        public Texture2D WhitePixel { get; private set; }

        public override void Entry(IModHelper helper) {
            ModCore.Instance = this;

            // White texture
            this.WhitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.WhitePixel.SetData(new[] { Color.White });

            // Json writer
            this.JsonHelper.AddSmapiConverters(helper);

            // Load config
            this.MainConfig = this.JsonHelper.ReadOrCreate<ConfigMain>("config.json", helper);

            // Save events
            SaveEvents.BeforeSave += (sender, e) => this.SaveHelper.SerializeCustomItems();
            SaveEvents.AfterSave += (sender, e) => this.SaveHelper.DeserializeCustomItems();
            SaveEvents.AfterLoad += (sender, e) => this.SaveHelper.DeserializeCustomItems();

            // Input events
            KeyboardInput.CharEntered += this.CharEntered;
            GameEvents.UpdateTick += this.UpdateTick;
            this.InputHelper.RepeatedKeystroke += this.RepeatedKeystroke;
        }

        #region Events
        private void CharEntered(object sender, CharacterEventArgs e) {
            if (!(Game1.activeClickableMenu is Menu menu))
                return;

            // Check if ctrl-v was pressed
            if (e.Character == '\u0016') {
                // Try to get the clipboard
                string clipboard = this.InputHelper.GetClipboardText();
                if (clipboard != null) {
                    menu.EnterText(clipboard);
                }
            }

            // Check if character is printable
            if (e.Character.IsPrintable()) {
                menu.EnterText(e.Character.ToString());
            }
        }

        private void UpdateTick(object o, EventArgs eventArgs) {
            if (!(Game1.activeClickableMenu is Menu menu))
                return;

            // Check if the chat is open and shouldn't be with this menu
            if (menu.MainElement.GetFocusedElement()?.BlockChatbox == true && Game1.chatBox?.isActive() == true) {
                // Close the chat
                Game1.chatBox.clickAway();
            }
        }

        private void RepeatedKeystroke(object o, EventArgsKeyRepeated e) {
            if (Game1.activeClickableMenu is Menu menu && menu.MainElement.GetFocusedElement()?.RepeatKeystrokes == true) {
                if (e.Character != null) {
                    //menu.EnterText(e.Character.ToString());
                }

                menu.receiveKeyPress(e.RepeatedKey);
            }
        }
        #endregion

        #region Menus
        public void ShowMenu(IClickableMenu menu) => Game1.activeClickableMenu = menu;

        public IClickableMenu GetActiveMenu() => Game1.activeClickableMenu;
        #endregion
    }
}
