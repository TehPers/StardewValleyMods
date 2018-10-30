using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.Core.Enums;
using TehPers.Core.Gui.Units.Base;
using TehPers.Core.Gui.Units.SDV;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Gui.Components {
    public class MenuComponent : ResizableGuiComponent {

        /// <inheritdoc />
        protected override ResponsiveGuiUnit PaddingTop => new ResponsiveGuiUnit(new PixelUnits(Game1.tileSize));

        /// <inheritdoc />
        protected override ResponsiveGuiUnit PaddingBottom => new ResponsiveGuiUnit(new PixelUnits(Game1.tileSize));

        /// <inheritdoc />
        protected override ResponsiveGuiUnit PaddingLeft => new ResponsiveGuiUnit(new PixelUnits(Game1.tileSize));

        /// <inheritdoc />
        protected override ResponsiveGuiUnit PaddingRight => new ResponsiveGuiUnit(new PixelUnits(Game1.tileSize));

        public MenuComponent() { }
        public MenuComponent(IGuiComponent parent) : base(parent) { }

        /// <inheritdoc />
        protected override void DrawSelf(SpriteBatch batch, ResolvedVector2 resolvedLocation, ResolvedVector2 resolvedSize) {
            batch.DrawMenuBox(new Rectangle((int) resolvedLocation.X, (int) resolvedLocation.Y, (int) resolvedSize.X, (int) resolvedSize.Y), this.GetGlobalDepth(0));
        }

        /// <inheritdoc />
        public override bool Click(ResolvedVector2 resolvedLocation, ResolvedVector2 resolvedSize, ResolvedVector2 relativeLocation, MouseButtons buttons) {
            return false;
        }

        /// <inheritdoc cref="GuiComponent.AddChild" />
        public new void AddChild(IGuiComponent child) {
            base.AddChild(child);
        }

        /// <inheritdoc cref="GuiComponent.RemoveChild" />
        public new bool RemoveChild(IGuiComponent child) {
            return base.RemoveChild(child);
        }
    }
}
