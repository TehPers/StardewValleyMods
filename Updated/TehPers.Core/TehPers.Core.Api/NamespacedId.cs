using System;
using System.Text.RegularExpressions;

namespace TehPers.Core.Api
{
    /// <summary>An identifier consisting of a namespace and another identifier.</summary>
    public readonly struct NamespacedId
    {
        private static readonly Regex ParseRegex = new Regex(@"^\s*(?<namespace>[^:\s]+)\s*:\s*(?<key>[^:\s]+)\s*$");
        private static readonly Regex ValidPart = new Regex(@"^[^:\s]+$");

        /// <summary>Tries to parse a <see cref="string"/> as a <see cref="NamespacedId"/>.</summary>
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
    }
}
