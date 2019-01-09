using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StardewModdingAPI;
using TehPers.CoreMod.Api.Conflux.Matching;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct GameAssetLocation : IEquatable<GameAssetLocation>, IComparable<GameAssetLocation> {
        public string Path { get; }

        public GameAssetLocation(string path) {
            this.Path = GameAssetLocation.Normalize(path);
        }

        public T Load<T>(IContentHelper contentHelper) {
            return contentHelper.Load<T>(this.Path, ContentSource.GameContent);
        }

        public override bool Equals(object obj) {
            return obj is GameAssetLocation other && this.Equals(other);
        }

        public bool Equals(GameAssetLocation other) {
            return string.Equals(this.Path, other.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() {
            return this.Path?.GetHashCode() ?? 0;
        }

        public int CompareTo(GameAssetLocation other) {
            return string.Compare(this.Path, other.Path, StringComparison.OrdinalIgnoreCase);
        }

        #region Static
        public static IEnumerable<string> GetParts(string path) {
            return path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(ImmutableStack<string>.Empty, (parts, part) => {
                    return part.Match<string, ImmutableStack<string>>()
                        .When(".", parts) // Do nothing
                        .When(p => p == ".." && !parts.Any(), parts.Push) // Move outside the root/current directory
                        .When(p => p == ".." && parts.Peek() == "..", parts.Push) // Move even further outside the root/current directory
                        .When(p => p == "..", _ => parts.Pop()) // Move up a directory
                        .Else(parts.Push);
                }
                // Move into a directory
                ).Reverse().AsEnumerable();
        }

        public static string Normalize(string path) {
            return string.Join("\\", GameAssetLocation.GetParts(path));
        }

        public static implicit operator GameAssetLocation(string path) {
            return new GameAssetLocation(path);
        }
        #endregion
    }
}