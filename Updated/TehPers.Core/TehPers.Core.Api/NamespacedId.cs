using System;
using System.Globalization;
using System.Text.RegularExpressions;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace TehPers.Core.Api
{
    /// <summary>An identifier consisting of a namespace and another identifier unique to that namespace.</summary>
    public readonly struct NamespacedId : IEquatable<NamespacedId>
    {
        private const string VanillaStub = "stardewvalley";

        /// <summary>
        /// The namespace for vanilla <see cref="SObject"/>s.
        /// </summary>
        public const string VanillaObjectsNamespace = NamespacedId.VanillaStub + "/objects";

        /// <summary>
        /// The namespace for vanilla <see cref="Sword"/>s.
        /// </summary>
        public const string VanillaSwordsNamespace = NamespacedId.VanillaStub + "/weapons";

        /// <summary>
        /// The namespace for vanilla <see cref="Boots"/>.
        /// </summary>
        public const string VanillaBootsNamespace = NamespacedId.VanillaStub + "/boots";

        /// <summary>
        /// The namespace for vanilla <see cref="Ring"/>s.
        /// </summary>
        public const string VanillaRingsNamespace = NamespacedId.VanillaStub + "/rings";

        private static readonly Regex ParseRegex = new Regex(@"^\s*(?<namespace>[^:\s]+)\s*:\s*(?<key>[^:\s]+)\s*$");
        private static readonly Regex ValidPart = new Regex(@"^[^:\s]+$");

        /// <summary>
        /// Tries to parse a <see cref="string"/> as a <see cref="NamespacedId"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to try to parse.</param>
        /// <param name="namespacedId">The resulting <see cref="NamespacedId"/> if parsing succeeds.</param>
        /// <returns><see langword="true"/> if parsing succeeded, <see langword="false"/> otherwise.</returns>
        public static bool TryParse(string value, out NamespacedId namespacedId)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var match = NamespacedId.ParseRegex.Match(value);
            if (!match.Success)
            {
                namespacedId = default;
                return false;
            }

            namespacedId = new NamespacedId(match.Groups["namespace"].Value, match.Groups["key"].Value);
            return true;
        }

        /// <summary>
        /// Constructs a <see cref="NamespacedId"/> from an existing item.
        /// </summary>
        /// <param name="item">The item to create the <see cref="NamespacedId"/> for.</param>
        /// <returns>A new <see cref="NamespacedId"/> identifying the given item.</returns>
        public static NamespacedId FromItem(Item item)
        {
            return item switch
            {
                Sword { InitialParentTileIndex: var index } => NamespacedId.FromSwordIndex(index),
                SObject { ParentSheetIndex: var index } => NamespacedId.FromObjectIndex(index),
                Boots { indexInTileSheet: { Value: var index } } => NamespacedId.FromBootsIndex(index),
                Ring { indexInTileSheet: { Value: var index } } => NamespacedId.FromRingIndex(index),
                _ => throw new NotImplementedException("If this functionality is needed, create an item provider for the type you're interested in and create the namespaced IDs from there."),
            };
        }

        /// <summary>
        /// Constructs a new <see cref="NamespacedId"/> from a vanilla <see cref="SObject"/>'s <see cref="Item.ParentSheetIndex"/>.
        /// </summary>
        /// <param name="parentSheetIndex">The <see cref="Item.ParentSheetIndex"/> of the item.</param>
        /// <returns>A new <see cref="NamespacedId"/> identifying that item.</returns>
        public static NamespacedId FromObjectIndex(int parentSheetIndex)
        {
            return new NamespacedId(NamespacedId.VanillaObjectsNamespace, parentSheetIndex.ToString("D", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Constructs a new <see cref="NamespacedId"/> from a vanilla <see cref="Sword"/>'s <see cref="Tool.InitialParentTileIndex"/>.
        /// </summary>
        /// <param name="initialParentTileIndex">The <see cref="Tool.InitialParentTileIndex"/> of the item.</param>
        /// <returns>A new <see cref="NamespacedId"/> identifying that item.</returns>
        public static NamespacedId FromSwordIndex(int initialParentTileIndex)
        {
            return new NamespacedId(NamespacedId.VanillaSwordsNamespace, initialParentTileIndex.ToString("D", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Constructs a new <see cref="NamespacedId"/> from a vanilla <see cref="Boots"/>' <see cref="Boots.indexInTileSheet"/>.
        /// </summary>
        /// <param name="indexInTileSheet">The <see cref="Boots.indexInTileSheet"/> of the item.</param>
        /// <returns>A new <see cref="NamespacedId"/> identifying that item.</returns>
        public static NamespacedId FromBootsIndex(int indexInTileSheet)
        {
            return new NamespacedId(NamespacedId.VanillaBootsNamespace, indexInTileSheet.ToString("D", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Constructs a new <see cref="NamespacedId"/> from a vanilla <see cref="Ring"/>'s <see cref="Ring.indexInTileSheet"/>.
        /// </summary>
        /// <param name="indexInTileSheet">The <see cref="Ring.indexInTileSheet"/> of the item.</param>
        /// <returns>A new <see cref="NamespacedId"/> identifying that item.</returns>
        public static NamespacedId FromRingIndex(int indexInTileSheet)
        {
            return new NamespacedId(NamespacedId.VanillaRingsNamespace, indexInTileSheet.ToString("D", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Checks for equality between two <see cref="NamespacedId"/>s.
        /// </summary>
        /// <param name="left">The first ID.</param>
        /// <param name="right">The second ID.</param>
        /// <returns><see langword="true"/> if the two values are equal, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(NamespacedId left, NamespacedId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks for inequality between two <see cref="NamespacedId"/>s.
        /// </summary>
        /// <param name="left">The first ID.</param>
        /// <param name="right">The second ID.</param>
        /// <returns><see langword="false"/> if the two values are equal, <see langword="true"/> otherwise.</returns>
        public static bool operator !=(NamespacedId left, NamespacedId right)
        {
            return !(left == right);
        }

        /// <summary>Gets the namespace for this <see cref="NamespacedId"/>.</summary>
        public string Namespace { get; }

        /// <summary>Gets the key for this <see cref="NamespacedId"/>.</summary>
        public string Key { get; }

        /// <summary>Initializes a new instance of the <see cref="NamespacedId"/> struct.</summary>
        /// <param name="namespace">The namespace for the <see cref="NamespacedId"/>. This should usually be a mod's unique ID.</param>
        /// <param name="key">The key for the <see cref="NamespacedId"/>.</param>
        public NamespacedId(string @namespace, string key)
        {
            this.Namespace = @namespace?.Trim() ?? throw new ArgumentNullException(nameof(@namespace));
            this.Key = key?.Trim() ?? throw new ArgumentNullException(nameof(key));

            if (!NamespacedId.ValidPart.IsMatch(this.Namespace))
            {
                throw new ArgumentException($"{nameof(this.Namespace)} contains invalid characters", nameof(@namespace));
            }

            if (!NamespacedId.ValidPart.IsMatch(this.Key))
            {
                throw new ArgumentNullException($"{nameof(this.Key)} contains invalid colors", nameof(key));
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Namespace}:{this.Key}";
        }

        /// <inheritdoc />
        public bool Equals(NamespacedId other)
        {
            return string.Equals(this.Namespace, other.Namespace, StringComparison.Ordinal) && string.Equals(this.Key, other.Key, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is NamespacedId other && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return unchecked(((this.Namespace != null ? this.Namespace.GetHashCode() : 0) * 397) ^ (this.Key != null ? this.Key.GetHashCode() : 0));
        }
    }
}