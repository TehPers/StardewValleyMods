using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Api.Items.Recipes {
    public interface IIngredient : IItemRequest {
        /// <summary>The sprite to draw for this ingredient.</summary>
        ISprite Sprite { get; }

        /// <summary>Gets the name of this ingredient.</summary>
        /// <returns>The name of this ingredient.</returns>
        string GetDisplayName();
    }
}