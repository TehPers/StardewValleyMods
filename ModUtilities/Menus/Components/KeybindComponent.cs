using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Menus.Components.Interfaces;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using Rectangle1 = xTile.Dimensions.Rectangle;
using Rectangle2 = Microsoft.Xna.Framework.Rectangle;

namespace ModUtilities.Menus.Components {
    public class KeybindComponent : Component, IValueComponent<Keys> {
        private Keys _selectedKey = Keys.None;
        public Keys SelectedKey {
            get => this._selectedKey;
            set {
                if (value == this._selectedKey)
                    return;

                this._selectedKey = value;
                this.OnValueChanged();
            }
        }

        public SpriteFont Font { get; set; } = Game1.smallFont;
        public Color Color { get; set; } = Game1.textColor;
        public override bool FocusOnClick { get; } = true;

        private readonly Texture2D _background;
        private bool _selectingKey;

        public KeybindComponent() {
            this._background = ModUtilities.Instance.Helper.Content.Load<Texture2D>(@"LooseSprites\textBox.xnb", ContentSource.GameContent);
            this.Size = new Size(100, this._background.Height);
        }

        protected override void OnDraw(SpriteBatch b) {
            // Make sure this component is still focused
            this._selectingKey = this._selectingKey && this.IsFocused;

            Rectangle1 bounds = this.AbsoluteBounds;

            // Draw the background
            float bgDepth = this.GetGlobalDepth(0);
            b.Draw(this._background, new Rectangle2(bounds.X, bounds.Y, Game1.tileSize / 4, bounds.Height), new Rectangle2(0, 0, Game1.tileSize / 4, this._background.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, bgDepth);
            b.Draw(this._background, new Rectangle2(bounds.X + Game1.tileSize / 4, bounds.Y, bounds.Width - Game1.tileSize / 2, bounds.Height), new Rectangle2(Game1.tileSize / 4, 0, 4, this._background.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, bgDepth);
            b.Draw(this._background, new Rectangle2(bounds.X + bounds.Width - Game1.tileSize / 4, bounds.Y, Game1.tileSize / 4, bounds.Height), new Rectangle2(this._background.Width - Game1.tileSize / 4, 0, Game1.tileSize / 4, this._background.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, bgDepth);

            // Draw the text
            const int xPadding = 14;
            const int yPadding = 4;
            string text = this._selectingKey ? "Press a key..." : Enum.GetName(typeof(Keys), this.SelectedKey);
            Vector2 origSize = this.Font.MeasureString(text);
            float scale = (bounds.Height - yPadding) / origSize.Y;
            b.DrawString(this.Font, text, new Vector2(bounds.X + xPadding, bounds.Y + yPadding), this.Color, 0, Vector2.Zero, scale, SpriteEffects.None, this.GetGlobalDepth(1));
        }

        protected override bool OnLeftClick(Location mousePos) {
            if (!base.OnLeftClick(mousePos))
                return false;

            this._selectingKey = !this._selectingKey;
            return true;
        }

        protected override bool OnKeyPressed(Keys key) {
            if (this._selectingKey) {
                this.SelectedKey = key;
                this._selectingKey = false;
                Game1.playSound("drumkit6");
                return true;
            }

            return base.OnKeyPressed(key);
        }

        public void SetValue(Keys value) => this.SelectedKey = value;

        public Keys GetValue() => this.SelectedKey;

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged() => this.ValueChanged?.Invoke(this, EventArgs.Empty);
    }
}
