using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Items {
    internal static class ItemDelegator {
        private static readonly Dictionary<ItemKey, ObjectInformation> _registry = new Dictionary<ItemKey, ObjectInformation>();
        private static readonly Dictionary<int, ObjectInformation> _indexRegistry = new Dictionary<int, ObjectInformation>();
        private static readonly Regex _legacyKeyRegex = new Regex("^(?<modId>[^:]+):(?<localKey>.*)$");
        private static bool _drawingOverridden = false;

        public const int STARTING_INDEX = 100000;
        public static IEnumerable<IObjectInformation> RegisteredObjects => ItemDelegator._registry.Values;

        public static bool Register(ItemKey key, ISpriteSheet parentSheet, IModObject objectManager) {
            if (ItemDelegator._registry.ContainsKey(key)) {
                return false;
            }

            // Add the item to the item registry
            ObjectInformation objectInformation = new ObjectInformation(key, objectManager);
            ItemDelegator._registry.Add(key, objectInformation);

            // Override when the item is drawn
            parentSheet.Drawing += (sender, info) => {
                if (parentSheet.GetIndex(info.SourceRectangle?.X ?? 0, info.SourceRectangle?.Y ?? 0) == objectInformation.Index) {
                    objectManager.OverrideDraw(info);
                }
            };

            return true;
        }

        public static bool TryGetInformation(ItemKey key, out IObjectInformation info) {
            if (ItemDelegator._registry.TryGetValue(key, out ObjectInformation objectInfo)) {
                info = objectInfo;
                return true;
            }

            info = default;
            return false;
        }

        public static bool TryGetInformation(int index, out IObjectInformation info) {
            if (ItemDelegator._indexRegistry.TryGetValue(index, out ObjectInformation objectInfo)) {
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

        public static void ReloadIndexes(IMod mod, Dictionary<ItemKey, int> saveIds) {
            // Load indexes for mod items for this save
            mod.Monitor.Log("Assigning indexes...", LogLevel.Debug);

            // Get all registered global object keys
            HashSet<ItemKey> registeredKeys = new HashSet<ItemKey>(ItemDelegator._registry.Keys);

            // Assign indexes based on the saved data
            foreach (KeyValuePair<ItemKey, int> kv in saveIds) {
                // Check if the key exists and is removed
                if (!registeredKeys.Remove(kv.Key)) continue;

                // Assign the index to that key
                mod.Monitor.Log($" - {kv.Key} assigned index {kv.Value} from save data", LogLevel.Debug);
                ItemDelegator._registry[kv.Key].SetIndex(kv.Value, ItemDelegator._indexRegistry);
            }

            // Assign missing indexes
            if (registeredKeys.Any()) {
                mod.Monitor.Log("New items detected, adding indexes for them:", LogLevel.Info);
                int highestIndex = ItemDelegator.STARTING_INDEX.Yield().Concat(saveIds.Values).Max();
                foreach (ItemKey key in registeredKeys) {
                    mod.Monitor.Log($" - {key} assigned new index {highestIndex + 1}");
                    ItemDelegator._registry[key].SetIndex(++highestIndex, ItemDelegator._indexRegistry);
                }
            }

            // Check for any items in the save that aren't registered
            // TODO: Lock these IDs from being assigned to anything so existing (broken) items with those IDs aren't treated as different items suddenly
            ItemKey[] notRegistered = saveIds.Keys.Except(ItemDelegator._registry.Keys).ToArray();
            if (notRegistered.Any()) {
                mod.Monitor.Log("Some items were detected in the save, but not registered:", LogLevel.Warn);

                foreach (ItemKey missingKey in notRegistered) {
                    mod.Monitor.Log($" - {missingKey}", LogLevel.Warn);
                }

                mod.Monitor.Log("Those items may be buggy and won't render correctly.", LogLevel.Warn);
            }
        }

        public static void ReloadIndexes(IMod mod) {
            // Load indexes for mod items for this save
            mod.Monitor.Log("Loading indexes from save...", LogLevel.Trace);

            // Try to load the custom indexes
            if (!(mod.Helper.Data.ReadSaveData<Dictionary<ItemKey, int>>("indexes2") is Dictionary<ItemKey, int> saveIds)) {
                saveIds = new Dictionary<ItemKey, int>();

                // Try to load legacy indexes
                if (mod.Helper.Data.ReadSaveData<Dictionary<string, int>>("indexes") is Dictionary<string, int> legacyIds) {
                    // Try to convert each legacy key into a new key
                    foreach (KeyValuePair<string, int> kv in legacyIds) {
                        Match match = ItemDelegator._legacyKeyRegex.Match(kv.Key);
                        if (match.Success) {
                            // Get info about the owner of the mod
                            IModInfo owner = mod.Helper.ModRegistry.Get(match.Groups["modId"].Value);
                            if (owner != null) {
                                // Create the item key and add it to the save IDs
                                saveIds.Add(new ItemKey(owner.Manifest, match.Groups["localKey"].Value), kv.Value);
                            } else {
                                mod.Monitor.Log($"Error parsing legacy key \"{kv.Key}\". No mod with unique ID \"{match.Groups["modId"].Value}\" was found. A new index will be assigned to items with this key, which may cause issues with existing save files.", LogLevel.Warn);
                            }
                        } else {
                            mod.Monitor.Log($"Error parsing legacy key \"{kv.Key}\". A new index will be assigned to items with this key, which may cause issues with existing save files.", LogLevel.Warn);
                        }
                    }
                }
            }

            ItemDelegator.ReloadIndexes(mod, saveIds);
        }

        public static Dictionary<ItemKey, int> GetIndexDictionary() {
            // Add all registered objects to a dictionary
            Dictionary<ItemKey, int> indexes = new Dictionary<ItemKey, int>();
            foreach (KeyValuePair<ItemKey, ObjectInformation> kv in ItemDelegator._registry) {
                if (kv.Value.Index is int index) {
                    indexes.Add(kv.Key, index);
                }
            }

            return indexes;
        }

        public static void SaveIndexes(IMod mod) {
            mod.Monitor.Log("Saving mod indexes...", LogLevel.Trace);

            // Save the dictionary
            mod.Helper.Data.WriteSaveData("indexes2", ItemDelegator.GetIndexDictionary());

            mod.Monitor.Log("Done!", LogLevel.Trace);
        }

        public static void RegisterMultiplayerEvents(IMod mod) {
            // Whenever item information is received, handle the message
            mod.Helper.Events.Multiplayer.ModMessageReceived += (sender, args) => {
                // Check if the message is from this mod
                if (args.FromModID != mod.ModManifest.UniqueID) {
                    return;
                }

                // Check the type of message
                if (args.Type == "indexes") {
                    // Process new item information
                    mod.Monitor.Log("Received item information from host", LogLevel.Info);
                    ItemDelegator.ReloadIndexes(mod, args.ReadAs<Dictionary<ItemKey, int>>());
                }
            };

            // Whenever a player connects to a game hosted by the current player, send them the required item information
            mod.Helper.Events.Multiplayer.PeerContextReceived += (sender, args) => {
                // Check if host
                if (!Context.IsMainPlayer) {
                    return;
                }

                // Check if the peer has this mod installed
                if (!args.Peer.HasSmapi || args.Peer.Mods.All(m => m.ID != mod.ModManifest.UniqueID)) {
                    return;
                }

                // Send item information
                mod.Monitor.Log($"Sending item information to new peer ({args.Peer.PlayerID})", LogLevel.Info);
                mod.Helper.Multiplayer.SendMessage(ItemDelegator.GetIndexDictionary(), "indexes", new[] { mod.ModManifest.UniqueID }, new[] { args.Peer.PlayerID });
            };
        }

        public static void RegisterSaveEvents(IMod mod) {
            // Whenever a save is loaded, load item information unless the player is a farmhand on a multiplayer server
            mod.Helper.Events.GameLoop.SaveLoaded += (sender, args) => {
                // Check if this player is a farmhand on a multiplayer server
                if (Context.IsMultiplayer && !Context.IsMainPlayer) {
                    return;
                }

                // Load all the key <-> index mapping for this save
                ItemDelegator.ReloadIndexes(mod);

                // Reload object data
                ItemDelegator.Invalidate(mod);
            };

            // Whenever the game is save, store the item information in the save as well
            mod.Helper.Events.GameLoop.Saving += (sender, args) => ItemDelegator.SaveIndexes(mod);

            // Whenever the player exits a save, reset all the indexes
            mod.Helper.Events.GameLoop.ReturnedToTitle += (sender, args) => ItemDelegator.ClearIndexes(mod);
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
                    dataSource[entry.Index] = entry.ModObject.GetRawObjectInformation();
                }
            }
        }

        public static void Invalidate(IMod mod) {
            mod.Monitor.Log("Invalidating data sources", LogLevel.Trace);

            // Get all data sources
            IEnumerable<string> sources = ItemDelegator._registry.Values.Select(info => info.Manager.GetDataSource()).Distinct();

            // Invalidate each one
            foreach (string source in sources) {
                mod.Monitor.Log($" - Invalidated {source}", LogLevel.Trace);
                mod.Helper.Content.InvalidateCache(source);
            }
        }

        internal static void OverrideDrawingIfNeeded(IDrawingApi drawingApi, TextureAssetTracker tracker) {
            if (ItemDelegator._drawingOverridden) {
                return;
            }
            ItemDelegator._drawingOverridden = true;

            // ITrackedTexture helper = drawingApi.GetTrackedTexture(new AssetLocation("Maps/springobjects", ContentSource.GameContent));
            // helper.Drawing += (sender, info) => {
            //     // Make sure that only a portion of the texture is being drawn
            //     if (!(info.SourceRectangle is Rectangle sourceRectangle)) {
            //         return;
            //     }
            // 
            //     // TODO: Get the items that override this texture
            //     foreach (ObjectInformation objectInfo in ItemDelegator._indexRegistry.Values) {
            //         // ITextureSourceInfo textureInfo = objectInfo.Manager.GetSprite();
            //         // 
            //         // // Make sure the index of the object matches the source region
            //         // if (objectInfo.Index != textureInfo.GetIndexFromUV(sourceRectangle.X, sourceRectangle.Y)) {
            //         //     continue;
            //         // }
            //         // 
            //         // // Override the drawing call
            //         // objectInfo.Manager.OverrideTexture(info);
            //         // return;
            //     }
            // };
        }
    }
}
