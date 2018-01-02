using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModUtilities.Helpers {
    public static class DrawHelpers {
        public static void FillRectangle(this SpriteBatch batch, int x, int y, int width, int height, Color color) => batch.FillRectangle(new Rectangle(x, y, width, height), color);
        public static void FillRectangle(this SpriteBatch batch, xTile.Dimensions.Rectangle bounds, Color color) => batch.FillRectangle(new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height), color);
        public static void FillRectangle(this SpriteBatch batch, Rectangle bounds, Color color) {
            Texture2D rect = new Texture2D(batch.GraphicsDevice, bounds.Width, bounds.Height);
            Color[] data = new Color[rect.Width * rect.Height];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            rect.SetData(data);
            batch.Draw(rect, bounds, Color.White);
        }

        public static Rectangle ToXnaRectangle(this xTile.Dimensions.Rectangle rect) => new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

        public static xTile.Dimensions.Rectangle ToXTileRectangle(this Rectangle rect) => new xTile.Dimensions.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
