using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// An entry for a fishing item.
    /// </summary>
    /// <typeparam name="T">The type of availability for this entry.</typeparam>
    [JsonDescribe]
    public abstract record Entry<T>(
        [property: JsonRequired] [property: Description("The availability information.")]
        T AvailabilityInfo
    )
        where T : AvailabilityInfo
    {
        [Description("Actions to perform when this is caught.")]
        public CatchActions? OnCatch { get; init; } = null;

        /// <summary>
        /// Tries to create an instance of the item this entry represents.
        /// </summary>
        /// <param name="fishingInfo">Information about the farmer that is fishing.</param>
        /// <param name="namespaceRegistry">The namespace registry.</param>
        /// <param name="item">The item that was created, if possible.</param>
        /// <returns><see langword="true"/> if the item was created, <see langword="false"/> otherwise.</returns>
        public abstract bool TryCreateItem(
            FishingInfo fishingInfo,
            INamespaceRegistry namespaceRegistry,
            [NotNullWhen(true)] out CaughtItem? item
        );
    }
}