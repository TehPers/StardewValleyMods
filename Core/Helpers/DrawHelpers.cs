using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Rectangle2 = Microsoft.Xna.Framework.Rectangle;
using Rectangle = xTile.Dimensions.Rectangle;

namespace TehPers.Core.Helpers {
    public static class DrawHelpers {
        public static void FillRectangle(this SpriteBatch batch, int x, int y, int width, int height, Color color) => batch.FillRectangle(new Rectangle2(x, y, width, height), color);
        public static void FillRectangle(this SpriteBatch batch, Rectangle bounds, Color color) => batch.FillRectangle(new Rectangle2(bounds.X, bounds.Y, bounds.Width, bounds.Height), color);
        public static void FillRectangle(this SpriteBatch batch, Rectangle2 bounds, Color color, float depth = 0f) {
            batch.Draw(ModCore.Instance.WhitePixel, bounds, null, color, 0F, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static void DrawMenuBox(this SpriteBatch batch, Rectangle2 bounds, float depth) {
            Rectangle2 sourceRect = new Rectangle2(Game1.tileSize, Game1.tileSize * 2, Game1.tileSize, Game1.tileSize);
            Game1.spriteBatch.Draw(Game1.menuTexture, new Rectangle2(28 + bounds.X, 28 + bounds.Y, bounds.Width - Game1.tileSize, bounds.Height - Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            sourceRect.Y = 0;
            sourceRect.X = 0;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Vector2(bounds.X, bounds.Y), sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            sourceRect.X = Game1.tileSize * 3;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Vector2(bounds.X + bounds.Width - Game1.tileSize, bounds.Y), sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            sourceRect.Y = Game1.tileSize * 3;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Vector2(bounds.X + bounds.Width - Game1.tileSize, bounds.Y + bounds.Height - Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            sourceRect.X = 0;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Vector2(bounds.X, bounds.Y + bounds.Height - Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            sourceRect.X = Game1.tileSize * 2;
            sourceRect.Y = 0;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Rectangle2(Game1.tileSize + bounds.X, bounds.Y, bounds.Width - Game1.tileSize * 2, Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            sourceRect.Y = 3 * Game1.tileSize;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Rectangle2(Game1.tileSize + bounds.X, bounds.Y + bounds.Height - Game1.tileSize, bounds.Width - Game1.tileSize * 2, Game1.tileSize), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            sourceRect.Y = Game1.tileSize * 2;
            sourceRect.X = 0;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Rectangle2(bounds.X, bounds.Y + Game1.tileSize, Game1.tileSize, bounds.Height - Game1.tileSize * 2), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            sourceRect.X = 3 * Game1.tileSize;
            Game1.spriteBatch.Draw(Game1.menuTexture, new Rectangle2(bounds.X + bounds.Width + -Game1.tileSize, bounds.Y + Game1.tileSize, Game1.tileSize, bounds.Height - Game1.tileSize * 2), sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static void DrawTextureBox(this SpriteBatch batch, Rectangle2 bounds, Rectangle2 source, float depth) => batch.DrawTextureBox(Game1.mouseCursors, bounds, source, depth, 1f, Color.White);
        public static void DrawTextureBox(this SpriteBatch batch, Rectangle2 bounds, Rectangle2 source, float depth, float scale) => batch.DrawTextureBox(Game1.mouseCursors, bounds, source, depth, scale, Color.White);
        public static void DrawTextureBox(this SpriteBatch batch, Rectangle2 bounds, Rectangle2 source, float depth, Color tint) => batch.DrawTextureBox(Game1.mouseCursors, bounds, source, depth, 1f, tint);
        public static void DrawTextureBox(this SpriteBatch batch, Texture2D texture, Rectangle2 bounds, Rectangle2 source, float depth, float scale, Color tint) {
            int num = source.Width / 3;
            batch.Draw(texture, new Rectangle2((int) (num * (double) scale) + bounds.X, (int) (num * (double) scale) + bounds.Y, bounds.Width - (int) (num * (double) scale * 2.0), bounds.Height - (int) (num * (double) scale * 2.0)), new Rectangle2(num + source.X, num + source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(texture, new Vector2(bounds.X, bounds.Y), new Rectangle2(source.X, source.Y, num, num), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            batch.Draw(texture, new Vector2(bounds.X + bounds.Width - (int) (num * (double) scale), bounds.Y), new Rectangle2(source.X + num * 2, source.Y, num, num), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            batch.Draw(texture, new Vector2(bounds.X, bounds.Y + bounds.Height - (int) (num * (double) scale)), new Rectangle2(source.X, num * 2 + source.Y, num, num), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            batch.Draw(texture, new Vector2(bounds.X + bounds.Width - (int) (num * (double) scale), bounds.Y + bounds.Height - (int) (num * (double) scale)), new Rectangle2(source.X + num * 2, num * 2 + source.Y, num, num), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            batch.Draw(texture, new Rectangle2(bounds.X + (int) (num * (double) scale), bounds.Y, bounds.Width - (int) (num * (double) scale) * 2, (int) (num * (double) scale)), new Rectangle2(source.X + num, source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(texture, new Rectangle2(bounds.X + (int) (num * (double) scale), bounds.Y + bounds.Height - (int) (num * (double) scale), bounds.Width - (int) (num * (double) scale) * 2, (int) (num * (double) scale)), new Rectangle2(source.X + num, num * 2 + source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(texture, new Rectangle2(bounds.X, bounds.Y + (int) (num * (double) scale), (int) (num * (double) scale), bounds.Height - (int) (num * (double) scale) * 2), new Rectangle2(source.X, num + source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(texture, new Rectangle2(bounds.X + bounds.Width - (int) (num * (double) scale), bounds.Y + (int) (num * (double) scale), (int) (num * (double) scale), bounds.Height - (int) (num * (double) scale) * 2), new Rectangle2(source.X + num * 2, num + source.Y, num, num), tint, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static Rectangle2 ToXnaRectangle(this Rectangle rect) => new Rectangle2(rect.X, rect.Y, rect.Width, rect.Height);
        public static Rectangle ToXTileRectangle(this Rectangle2 rect) => new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

        public static Rectangle2? Intersection(this Rectangle2 a, Rectangle2 b) => a.ToXTileRectangle().Intersection(b.ToXTileRectangle())?.ToXnaRectangle();
        public static Rectangle? Intersection(this Rectangle a, Rectangle b) {
            int x1 = Math.Max(a.X, b.X);
            int y1 = Math.Max(a.Y, b.Y);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            // Make sure it's a valid rectangle (the rectangles intersect)
            if (x1 >= x2 || y1 >= y2)
                return null;

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static void DrawStringWithShadow(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, float depth = 0F) {
            batch.DrawString(font, text, position + Vector2.One * Game1.pixelZoom / 2f, Color.Black, 0F, Vector2.Zero, Vector2.One, SpriteEffects.None, depth - 0.001F);
            batch.DrawString(font, text, position, color, 0F, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
        }

        public static void DrawSpeechBubble(this Character who, string text, Color color, int style = 1) {
            Vector2 local = Game1.GlobalToLocal(new Vector2(who.getStandingX(), who.getStandingY() - 192 + who.yJumpOffset));
            if (style == 0)
                local += new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
            SpriteText.drawStringWithScrollCenteredAt(Game1.spriteBatch, text, (int) local.X, (int) local.Y, "", 1F, (int) color.PackedValue, 1, (float) (who.getTileY() * 64 / 10000.0 + 1.0 / 1000.0 + who.getTileX() / 10000.0));
        }
    }
}
