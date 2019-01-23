using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public class ModObject : IModObject {
        /// <summary>This object's translation helper.</summary>
        protected ICoreTranslationHelper TranslationHelper { get; }

        /// <summary>The raw name of this object.</summary>
        protected virtual string RawName { get; }

        /// <summary>This object's cost.</summary>
        protected virtual int Cost { get; }

        /// <summary>This object's edibility.</summary>
        protected virtual int Edibility { get; }

        /// <summary>This object's category.</summary>
        protected virtual Category Category { get; }

        /// <summary>Amount of energy that is restored when this is consumed.</summary>
        protected float EnergyRestored => this.Edibility * 2.5F;

        /// <summary>Amount of health that is restored when this is consumed.</summary>
        protected float HealthRestored => this.EnergyRestored * 0.45F;

        /// <summary>The sprite for this item.</summary>
        public virtual ISprite Sprite { get; }

        /// <summary>The color to tint the item's sprite when drawn.</summary>
        public virtual Color Tint { get; set; } = Color.White;

        public ModObject(ICoreTranslationHelper translationHelper, ISprite sprite, string rawName, int cost, Category category) : this(translationHelper, sprite, rawName, cost, category, -300) { }
        public ModObject(ICoreTranslationHelper translationHelper, ISprite sprite, string rawName, int cost, Category category, int edibility) {
            this.TranslationHelper = translationHelper;
            this.RawName = rawName;
            this.Cost = cost;
            this.Edibility = edibility;
            this.Sprite = sprite;
            this.Category = category;
        }

        /// <inheritdoc />
        public virtual string GetRawObjectInformation() {
            ICoreTranslation displayName = this.TranslationHelper.Get($"item.{this.RawName}").WithDefault($"item.{this.RawName}");
            ICoreTranslation description = this.TranslationHelper.Get($"item.{this.RawName}.description").WithDefault("No description available.");
            return $"{displayName}/{this.Cost}/{this.Edibility}/{this.Category}/{displayName}/{description}";
        }

        /// <inheritdoc />
        public string GetDataSource() {
            return this.Category.DataSource;
        }

        /// <inheritdoc />
        public virtual void OverrideDraw(IDrawingInfo info) {
            info.SetSource(this.Sprite.ParentSheet.TrackedTexture.CurrentTexture, this.Sprite.SourceRectangle);
            info.AddTint(this.Tint);
        }
    }
}