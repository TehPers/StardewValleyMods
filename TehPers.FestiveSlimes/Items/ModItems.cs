using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using TehPers.FestiveSlimes.Drawing;

namespace TehPers.FestiveSlimes.Items {
    public static class ModItems {
        public static Dictionary<int, IItemDescription> RegisteredItems { get; } = new Dictionary<int, IItemDescription>();
        private static bool _drawingOverridden = false;
        private static bool _locked = false;

        public static T Register<T>(this T description, int index) where T : IItemDescription {
            if (ModItems._locked) {
                throw new InvalidOperationException("Object information has already been added to the game.");
            }

            ModItems.OverrideDrawingIfNeeded();
            ModItems.RegisteredItems.Add(index, description);
            return description;
        }

        public static void AddObjectInformation(IAssetDataForDictionary<int, string> data) {
            ModItems._locked = true;
            foreach (KeyValuePair<int, IItemDescription> description in ModItems.RegisteredItems) {
                data.Set(description.Key, description.Value.GetRawInformation());
            }
        }

        private static void OverrideDrawingIfNeeded() {
            if (ModItems._drawingOverridden) {
                return;
            }
            ModItems._drawingOverridden = true;

            DrawingDelegator.PatchIfNeeded();
            DrawingDelegator.AddOverride(info => {
                if (info.Texture != Game1.objectSpriteSheet) {
                    return;
                }

                int index = DrawingDelegator.GetIndexForSourceRectangle(info.SourceRectangle ?? default);
                if (!ModItems.RegisteredItems.TryGetValue(index, out IItemDescription description)) {
                    return;
                }

                description.OverrideTexture(info);
            });
        }

        #region Item Indexes
        public const int CANDY_INDEX = 1999;
        #endregion

        #region Items
        public static FoodDescription Candy { get; }
        #endregion

        static ModItems() {
            ModItems.Candy = new FoodDescription("candy", 20, 5, Category.Trash, TextureInformation.FromAssetFile("candy.png", null, Color.Red), false, new BuffDescription(TimeSpan.FromMinutes(2.5)) {
                Speed = 1
            }).Register(ModItems.CANDY_INDEX);
        }
    }

    internal class ItemsAssetEditor : IAssetEditor {
        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals(@"Data\ObjectInformation");
        }

        public void Edit<T>(IAssetData asset) {
            ModItems.AddObjectInformation(asset.AsDictionary<int, string>());
        }
    }
}
