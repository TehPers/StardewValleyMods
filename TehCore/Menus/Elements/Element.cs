using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehCore.Menus.Elements {
    public abstract class Element {
        public OutsideSize Margin { get; set; } = OutsideSize.Zero;

        public bool Visible { get; set; } = true;

        public void Draw(SpriteBatch batch) {

        }

        protected virtual void PerformDraw(SpriteBatch batch) {

        }

        protected virtual void WithClipping(SpriteBatch batch, Rectangle clippingRectangle) {

        }
    }
}
