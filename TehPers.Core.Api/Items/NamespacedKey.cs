using System;
using System.ComponentModel;
using System.Globalization;
using StardewModdingAPI;

namespace TehPers.Core.Api.Items
{
    [TypeConverter(typeof(TypeConverter))]
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

        public void Deconstruct(out string @namespace, out string key)
        {
            @namespace = this.Namespace;
            key = this.Key;
        }

        public override string ToString()
        {
            return $"{this.Namespace}:{this.Key}";
        }

        public bool Equals(NamespacedKey other)
        {
            return this.Namespace == other.Namespace && this.Key == other.Key;
        }

        public override bool Equals(object? obj)
        {
            return obj is NamespacedKey other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Namespace, this.Key);
        }

        public static bool operator ==(NamespacedKey left, NamespacedKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NamespacedKey left, NamespacedKey right)
        {
            return !(left == right);
        }

        public static bool TryParse(string raw, out NamespacedKey key)
        {
            var parts = raw.Split(':', 2);
            if (parts.Length < 2)
            {
                key = default;
                return false;
            }

            key = new(parts[0], parts[1]);
            return true;
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

        internal class TypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value
            )
            {
                if (value is string raw && NamespacedKey.TryParse(raw, out var key))
                {
                    return key;
                }

                return base.ConvertFrom(context, culture, value)!;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string)
                    || base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value,
                Type destinationType
            )
            {
                if (value is NamespacedKey key && destinationType == typeof(string))
                {
                    return key.ToString();
                }

                return base.ConvertTo(context, culture, value, destinationType)!;
            }
        }
    }
}