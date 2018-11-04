using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Static.Extensions;
using TehPers.CoreMod.Internal.Drawing;

namespace TehPers.CoreMod.Internal.Items {
    internal static class ItemDelegator {
        private static readonly Dictionary<string, IModObject> _modObjects = new Dictionary<string, IModObject>();
        private static readonly Dictionary<string, int> _keyToIndex = new Dictionary<string, int>();
        private static readonly Dictionary<int, string> _indexToKey = new Dictionary<int, string>();
        private static bool _drawingOverridden = false;

        public const int STARTING_INDEX = 850;
        public static IEnumerable<string> RegisteredKeys => ItemDelegator._modObjects.Keys;

        public static bool Register(string key, IModObject description) {
            ItemDelegator.OverrideDrawingIfNeeded();
            return object.Equals(ItemDelegator._modObjects.GetOrAdd(key, () => description), description);
        }

        public static bool TryGetIndex(string key, out int index) {
            return ItemDelegator._keyToIndex.TryGetValue(key, out index);
        }

        public static void ClearIndexes(IMod mod) {
            mod.Monitor.Log("Removing all item indexes");
            ItemDelegator._keyToIndex.Clear();
            ItemDelegator._indexToKey.Clear();
        }

        public static void ReloadIndexes(IMod mod) {
            // Load indexes for mod items for this save
            mod.Monitor.Log("Loading indexes for save");
            Dictionary<string, int> saveIds = mod.Helper.ReadJsonFile<Dictionary<string, int>>(ItemDelegator.GetIndexPathForSave(Constants.SaveFolderName)) ?? new Dictionary<string, int>();

            const LogLevel traceLevel = LogLevel.Debug;

            // Assign indexes based on the saved data
            HashSet<string> objectKeys = new HashSet<string>(ItemDelegator._modObjects.Keys);
            foreach (KeyValuePair<string, int> kv in saveIds) {
                // Assign the new index if the key is registered
                if (objectKeys.Remove(kv.Key)) {
                    mod.Monitor.Log($" - {kv.Key} assigned index {kv.Value} from save data", traceLevel);
                    ItemDelegator._keyToIndex[kv.Key] = kv.Value;
                    ItemDelegator._indexToKey[kv.Value] = kv.Key;
                }
            }

            // Assign missing indexes
            if (objectKeys.Any()) {
                mod.Monitor.Log("New items detected, adding indexes for them", LogLevel.Info);
                int highestIndex = ItemDelegator.STARTING_INDEX.Yield().Concat(saveIds.Values).Max();
                foreach (string key in objectKeys) {
                    mod.Monitor.Log($" - {key} assigned new index {highestIndex + 1}", traceLevel);
                    ItemDelegator._keyToIndex[key] = ++highestIndex;
                    ItemDelegator._indexToKey[highestIndex] = key;
                }
            }

            // Check for any items in the save that aren't registered
            string[] notRegistered = saveIds.Keys.Except(ItemDelegator._keyToIndex.Keys).ToArray();
            if (notRegistered.Any()) {
                mod.Monitor.Log("Some items were detected in the save, but not registed:", LogLevel.Warn);

                foreach (string missingKey in notRegistered) {
                    mod.Monitor.Log($" - {missingKey}", LogLevel.Warn);
                }

                mod.Monitor.Log("Those items may be buggy and won't render correctly.", LogLevel.Warn);
            }
        }

        public static void SaveIndexes(IMod mod) {
            mod.Monitor.Log("Saving mod indexes...");
            mod.Helper.WriteJsonFile(ItemDelegator.GetIndexPathForSave(Constants.SaveFolderName), ItemDelegator._keyToIndex);
            mod.Monitor.Log("Done!");
        }

        private static string GetIndexPathForSave(string savePath) {
            return $"ids/{savePath}/ids.json";
        }

        public static bool CanEdit(IAssetInfo asset) {
            return ItemDelegator._modObjects.Values.Any(modObject => asset.AssetNameEquals(modObject.GetDataSource()));
        }

        public static void Edit(IAssetData asset) {
            // Group all mod objects by their data source
            var dataGroups = from modObjectKV in ItemDelegator._modObjects
                             where ItemDelegator._keyToIndex.ContainsKey(modObjectKV.Key)
                             let index = ItemDelegator._keyToIndex[modObjectKV.Key]
                             let key = modObjectKV.Key
                             let modObject = modObjectKV.Value
                             group new { Index = index, ModObject = modObject } by modObjectKV.Value.GetDataSource() into g
                             select g;

            // Loop through each data source to see if it matches the asset
            foreach (var dataGroup in dataGroups) {
                // Check if this data source matches
                if (!asset.AssetNameEquals(dataGroup.Key)) {
                    continue;
                }

                // Add all entries for it
                IAssetDataForDictionary<int, string> dataSource = asset.AsDictionary<int, string>();
                foreach (var entry in dataGroup) {
                    dataSource.Set(entry.Index, entry.ModObject.GetRawInformation());
                }
            }
        }

        public static void Invalidate(IMod mod) {
            mod.Monitor.Log("Invalidating data sources");
            const LogLevel traceLevel = LogLevel.Debug;
            IEnumerable<string> sources = ItemDelegator._modObjects.Values.Select(modObject => modObject.GetDataSource()).Distinct();
            foreach (string source in sources) {
                mod.Monitor.Log($" - Invalidated {source}", traceLevel);
                mod.Helper.Content.InvalidateCache(source);
            }
        }

        private static void OverrideDrawingIfNeeded() {
            if (ItemDelegator._drawingOverridden) {
                return;
            }
            ItemDelegator._drawingOverridden = true;
            
            DrawingDelegator.AddOverride(info => {
                // TODO: allow ovverring other texture sources, like TileSheets/Craftables
                if (info.Texture != Game1.objectSpriteSheet) {
                    return;
                }

                int index = DrawingDelegator.GetIndexForSourceRectangle(info.SourceRectangle ?? default);
                if (!ItemDelegator._indexToKey.TryGetValue(index, out string key) || !ItemDelegator._modObjects.TryGetValue(key, out IModObject modObject)) {
                    return;
                }

                modObject.OverrideTexture(info);
            });
        }
    }
}
