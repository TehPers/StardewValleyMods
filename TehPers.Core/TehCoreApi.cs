using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.Core.Events;
using TehPers.Core.Helpers;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Menus;

namespace TehPers.Core {
    public class TehCoreApi {
        #region Static
        private static readonly Dictionary<IMod, TehCoreApi> _apis = new Dictionary<IMod, TehCoreApi>();

        public static InputHelper InputHelper { get; } = new InputHelper();

        static TehCoreApi() {
            KeyboardInput.CharEntered += TehCoreApi.CharEntered;
            GameEvents.UpdateTick += TehCoreApi.UpdateTick;
            TehCoreApi.InputHelper.RepeatedKeystroke += TehCoreApi.RepeatedKeystroke;
        }

        public static TehCoreApi Create(IMod owner) {
            if (!TehCoreApi._apis.TryGetValue(owner, out TehCoreApi api)) {
                api = new TehCoreApi(owner);
                TehCoreApi._apis[owner] = api;
            }

            return api;
        }
        #endregion

        public IMod Owner { get; }
        public JsonHelper JsonHelper { get; }
        public Action<string, LogLevel> Log { get; set; }

        private TehCoreApi(IMod owner) {
            this.Owner = owner;
            this.JsonHelper = new JsonHelper(this);

            this.Log = (message, level) => owner.Monitor.Log($"[TehPers.Core] {message}", level);
        }

        #region Events
        private static void CharEntered(object sender, CharacterEventArgs e) {
            if (!(Game1.activeClickableMenu is Menu menu))
                return;

            // Check if ctrl-v was pressed
            if (e.Character == '\u0016') {
                // Try to get the clipboard
                string clipboard = TehCoreApi.InputHelper.GetClipboardText();
                if (clipboard != null) {
                    menu.EnterText(clipboard);
                }
            }

            // Check if character is printable
            if (e.Character.IsPrintable()) {
                menu.EnterText(e.Character.ToString());
            }
        }

        private static void UpdateTick(object sender, EventArgs eventArgs) {
            if (!(Game1.activeClickableMenu is Menu menu))
                return;

            // Check if the chat is open and shouldn't be with this menu
            if (menu.MainElement.GetFocusedElement()?.BlockChatbox == true && Game1.chatBox?.isActive() == true) {
                // Close the chat
                Game1.chatBox.clickAway();
            }
        }

        private static void RepeatedKeystroke(object o, EventArgsKeyRepeated e) {
            if (!(Game1.activeClickableMenu is Menu menu) || menu.MainElement.GetFocusedElement()?.RepeatKeystrokes != true)
                return;

            // Send repeated keystroke to the menu
            menu.receiveKeyPress(e.RepeatedKey);
        }
        #endregion
    }
}
