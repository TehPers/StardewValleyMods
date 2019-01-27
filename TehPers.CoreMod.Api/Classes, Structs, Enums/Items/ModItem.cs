using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Api.Items {
    public abstract class ModItem : IModItem {
        /// <summary>This object's translation helper.</summary>
        protected ICoreTranslationHelper TranslationHelper { get; }

        /// <summary>The raw name of this object.</summary>
        protected virtual string RawName { get; }

        /// <inheritdoc />
        public virtual ISprite Sprite { get; }

        /// <inheritdoc />
        public virtual SColor Tint { get; set; } = Color.White;

        protected ModItem(ICoreTranslationHelper translationHelper, string rawName, ISprite sprite) {
            this.TranslationHelper = translationHelper;
            this.RawName = rawName;
            this.Sprite = sprite;
        }

        /// <inheritdoc />
        public void OverrideDraw(IDrawingInfo info, Vector2 sourcePositionOffsetPercentage, Vector2 sourceSizePercentage) {
            int newU = this.Sprite.U + (int) (sourcePositionOffsetPercentage.X * this.Sprite.Width);
            int newV = this.Sprite.V + (int) (sourcePositionOffsetPercentage.Y * this.Sprite.Height);
            int newWidth = (int) (this.Sprite.Width * sourceSizePercentage.X);
            int newHeight = (int) (this.Sprite.Height * sourceSizePercentage.Y);
            SRectangle newSourceRect = new SRectangle(newU, newV, newWidth, newHeight);
            info.SetSource(this.Sprite.ParentSheet.TrackedTexture.CurrentTexture, newSourceRect);
            info.AddTint(this.Tint);
        }
    }
}