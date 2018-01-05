using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Helpers;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;
using Rectangle2 = Microsoft.Xna.Framework.Rectangle;

namespace ModUtilities.Menus.Components {
    public class ScrollbarComponent : Component {
        public override bool FocusOnClick { get; } = true;

        public int Value { get; set; }
        public int Maximum { get; set; } = 100;
        public bool Horizontal { get; set; }
        public bool Vertical { get => !this.Horizontal; set => this.Horizontal = !value; }
        public int ButtonThickness => (this.Horizontal ? this.Size.Height : this.Size.Width) * OptionsSlider.sliderButtonRect.Height / OptionsSlider.sliderButtonRect.Width;

        /// <summary>The number of values incremented/decremented per notch</summary>
        public int ScrollSpeed { get; set; } = 1;

        private int _lastValue;

        public ScrollbarComponent(bool horizontal) {
            this.Horizontal = horizontal;
            this.Size = horizontal ? new Size(this.Size.Width, OptionsSlider.sliderButtonRect.Width * Game1.pixelZoom) : new Size(OptionsSlider.sliderButtonRect.Width * Game1.pixelZoom, this.Size.Height);

            this._lastValue = this.Value;
        }

        protected override void OnDraw(SpriteBatch b) {
            Rectangle bounds = this.AbsoluteBounds;

            if (this._lastValue != this.Value) {
                this.OnValueChanged(this.Value - this._lastValue);
                this._lastValue = this.Value;
            }

            float trackDepth = this.GetGlobalDepth(0);
            float buttonDepth = this.GetGlobalDepth(1);
            if (this.Horizontal) {
                // Draw track
                b.DrawTextureBox(bounds.ToXnaRectangle(), OptionsSlider.sliderBGSource, trackDepth, Game1.pixelZoom);

                // Draw button
                if (this.Maximum > 0) {
                    Size buttonSize = new Size(this.ButtonThickness, bounds.Height);
                    Location buttonLoc = new Location((int) (bounds.X + (bounds.Width - buttonSize.Width) * ((float) this.Value / this.Maximum).Clamp(0, 1)), bounds.Y);
                    b.Draw(Game1.mouseCursors, new Rectangle2(buttonLoc.X + buttonSize.Width, buttonLoc.Y, buttonSize.Height, buttonSize.Width), OptionsSlider.sliderButtonRect, Color.White, (float) (Math.PI / 2F), Vector2.Zero, SpriteEffects.None, buttonDepth);
                }
            } else {
                // Draw track
                b.DrawTextureBox(bounds.ToXnaRectangle(), OptionsSlider.sliderBGSource, trackDepth, Game1.pixelZoom);

                // Draw button
                if (this.Maximum > 0) {
                    Size buttonSize = new Size(bounds.Width, this.ButtonThickness);
                    Location buttonLoc = new Location(bounds.X, (int) (bounds.Y + (bounds.Height - buttonSize.Height) * ((float) this.Value / this.Maximum).Clamp(0, 1)));
                    b.Draw(Game1.mouseCursors, new Rectangle2(buttonLoc.X, buttonLoc.Y, buttonSize.Width, buttonSize.Height), OptionsSlider.sliderButtonRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, buttonDepth);
                }
            }
        }

        protected override bool OnLeftClick(Location mousePos) {
            return base.OnLeftClick(mousePos) && this.OnDrag(mousePos);
        }

        protected override bool OnDrag(Location mousePos) {
            Rectangle bounds = this.AbsoluteBounds;
            int sliderThickness = this.ButtonThickness;
            float percent;
            if (this.Horizontal) {
                percent = ((mousePos.X - bounds.X - sliderThickness / 2F) / (bounds.Width - sliderThickness)).Clamp(0F, 1F);
            } else {
                percent = ((mousePos.Y - bounds.Y - sliderThickness / 2F) / (bounds.Height - sliderThickness)).Clamp(0F, 1F);
            }
            this.Value = (int) Math.Round(percent * this.Maximum);
            return true;
        }

        protected override bool OnKeyPressed(Keys key) {
            if (this.Horizontal) {
                if (key == Keys.Right) {
                    this.Value = Math.Max(this.Value - 1, 0);
                } else if (key == Keys.Left) {
                    this.Value = Math.Min(this.Value + 1, this.Maximum);
                }
            } else {
                if (key == Keys.Up) {
                    this.Value = Math.Max(this.Value - 1, 0);
                } else if (key == Keys.Down) {
                    this.Value = Math.Min(this.Value + 1, this.Maximum);
                }
            }

            return base.OnKeyPressed(key);
        }

        protected override bool OnScroll(Location mousePos, int direction) {
            this.Value = (int) Math.Round(this.Value - (float) direction / ModUtilities.Instance.Config.ScrollSpeed * this.ScrollSpeed).Clamp(0, this.Maximum);
            return true;
        }

        public event Action<int> ValueChanged;

        protected virtual void OnValueChanged(int delta) {
            this.ValueChanged?.Invoke(delta);
        }
    }
}
