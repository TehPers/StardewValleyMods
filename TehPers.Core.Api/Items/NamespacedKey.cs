using System;
using StardewModdingAPI;

namespace TehPers.Core.Api.Items
{
    public readonly struct NamespacedKey : IEquatable<NamespacedKey>
    {
        public const string StardewValleyNamespace = "StardewValley";

        public string Namespace { get; }
        public string Key { get; }

        public NamespacedKey(string @namespace, string key)
        {
            this.Namespace = @namespace;
            this.Key = key;
        }

        public NamespacedKey(IManifest manifest, string key)
            : this(manifest.UniqueID, key)
        {
        }

        public override string ToString()
        {
            return $"{this.Namespace}:{this.Key}";
        }

        public static NamespacedKey SdvTool(string toolType)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Tool, toolType);
        }

        public static NamespacedKey SdvTool(string toolType, int quality)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Tool, $"{toolType}/{quality}");
        }

        public static NamespacedKey SdvClothing(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Clothing, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvWallpaper(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Wallpaper, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvFlooring(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Flooring, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvBoots(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Boots, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvHat(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Hat, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvWeapon(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Weapon, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvFurniture(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Furniture, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvBigCraftable(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.BigCraftable, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvRing(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Ring, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvObject(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Object, parentSheetIndex.ToString());
        }

        public static NamespacedKey SdvCustom(string itemType, string key)
        {
            return new NamespacedKey(NamespacedKey.StardewValleyNamespace, $"{itemType}/{key}");
        }

        public bool Equals(NamespacedKey other)
        {
            return this.Namespace == other.Namespace
                && this.Key == other.Key;
        }

        public override bool Equals(object? obj)
        {
            return obj is NamespacedKey other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Namespace, this.Key);
        }
    }
}