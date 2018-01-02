using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModUtilities.Helpers;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace ModUtilities.Menus.Components {
    public class CheckboxComponent : Component {
        public override bool FocusOnClick { get; } = true;

        public bool IsChecked { get; set; }

        public CheckboxComponent() {
            this.Size = new Size(OptionsCheckbox.sourceRectUnchecked.Width * Game1.pixelZoom, OptionsCheckbox.sourceRectUnchecked.Height * Game1.pixelZoom);
        }

        public CheckboxComponent(bool isChecked) : this() {
            this.IsChecked = isChecked;
        }

        protected override void OnDraw(SpriteBatch b) {
            Rectangle bounds = this.AbsoluteBounds;

            // Draw the checkbox
            Microsoft.Xna.Framework.Rectangle checkboxSource = this.IsChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked;
            b.Draw(Game1.mouseCursors, bounds.ToXnaRectangle(), checkboxSource, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.4f);
        }

        protected override void OnLeftClick(Location mousePos, bool playSound) {
            this.IsChecked = !this.IsChecked;
            Game1.playSound("drumkit6");
        }
    }
}
