using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace TehPers.Core.Api.Gui
{
    internal class SimpleManagedMenu : ManagedMenu
    {
        private readonly IGuiComponent<Unit> root;

        public SimpleManagedMenu(IGuiComponent<Unit> root)
        {
            this.root = root;
        }

        protected override IGuiComponent<Unit> CreateRoot()
        {
            return this.root;
        }
    }

    /// <summary>
    /// A fully managed menu that wraps an <see cref="IGuiComponent{TState}"/>.
    /// </summary>
    public abstract class ManagedMenu : IClickableMenu
    {
        /// <summary>
        /// Creates the root component.
        /// </summary>
        /// <returns>The root component.</returns>
        protected abstract IGuiComponent<Unit> CreateRoot();

        private void OnGuiEvent(GuiEvent e)
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
        protected virtual Rectangle GetBounds(IGuiComponent<Unit> root)
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
            this.OnGuiEvent(new GuiEvent.UpdateTick(time));
            base.update(time);
        }

        /// <inheritdoc />
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.OnGuiEvent(new GuiEvent.ReceiveClick(new(x, y), ClickType.Left));
            base.receiveLeftClick(x, y, playSound);
        }

        /// <inheritdoc />
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            this.OnGuiEvent(new GuiEvent.ReceiveClick(new(x, y), ClickType.Right));
            base.receiveRightClick(x, y, playSound);
        }


        /// <inheritdoc />
        public override void draw(SpriteBatch batch)
        {
            this.OnGuiEvent(new GuiEvent.Draw(batch));
            this.DrawCursor(batch);
        }

        /// <summary>
        /// Draws the mouse cursor.
        /// </summary>
        /// <param name="batch">The sprite batch to draw with.</param>
        protected virtual void DrawCursor(SpriteBatch batch)
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
    }
}
