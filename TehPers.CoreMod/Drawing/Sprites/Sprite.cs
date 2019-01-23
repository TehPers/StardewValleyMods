using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Drawing.Sprites {
    internal class Sprite : SpriteBase {
        public override Rectangle? SourceRectangle { get; }

        public Sprite(int index, ISpriteSheet parentSheet, Rectangle sourceRectangle) : base(index, parentSheet) {
            this.SourceRectangle = sourceRectangle;
        }
    }
}