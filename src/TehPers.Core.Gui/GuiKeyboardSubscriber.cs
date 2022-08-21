using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.SDKs;
using System;
using System.Reflection;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui;

internal class GuiKeyboardSubscriber : IKeyboardSubscriber
{
    private static readonly Func<SDKHelper> getSdkHelper =
        typeof(Program).GetProperty("sdk", BindingFlags.Static | BindingFlags.NonPublic) !
            .GetMethod!.CreateDelegate<Func<SDKHelper>>();

    private readonly ManagedMenu menu;

    private bool selected;

    /// <inheritdoc />
    public bool Selected
    {
        get => this.selected;
        set
        {
            if (this.selected == value)
            {
                return;
            }

            this.selected = value;
            if (this.selected)
            {
                Game1.keyboardDispatcher.Subscriber = this;
                return;
            }

            if (GuiKeyboardSubscriber.getSdkHelper() is SteamHelper {active: true} steamHelper)
            {
                steamHelper.CancelKeyboard();
            }

            if (Game1.keyboardDispatcher.Subscriber != this)
            {
                return;
            }

            Game1.keyboardDispatcher.Subscriber = null;
        }
    }

    /// <summary>
    /// Creates a new keyboard subscriber that sends events to a GUI.
    /// </summary>
    /// <param name="menu"></param>
    public GuiKeyboardSubscriber(ManagedMenu menu)
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
