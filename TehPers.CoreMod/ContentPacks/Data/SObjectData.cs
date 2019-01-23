using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.ContentPacks.Data {
    // TODO: Use IContentPackValue instead and support tokens that change values
    internal class SObjectData {
        /// <summary>The path to the sprite relative to the current content pack.</summary>
        public string Sprite { get; set; } = null;

        /// <summary>The color to tint the sprite.</summary>
        public SColor Tint { get; set; } = Color.White;

        /// <summary>The cost of the item at normal quality.</summary>
        public int Cost { get; set; } = 0;

        /// <summary>The item's edibility.</summary>
        public int Edibility { get; set; } = -300;

        /// <summary>The name of the category the item is in.</summary>
        public string CategoryName { get; set; } = Api.Items.Category.Trash.Name;

        /// <summary>The number associated with the category the item is in.</summary>
        public int CategoryNumber { get; set; } = Api.Items.Category.Trash.Index;

        /// <summary>The item's recipes.</summary>
        public List<Dictionary<string, int>> Recipes { get; set; } = null;
    }
}
