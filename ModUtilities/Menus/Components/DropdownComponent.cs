using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    public class DropdownComponent : Component {
        public override bool FocusOnClick { get; } = true;

        public string[] Options { get; set; }
        public int Selected {
            get {
                this._selected = Math.Min(Math.Max(this._selected, 0), this.Options.Length);
                return this._selected;
            }
            set => this._selected = value;
        }
        public string SelectedText => this.Selected < this.Options.Length ? this.Options[this.Selected] : null;
        public SpriteFont Font { get; set; } = Game1.smallFont;
        public Color Color { get; set; } = Game1.textColor;
        public bool Opened => this._dropComponent.Visible && this._dropComponent.Enabled;

        private int _selected;
        private readonly ScrollableComponent _dropComponent;

        public DropdownComponent() : this(new string[0]) { }

        public DropdownComponent(string[] options) {
            this.Options = options;
            this._dropComponent = new ScrollableComponent()
                .Chain(c => c.Enabled = c.Visible = false)
                .Chain(c => c.ScrollbarPadding = 0);
            //this._dropComponent.DrawBackground += batch => batch.DrawMenuBox(this._dropComponent.AbsoluteBounds.ToXnaRectangle(), this._dropComponent.GetGlobalDepth(0));
            this.AddChild(this._dropComponent);
        }

        protected virtual Rectangle2 GetTextBounds() {
            Rectangle bounds = this.AbsoluteBounds;
            Rectangle2 buttonArea = this.GetButtonBounds();
            int xOffset = Game1.pixelZoom;
            int yOffset = Game1.pixelZoom * 2;

            return new Rectangle2(bounds.X + xOffset, bounds.Y + yOffset, bounds.Width - buttonArea.Width - xOffset * 2, bounds.Height - yOffset * 2);
        }

        protected virtual Rectangle2 GetButtonBounds() {
            Rectangle bounds = this.AbsoluteBounds;
            int height = bounds.Height;
            int width = height * OptionsDropDown.dropDownButtonSource.Width / OptionsDropDown.dropDownButtonSource.Height;
            return new Rectangle2(bounds.X + bounds.Width - width, bounds.Y, width, height);
        }

        protected override void OnDraw(SpriteBatch b) {
            Rectangle bounds = this.AbsoluteBounds;
            float scale = this.Enabled ? 1f : 0.33f;

            // Main text area
            Rectangle2 textBounds = this.GetTextBounds();
            Rectangle2 buttonBounds = this.GetButtonBounds();
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsDropDown.dropDownBGSource, bounds.X, bounds.Y, bounds.Width - buttonBounds.Width, bounds.Height, Color.White * scale, Game1.pixelZoom, false);

            // Text
            string text = this.SelectedText;
            if (this.SelectedText != null) {
                float textScale = textBounds.Height / this.Font.MeasureString(text).Y;
                b.DrawString(this.Font, text, new Vector2(textBounds.X, textBounds.Y), this.Color * scale, 0f, Vector2.Zero, textScale, SpriteEffects.None, this.GetGlobalDepth(0.1f));
            }

            // Arrow button
            b.Draw(Game1.mouseCursors, buttonBounds, OptionsDropDown.dropDownButtonSource, Color.White * scale, 0f, Vector2.Zero, SpriteEffects.None, this.GetGlobalDepth(0));
        }

        protected override bool OnLeftClick(Location mousePos) {
            this.Open();
            return true;
        }

        protected override bool OnKeyPressed(Keys key) {
            if (this.Opened && key == Keys.Escape) {
                this.Close();
                return true;
            }

            return base.OnKeyPressed(key);
        }

        public virtual void Open() {
            if (this.Opened)
                return;

            // Update the dropdown location and size
            this._dropComponent.Location = new Location(this.Location.X, this.Size.Height);
            this._dropComponent.Size = new Size(this.Size.Width - this.GetButtonBounds().Width, this.Size.Height * 3);

            // Clear all option boxes
            Component[] removed = this._dropComponent.Children.Where(c => c is OptionBoxComponent).ToArray();
            foreach (Component c in removed) {
                this._dropComponent.Children.Remove(c);
            }

            // Add new option boxes
            int y = 0;
            for (int i = 0; i < this.Options.Length; i++) {
                // Create new option box
                int curY = y;
                OptionBoxComponent optionComponent = new OptionBoxComponent(this.Options[i], i, this.OptionBoxCallback)
                    .Chain(c => c.Size = new Size(this._dropComponent.Size.Width, c.Size.Height))
                    .Chain(c => c.Location = new Location(0, curY))
                    .Chain(c => c.Font = this.Font)
                    .Chain(c => c.TextColor = this.Color);
                this._dropComponent.AddChild(optionComponent);

                // Update the y
                y += optionComponent.Size.Height;
            }

            // Make the dropdown menu visible
            this._dropComponent.Visible = this._dropComponent.Enabled = true;
        }

        public virtual void Close() {
            this._dropComponent.Visible = this._dropComponent.Enabled = false;
        }

        private void OptionBoxCallback(OptionBoxComponent optionBoxComponent) {
            this.Selected = optionBoxComponent.Index;
            this.Close();
        }

        public class OptionBoxComponent : Component {
            public string Text { get; }
            public int Index { get; }
            public SpriteFont Font { get; set; } = Game1.smallFont;
            public Color TextColor { get; set; } = Game1.textColor;

            private readonly Action<OptionBoxComponent> _callback;

            public OptionBoxComponent(string text, int index, Action<OptionBoxComponent> callback) {
                this.Text = text;
                this.Index = index;
                this._callback = callback;
                this.Size = new Size(9 * Game1.pixelZoom, 9 * Game1.pixelZoom);
            }

            protected override void OnDraw(SpriteBatch b) {
                Rectangle bounds = this.AbsoluteBounds;

                // Draw background
                b.Draw(Game1.staminaRect, bounds.ToXnaRectangle(), new Rectangle2(0, 0, 1, 1), Color.Wheat, 0f, Vector2.Zero, SpriteEffects.None, this.GetGlobalDepth(0));

                // Draw text
                b.DrawString(this.Font, this.Text, new Vector2(bounds.X + Game1.pixelZoom, bounds.Y + Game1.pixelZoom * 2), this.TextColor, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, this.GetGlobalDepth(1));

                // b.Draw(Game1.staminaRect, new Rectangle(slotX + this.dropDownBounds.X, slotY + this.dropDownBounds.Y + i * this.bounds.Height, this.dropDownBounds.Width, this.bounds.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Wheat, 0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                // b.DrawString(Game1.smallFont, this.dropDownDisplayOptions[i], new Vector2((float)(slotX + this.dropDownBounds.X + Game1.pixelZoom), (float)(slotY + this.dropDownBounds.Y + Game1.pixelZoom * 2 + this.bounds.Height * i)), Game1.textColor * scale, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
            }

            protected override bool OnLeftClick(Location mousePos) {
                this._callback(this);
                return true;
            }
        }
    }
}
