using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Api.Effects;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// An entry for an effect that may be applied while fishing.
    /// </summary>
    public abstract record FishingEffectEntry
    {
        /// <summary>
        /// Conditions for when this effect should be applied.
        /// </summary>
        public AvailabilityConditions Conditions { get; init; } = new();

        /// <summary>
        /// Creates the fishing effect associated with this entry.
        /// </summary>
        /// <param name="kernel">The global kernel.</param>
        /// <returns>The fishing effect associated with this entry.</returns>
        public abstract IFishingEffect CreateEffect(IGlobalKernel kernel);
    }
}
