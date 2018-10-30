namespace TehPers.FestiveSlimes.Items {
    public class Category {
        public int Index { get; }
        public string Name { get; }

        public Category(int index, string name) {
            this.Index = index;
            this.Name = name;
        }

        public override string ToString() {
            return string.IsNullOrEmpty(this.Name) ? this.Index.ToString() : $"{this.Name} {this.Index}";
        }

        #region Static
        public static Category Gem { get; } = new Category(-2, "Mineral");
        public static Category Fish { get; } = new Category(-4, "Fish");
        public static Category Egg { get; } = new Category(-5, "Animal Product");
        public static Category Milk { get; } = new Category(-6, "Animal Product");
        public static Category Cooking { get; } = new Category(-7, "Cooking");
        public static Category Crafting { get; } = new Category(-8, "Crafting");
        public static Category BigCraftable { get; } = new Category(-9, null);
        public static Category Mineral { get; } = new Category(-12, "Mineral");
        public static Category Meat { get; } = new Category(-14, "Animal Product");
        public static Category Metal { get; } = new Category(-15, "Resource");
        public static Category BuildingMaterial { get; } = new Category(-16, "Resource");
        public static Category SellAtPierres { get; } = new Category(-17, null);
        public static Category SellAtPierresAndMarnies { get; } = new Category(-18, "Animal Product");
        public static Category Fertilizer { get; } = new Category(-19, "Fertilizer");
        public static Category Trash { get; } = new Category(-20, "Trash");
        public static Category Bait { get; } = new Category(-21, "Bait");
        public static Category FishingTackle { get; } = new Category(-22, "Fishing Tackle");
        public static Category SellAtFishShop { get; } = new Category(-23, null);
        public static Category Furniture { get; } = new Category(-24, "Decor");
        public static Category Ingredient { get; } = new Category(-25, "Cooking");
        public static Category ArtisanGoods { get; } = new Category(-26, "Artisan Goods");
        public static Category Syrup { get; } = new Category(-27, "Artisan Goods");
        public static Category MonsterLoot { get; } = new Category(-28, "Monster Loot");
        public static Category Equipment { get; } = new Category(-29, null);
        public static Category Seed { get; } = new Category(-74, "Seed");
        public static Category Vegetable { get; } = new Category(-75, "Vegetable");
        public static Category Fruit { get; } = new Category(-79, "Fruit");
        public static Category Flower { get; } = new Category(-80, "Flower");
        public static Category Forage { get; } = new Category(-81, "Forage");
        public static Category Hat { get; } = new Category(-95, null);
        public static Category Ring { get; } = new Category(-96, null);
        public static Category Weapon { get; } = new Category(-98, null);
        public static Category Tool { get; } = new Category(-99, null);
        #endregion
    }
}