using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.ContentPacks.Data {
    internal class FoodData : SObjectData {
        /// <summary>Whether this is a drink.</summary>
        public bool IsDrink { get; set; } = false;

        /// <summary>The buffs the farmer gets when consuming this item.</summary>
        public BuffDescription? Buffs { get; set; } = null;
    }
}