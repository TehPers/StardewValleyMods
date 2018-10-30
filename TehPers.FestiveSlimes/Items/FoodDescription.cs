namespace TehPers.FestiveSlimes.Items {
    public class FoodDescription : ItemDescription {
        protected virtual BuffDescription Buffs { get; }
        protected bool IsDrink { get; }

        public FoodDescription(string rawName, int cost, int edibility, Category category, TextureInformation textureInfo, bool isDrink) : this(rawName, cost, edibility, category, textureInfo, isDrink, new BuffDescription(0)) { }
        public FoodDescription(string rawName, int cost, int edibility, Category category, TextureInformation textureInfo, bool isDrink, BuffDescription buffs) : base(rawName, cost, category, textureInfo, edibility) {
            this.IsDrink = isDrink;
            this.Buffs = buffs;
        }

        public override string GetRawInformation() {
            return $"{base.GetRawInformation()}/{(this.IsDrink ? "drink" : "food")}/{this.Buffs.GetRawBuffInformation()}/{this.Buffs.Duration}";
        }
    }
}