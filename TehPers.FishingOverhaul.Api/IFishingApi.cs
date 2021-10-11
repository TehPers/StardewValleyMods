using System;
using StardewValley;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Weighted;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// API for working with fishing.
    /// </summary>
    public interface IFishingApi : ISimplifiedFishingApi
    {
        /// <summary>
        /// Invoked whenever an item is caught from the water.
        /// </summary>
        public event EventHandler<CatchInfo>? CaughtItem;

        /// <summary>
        /// Invoked whenever a treasure chest is opened.
        /// </summary>
        public event EventHandler<List<Item>>? OpenedChest;

        /// <summary>
        /// Invoked whenever an item is caught and raises a custom event.
        /// </summary>
        public event EventHandler<CustomEvent>? CustomEvent;

        /// <summary>
        /// Gets the weighted chances of catching any fish. This does not take into account fish
        /// ponds.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable fish and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<FishEntry>> GetFishChances(FishingInfo fishingInfo);

        IEnumerable<string> ISimplifiedFishingApi.GetCatchableFish(Farmer farmer, int depth)
        {
            return this.GetFishChances(new(farmer) { BobberDepth = depth })
                .Select(weightedValue => weightedValue.Value.FishKey.ToString());
        }

        /// <summary>
        /// Gets the fish from a <see cref="FishPond"/> at the given tile if possible.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <param name="bobberTile">The tile the bobber is on.</param>
        /// <param name="takeFish">If <see langword="false"/>, simulates taking the fish. Otherwise, actually pulls the fish from the pond.</param>
        /// <returns>The fish to get from the pond, if any.</returns>
        NamespacedKey? GetFishPondFish(Farmer farmer, Vector2 bobberTile, bool takeFish = false)
        {
            // Fish ponds are buildings
            if (farmer.currentLocation is not BuildableGameLocation buildableLocation)
            {
                return null;
            }

            // Get the fish in that fish pond, if any
            return buildableLocation.buildings.OfType<FishPond>()
                .Where(pond => pond.isTileFishable(bobberTile))
                .Select(
                    pond =>
                    {
                        int? parentSheetIndex = takeFish switch
                        {
                            true when pond.CatchFish() is { ParentSheetIndex: var id } => id,
                            false when pond.currentOccupants.Value > 0 => pond.fishType.Value,
                            _ => null,
                        };
                        return parentSheetIndex.Select(NamespacedKey.SdvObject);
                    }
                )
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the weighted chances of catching any trash.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable trash and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<TrashEntry>> GetTrashChances(FishingInfo fishingInfo);

        IEnumerable<string> ISimplifiedFishingApi.GetCatchableTrash(Farmer farmer)
        {
            return this.GetTrashChances(new(farmer))
                .Select(weightedValue => weightedValue.Value.ItemKey.ToString());
        }

        /// <summary>
        /// Gets the weighted chances of catching any treasure.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable treasure and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<TreasureEntry>> GetTreasureChances(FishingInfo fishingInfo);

        IEnumerable<string> ISimplifiedFishingApi.GetCatchableTreasure(Farmer farmer)
        {
            return this.GetTreasureChances(new(farmer))
                .SelectMany(weightedValue => weightedValue.Value.ItemKeys)
                .Select(key => key.ToString())
                .Distinct();
        }

        /// <summary>
        /// Tries to get the traits for a fish.
        /// </summary>
        /// <param name="fishKey">The fish's <see cref="NamespacedKey"/>.</param>
        /// <param name="traits">The fish's traits.</param>
        /// <returns><see langword="true"/> if the fish has registered traits, otherwise <see langword="false"/>.</returns>
        bool TryGetFishTraits(NamespacedKey fishKey, [NotNullWhen(true)] out FishTraits? traits);

        /// <summary>
        /// Gets whether a fish is legendary.
        /// </summary>
        /// <param name="fishKey">The item key of the fish.</param>
        /// <returns>Whether that fish is legendary.</returns>
        bool IsLegendary(NamespacedKey fishKey);

        bool ISimplifiedFishingApi.IsLegendary(string fishKey)
        {
            return NamespacedKey.TryParse(fishKey, out var key) && this.IsLegendary(key);
        }

        /// <summary>
        /// Selects a random catch. A player may catch either a fish or trash item.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>A possible catch.</returns>
        PossibleCatch GetPossibleCatch(FishingInfo fishingInfo);

        string ISimplifiedFishingApi.GetPossibleCatch(
            Farmer farmer,
            int bobberDepth,
            out bool isFish
        )
        {
            var possibleCatch = this.GetPossibleCatch(new(farmer) { BobberDepth = bobberDepth });
            switch (possibleCatch)
            {
                case PossibleCatch.Fish(var entry):
                {
                    isFish = true;
                    return entry.FishKey.ToString();
                }
                case PossibleCatch.Trash(var entry):
                {
                    isFish = false;
                    return entry.ItemKey.ToString();
                }
                default:
                {
                    throw new InvalidOperationException(
                        $"Unknown possible catch type: {possibleCatch}"
                    );
                }
            }
        }

        /// <summary>
        /// Selects random treasure.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>Possible loot from a treasure chest.</returns>
        IList<Item> GetPossibleTreasure(FishingInfo fishingInfo);

        IList<Item> ISimplifiedFishingApi.GetPossibleTreasure(Farmer farmer)
        {
            return this.GetPossibleTreasure(new(farmer));
        }

        /// <summary>
        /// Raises a custom event for other mods to handle.
        /// </summary>
        /// <param name="customEvent">The event to raise.</param>
        void RaiseCustomEvent(CustomEvent customEvent);

        /// <summary>
        /// Requests fishing data to be reloaded.
        /// </summary>
        void RequestReload();
    }
}