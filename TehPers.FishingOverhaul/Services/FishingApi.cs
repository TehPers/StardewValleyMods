using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Api.Weighted;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Integrations.Emp;

namespace TehPers.FishingOverhaul.Services
{
    /// <summary>
    /// Default API for working with fishing.
    /// </summary>
    /// <inheritdoc cref="IFishingApi" />
    public sealed partial class FishingApi : IFishingApi
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly IManifest manifest;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;
        private readonly Func<IEnumerable<IFishingContentSource>> contentSourcesFactory;

        private readonly EntryManagerFactory<FishEntry, FishAvailabilityInfo>
            fishEntryManagerFactory;

        private readonly EntryManagerFactory<TrashEntry, AvailabilityInfo> trashEntryManagerFactory;

        private readonly EntryManagerFactory<TreasureEntry, AvailabilityInfo>
            treasureEntryManagerFactory;

        private readonly Lazy<IOptional<IEmpApi>> empApi;

        internal Dictionary<NamespacedKey, FishTraits> fishTraits;
        internal List<EntryManager<FishEntry, FishAvailabilityInfo>> fishEntries;
        internal List<EntryManager<TrashEntry, AvailabilityInfo>> trashEntries;
        internal List<EntryManager<TreasureEntry, AvailabilityInfo>> treasureEntries;
        private readonly string stateKey;

        private bool reloadRequested;

        public event EventHandler<CatchInfo>? CaughtItem;
        public event EventHandler<List<Item>>? OpenedChest;
        public event EventHandler<CustomEvent>? CustomEvent;

        internal FishingApi(
            IModHelper helper,
            IMonitor monitor,
            IManifest manifest,
            FishConfig fishConfig,
            TreasureConfig treasureConfig,
            Func<IEnumerable<IFishingContentSource>> contentSourcesFactory,
            EntryManagerFactory<FishEntry, FishAvailabilityInfo> fishEntryManagerFactory,
            EntryManagerFactory<TrashEntry, AvailabilityInfo> trashEntryManagerFactory,
            EntryManagerFactory<TreasureEntry, AvailabilityInfo> treasureEntryManagerFactory,
            Lazy<IOptional<IEmpApi>> empApi
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.treasureConfig =
                treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
            this.contentSourcesFactory = contentSourcesFactory
                ?? throw new ArgumentNullException(nameof(contentSourcesFactory));
            this.fishEntryManagerFactory = fishEntryManagerFactory
                ?? throw new ArgumentNullException(nameof(fishEntryManagerFactory));
            this.trashEntryManagerFactory = trashEntryManagerFactory
                ?? throw new ArgumentNullException(nameof(trashEntryManagerFactory));
            this.treasureEntryManagerFactory = treasureEntryManagerFactory
                ?? throw new ArgumentNullException(nameof(treasureEntryManagerFactory));
            this.empApi = empApi ?? throw new ArgumentNullException(nameof(empApi));

            this.fishTraits = new();
            this.fishEntries = new();
            this.trashEntries = new();
            this.treasureEntries = new();
            this.stateKey = $"{manifest.UniqueID}/fishing-state";

            this.reloadRequested = true;
        }

        public FishingInfo CreateDefaultFishingInfo(Farmer farmer)
        {
            var fishingInfo = new FishingInfo(farmer);

            // Apply farm map overrides
            if (farmer.currentLocation is Farm farm
                && this.GetFarmLocationOverride(farm) is var (overrideLocation, overrideChance)
                && Game1.random.NextDouble() < overrideChance)
            {
                fishingInfo = fishingInfo with
                {
                    Locations = FishingInfo.GetDefaultLocationNames(
                            Game1.getLocationFromName(overrideLocation)
                        )
                        .ToImmutableArray(),
                };
            }

            // Apply EMP changes
            if (this.empApi.Value.TryGetValue(out var empApi))
            {
                // Get EMP info
                empApi.GetFishLocationsData(
                    farmer.currentLocation,
                    fishingInfo.BobberPosition,
                    out var empLocationName,
                    out var empZone,
                    out _
                );

                // Override data
                fishingInfo = fishingInfo with
                {
                    Locations = empLocationName switch
                    {
                        null => fishingInfo.Locations,
                        _ when Game1.getLocationFromName(empLocationName) is { } empLocation =>
                            FishingInfo.GetDefaultLocationNames(empLocation).ToImmutableArray(),
                        _ => ImmutableArray.Create(empLocationName),
                    },
                    WaterTypes = empZone switch
                    {
                        null => fishingInfo.WaterTypes,
                        -1 => WaterTypes.All,
                        0 => WaterTypes.River,
                        1 => WaterTypes.PondOrOcean,
                        2 => WaterTypes.Freshwater,
                        _ => WaterTypes.All,
                    },
                };
            }

            return fishingInfo;
        }

        private (string, float)? GetFarmLocationOverride(Farm farm)
        {
            var overrideLocationField =
                this.helper.Reflection.GetField<string?>(farm, "_fishLocationOverride");
            var overrideChanceField =
                this.helper.Reflection.GetField<float>(farm, "_fishChanceOverride");

            // Set override
            float overrideChance;
            if (overrideLocationField.GetValue() is not { } overrideLocation)
            {
                // Read from the map properties
                var mapProperty = farm.getMapProperty("FarmFishLocationOverride");
                if (mapProperty == string.Empty)
                {
                    overrideLocation = string.Empty;
                    overrideChance = 0.0f;
                }
                else
                {
                    var splitProperty = mapProperty.Split(' ');
                    if (splitProperty.Length >= 2
                        && float.TryParse(splitProperty[1], out overrideChance))
                    {
                        overrideLocation = splitProperty[0];
                    }
                    else
                    {
                        overrideLocation = string.Empty;
                        overrideChance = 0.0f;
                    }
                }

                // Set the fields
                overrideLocationField.SetValue(overrideLocation);
                overrideChanceField.SetValue(overrideChance);
            }
            else
            {
                overrideChance = overrideChanceField.GetValue();
            }

            if (overrideChance > 0.0)
            {
                // Overridden
                return (overrideLocation, overrideChance);
            }

            // No override
            return null;
        }

        public IEnumerable<IWeightedValue<FishEntry>> GetFishChances(FishingInfo fishingInfo)
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // Get the attachments on the user's rod
            var (bait, bobber) = fishingInfo.User.CurrentTool is FishingRod rod
                ? (rod.getBaitAttachmentIndex(), rod.getBobberAttachmentIndex())
                : (-1, -1);

            // TODO: Add support for custom bait and bobber attachments added by other mods

            // Magic bait
            if (bait is 908)
            {
                // Update fishing info to allow catches from all seasons, weathers, and times
                fishingInfo = fishingInfo with
                {
                    Seasons = Core.Api.Gameplay.Seasons.All,
                    Weathers = Core.Api.Gameplay.Weathers.All,
                    Times = Enumerable.Range(600, 2000).ToImmutableArray(),
                };
            }

            // Get fish chances
            var chances = this.fishEntries.SelectMany(
                manager => manager.ChanceCalculator.GetWeightedChance(fishingInfo)
                    .AsEnumerable()
                    .ToWeighted(weight => weight, _ => manager.Entry)
            );

            // Curiosity lure
            if (bobber is 856)
            {
                chances = chances.ToWeighted(
                    weightedValue =>
                        weightedValue.Weight >= 0 ? Math.Log(weightedValue.Weight + 1) : 0,
                    weightedValue => weightedValue.Value
                );
            }

            return chances;
        }

        public bool TryGetFishTraits(
            NamespacedKey fishKey,
            [NotNullWhen(true)] out FishTraits? traits
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            if (!this.fishTraits.TryGetValue(fishKey, out traits))
            {
                return false;
            }

            var dartFrequency =
                (int)(this.fishConfig.GlobalDartFrequencyFactor * traits.DartFrequency);
            traits = traits with { DartFrequency = dartFrequency };
            return true;
        }

        public IEnumerable<IWeightedValue<TrashEntry>> GetTrashChances(FishingInfo fishingInfo)
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            return this.trashEntries.SelectMany(
                    manager => manager.ChanceCalculator.GetWeightedChance(fishingInfo)
                        .Select(weight => (entry: manager.Entry, weight))
                        .AsEnumerable()
                )
                .ToWeighted(item => item.weight, item => item.entry)
                .Condense();
        }

        public IEnumerable<IWeightedValue<TreasureEntry>> GetTreasureChances(
            FishingInfo fishingInfo
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            return this.treasureEntries.SelectMany(
                    manager => manager.ChanceCalculator.GetWeightedChance(fishingInfo)
                        .Select(weight => (entry: manager.Entry, weight))
                        .AsEnumerable()
                )
                .ToWeighted(item => item.weight, item => item.entry)
                .Condense();
        }

        public double GetChanceForFish(Farmer farmer)
        {
            var streak = this.GetStreak(farmer);
            return this.fishConfig.FishChances.GetChance(farmer, streak);
        }

        public double GetChanceForTreasure(Farmer farmer)
        {
            var streak = this.GetStreak(farmer);
            return this.treasureConfig.TreasureChances.GetChance(farmer, streak);
        }

        public bool IsLegendary(NamespacedKey fishKey)
        {
            return this.TryGetFishTraits(fishKey, out var traits) && traits.IsLegendary;
        }

        public int GetStreak(Farmer farmer)
        {
            var key = $"{this.stateKey}/streak";
            return farmer.modData.TryGetValue(key, out var rawData)
                && int.TryParse(rawData, out var streak)
                    ? streak
                    : 0;
        }

        public void SetStreak(Farmer farmer, int streak)
        {
            var key = $"{this.stateKey}/streak";
            farmer.modData[key] = streak.ToString();
        }

        public PossibleCatch GetPossibleCatch(FishingInfo fishingInfo)
        {
            // Choose a random fish if one hasn't been chosen yet
            var fishChance = this.GetChanceForFish(fishingInfo.User);
            IEnumerable<IWeightedValue<FishEntry?>> possibleFish = this.GetFishChances(fishingInfo)
                .Normalize(fishChance);
            var fishEntry = possibleFish.Append(new WeightedValue<FishEntry?>(null, 1 - fishChance))
                .ChooseOrDefault(Game1.random)
                ?.Value;

            // Return if a fish was chosen
            if (fishEntry is not null)
            {
                return new PossibleCatch.Fish(fishEntry);
            }

            // Trash
            var trashEntry = this.GetTrashChances(fishingInfo).ChooseOrDefault(Game1.random)?.Value;
            if (trashEntry is not null)
            {
                return new PossibleCatch.Trash(trashEntry);
            }

            // Default trash item
            this.monitor.Log("No valid trash, selecting a default item.", LogLevel.Warn);
            var defaultTrashKey = NamespacedKey.SdvObject(168);
            return new PossibleCatch.Trash(new(defaultTrashKey, new(0.0)));
        }

        public IEnumerable<TreasureEntry> GetPossibleTreasure(CatchInfo.FishCatch catchInfo)
        {
            // Get possible loot
            var possibleLoot = this.GetTreasureChances(catchInfo.FishingInfo).ToList();

            // Perfect catch + treasure inverts the chances
            if (this.treasureConfig.InvertChancesOnPerfectCatch && catchInfo.State.IsPerfect)
            {
                possibleLoot = possibleLoot.Normalize()
                    .ToWeighted(item => 1.0 - item.Weight, item => item.Value)
                    .ToList();
            }

            // Select rewards
            var streak = this.GetStreak(catchInfo.FishingInfo.User);
            var chance = 1d;
            var rewards = 0;
            while (possibleLoot.Any()
                   && rewards < this.treasureConfig.MaxTreasureQuantity
                   && Game1.random.NextDouble() <= chance)
            {
                // Choose a reward
                var treasure = possibleLoot.Choose(Game1.random);

                // Yield it
                rewards += 1;
                yield return treasure.Value;

                // Check if this reward shouldn't be duplicated
                if (!this.treasureConfig.AllowDuplicateLoot || !treasure.Value.AllowDuplicates)
                {
                    possibleLoot.Remove(treasure);
                }

                // Update chance
                chance *= this.treasureConfig.AdditionalLootChances.GetChance(
                    catchInfo.FishingInfo.User,
                    streak
                );
            }
        }

        public void RaiseCustomEvent(CustomEvent customEvent)
        {
            this.CustomEvent?.Invoke(this, customEvent);
        }

        public void OnCaughtItem(CatchInfo e)
        {
            this.CaughtItem?.Invoke(this, e);
        }

        public void OnOpenedChest(List<Item> e)
        {
            this.OpenedChest?.Invoke(this, e);
        }
    }
}