using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace ModUtilities.Menus.Components {
    public class LabelComponent : Component {
        public string Text { get; set; } = "";
        public Color Color { get; set; } = Game1.textColor;
        public float Rotation { get; set; } = 0;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; } = 1;
        public SpriteFont Font { get; set; } = Game1.smallFont;

        private Vector2 _textScale = Vector2.One;

        /// <summary>Sets the text and updates the size of the label</summary>
        /// <param name="text">The new text for the label</param>
        /// <returns>This label</returns>
        public LabelComponent SetText(string text) {
            this.Text = text;

            // Update control size
            Size newSize;
            if (this._textScale == Vector2.Zero) {
                newSize = Size.Zero;
            } else {
                Vector2 v = this.Font.MeasureString(text);
                newSize = new Size((int) (v.X * this._textScale.X), (int) (v.Y * this._textScale.Y));
            }

            this.Bounds = new Rectangle(this.Bounds.Location, newSize);
            return this;
        }

        /// <summary>Sets the size of the text</summary>
        /// <param name="height">The new height of the text</param>
        /// <returns>This label</returns>
        public LabelComponent SetSize(int height) {
            Vector2 v = this.Font.MeasureString(this.Text);
            float scale = height / v.Y;
            this._textScale = new Vector2(scale, scale);
            return this;
        }

        /// <summary>Sets the size of the text</summary>
        /// <param name="size">The new size of the text</param>
        /// <returns>This label</returns>
        public LabelComponent SetSize(Size size) {
            Vector2 v = this.Font.MeasureString(this.Text);
            this._textScale = new Vector2(size.Width / v.X, size.Height / v.Y);
            return this;
        }

        /// <summary>Sets the scale of the text</summary>
        /// <param name="scale">The new scale of the text</param>
        /// <returns>This label</returns>
        public LabelComponent SetScale(float scale) => this.SetScale(new Vector2(scale, scale));

        /// <summary>Sets the scale of the text</summary>
        /// <param name="scale">The new scale of the text</param>
        /// <returns>This label</returns>
        public LabelComponent SetScale(Vector2 scale) {
            this.Size = new Size((int) (this.Size.Width * scale.X / this._textScale.X), (int) (this.Size.Height * scale.Y / this._textScale.Y));
            this._textScale = scale;
            return this;
        }

        protected override void OnDraw(SpriteBatch b) {
            Location loc = this.AbsoluteLocation;
            b.DrawString(this.Font, this.Text, new Vector2(loc.X, loc.Y), this.Color, this.Rotation, this.Origin, this._textScale, this.Effects, this.LayerDepth);
        }
    }
}
