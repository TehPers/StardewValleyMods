using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ModUtilities.Menus.Components2 {
    public class LabelComponent2 : Component2 {
        public string Text { get; set; } = "";
        public Color Color { get; set; } = Game1.textColor;
        public float Rotation { get; set; } = 0;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; } = 0.8f;
        public SpriteFont Font { get; set; } = Game1.smallFont;

        public override RelativeSize Size {
            get {
                Vector2 textSize = this.Font.MeasureString(this.Text);
                return new RelativeSize(0, 0, (int) (textSize.X * this._textScale.X), (int) (textSize.Y * textSize.Y));
            }
            set {
                Vector2 textSize = this.Font.MeasureString(this.Text);
                Rectangle absoluteRect = this.GetAbsoluteRectangle(new RelativeRectangle(RelativeLocation.BottomLeft, value));
                this._textScale = new Vector2(absoluteRect.Width / textSize.X, absoluteRect.Height / textSize.Y);
            }
        }

        private Vector2 _textScale = Vector2.One;
        
        /// <summary>Sets the scale of the text</summary>
        /// <param name="scale">The new scale of the text</param>
        /// <returns>This label</returns>
        public LabelComponent2 SetScale(float scale) => this.SetScale(new Vector2(scale, scale));

        /// <summary>Sets the scale of the text</summary>
        /// <param name="scale">The new scale of the text</param>
        /// <returns>This label</returns>
        public LabelComponent2 SetScale(Vector2 scale) {
            this._textScale = scale;
            return this;
        }

        protected override void OnDraw(SpriteBatch b) {
            Point loc = this.AbsoluteLocation;
            b.DrawString(this.Font, this.Text, new Vector2(loc.X, loc.Y), this.Color, this.Rotation, this.Origin, this._textScale, this.Effects, this.LayerDepth);
        }
    }
}
