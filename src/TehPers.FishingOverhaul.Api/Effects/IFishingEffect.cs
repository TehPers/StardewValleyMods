using StardewValley;

namespace TehPers.FishingOverhaul.Api.Effects
{
    /// <summary>
    /// An effect that can be applied while fishing.
    /// </summary>
    public interface IFishingEffect
    {
        /// <summary>
        /// Applies this effect.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        void Apply(FishingInfo fishingInfo);

        /// <summary>
        /// Unapplies this effect.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        void Unapply(FishingInfo fishingInfo);

        /// <summary>
        /// Unapplies this effect from all players.
        /// </summary>
        void UnapplyAll();
    }
}
