using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.SDKs;
using System;
using System.Reflection;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Api.Guis;

internal class GuiKeyboardSubscriber<TMessage> : IKeyboardSubscriber
{
    private static readonly Func<SDKHelper> getSdkHelper =
        typeof(Program).GetProperty("sdk", BindingFlags.Static | BindingFlags.NonPublic)!.GetMethod!
            .CreateDelegate<Func<SDKHelper>>();

    private readonly ManagedGuiMenu<TMessage> menu;

    private bool selected;

    /// <inheritdoc />
    public bool Selected
    {
        get => this.selected;
        set
        {
            switch (this.selected, value)
            {
                case (false, true):
                    Game1.keyboardDispatcher.Subscriber = this;
                    break;
                case (true, false) when Game1.keyboardDispatcher.Subscriber == this:
                    if (GuiKeyboardSubscriber<TMessage>.getSdkHelper() is SteamHelper
                        {
                            active: true
                        } steamHelper)
                    {
                        steamHelper.CancelKeyboard();
                    }

                    Game1.keyboardDispatcher.Subscriber = null;
                    break;
            }

            this.selected = value;
        }
    }

    /// <summary>
    /// Creates a new keyboard subscriber that sends events to a GUI.
    /// </summary>
    /// <param name="menu"></param>
    public GuiKeyboardSubscriber(ManagedGuiMenu<TMessage> menu)
    {
        this.menu = menu;
    }

    /// <inheritdoc />
    public void RecieveTextInput(char inputChar)
    {
        this.menu.ReceiveEvent(new GuiEvent.TextInput(inputChar.ToString()));
    }

    /// <inheritdoc />
    public void RecieveTextInput(string text)
    {
        this.menu.ReceiveEvent(new GuiEvent.TextInput(text));
    }

    /// <inheritdoc />
    public void RecieveCommandInput(char command)
    {
        // Handled by a separate event
    }

    /// <inheritdoc />
    public void RecieveSpecialInput(Keys key)
    {
        // Handled by a separate event
    }
}
