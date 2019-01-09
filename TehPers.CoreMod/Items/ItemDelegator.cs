using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Drawing;

namespace TehPers.CoreMod.Items {
    internal static class ItemDelegator {
        private static readonly Dictionary<string, ObjectInformation> _registry = new Dictionary<string, ObjectInformation>();
        private static readonly Dictionary<int, ObjectInformation> _indexRegistry = new Dictionary<int, ObjectInformation>();
        private static bool _drawingOverridden = false;

        public const int STARTING_INDEX = 100000;
        public static IEnumerable<IObjectInformation> RegisteredObjects => ItemDelegator._registry.Values;

        public static bool Register(string key, IModObject objectManager) {
            if (ItemDelegator._registry.ContainsKey(key)) {
                return false;
            }

            ItemDelegator._registry.Add(key, new ObjectInformation(key, objectManager));
            return true;
        }

        public static bool TryGetInformation(string key, out IObjectInformation info) {
            if (ItemDelegator._registry.TryGetValue(key, out var objectInfo)) {
                info = objectInfo;
                return true;
            }

            info = default;
            return false;
        }

        public static bool TryGetInformation(int index, out IObjectInformation info) {
            if (ItemDelegator._indexRegistry.TryGetValue(index, out var objectInfo)) {
                info = objectInfo;
                return true;
            }

            info = default;
            return false;
        }

        public static void ClearIndexes(IMod mod) {
            mod.Monitor.Log("Removing all item indexes");

            // Remove the index for each object with an index assigned to it
            foreach (ObjectInformation info in ItemDelegator._indexRegistry.Values.ToArray()) {
                info.RemoveIndex(ItemDelegator._indexRegistry);
            }

            // This line shouldn't do anything normally
            ItemDelegator._indexRegistry.Clear();
        }

        public static void ReloadIndexes(IMod mod) {
            // Load indexes for mod items for this save
            // TODO: use custom json api
            mod.Monitor.Log("Loading indexes for save");
            Dictionary<string, int> saveIds = mod.Helper.Data.ReadJsonFile<Dictionary<string, int>>(ItemDelegator.GetIndexPathForSave(Constants.SaveFolderName)) ?? new Dictionary<string, int>();

            // Get all registered object keys
            HashSet<string> objectKeys = new HashSet<string>(ItemDelegator._registry.Keys);

            // Assign indexes based on the saved data
            foreach (KeyValuePair<string, int> kv in saveIds) {
                // Assign the new index if the key is registered, removing it from the set
                if (objectKeys.Remove(kv.Key)) {
                    mod.Monitor.Log($" - {kv.Key} assigned index {kv.Value} from save data");
                    ItemDelegator._registry[kv.Key].SetIndex(kv.Value, ItemDelegator._indexRegistry);
                }
            }

            // Assign missing indexes
            if (objectKeys.Any()) {
                mod.Monitor.Log("New items detected, adding indexes for them", LogLevel.Info);
                int highestIndex = ItemDelegator.STARTING_INDEX.Yield().Concat(saveIds.Values).Max();
                foreach (string key in objectKeys) {
                    mod.Monitor.Log($" - {key} assigned new index {highestIndex + 1}");
                    ItemDelegator._registry[key].SetIndex(++highestIndex, ItemDelegator._indexRegistry);
                }
            }

            // Check for any items in the save that aren't registered
            string[] notRegistered = saveIds.Keys.Except(ItemDelegator._registry.Keys).ToArray();
            if (notRegistered.Any()) {
                mod.Monitor.Log("Some items were detected in the save, but not registered:", LogLevel.Warn);

                foreach (string missingKey in notRegistered) {
                    mod.Monitor.Log($" - {missingKey}", LogLevel.Warn);
                }

                mod.Monitor.Log("Those items may be buggy and won't render correctly.", LogLevel.Warn);
            }
        }

        public static void SaveIndexes(IMod mod) {
            mod.Monitor.Log("Saving mod indexes...");

            // Add all registered objects to a dictionary
            Dictionary<string, int> saveData = new Dictionary<string, int>();
            foreach (KeyValuePair<string, ObjectInformation> kv in ItemDelegator._registry) {
                if (kv.Value.Index is int index) {
                    saveData.Add(kv.Key, index);
                }
            }

            // Save the dictionary
            // TODO: use custom json api for minified json file
            mod.Helper.Data.WriteJsonFile(ItemDelegator.GetIndexPathForSave(Constants.SaveFolderName), saveData);

            mod.Monitor.Log("Done!");
        }

        private static string GetIndexPathForSave(string savePath) {
            return $"ids/{savePath}/ids.json";
        }

        public static bool CanEdit(IAssetInfo asset) {
            return ItemDelegator._indexRegistry.Values.Any(modObject => asset.AssetNameEquals(modObject.Manager.GetDataSource()));
        }

        public static void Edit(IAssetData asset) {
            // Group all mod objects by their data source
            var dataGroups = from kv in ItemDelegator._indexRegistry
                             let manager = kv.Value.Manager
                             group new { Index = kv.Key, ModObject = manager } by manager.GetDataSource() into g
                             select g;

            // Loop through each data source to see if it matches the asset
            foreach (var dataGroup in dataGroups) {
                // Check if this data source matches
                if (!asset.AssetNameEquals(dataGroup.Key)) {
                    continue;
                }

                // Add all entries for it
                IDictionary<int, string> dataSource = asset.AsDictionary<int, string>().Data;
                foreach (var entry in dataGroup) {
                    dataSource[entry.Index] = entry.ModObject.GetRawInformation();
                }
            }
        }

        public static void Invalidate(IMod mod) {
            mod.Monitor.Log("Invalidating data sources");

            // Get all data sources
            IEnumerable<string> sources = ItemDelegator._registry.Values.Select(info => info.Manager.GetDataSource()).Distinct();

            // Invalidate each one
            foreach (string source in sources) {
                mod.Monitor.Log($" - Invalidated {source}");
                mod.Helper.Content.InvalidateCache(source);
            }
        }

        internal static void OverrideDrawingIfNeeded(TextureAssetTracker tracker) {
            if (ItemDelegator._drawingOverridden) {
                return;
            }
            ItemDelegator._drawingOverridden = true;

            // TODO
            /*
            DrawingDelegator.AddOverride(info => {
                // Make sure that only a portion of the texture is being drawn
                if (!(info.SourceRectangle is Rectangle sourceRectangle)) {
                    return;
                }

                // Try to get the tracked texture name
                if (!tracker.TryGetTracked(info.Texture, out string textureName)) {
                    return;
                }

                // Get the items that override this texture
                foreach (ObjectInformation objectInfo in ItemDelegator._indexRegistry.Values) {
                    ITextureSourceInfo textureInfo = objectInfo.Manager.GetTextureSource();
                    if (!string.Equals(textureInfo.TextureName, textureName, StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }

                    // Make sure the index of the object matches the source region
                    if (objectInfo.Index != textureInfo.GetIndexFromUV(sourceRectangle.X, sourceRectangle.Y)) {
                        continue;
                    }

                    // Override the drawing call
                    objectInfo.Manager.OverrideTexture(info);
                    return;
                }
            });
            */
        }

        private class ObjectInformation : IObjectInformation {
            /// <inheritdoc />
            public int? Index { get; private set; }

            /// <inheritdoc />
            public IModObject Manager { get; }

            /// <inheritdoc />
            public string Key { get; }

            public ObjectInformation(string key, IModObject manager) : this(key, manager, null) { }
            public ObjectInformation(string key, IModObject manager, int? index) {
                this.Key = key;
                this.Manager = manager;
                this.Index = index;
            }

            internal void SetIndex(int index, IDictionary<int, ObjectInformation> indexDict) {
                if (this.Index is int curIndex) {
                    indexDict.Remove(curIndex);
                }

                indexDict.Add(index, this);
                this.Index = index;
            }

            internal void RemoveIndex(IDictionary<int, ObjectInformation> indexDict) {
                if (this.Index is int curIndex) {
                    indexDict.Remove(curIndex);
                }
            }
        }
    }
}
