using StardewModdingAPI;
using TehPers.CoreMod.Api.Drawing;

namespace TehPers.CoreMod.Api.Items {
    public class ModFood : ModObject {
        protected virtual BuffDescription Buffs { get; }
        protected bool IsDrink { get; }

        public ModFood(IMod owner, string rawName, int cost, int edibility, Category category, TextureInformation textureInfo, bool isDrink) : this(owner, rawName, cost, edibility, category, textureInfo, isDrink, new BuffDescription(0)) { }
        public ModFood(IMod owner, string rawName, int cost, int edibility, Category category, TextureInformation textureInfo, bool isDrink, BuffDescription buffs) : base(owner, rawName, cost, category, textureInfo, edibility) {
            this.IsDrink = isDrink;
            this.Buffs = buffs;
        }

        public override string GetRawInformation() {
            return $"{base.GetRawInformation()}/{(this.IsDrink ? "drink" : "food")}/{this.Buffs.GetRawBuffInformation()}/{this.Buffs.Duration}";
        }
    }
}