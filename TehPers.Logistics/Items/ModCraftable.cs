using StardewModdingAPI;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Items;

namespace TehPers.Logistics.Items {
    public class ModCraftable : ModObject {
        public virtual bool CanSetOutdoors => true;
        public virtual bool CanSetIndoors => true;
        public virtual int Fragility { get; }

        public ModCraftable(IMod owner, string rawName, int cost, TextureInformation textureInfo) : this(owner, rawName, cost, textureInfo, 0) { }
        public ModCraftable(IMod owner, string rawName, int cost, TextureInformation textureInfo, int fragility) : base(owner, rawName, cost, Category.BigCraftable, textureInfo, -300) {
            this.Fragility = fragility;
        }

        public override string GetRawInformation() {
            Translation displayName = this.Owner.Helper.Translation.Get($"item.{this.RawName}").Default($"item.{this.RawName}");
            Translation description = this.Owner.Helper.Translation.Get($"item.{this.RawName}.description").Default("No description available.");
            return $"{displayName}/{this.Cost}/{this.Edibility}/{this.Category}/{description}/{(this.CanSetOutdoors ? "true" : "false")}/{(this.CanSetIndoors ? "true" : "false")}/{this.Fragility}/{displayName}";
        }
    }
}