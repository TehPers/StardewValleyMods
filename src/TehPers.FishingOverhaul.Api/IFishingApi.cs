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
using TehPers.FishingOverhaul.Api.Events;
using TehPers.FishingOverhaul.Api.Weighted;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// API for working with fishing.
    /// </summary>
    public interface IFishingApi : ISimplifiedFishingApi
    {
        /// <summary>
        /// Invoked after an item is caught from the water.
        /// </summary>
        public event EventHandler<CaughtItemEventArgs>? CaughtItem;

        /// <summary>
        /// Invoked before a treasure chest is opened.
        /// </summary>
        public event EventHandler<OpeningChestEventArgs>? OpeningChest;

        /// <summary>
        /// Invoked whenever an item is caught and raises a custom event.
        /// </summary>
        public event EventHandler<CustomEventArgs>? CustomEvent;

        /// <summary>
        /// Invoked after the default fishing info is created.
        /// </summary>
        public event EventHandler<CreatedDefaultFishingInfoEventArgs>? CreatedDefaultFishingInfo;

        /// <summary>
        /// Invoked after fish chances are calculated. This is invoked at the end of
        /// <see cref="IFishingApi.GetFishChances"/>.
        /// </summary>
        public event EventHandler<PreparedFishEventArgs>? PreparedFishChances;

        /// <summary>
        /// Invoked after trash chances are calculated. This is invoked at the end of
        /// <see cref="IFishingApi.GetTrashChances"/>.
        /// </summary>
        public event EventHandler<PreparedTrashEventArgs>? PreparedTrashChances;

        /// <summary>
        /// Invoked after treasure chances are calculated. This is invoked at the end of
        /// <see cref="IFishingApi.GetTreasureChances"/>.
        /// </summary>
        public event EventHandler<PreparedTreasureEventArgs>? PreparedTreasureChances;

        /// <summary>
        /// Invoked after the chance of catching a fish (instead of trash) is calculated. This is
        /// invoked at the end of <see cref="IFishingApi.GetChanceForFish(FishingInfo)"/>.
        /// </summary>
        public event EventHandler<CalculatedFishChanceEventArgs>? CalculatedFishChance;

        /// <summary>
        /// Invoked after the chance of finding a treasure chest is calculated. This is invoked at
        /// the end of <see cref="IFishingApi.GetChanceForTreasure(FishingInfo)"/>.
        /// </summary>
        public event EventHandler<CalculatedTreasureChanceEventArgs>? CalculatedTreasureChance;

        /// <summary>
        /// Creates a default <see cref="FishingInfo"/> for a farmer.
        /// </summary>
        /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
        /// <returns>A default <see cref="FishingInfo"/> for that farmer.</returns>
        FishingInfo CreateDefaultFishingInfo(Farmer farmer);

        /// <summary>
        /// Gets the weighted chances of catching any fish. This does not take into account fish
        /// ponds.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The catchable fish and their chances of being caught.</returns>
        IEnumerable<IWeightedValue<FishEntry>> GetFishChances(FishingInfo fishingInfo);

        IEnumerable<string> ISimplifiedFishingApi.GetCatchableFish(Farmer farmer, int depth)
        {
            return this
                .GetFishChances(this.CreateDefaultFishingInfo(farmer) with { BobberDepth = depth })
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
            return this.GetTrashChances(this.CreateDefaultFishingInfo(farmer))
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
            return this.GetTreasureChances(this.CreateDefaultFishingInfo(farmer))
                .SelectMany(weightedValue => weightedValue.Value.ItemKeys)
                .Select(key => key.ToString())
                .Distinct();
        }

        /// <summary>
        /// Gets the chance that a fish would be caught. This does not take into account whether
        /// there are actually fish to catch at the <see cref="Farmer"/>'s location. If no fish
        /// can be caught, then the <see cref="Farmer"/> will always catch trash.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The chance a fish would be caught instead of trash.</returns>
        public double GetChanceForFish(FishingInfo fishingInfo);

        double ISimplifiedFishingApi.GetChanceForFish(Farmer farmer)
        {
            var fishingInfo = this.CreateDefaultFishingInfo(farmer);
            return this.GetChanceForFish(fishingInfo);
        }

        /// <summary>
        /// Gets the chance that treasure will be found during the fishing minigame.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>The chance for treasure to appear during the fishing minigame.</returns>
        public double GetChanceForTreasure(FishingInfo fishingInfo);

        double ISimplifiedFishingApi.GetChanceForTreasure(Farmer farmer)
        {
            var fishingInfo = this.CreateDefaultFishingInfo(farmer);
            return this.GetChanceForTreasure(fishingInfo);
        }

        void ISimplifiedFishingApi.ModifyChanceForFish(Func<Farmer, double, double> chanceModifier)
        {
            _ = chanceModifier ?? throw new ArgumentNullException(nameof(chanceModifier));

            this.CalculatedFishChance += (_, e) =>
                e.ChanceForFish = chanceModifier(e.FishingInfo.User, e.ChanceForFish);
        }

        void ISimplifiedFishingApi.ModifyChanceForTreasure(
            Func<Farmer, double, double> chanceModifier
        )
        {
            _ = chanceModifier ?? throw new ArgumentNullException(nameof(chanceModifier));

            this.CalculatedTreasureChance += (_, e) =>
                e.ChanceForTreasure = chanceModifier(e.FishingInfo.User, e.ChanceForTreasure);
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
            var possibleCatch = this.GetPossibleCatch(
                this.CreateDefaultFishingInfo(farmer) with { BobberDepth = bobberDepth }
            );
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
        /// <param name="catchInfo">Information about the caught fish.</param>
        /// <returns>Possible loot from a treasure chest.</returns>
        IEnumerable<TreasureEntry> GetPossibleTreasure(CatchInfo.FishCatch catchInfo);

        /// <summary>
        /// Selects random treasure.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <returns>Possible loot from a treasure chest.</returns>
        [Obsolete(
            "This overload will be removed soon. Pass a "
            + nameof(CatchInfo.FishCatch)
            + " instead."
        )]
        IEnumerable<TreasureEntry> GetPossibleTreasure(FishingInfo fishingInfo)
        {
            var catchInfo = new CatchInfo.FishCatch(
                fishingInfo,
                new(NamespacedKey.SdvObject(0), new(0.0)),
                new StardewValley.Object(0, 1),
                0,
                false,
                0,
                0,
                new(false, TreasureState.None),
                false
            );
            return this.GetPossibleTreasure(catchInfo);
        }

        IEnumerable<string> ISimplifiedFishingApi.GetPossibleTreasure(Farmer farmer)
        {
            var fishingInfo = this.CreateDefaultFishingInfo(farmer);
            var catchInfo = new CatchInfo.FishCatch(
                fishingInfo,
                new(NamespacedKey.SdvObject(0), new(0.0)),
                new StardewValley.Object(0, 1),
                0,
                false,
                0,
                0,
                new(false, TreasureState.None),
                false
            );
            return this.GetPossibleTreasure(catchInfo)
                .SelectMany(entry => entry.ItemKeys)
                .Select(key => key.ToString());
        }

        /// <summary>
        /// Raises a custom event for other mods to handle.
        /// </summary>
        /// <param name="customEventArgs">The event to raise.</param>
        void RaiseCustomEvent(CustomEventArgs customEventArgs);

        /// <summary>
        /// Requests fishing data to be reloaded.
        /// </summary>
        void RequestReload();
    }
}