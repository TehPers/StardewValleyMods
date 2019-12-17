using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace TehPers.Core.Api.Content
{
    /// <summary>
    /// A location for a resource.
    /// </summary>
    public readonly struct AssetLocation : IEquatable<AssetLocation>
    {
        public static IEnumerable<string> GetParts(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(ImmutableStack<string>.Empty, (parts, part) =>
                {
                    return part switch
                    {
                        "." => parts, // Do nothing
                        ".." when !parts.Any() => parts.Push(part), // Move outside the root/current directory
                        ".." when parts.Peek() == ".." => parts.Push(part), // Move even further outside the root/current directory
                        ".." => parts.Pop(), // Move up a directory
                        _ => parts.Push(part), // Move into a directory
                    };
                })
                .Reverse()
                .AsEnumerable();
        }

        public static string Normalize(string path)
        {
            return string.Join("\\", AssetLocation.GetParts(path));
        }

        public static bool operator ==(AssetLocation left, AssetLocation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetLocation left, AssetLocation right)
        {
            return !(left == right);
        }

        public string Path { get; }

        public ContentSource Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetLocation"/> struct.
        /// </summary>
        /// <param name="path">The relative path to the asset.</param>
        /// <param name="source">The asset's source directory.</param>
        public AssetLocation(string path, ContentSource source)
        {
            this.Source = source;
            this.Path = AssetLocation.Normalize(path);
        }

        public T Load<T>(IContentSource contentSource)
        {
            switch (this.Source)
            {
                case ContentSource.GameContent:
                    return Game1.content.Load<T>(this.Path);
                case ContentSource.ModFolder:
                    return contentSource.Load<T>(this.Path);
                default:
                    throw new InvalidOperationException($"Could not load from content source: {this.Source}");
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is AssetLocation other && this.Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(AssetLocation other)
        {
            return this.Source == other.Source && string.Equals(this.Path, other.Path, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return unchecked(((this.Path?.GetHashCode() ?? 0) * 397) ^ (int)this.Source);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Path} from {this.Source}";
        }
    }
}