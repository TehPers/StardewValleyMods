using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TehPers.Stardew.FishingOverhaul {
    internal class ModObject : StardewValley.Object {

        public ModObject(Vector2 location, int id, int initialStack) : base(location, id, initialStack) {

        }

        public ModObject(int id, int initialStack) : this(Vector2.Zero, id, initialStack) {

        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1) {
            base.draw(spriteBatch, x, y, alpha);
        }

        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1) {
            base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber) {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber);
        }

        public override void drawAttachments(SpriteBatch b, int x, int y) {
            base.drawAttachments(b, x, y);
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location) {
            base.drawPlacementBounds(spriteBatch, location);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f) {
            base.drawWhenHeld(spriteBatch, objectPosition, f);
        }
    }
}
