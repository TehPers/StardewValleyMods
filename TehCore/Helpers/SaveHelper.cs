using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using TehCore.Enums;
using TehCore.Helpers.Json;
using TehCore.Saves;
using SObject = StardewValley.Object;
using Type = System.Type;

namespace TehCore.Helpers {
    public class SaveHelper {
        private static readonly MethodInfo _saveMethod = typeof(SaveHelper).GetMethod(nameof(SaveHelper.GetSaveData), BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo _loadMethod = typeof(SaveHelper).GetMethod(nameof(SaveHelper.LoadSaveData), BindingFlags.Static | BindingFlags.NonPublic);

        public void SerializeCustomItems() {
            Dictionary<InventorySource, IList<Item>> inventories = new Dictionary<InventorySource, IList<Item>>();

            // Player inventory
            inventories.Add(new InventorySource(InventoryType.PLAYER, null, Vector2.Zero, false), Game1.player.Items);

            foreach (GameLocation location in Game1.locations) {
                foreach (SerializableDictionary<Vector2, SObject> objectDict in location.Objects) {
                    foreach (KeyValuePair<Vector2, SObject> kv in objectDict) {
                        switch (kv.Value) {
                            case Chest chest:
                                // Chests
                                inventories.Add(new InventorySource(InventoryType.CHEST, location.Name, kv.Key, location.isStructure.Value), chest.items);
                                break;
                            case IStorageObject storageObject:
                                // Custom storage objects
                                // TODO: actually add support for custom game objects
                                inventories.Add(new InventorySource(InventoryType.CUSTOM, location.Name, kv.Key, location.isStructure.Value), storageObject.Inventory);
                                break;
                        }
                    }
                }
            }

            // Remove all the items in those inventories
            this.SerializeCustomItems(inventories);
        }

        private void SerializeCustomItems(IDictionary<InventorySource, IList<Item>> inventories) {
            string path = Path.Combine(Constants.CurrentSavePath, "tpc-inventories.json");
            List<CustomItemData> itemData = new List<CustomItemData>();

            // Loop through each inventory
            foreach (KeyValuePair<InventorySource, IList<Item>> inventoryKV in inventories) {
                // Loop through each item in the inventory
                for (int slot = 0; slot < inventoryKV.Value.Count; slot++) {
                    // Get the current item
                    Item item = inventoryKV.Value[slot];
                    if (item == null)
                        continue;

                    // Check if that item is ICustomItem<T>
                    Type itemType = item.GetType();
                    Type interfaceType = itemType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICustomItem<>));
                    if (interfaceType != null) {
                        // Make sure the item can be deserialized
                        if (itemType.GetConstructor(Type.EmptyTypes) == null) {
                            ModCore.Instance.Monitor.Log($"Unable to save item of type {itemType.FullName} because it doesn't contain a default constructor", LogLevel.Error);
                            continue;
                        }

                        // Get the save data
                        object saveData = SaveHelper._saveMethod.MakeGenericMethod(interfaceType.GetGenericArguments()).Invoke(null, new object[] { item });
                        itemData.Add(new CustomItemData(inventoryKV.Key, slot, saveData, item.GetType()));

                        // Remove the custom item to avoid issues while serializing
                        inventoryKV.Value[slot] = null;
                    }
                }
            }

            // Write the custom items to the save
            ModCore.Instance.JsonHelper.WriteJson(path, itemData, ModCore.Instance.Helper, settings => { settings.TypeNameHandling = TypeNameHandling.All; }, true);
        }

        public void DeserializeCustomItems() {
            // Load the custom item data
            string path = Path.Combine(Constants.CurrentSavePath, "tpc-inventories.json");
            List<CustomItemData> itemData = ModCore.Instance.JsonHelper.ReadJson<List<CustomItemData>>(path, null, settings => { settings.TypeNameHandling = TypeNameHandling.All; });
            if (itemData == null)
                return;

            // Construct each item
            foreach (CustomItemData data in itemData) {
                // Check if that item is ICustomItem<T>
                Type itemType = data.ItemType;
                Type interfaceType = itemType?.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICustomItem<>));
                if (interfaceType != null) {
                    // Make sure the item can be deserialized
                    ConstructorInfo constructor = itemType.GetConstructor(Type.EmptyTypes);
                    if (constructor == null) {
                        ModCore.Instance.Monitor.Log($"Unable to load item of type {itemType.FullName} because it doesn't contain a default constructor", LogLevel.Error);
                        continue;
                    }

                    // Find the inventory to put the item in
                    IList<Item> inventory = null;
                    if (data.Source.Type == InventoryType.PLAYER) {
                        inventory = Game1.player.Items;
                    } else {
                        GameLocation location = Game1.getLocationFromName(data.Source.LocationName, data.Source.IsStructure);
                        if (data.Source.Type == InventoryType.CHEST) {
                            if (location.Objects.TryGetValue(data.Source.Position, out SObject obj) && obj is Chest chest) {
                                inventory = chest.items;
                            }
                        } else if (data.Source.Type == InventoryType.CHEST) {
                            if (location.Objects.TryGetValue(data.Source.Position, out SObject obj) && obj is Chest chest) {
                                inventory = chest.items;
                            }
                        }
                    }

                    // Put the item in the inventory
                    if (inventory == null) {
                        ModCore.Instance.Monitor.Log("Failed to find inventory for an item", LogLevel.Error);
                    } else {
                        // Create the item
                        Item item = constructor.Invoke(new object[0]) as Item;
                        SaveHelper._loadMethod.MakeGenericMethod(interfaceType.GetGenericArguments()).Invoke(null, new[] { item, data.Data });

                        // Put it in the inventory
                        inventory[data.Slot] = item;
                    }
                } else {
                    ModCore.Instance.Monitor.Log($"{itemType?.FullName} is not an ICustomItem, skipping", LogLevel.Error);
                }
            }
        }

        private static object GetSaveData<TModel>(ICustomItem<TModel> item) {
            return item.Save();
        }

        private static void LoadSaveData<TModel>(ICustomItem<TModel> item, TModel model) {
            item.Load(model);
        }

        /// <summary>Converts all XML serializable fields/properties on an object to a dictionary containing the tag name and value. This is useful for serializing an object designed for XML serialization into a JSON file.</summary>
        /// <typeparam name="T">The type of object to convert to a dictionary.</typeparam>
        /// <param name="source">The object to convert to a dictionary.</param>
        /// <returns>A dictionary containing all the XML tag names and values.</returns>
        public static Dictionary<string, object> XmlSerializableToDictionary<T>(T source) {
            // Get all the types the source has
            Stack<Type> types = new Stack<Type>();
            Type curType = typeof(T);
            while (curType != null && curType != typeof(object)) {
                types.Push(curType);
                curType = curType.BaseType;
            }

            // Go through each type
            Dictionary<string, object> dict = new Dictionary<string, object>();
            while (types.Any()) {
                Type type = types.Pop();

                // Get all serializable properties
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                    if (property.GetCustomAttribute<XmlIgnoreAttribute>() != null)
                        continue;

                    // Make sure it's serializable
                    if (property.CanRead && property.CanWrite) {
                        // Get name
                        XmlElementAttribute elementAttribute = property.GetCustomAttribute<XmlElementAttribute>();
                        string name = elementAttribute?.ElementName ?? property.Name;

                        // Get value
                        if (!dict.ContainsKey(name)) {
                            dict.Add($"{type.FullName}.{name}", property.GetValue(source));
                        } else {
                            ModCore.Instance.Monitor.Log("XML serializable object contains duplicate members with the same tag name. Only one will be serialized.", LogLevel.Warn);
                        }
                    }
                }

                // Get all serializable fields
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                    if (field.GetCustomAttribute<XmlIgnoreAttribute>() != null)
                        continue;

                    // Get name
                    XmlElementAttribute elementAttribute = field.GetCustomAttribute<XmlElementAttribute>();
                    string name = elementAttribute?.ElementName ?? field.Name;

                    // Get value
                    if (!dict.ContainsKey(name)) {
                        dict.Add($"{type.FullName}.{name}", field.GetValue(source));
                    } else {
                        ModCore.Instance.Monitor.Log("XML serializable object contains duplicate members with the same tag name. Only one will be serialized.", LogLevel.Warn);
                    }
                }
            }

            return dict;
        }

        /// <summary>Converts all XML serializable fields/properties on an object to a dictionary containing the tag name and value. This is useful for serializing an object designed for XML serialization into a JSON file.</summary>
        /// <typeparam name="T">The type of object to convert to a dictionary.</typeparam>
        /// <param name="source">The object to convert to a dictionary.</param>
        /// <param name="values">A dictionary containing the XML tag names and associated values.</param>
        /// <returns>A dictionary containing all the XML tag names and values.</returns>
        public static void DictionaryToXmlSerializable<T>(T source, IDictionary<string, object> values) {
            // Get all the types the source has
            Stack<Type> types = new Stack<Type>();
            Type curType = typeof(T);
            while (curType != null && curType != typeof(object)) {
                types.Push(curType);
                curType = curType.BaseType;
            }

            // Go through each type
            while (types.Any()) {
                Type type = types.Pop();

                // Get all serializable properties
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                    if (property.GetCustomAttribute<XmlIgnoreAttribute>() != null)
                        continue;

                    // Make sure property is read/write
                    if (property.CanRead && property.CanWrite) {
                        // Get the name of the property
                        XmlElementAttribute elementAttribute = property.GetCustomAttribute<XmlElementAttribute>();
                        string name = elementAttribute?.ElementName ?? property.Name;

                        // Make sure that element is in the dictionary
                        if (!values.TryGetValue($"{type.FullName}.{name}", out object value))
                            continue;

                        // Set the value
                        if (value is NetExposer exposer) {
                            property.SetValue(source, exposer.Unwrap());
                        } else {
                            property.SetValue(source, value.DynamicCast(property.PropertyType));
                        }
                    }
                }

                // Get all serializable fields
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                    if (field.GetCustomAttribute<XmlIgnoreAttribute>() != null)
                        continue;

                    // Get the name of the field
                    XmlElementAttribute elementAttribute = field.GetCustomAttribute<XmlElementAttribute>();
                    string name = elementAttribute?.ElementName ?? field.Name;

                    // Make sure that element is in the dictionary
                    if (!values.TryGetValue($"{type.FullName}.{name}", out object value))
                        continue;

                    // Set the value
                    if (value is NetExposer exposer) {
                        field.SetValue(source, exposer.Unwrap());
                    } else {
                        field.SetValue(source, value.DynamicCast(field.FieldType));
                    }
                }
            }
        }
    }
}
