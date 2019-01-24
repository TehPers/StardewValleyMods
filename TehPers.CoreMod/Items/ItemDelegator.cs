using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.ItemProviders;

namespace TehPers.CoreMod.Items {
    // internal static class ItemDelegator_ {
    //     private static readonly Dictionary<ItemKey, ObjectInformation> _registry = new Dictionary<ItemKey, ObjectInformation>();
    //     private static readonly Dictionary<int, ObjectInformation> _indexRegistry = new Dictionary<int, ObjectInformation>();
    //     private static Dictionary<ItemKey, int> _saveData = new Dictionary<ItemKey, int>();
    //     private static readonly Regex _legacyKeyRegex = new Regex("^(?<modId>[^:]+):(?<localKey>.*)$");
    //     private static readonly List<IItemProvider> _itemProviders = new List<IItemProvider>();
    // 
    //     public const int STARTING_INDEX = 100000;
    //     public static IEnumerable<IObjectInformation> RegisteredObjects => ItemDelegator._registry.Values;
    // 
    //     public static bool Register(ItemKey key, ISpriteSheet parentSheet, IModObject objectManager) {
    //         if (ItemDelegator._registry.ContainsKey(key)) {
    //             return false;
    //         }
    // 
    //         // Add the item to the item registry
    //         ObjectInformation objectInformation = new ObjectInformation(key, objectManager);
    //         ItemDelegator._registry.Add(key, objectInformation);
    // 
    //         // Override when the item is drawn
    //         parentSheet.Drawing += (sender, info) => {
    //             if (parentSheet.GetIndex(info.SourceRectangle?.X ?? 0, info.SourceRectangle?.Y ?? 0) == objectInformation.Index) {
    //                 objectManager.OverrideDraw(info);
    //             }
    //         };
    // 
    //         return true;
    //     }
    // 
    //     public static bool TryGetInformation(ItemKey key, out IObjectInformation info) {
    //         if (ItemDelegator._registry.TryGetValue(key, out ObjectInformation objectInfo)) {
    //             info = objectInfo;
    //             return true;
    //         }
    // 
    //         info = default;
    //         return false;
    //     }
    // 
    //     public static bool TryGetInformation(int index, out IObjectInformation info) {
    //         if (ItemDelegator._indexRegistry.TryGetValue(index, out ObjectInformation objectInfo)) {
    //             info = objectInfo;
    //             return true;
    //         }
    // 
    //         info = default;
    //         return false;
    //     }
    // 
    //     public static bool TryCreate(ItemKey key, out Item item) {
    //         foreach (IItemProvider provider in ItemDelegator._itemProviders) {
    //             if (provider.TryCreateItem(key, out item)) {
    //                 return true;
    //             }
    //         }
    // 
    //         item = default;
    //         return false;
    //     }
    // 
    //     public static void AddItemProvider(IItemProvider itemProvider) {
    //         ItemDelegator._itemProviders.Add(itemProvider);
    //     }
    // 
    //     public static void ClearIndexes(IMod mod) {
    //         mod.Monitor.Log("Removing all item indexes");
    // 
    //         // Remove the index for each object with an index assigned to it
    //         foreach (ObjectInformation info in ItemDelegator._indexRegistry.Values.ToArray()) {
    //             info.RemoveIndex(ItemDelegator._indexRegistry);
    //         }
    // 
    //         // This line shouldn't do anything normally
    //         ItemDelegator._indexRegistry.Clear();
    //     }
    // 
    //     public static void ReloadIndexes(IMod mod, Dictionary<ItemKey, int> saveIds, Action<IEnumerable<(ItemKey key, int index)>> missingItemCallback) {
    //         ItemDelegator._saveData = saveIds;
    // 
    //         // Load indexes for mod items for this save
    //         mod.Monitor.Log("Assigning indexes...", LogLevel.Info);
    // 
    //         // Get all registered global object keys
    //         HashSet<ItemKey> registeredKeys = new HashSet<ItemKey>(ItemDelegator._registry.Keys);
    // 
    //         // Assign indexes based on the saved data
    //         foreach ((ItemKey key, int index) in saveIds) {
    //             // Check if the key exists and is removed
    //             if (!registeredKeys.Remove(key)) continue;
    // 
    //             // Assign the index to that key
    //             mod.Monitor.Log($" - {key} assigned index {index} from save data", LogLevel.Debug);
    //             ItemDelegator._registry[key].SetIndex(index, ItemDelegator._indexRegistry);
    //         }
    // 
    //         // Assign missing indexes
    //         if (registeredKeys.Any()) {
    //             mod.Monitor.Log("New items detected, adding indexes for them:", LogLevel.Info);
    //             int highestIndex = ItemDelegator.STARTING_INDEX.Yield().Concat(saveIds.Values).Max();
    //             foreach (ItemKey key in registeredKeys) {
    //                 mod.Monitor.Log($" - {key} assigned new index {highestIndex + 1}");
    //                 ItemDelegator._registry[key].SetIndex(++highestIndex, ItemDelegator._indexRegistry);
    //             }
    //         }
    // 
    //         // Check for any items in the save that aren't registered
    //         HashSet<ItemKey> registered = ItemDelegator._registry.Keys.ToHashSet();
    // 
    //         (ItemKey Key, int Value)[] notRegistered = saveIds.Where(kv => !registered.Contains(kv.Key)).Select(kv => (kv.Key, kv.Value)).ToArray();
    //         if (notRegistered.Any()) {
    //             missingItemCallback?.Invoke(notRegistered);
    //         }
    //     }
    // 
    //     public static void ReloadIndexes(IMod mod) {
    //         // Load indexes for mod items for this save
    //         mod.Monitor.Log("Loading indexes from save...", LogLevel.Trace);
    // 
    //         // Try to load the custom indexes
    //         if (!(mod.Helper.Data.ReadSaveData<Dictionary<ItemKey, int>>("indexes2") is Dictionary<ItemKey, int> saveIds)) {
    //             saveIds = new Dictionary<ItemKey, int>();
    // 
    //             // Try to load legacy indexes
    //             if (mod.Helper.Data.ReadSaveData<Dictionary<string, int>>("indexes") is Dictionary<string, int> legacyIds) {
    //                 // Try to convert each legacy key into a new key
    //                 foreach ((string key, int value) in legacyIds) {
    //                     Match match = ItemDelegator._legacyKeyRegex.Match(key);
    //                     if (match.Success) {
    //                         // Get info about the owner of the mod
    //                         IModInfo owner = mod.Helper.ModRegistry.Get(match.Groups["modId"].Value);
    //                         if (owner != null) {
    //                             // Create the item key and add it to the save IDs
    //                             saveIds.Add(new ItemKey(owner.Manifest, match.Groups["localKey"].Value), value);
    //                         } else {
    //                             mod.Monitor.Log($"Error parsing legacy key \"{key}\". No mod with unique ID \"{match.Groups["modId"].Value}\" was found. A new index will be assigned to items with this key, which may cause issues with existing save files.", LogLevel.Warn);
    //                         }
    //                     } else {
    //                         mod.Monitor.Log($"Error parsing legacy key \"{key}\". A new index will be assigned to items with this key, which may cause issues with existing save files.", LogLevel.Warn);
    //                     }
    //                 }
    //             }
    //         }
    // 
    //         ItemDelegator.ReloadIndexes(mod, saveIds, missingItems => {
    //             mod.Monitor.Log("Some items were detected in the save, but not registered:", LogLevel.Error);
    // 
    //             foreach ((ItemKey key, int index) in missingItems) {
    //                 mod.Monitor.Log($" - {key} (#{index})", LogLevel.Error);
    //             }
    // 
    //             mod.Monitor.Log("Those items may be buggy and won't render correctly. (They will become \"Error Item\"s)", LogLevel.Error);
    //         });
    //     }
    // 
    //     public static Dictionary<ItemKey, int> GetIndexDictionary() {
    //         // Add all registered objects to a dictionary
    //         Dictionary<ItemKey, int> indexes = new Dictionary<ItemKey, int>();
    //         foreach ((ItemKey key, ObjectInformation objectInformation) in ItemDelegator._registry) {
    //             if (objectInformation.Index is int index) {
    //                 indexes.Add(key, index);
    //             }
    //         }
    // 
    //         return indexes;
    //     }
    // 
    //     public static void SaveIndexes(IMod mod) {
    //         mod.Monitor.Log("Saving mod indexes...", LogLevel.Trace);
    // 
    //         // Update save data dictionary
    //         foreach ((ItemKey key, int index) in ItemDelegator.GetIndexDictionary()) {
    //             ItemDelegator._saveData[key] = index;
    //         }
    // 
    //         // Save the dictionary
    //         mod.Helper.Data.WriteSaveData("indexes2", ItemDelegator._saveData);
    // 
    //         mod.Monitor.Log("Done!", LogLevel.Trace);
    //     }
    // 
    //     public static void RegisterMultiplayerEvents(IMod mod) {
    //         // Whenever item information is received, handle the message
    //         mod.Helper.Events.Multiplayer.ModMessageReceived += (sender, args) => {
    //             // Check if the message is from this mod
    //             if (args.FromModID != mod.ModManifest.UniqueID) {
    //                 return;
    //             }
    // 
    //             // Check the type of message
    //             if (args.Type == "indexes") {
    //                 // Process new item information
    //                 mod.Monitor.Log("Received item information from host", LogLevel.Info);
    //                 ItemDelegator.ReloadIndexes(mod, args.ReadAs<Dictionary<ItemKey, int>>(), missingItems => {
    //                     mod.Monitor.Log("Some items were registered by the server, but not by your client:", LogLevel.Error);
    // 
    //                     foreach ((ItemKey key, int index) in missingItems) {
    //                         mod.Monitor.Log($" - {key} (#{index})", LogLevel.Error);
    //                     }
    // 
    //                     mod.Monitor.Log("Those items may be buggy and won't render correctly. (They will appear as \"Error Item\"s for you)", LogLevel.Error);
    //                 });
    //             }
    //         };
    // 
    //         // Whenever a player connects to a game hosted by the current player, send them the required item information
    //         mod.Helper.Events.Multiplayer.PeerContextReceived += (sender, args) => {
    //             // Check if host
    //             if (!Context.IsMainPlayer) {
    //                 return;
    //             }
    // 
    //             // Check if the peer has this mod installed
    //             if (!args.Peer.HasSmapi || args.Peer.Mods.All(m => m.ID != mod.ModManifest.UniqueID)) {
    //                 return;
    //             }
    // 
    //             // Send item information
    //             mod.Monitor.Log($"Sending item information to new peer ({args.Peer.PlayerID})", LogLevel.Info);
    //             mod.Helper.Multiplayer.SendMessage(ItemDelegator.GetIndexDictionary(), "indexes", new[] { mod.ModManifest.UniqueID }, new[] { args.Peer.PlayerID });
    //         };
    //     }
    // 
    //     public static void RegisterSaveEvents(IMod mod) {
    //         // Whenever a save is loaded, load item information unless the player is a farmhand on a multiplayer server
    //         mod.Helper.Events.GameLoop.SaveLoaded += (sender, args) => {
    //             // Check if this player is a farmhand on a multiplayer server
    //             if (Context.IsMultiplayer && !Context.IsMainPlayer) {
    //                 return;
    //             }
    // 
    //             // Load all the key <-> index mapping for this save
    //             ItemDelegator.ReloadIndexes(mod);
    // 
    //             // Reload object data
    //             ItemDelegator.Invalidate(mod);
    //         };
    // 
    //         // Whenever the game is save, store the item information in the save as well
    //         mod.Helper.Events.GameLoop.Saving += (sender, args) => ItemDelegator.SaveIndexes(mod);
    // 
    //         // Whenever the player exits a save, reset all the indexes
    //         mod.Helper.Events.GameLoop.ReturnedToTitle += (sender, args) => ItemDelegator.ClearIndexes(mod);
    //     }
    // 
    //     public static bool CanEdit(IAssetInfo asset) {
    //         return ItemDelegator._indexRegistry.Values.Any(modObject => asset.AssetNameEquals(modObject.Manager.GetDataSource()));
    //     }
    // 
    //     public static void Edit(IAssetData asset) {
    //         // Group all mod objects by their data source
    //         var dataGroups = from kv in ItemDelegator._indexRegistry
    //                          let manager = kv.Value.Manager
    //                          group new { Index = kv.Key, ModObject = manager } by manager.GetDataSource() into g
    //                          select g;
    // 
    //         // Loop through each data source to see if it matches the asset
    //         foreach (var dataGroup in dataGroups) {
    //             // Check if this data source matches
    //             if (!asset.AssetNameEquals(dataGroup.Key)) {
    //                 continue;
    //             }
    // 
    //             // Add all entries for it
    //             IDictionary<int, string> dataSource = asset.AsDictionary<int, string>().Data;
    //             foreach (var entry in dataGroup) {
    //                 dataSource[entry.Index] = entry.ModObject.GetRawObjectInformation();
    //             }
    //         }
    //     }
    // 
    //     public static void Invalidate(IMod mod) {
    //         mod.Monitor.Log("Invalidating data sources", LogLevel.Trace);
    // 
    //         // Get all data sources
    //         IEnumerable<string> sources = ItemDelegator._registry.Values.Select(info => info.Manager.GetDataSource()).Distinct();
    // 
    //         // Invalidate each one
    //         foreach (string source in sources) {
    //             mod.Monitor.Log($" - Invalidated {source}", LogLevel.Trace);
    //             mod.Helper.Content.InvalidateCache(source);
    //         }
    //     }
    // }
}
