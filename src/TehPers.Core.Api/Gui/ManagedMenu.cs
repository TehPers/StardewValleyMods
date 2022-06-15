using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A fully managed menu that wraps an <see cref="IGuiComponent{TState}"/>.
    /// </summary>
    public class ManagedMenu<TState> : IClickableMenu
    {
        private readonly Texture2D whitePixel;

        private readonly IGuiComponent<TState> root;
        private TState rootState;

        /// <summary>
        /// Creates a <see cref="ManagedMenu{TState}"/> from an <see cref="IGuiComponent{TState}"/>.
        /// </summary>
        /// <param name="root">The root component.</param>
        public ManagedMenu(IGuiComponent<TState> root)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));

            this.whitePixel = new(Game1.graphics.GraphicsDevice, 1, 1);
            this.whitePixel.SetData(new[] {Color.White});

            // Initialize root state
            var bounds = ManagedMenu<TState>.GetBounds(this.root);
            this.rootState = this.root.Initialize(bounds);
        }

        private void OnGuiEvent(GuiEvent e)
        {
            if (!this.root.Update(e, this.rootState, out var newState))
            {
                return;
            }

            this.rootState = newState;
            this.RecalculateLayouts();
        }

        private static Rectangle GetBounds(IGuiComponent<TState> component)
        {
            var constraints = component.GetConstraints();
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

        private void RecalculateLayouts()
        {
            // Resize from constraints
            var newBounds = ManagedMenu<TState>.GetBounds(this.root);
            this.rootState = this.root.Reposition(this.rootState, newBounds);
            this.SetBounds(newBounds);
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

        private void DrawRect(SpriteBatch batch, Rectangle rect, Color color)
        {
            batch.Draw(this.whitePixel, new Rectangle(rect.Left, rect.Top, rect.Width, 2), color);
            batch.Draw(
                this.whitePixel,
                new Rectangle(rect.Left, rect.Bottom - 2, rect.Width, 2),
                color
            );
            batch.Draw(this.whitePixel, new Rectangle(rect.Left, rect.Top, 2, rect.Height), color);
            batch.Draw(
                this.whitePixel,
                new Rectangle(rect.Right - 2, rect.Top, 2, rect.Height),
                color
            );
        }

        /// <inheritdoc />
        public override void draw(SpriteBatch batch)
        {
            // Draw components
            this.root.Draw(batch, this.rootState);

            // foreach (var layout in this.layouts)
            // {
            //     var color = new Color(0f, 0f, 1f, 0.1f);
            // }

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
