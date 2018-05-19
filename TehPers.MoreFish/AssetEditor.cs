using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace TehPers.MoreFish {
    public class AssetEditor : IAssetEditor {
        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals(@"Data\ObjectInformation")
                   || asset.AssetNameEquals(@"Maps\springobjects");
        }

        public void Edit<T>(IAssetData asset) {
            if (asset.AssetNameEquals(@"Data\ObjectInformation")) {
                IAssetDataForDictionary<int, string> objectInformation = asset.AsDictionary<int, string>();
                foreach (AddedFish fish in AddedFish.Fish) {
                    FishTraits traits = fish.FishTraits;
                    string displayName = ModFish.Translate($"{fish.AssetKey}.name");
                    string description = ModFish.Translate($"{fish.AssetKey}.description");

                    // Add to the dictionary
                    objectInformation.Set(fish.ParentSheetIndex, $"{traits.Name}/{traits.Price}/{traits.Edibility}/Fish -4/{displayName}/{description}");
                }
            } else if (asset.AssetNameEquals(@"Maps\springobjects")) {
                IAssetDataForImage springObjects = asset.AsImage();

                // Get all destinations
                Dictionary<AddedFish, Rectangle> fishDestinations = AddedFish.Fish.ToDictionary(fish => fish, fish => GameLocation.getSourceRectForObject(fish.ParentSheetIndex));

                // Get required height for texture
                int neededHeight = fishDestinations.Max(kv => kv.Value.Bottom);

                // Resize if needed
                if (neededHeight > springObjects.Data.Height) {
                    Texture2D original = springObjects.Data;
                    Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, original.Width, neededHeight);
                    springObjects.ReplaceWith(texture);
                    springObjects.PatchImage(original);
                }

                // Patch fish into texture
                foreach (KeyValuePair<AddedFish, Rectangle> fishDestination in fishDestinations) {
                    AddedFish fish = fishDestination.Key;
                    Rectangle dest = fishDestination.Value;

                    using (Texture2D fishTexture = ModFish.Instance.Helper.Content.Load<Texture2D>($"assets/{fish.AssetName}")) {
                        springObjects.PatchImage(fishTexture, null, dest);
                    }
                }
            }
        }
    }
}