using StardewModdingAPI;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Static.Extensions;

namespace TehPers.CoreMod.Api.Items {
    public class ModObject : IModObject {

        /// <summary>Information about how to render this object.</summary>
        protected virtual TextureInformation TextureInfo { get; }

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

        public IMod Owner { get; }

        public ModObject(IMod owner, string rawName, int cost, Category category, TextureInformation textureInfo) : this(owner, rawName, cost, category, textureInfo, -300) { }
        public ModObject(IMod owner, string rawName, int cost, Category category, TextureInformation textureInfo, int edibility) {
            this.Owner = owner;
            this.RawName = rawName;
            this.Cost = cost;
            this.Edibility = edibility;
            this.Category = category;
            this.TextureInfo = textureInfo;
        }

        /// <inheritdoc />
        public virtual string GetRawInformation() {
            Translation displayName = this.Owner.Helper.Translation.Get($"item.{this.RawName}").Default($"item.{this.RawName}");
            Translation description = this.Owner.Helper.Translation.Get($"item.{this.RawName}.description").Default("No description available.");
            return $"{displayName}/{this.Cost}/{this.Edibility}/{this.Category}/{displayName}/{description}";
        }

        public virtual string GetDataSource() {
            return this.Category.DataSource;
        }

        /// <inheritdoc />
        public virtual void OverrideTexture(IDrawingInfo info) {
            info.SetSource(this.TextureInfo.Texture, this.TextureInfo.SourceRectangle);

            // Multiply tint colors
            info.SetTint(info.Tint.Multiply(this.TextureInfo.Tint));
        }
    }
}