using StardewValley;

namespace TehPers.CoreMod.Items.Recipes {
    public interface IIngredient {
        /// <summary>Tries to create an instance of this ingredient (of any quantity). This is used to draw the ingredient, for example.</summary>
        /// <param name="item">An instance of this ingredient.</param>
        /// <returns>True if successful, false otherwise. If this returns false, any recipes requiring this ingredient will be disabled.</returns>
        bool TryGetOne(out Item item);

        /// <summary>Checks whether an item is an instance of this ingredient.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is an instance of this ingredient, false otherwise.</returns>
        bool IsInstanceOf(Item item);
    }
}