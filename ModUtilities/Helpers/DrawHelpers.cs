using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using xTile.Dimensions;
using Rectangle2 = Microsoft.Xna.Framework.Rectangle;
using Rectangle = xTile.Dimensions.Rectangle;

namespace ModUtilities.Helpers {
    public static class DrawHelpers {
        public static void FillRectangle(this SpriteBatch batch, int x, int y, int width, int height, Color color) => batch.FillRectangle(new Rectangle2(x, y, width, height), color);
        public static void FillRectangle(this SpriteBatch batch, Rectangle bounds, Color color) => batch.FillRectangle(new Rectangle2(bounds.X, bounds.Y, bounds.Width, bounds.Height), color);
        public static void FillRectangle(this SpriteBatch batch, Rectangle2 bounds, Color color) {
            Texture2D rect = new Texture2D(batch.GraphicsDevice, bounds.Width, bounds.Height);
            Color[] data = new Color[rect.Width * rect.Height];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            rect.SetData(data);
            batch.Draw(rect, bounds, Color.White);
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
            batch.Draw(texture, new Rectangle2((int) ((double) num * (double) scale) + bounds.X, (int) ((double) num * (double) scale) + bounds.Y, bounds.Width - (int) ((double) num * (double) scale * 2.0), bounds.Height - (int) ((double) num * (double) scale * 2.0)), new Rectangle2?(new Rectangle2(num + source.X, num + source.Y, num, num)), tint, 0.0f, Vector2.Zero, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
            batch.Draw(texture, new Vector2((float) bounds.X, (float) bounds.Y), new Rectangle2?(new Rectangle2(source.X, source.Y, num, num)), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
            batch.Draw(texture, new Vector2((float) (bounds.X + bounds.Width - (int) ((double) num * (double) scale)), (float) bounds.Y), new Rectangle2?(new Rectangle2(source.X + num * 2, source.Y, num, num)), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
            batch.Draw(texture, new Vector2((float) bounds.X, (float) (bounds.Y + bounds.Height - (int) ((double) num * (double) scale))), new Rectangle2?(new Rectangle2(source.X, num * 2 + source.Y, num, num)), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
            batch.Draw(texture, new Vector2((float) (bounds.X + bounds.Width - (int) ((double) num * (double) scale)), (float) (bounds.Y + bounds.Height - (int) ((double) num * (double) scale))), new Rectangle2?(new Rectangle2(source.X + num * 2, num * 2 + source.Y, num, num)), tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
            batch.Draw(texture, new Rectangle2(bounds.X + (int) ((double) num * (double) scale), bounds.Y, bounds.Width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle2?(new Rectangle2(source.X + num, source.Y, num, num)), tint, 0.0f, Vector2.Zero, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
            batch.Draw(texture, new Rectangle2(bounds.X + (int) ((double) num * (double) scale), bounds.Y + bounds.Height - (int) ((double) num * (double) scale), bounds.Width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle2?(new Rectangle2(source.X + num, num * 2 + source.Y, num, num)), tint, 0.0f, Vector2.Zero, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
            batch.Draw(texture, new Rectangle2(bounds.X, bounds.Y + (int) ((double) num * (double) scale), (int) ((double) num * (double) scale), bounds.Height - (int) ((double) num * (double) scale) * 2), new Rectangle2?(new Rectangle2(source.X, num + source.Y, num, num)), tint, 0.0f, Vector2.Zero, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
            batch.Draw(texture, new Rectangle2(bounds.X + bounds.Width - (int) ((double) num * (double) scale), bounds.Y + (int) ((double) num * (double) scale), (int) ((double) num * (double) scale), bounds.Height - (int) ((double) num * (double) scale) * 2), new Rectangle2?(new Rectangle2(source.X + num * 2, num + source.Y, num, num)), tint, 0.0f, Vector2.Zero, SpriteEffects.None, (float) (0.800000011920929 - (double) bounds.Y * 9.99999997475243E-07));
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
    }
}
