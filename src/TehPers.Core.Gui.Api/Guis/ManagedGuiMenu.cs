using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Api.Guis;

/// <summary>
/// A fully managed menu that wraps an <see cref="IGuiComponent"/>.
/// </summary>
internal class ManagedGuiMenu<TMessage> : IClickableMenu, IDisposable
{
    private readonly IGui<TMessage> gui;
    private readonly GuiContext ctx;
    private readonly IModHelper helper;
    private IKeyboardSubscriber keyboardSubscriber;

    /// <summary>
    /// Creates a new managed menu.
    /// </summary>
    /// <param name="gui">The GUI to display.</param>
    /// <param name="ui">The GUI builder.</param>
    /// <param name="helper">The mod helper to use.</param>
    public ManagedGuiMenu(IGui<TMessage> gui, IGuiBuilder ui, IModHelper helper)
    {
        this.gui = gui;
        this.ctx = new(ui);
        this.helper = helper ?? throw new ArgumentNullException(nameof(helper));

        // Create keyboard subscriber
        this.keyboardSubscriber = new GuiKeyboardSubscriber<TMessage>(this);

        // Add event handlers for additional types of events
        this.helper.Events.Input.ButtonReleased += this.OnButtonReleased;
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);

        // Deselect keyboard subscriber
        if (this.keyboardSubscriber is not null)
        {
            this.keyboardSubscriber.Selected = false;
        }

        // Remove event handlers
        this.helper.Events.Input.ButtonReleased -= this.OnButtonReleased;
    }

    private IGuiComponent CreateRoot()
    {
        // Send enqueued messages
        while (this.ctx.messages.TryDequeue(out var message))
        {
            this.gui.Update(message);
        }

        return this.gui.View(this.ctx);
    }

    /// <summary>
    /// Handles a GUI event.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    public virtual void ReceiveEvent(IGuiEvent e)
    {
        var root = this.CreateRoot();
        var bounds = this.GetBounds(root);
        this.SetBounds(bounds);
        root.Handle(e, bounds);
    }

    /// <summary>
    /// Caclulates the bounds of the menu.
    /// </summary>
    /// <param name="root">The root component.</param>
    /// <returns>The menu's bounds.</returns>
    protected virtual Rectangle GetBounds(IGuiComponent root)
    {
        var constraints = root.GetConstraints();
        var width = (int)Math.Ceiling(constraints.MinSize.Width);
        var height = (int)Math.Ceiling(constraints.MinSize.Height);
        var x = (Game1.uiViewport.Width - width) / 2;
        var y = (Game1.uiViewport.Height - height) / 2;
        return new(x, y, width, height);
    }

    private void SetBounds(Rectangle bounds)
    {
        this.xPositionOnScreen = bounds.X;
        this.yPositionOnScreen = bounds.Y;
        this.width = bounds.Width;
        this.height = bounds.Height;
    }

    /// <inheritdoc />
    public override void update(GameTime time)
    {
        this.ReceiveEvent(new GuiEvent.UpdateTick(time));
        base.update(time);
    }

    /// <inheritdoc/>
    public override void performHoverAction(int x, int y)
    {
        // Capture keyboard input if needed
        this.keyboardSubscriber.Selected = this.gui.CaptureInput;

        this.ReceiveEvent(new GuiEvent.Hover(new(x, y)));
        base.performHoverAction(x, y);
    }

    /// <inheritdoc />
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        var handled = false;
        var @event = new GuiEvent.ReceiveClick(new(x, y), ClickType.Left)
        {
            GetIsHandled = () => handled,
            SetHandled = () => handled = true,
        };
        this.ReceiveEvent(@event);
        base.receiveLeftClick(x, y, playSound);
    }

    /// <inheritdoc />
    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        this.ReceiveEvent(new GuiEvent.ReceiveClick(new(x, y), ClickType.Right));
        base.receiveRightClick(x, y, playSound);
    }

    /// <inheritdoc />
    public override void receiveScrollWheelAction(int direction)
    {
        base.receiveScrollWheelAction(direction);
        this.ReceiveEvent(new GuiEvent.Scroll(direction));
    }

    /// <inheritdoc />
    public override void receiveKeyPress(Keys key)
    {
        this.ReceiveEvent(new GuiEvent.KeyboardInput(key));

        // Don't exit if this is capturing keyboard input
        // TODO: configurable?
        var isExitButton = Game1.options.doesInputListContain(Game1.options.menuButton, key);
        if (!isExitButton || this.keyboardSubscriber is not {Selected: true})
        {
            base.receiveKeyPress(key);
        }
    }

    /// <inheritdoc />
    public override void receiveGamePadButton(Buttons b)
    {
        base.receiveGamePadButton(b);
        this.ReceiveEvent(new GuiEvent.GamePadInput(b));
    }

    private void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
    {
        if (e.IsSuppressed())
        {
            return;
        }

        switch (e.Button)
        {
            case SButton.MouseMiddle:
                this.ReceiveEvent(
                    new GuiEvent.ReceiveClick(Game1.getMousePosition(), ClickType.Middle)
                );
                break;
        }
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch batch)
    {
        this.ReceiveEvent(new GuiEvent.Draw(batch));
        this.DrawCursor(batch);
    }

    /// <summary>
    /// Draws the mouse cursor.
    /// </summary>
    /// <param name="batch">The sprite batch to draw with.</param>
    private void DrawCursor(SpriteBatch batch)
    {
        batch.Draw(
            Game1.mouseCursors,
            new(Game1.getOldMouseX(), Game1.getOldMouseY()),
            Game1.getSourceRectForStandardTileSheet(
                Game1.mouseCursors,
                Game1.options.gamepadControls ? 44 : 0,
                16,
                16
            ),
            Color.White,
            0f,
            Vector2.Zero,
            Game1.pixelZoom + Game1.dialogueButtonScale / 150f,
            SpriteEffects.None,
            1f
        );
    }

    private class GuiContext : IGuiContext<TMessage>
    {
        private readonly IGuiBuilder inner;
        public readonly Queue<TMessage> messages;

        public GuiContext(IGuiBuilder inner)
        {
            this.inner = inner;
            this.messages = new();
        }

        /// <inheritdoc />
        public void Update(TMessage message)
        {
            this.messages.Enqueue(message);
        }

        /// <inheritdoc />
        public bool TryAddExtension(string key, object extension)
        {
            return this.inner.TryAddExtension(key, extension);
        }

        /// <inheritdoc />
        public object? TryGetExtension(string key)
        {
            return this.inner.TryGetExtension(key);
        }
    }
}
