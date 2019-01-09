using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Structs;
using TehPers.CoreMod.ContentPacks;
using Object = System.Object;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.ContentPacks {
    public class RecipeParser {
    }
}

namespace TehPers.CoreMod.Api.Items {
    public interface IIngredient {
        int? ParentSheetIndex { get; }
        GameAssetLocation GameAsset { get; }

        /// <summary>Checks whether an item is this ingredient.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is this ingredient, false otherwise.</returns>
        bool Matches(Item item);

        /// <summary>Creates an instance of this ingredient with the specified quantity.</summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Item Create(int quantity);
    }

    public abstract class Ingredient : IIngredient {
        /// <inheritdoc />
        public int? ParentSheetIndex { get; }

        /// <inheritdoc />
        public GameAssetLocation GameAsset { get; }

        protected Ingredient(int parentSheetIndex, in GameAssetLocation gameAsset) {
            this.ParentSheetIndex = parentSheetIndex;
            this.GameAsset = gameAsset;
        }

        /// <inheritdoc />
        public abstract bool Matches(Item item);

        /// <inheritdoc />
        public abstract Item Create(int quantity);
    }

    public class ModObjectIngredient : IIngredient {
        private readonly string _name;
        private readonly IItemApi _itemApi;

        /// <inheritdoc />
        public int? ParentSheetIndex => this._itemApi.TryGetInformation(this._name, out IObjectInformation objectInformation) ? objectInformation.Index : default;

        /// <inheritdoc />
        public GameAssetLocation GameAsset { get; } = new GameAssetLocation(@"Maps\springobjects");

        public ModObjectIngredient(string name, IItemApi itemApi) {
            this._name = name;
            this._itemApi = itemApi;
        }

        /// <inheritdoc />
        public bool Matches(Item item) {
            return this.ParentSheetIndex is int index && item.ParentSheetIndex == index && item is SObject obj && !obj.bigCraftable.Value;
        }

        /// <inheritdoc />
        public Item Create(int quantity) {
            if (this.ParentSheetIndex is int index) {
                return new SObject(Vector2.Zero, index, 1);
            }

            throw new InvalidOperationException($"Index is not assigned for item {this._name}");
        }
    }

    public class ObjectIngredient : IIngredient {
        private readonly int _parentSheetIndex;

        /// <inheritdoc />
        public int? ParentSheetIndex => this._parentSheetIndex;

        /// <inheritdoc />
        public GameAssetLocation GameAsset { get; } = new GameAssetLocation(@"Maps\springobjects");

        public ObjectIngredient(int parentSheetIndex) {
            this._parentSheetIndex = parentSheetIndex;
        }

        /// <inheritdoc />
        public bool Matches(Item item) {
            return item.ParentSheetIndex == this.ParentSheetIndex && item is SObject obj && !obj.bigCraftable.Value;
        }

        /// <inheritdoc />
        public Item Create(int quantity) {
            return new SObject(Vector2.Zero, this._parentSheetIndex, 1);
        }
    }

    public class BigCraftableIngredient : IIngredient {
        private readonly int _parentSheetIndex;

        /// <inheritdoc />
        public int? ParentSheetIndex => this._parentSheetIndex;

        /// <inheritdoc />
        public GameAssetLocation GameAsset { get; }

        public BigCraftableIngredient(int parentSheetIndex, in GameAssetLocation gameAsset) {
            this._parentSheetIndex = parentSheetIndex;
            this.GameAsset = gameAsset;
        }

        /// <inheritdoc />
        public bool Matches(Item item) {
            return item.ParentSheetIndex == this.ParentSheetIndex && item is SObject obj && obj.bigCraftable.Value;
        }

        /// <inheritdoc />
        public Item Create(int quantity) {
            return new SObject(Vector2.Zero, this._parentSheetIndex);
        }
    }

    public readonly struct IngredientPayload {
        public IIngredient Ingredient { get; }
        public int Quantity { get; }

        public IngredientPayload(IIngredient ingredient, int quantity) {
            this.Ingredient = ingredient;
            this.Quantity = quantity;
        }
    }

    public interface IRecipe {

    }

    public class Recipe : IRecipe {
        public IEnumerable<IngredientPayload> Ingredients { get; }


    }
}
