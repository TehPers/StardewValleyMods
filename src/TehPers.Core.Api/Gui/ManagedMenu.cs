using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A fully managed menu that wraps an <see cref="IGuiComponent"/>.
    /// </summary>
    public class ManagedMenu : IClickableMenu
    {
        private readonly Texture2D whitePixel;

        private IGuiComponent root;
        private RootGuiState rootState;

        private readonly List<ComponentLayout> layouts;
        private ImmutableDictionary<IGuiComponent, Rectangle> componentBounds;

        /// <summary>
        /// Creates a <see cref="ManagedMenu"/> from an <see cref="IGuiComponent"/>.
        /// </summary>
        /// <param name="root">The root component.</param>
        public ManagedMenu(IGuiComponent root)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
            this.rootState = new();
            this.layouts = new();
            this.componentBounds = ImmutableDictionary<IGuiComponent, Rectangle>.Empty;

            this.whitePixel = new(Game1.graphics.GraphicsDevice, 1, 1);
            this.whitePixel.SetData(new[] {Color.White});

            this.RecalculateLayouts();
        }

        private void OnGuiEvent(GuiEvent e)
        {
            if (!this.root.Update(e, this.componentBounds, out var newComponent))
            {
                return;
            }

            this.root = newComponent;
            this.RecalculateLayouts();
        }

        private void RecalculateLayouts()
        {
            // Resize from constraints
            var constraints = this.root.Constraints;
            this.width = (int)Math.Ceiling(constraints.MinSize.Width);
            this.height = (int)Math.Ceiling(constraints.MinSize.Height);
            this.xPositionOnScreen = (Game1.uiViewport.Width - this.width) / 2;
            this.yPositionOnScreen = (Game1.uiViewport.Height - this.height) / 2;
            var bounds = new Rectangle(
                this.xPositionOnScreen,
                this.yPositionOnScreen,
                this.width,
                this.height
            );

            // Calculate layouts based on computed size
            this.layouts.Clear();
            this.root.CalculateLayouts(bounds, this.layouts);
            var componentBoundsBuilder =
                ImmutableDictionary.CreateBuilder<IGuiComponent, Rectangle>(
                    ReferenceEqualityComparer.Instance
                );
            foreach (var layout in this.layouts)
            {
                componentBoundsBuilder[layout.Component] = layout.Bounds;
            }

            this.componentBounds = componentBoundsBuilder.ToImmutable();
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
            // Draw components
            foreach (var layout in this.layouts)
            {
                layout.Draw(batch);
            }

            foreach (var layout in this.layouts)
            {
                var color = new Color(0f, 0f, 1f, 0.1f);
                batch.Draw(
                    this.whitePixel,
                    new Rectangle(layout.Bounds.Left, layout.Bounds.Top, layout.Bounds.Width, 2),
                    color
                );
                batch.Draw(
                    this.whitePixel,
                    new Rectangle(
                        layout.Bounds.Left,
                        layout.Bounds.Bottom - 2,
                        layout.Bounds.Width,
                        2
                    ),
                    color
                );
                batch.Draw(
                    this.whitePixel,
                    new Rectangle(layout.Bounds.Left, layout.Bounds.Top, 2, layout.Bounds.Height),
                    color
                );
                batch.Draw(
                    this.whitePixel,
                    new Rectangle(
                        layout.Bounds.Right - 2,
                        layout.Bounds.Top,
                        2,
                        layout.Bounds.Height
                    ),
                    color
                );
            }

            // Draw cursor
            // var cursorConstraints = this.rootState.CursorComponent.Constraints;
            // var cursorWidth = (int)Math.Ceiling(cursorConstraints.MinSize.Width);
            // var cursorHeight = (int)Math.Ceiling(cursorConstraints.MinSize.Height);
            // this.rootState.CursorComponent.Draw(
            //     batch,
            //     new(Game1.getOldMouseX(), Game1.getOldMouseY(), cursorWidth, cursorHeight)
            // );

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
