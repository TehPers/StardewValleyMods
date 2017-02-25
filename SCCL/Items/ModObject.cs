using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.Stardew.SCCL.Items {
    public class OverriddenItem : StardewValley.Object {

        public ItemTemplate template;
        public Dictionary<string, object> data;

        internal OverriddenItem(ItemTemplate template, Dictionary<string, object> data) : base(Vector2.Zero, 9000) {
            this.name = template.GetName(data);
            
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber) {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber);
        }
    }
}
