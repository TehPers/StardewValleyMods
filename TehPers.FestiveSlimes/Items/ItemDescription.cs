using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TehPers.Core.Helpers.Static;
using TehPers.FestiveSlimes.Drawing;
using SObject = StardewValley.Object;

namespace TehPers.FestiveSlimes.Items {
    public class ItemDescription : IItemDescription {
        protected virtual TextureInformation TextureInfo { get; }

        /// <summary>The raw name of this item.</summary>
        protected virtual string RawName { get; }

        /// <summary>This item's cost.</summary>
        protected virtual int Cost { get; }

        /// <summary>This item's edibility.</summary>
        protected virtual int Edibility { get; }

        /// <summary>This item's category.</summary>
        protected virtual Category Category { get; }

        /// <summary>Amount of energy that is restored when this is consumed.</summary>
        protected float EnergyRestored => this.Edibility * 2.5F;

        /// <summary>Amount of health that is restored when this is consumed.</summary>
        protected float HealthRestored => this.EnergyRestored * 0.45F;

        public ItemDescription(string rawName, int cost, Category category, TextureInformation textureInfo) : this(rawName, cost, category, textureInfo, -300) { }
        public ItemDescription(string rawName, int cost, Category category, TextureInformation textureInfo, int edibility) {
            this.RawName = rawName;
            this.Cost = cost;
            this.Edibility = edibility;
            this.Category = category;
            this.TextureInfo = textureInfo;
        }

        /// <inheritdoc />
        public virtual string GetRawInformation() {
            Translation displayName = ModFestiveSlimes.Instance.Helper.Translation.Get($"item.{this.RawName}").Default($"item.{this.RawName}");
            Translation description = ModFestiveSlimes.Instance.Helper.Translation.Get($"item.{this.RawName}.description").Default("No description available.");
            return $"{displayName}/{this.Cost}/{this.Edibility}/{this.Category}/{displayName}/{description}";
        }

        /// <inheritdoc />
        public virtual SObject CreateObject(int index) {
            return new SObject(Vector2.Zero, index);
        }

        /// <inheritdoc />
        public virtual void OverrideTexture(DrawingInfo info) {
            info.SetSource(this.TextureInfo.Texture, this.TextureInfo.SourceRectangle);

            // Multiply tint colors
            info.SetTint(info.Tint.Multiply(this.TextureInfo.Tint));
        }
    }
}