using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace TehCore.Menus.BoxModel {
    public struct Rectangle2I {
        /// <summary>The top, left point of this rectangle.</summary>
        public Vector2I Location { get; }
        /// <summary>The size of this rectangle.</summary>
        public Vector2I Size { get; }

        public int X => this.Location.X;
        public int Y => this.Location.Y;
        public int Width => this.Size.X;
        public int Height => this.Size.Y;

        public Rectangle2I(int x, int y, int width, int height) : this(new Vector2I(x, y), new Vector2I(width, height)) { }
        public Rectangle2I(Vector2I location, Vector2I size) {
            this.Location = location;
            this.Size = size;
        }

        public bool Contains(Vector2I location) {
            Vector2I bottomRight = this.Location + this.Size;
            return this.Location.X <= location.X && this.Location.Y <= location.Y && bottomRight.X > location.X && bottomRight.Y > location.Y;
        }

        public Rectangle ToRectangle() => new Rectangle(this.Location.X, this.Location.Y, this.Size.X, this.Size.Y);
    }
}
