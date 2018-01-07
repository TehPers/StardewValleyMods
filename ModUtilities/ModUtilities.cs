using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
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

        public Assembly WinForms { get; private set; }
        public Type Clipboard { get; private set; }
        public Func<string> GetClipboard { get; private set; }

        public ModUtilities() {
            ModUtilities.Instance = this;
        }

        public override void Entry(IModHelper helper) {
            this.Config = helper.ReadConfig<ConfigMain>();

            // Try to import winforms
            this.GetClipboard = () => null;
            try {
                this.WinForms = Assembly.Load("System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                this.Clipboard = this.WinForms.GetType("System.Windows.Forms.Clipboard");
                MethodInfo[] clipboardMethods = this.Clipboard.GetMethods();

                // Clipboard.GetText()
                MethodInfo getClipboardInfo = clipboardMethods.FirstOrDefault(m => m.IsStatic && m.Name == "GetText" && m.GetParameters().Length == 0);
                if (getClipboardInfo != null) {
                    Func<string> getClipboardRaw = (Func<string>) Delegate.CreateDelegate(typeof(Func<string>), getClipboardInfo);

                    // Clipboard.GetText() must be called from a STA thread
                    this.GetClipboard = () => {
                        string clipboardText = "";
                        Thread clipboardThread = new Thread(() => clipboardText = getClipboardRaw());
                        clipboardThread.SetApartmentState(ApartmentState.STA);
                        clipboardThread.Start();
                        clipboardThread.Join();
                        return clipboardText;
                    };
                }
            } catch {
                this.Monitor.Log("WinForms assembly could not be loaded. Clipboard actions will be disabled. (This is normal for Linux/Mac)", LogLevel.Warn);
            }

            KeyboardInput.CharEntered += this.CharEntered;
            ControlEvents.KeyPressed += this.KeyPressed;
        }

        private void CharEntered(object sender, CharacterEventArgs e) {
            if (Game1.activeClickableMenu is ModMenu menu) {
                // Check if ctrl-v was pressed
                if (e.Character == '\u0016') {
                    // Try to get the clipboard
                    string clipboard = this.GetClipboard();
                    if (clipboard != null) {
                        menu.EnterText(clipboard);
                    }
                }

                // Check if character is printable (https://stackoverflow.com/a/45928048/8430206)
                if (e.Character.IsPrintable()) {
                    menu.EnterText(e.Character.ToString());
                }
            }
        }

        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            if (e.KeyPressed == this.Config.ModConfigKey && Game1.activeClickableMenu == null) {
                this.ShowModConfig(this.Config);
            }
        }

        /// <summary>Sets the active clickable menu to a config editor menu that wraps the given config</summary>
        /// <param name="config">The config for the menu to wrap</param>
        public void ShowModConfig(IAutoConfig config) {
            ModConfigMenu configMenu = new ModConfigMenu();
            config.BuildConfig(configMenu);
            Game1.activeClickableMenu = configMenu;
        }
    }
}
