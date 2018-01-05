using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace ModUtilities.Menus.Components {
    public class TextboxComponent : Component {
        private static readonly Location Padding = new Location(14, 4);
        private static readonly Regex NewlineRegex = new Regex(@"\r\n?|\n");

        public override bool FocusOnClick { get; } = true;

        public string Text { get; set; } = "";
        public SpriteFont Font { get; set; } = Game1.smallFont;
        public int Cursor {
            get {
                this._cursor = Math.Max(Math.Min(this._cursor, this.Text.Length), 0);
                return this._cursor;
            }
            set => this._cursor = value;
        }
        public Color Color { get; set; } = Game1.textColor;

        private readonly Texture2D _background;
        private int _cursor;

        public TextboxComponent() {
            this._background = ModUtilities.Instance.Helper.Content.Load<Texture2D>(@"LooseSprites\textBox", ContentSource.GameContent);
            this.Size = new Size(100, this._background.Height);
        }

        protected override void OnDraw(SpriteBatch b) {
            Rectangle bounds = this.AbsoluteBounds;

            // Draw the background
            float depth = this.GetGlobalDepth(0);
            b.Draw(this._background, new Microsoft.Xna.Framework.Rectangle(bounds.X, bounds.Y, Game1.tileSize / 4, bounds.Height), new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.tileSize / 4, this._background.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            b.Draw(this._background, new Microsoft.Xna.Framework.Rectangle(bounds.X + Game1.tileSize / 4, bounds.Y, bounds.Width - Game1.tileSize / 2, bounds.Height), new Microsoft.Xna.Framework.Rectangle(Game1.tileSize / 4, 0, 4, this._background.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            b.Draw(this._background, new Microsoft.Xna.Framework.Rectangle(bounds.X + bounds.Width - Game1.tileSize / 4, bounds.Y, Game1.tileSize / 4, bounds.Height), new Microsoft.Xna.Framework.Rectangle(this._background.Width - Game1.tileSize / 4, 0, Game1.tileSize / 4, this._background.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);

            // Draw the text
            Vector2 origSize = this.Font.MeasureString(this.Text);
            float scale = origSize.Y > 0 ? (bounds.Height - TextboxComponent.Padding.Y) / origSize.Y : 0;
            b.DrawString(this.Font, this.Text, new Vector2(bounds.X + TextboxComponent.Padding.X, bounds.Y + TextboxComponent.Padding.Y), this.Color, 0, Vector2.Zero, scale, SpriteEffects.None, this.GetGlobalDepth(1));

            // Draw the cursor
            if (this.IsFocused && DateTime.Now.Millisecond % 1000 >= 500) {
                float cursorOffset = this.Font.MeasureString(this.Text.Substring(0, this.Cursor)).X * scale;
                Microsoft.Xna.Framework.Rectangle cursorRect = new Microsoft.Xna.Framework.Rectangle(bounds.X + (int) cursorOffset - 2 + TextboxComponent.Padding.X, bounds.Y + 6 + TextboxComponent.Padding.Y, 4, bounds.Height - 20);
                b.Draw(Game1.staminaRect, cursorRect, cursorRect, this.Color, 0f, Vector2.Zero, SpriteEffects.None, this.GetGlobalDepth(0.9f));
            }
        }

        protected override bool OnKeyPressed(Keys key) {
            int length = this.Text.Length;
            if (key == Keys.Back) {
                if (length > 0) {
                    StringBuilder newText = new StringBuilder();
                    if (this.Cursor > 0) {
                        newText.Append(this.Text.Substring(0, this.Cursor - 1));
                    }

                    newText.Append(this.Text.Substring(this.Cursor));
                    if (this.Cursor < this.Text.Length) {
                        this.Cursor--;
                    }
                    this.Text = newText.ToString();
                }
            } else if (key == Keys.Delete) {
                if (length > 0) {
                    StringBuilder newText = new StringBuilder();
                    newText.Append(this.Text.Substring(0, this.Cursor));
                    if (this.Cursor < this.Text.Length) {
                        newText.Append(this.Text.Substring(this.Cursor + 1));
                    }

                    this.Text = newText.ToString();
                }
            } else if (key == Keys.Left) {
                this.Cursor--;
            } else if (key == Keys.Right) {
                this.Cursor++;
            } else if (key == Keys.Home) {
                this.Cursor = 0;
            } else if (key == Keys.End) {
                this.Cursor = this.Text.Length;
            } else {
                return key.IsPrintable();
            }

            return true;
        }

        protected override bool OnTextEntered(string text) {
            this.Text = this.Text.Substring(0, this.Cursor) + TextboxComponent.NewlineRegex.Replace(text, "") + this.Text.Substring(this.Cursor);
            this.Cursor += text.Length;
            return true;
        }

        protected override bool OnLeftClick(Location mousePos) {
            if (!base.OnLeftClick(mousePos))
                return false;

            Rectangle bounds = this.AbsoluteBounds;

            int curLen = this.Text.Length + 1;
            Vector2 curSize = this.Font.MeasureString(this.Text);
            while (curLen > 0 && curSize.X + bounds.X + TextboxComponent.Padding.X > mousePos.X) {
                curLen--;
                curSize = this.Font.MeasureString(this.Text.Substring(0, curLen));
            }

            this.Cursor = curLen;
            return true;
        }
    }
}
