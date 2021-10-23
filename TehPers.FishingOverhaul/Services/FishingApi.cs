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
    /// <inheritdoc cref="IFishingApi" />
    public sealed class FishingApi : IFishingApi
    {
        private readonly IMonitor monitor;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;
        private readonly Func<IEnumerable<IFishingContentSource>> contentSourcesFactory;

        private readonly EntryManagerFactory<FishEntry, FishAvailabilityInfo>
            fishEntryManagerFactory;

        private readonly EntryManagerFactory<TrashEntry, AvailabilityInfo> trashEntryManagerFactory;

        private readonly EntryManagerFactory<TreasureEntry, AvailabilityInfo>
            treasureEntryManagerFactory;

        private readonly Lazy<IOptional<IEmpApi>> empApi;

        internal readonly Dictionary<NamespacedKey, FishTraits> fishTraits;
        internal readonly List<EntryManager<FishEntry, FishAvailabilityInfo>> fishEntries;
        internal readonly List<EntryManager<TrashEntry, AvailabilityInfo>> trashEntries;
        internal readonly List<EntryManager<TreasureEntry, AvailabilityInfo>> treasureEntries;
        private readonly string stateKey;

        private bool reloadRequested;

        public event EventHandler<CatchInfo>? CaughtItem;
        public event EventHandler<List<Item>>? OpenedChest;
        public event EventHandler<CustomEvent>? CustomEvent;

        internal FishingApi(
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
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
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

        public IEnumerable<IWeightedValue<FishEntry>> GetFishChances(FishingInfo fishingInfo)
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // Get fish chances
            var chances = this.fishEntries.SelectMany(
                manager => manager.ChanceCalculator.GetWeightedChance(fishingInfo)
                    .AsEnumerable()
                    .ToWeighted(weight => weight, _ => manager.Entry)
            );

            // Curiosity lure
            if (fishingInfo.User.CurrentTool is FishingRod rod
                && rod.getBobberAttachmentIndex() is 856)
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

            return this.fishTraits.TryGetValue(fishKey, out traits);
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

        public IEnumerable<TreasureEntry> GetPossibleTreasure(FishingInfo fishingInfo)
        {
            // Select rewards
            var possibleLoot = this.GetTreasureChances(fishingInfo).ToList();
            var streak = this.GetStreak(fishingInfo.User);
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
                    fishingInfo.User,
                    streak
                );
            }
        }

        public void RaiseCustomEvent(CustomEvent customEvent)
        {
            this.CustomEvent?.Invoke(this, customEvent);
        }

        public void RequestReload()
        {
            this.reloadRequested = true;
        }

        private void ReloadIfRequested()
        {
            if (!this.reloadRequested)
            {
                return;
            }

            this.reloadRequested = false;

            // Reset fishing data
            this.fishTraits.Clear();
            this.fishEntries.Clear();
            this.trashEntries.Clear();
            this.treasureEntries.Clear();

            // Reload fishing data
            var data = this.contentSourcesFactory().SelectMany(source => source.Reload());
            foreach (var content in data)
            {
                // Fish traits
                foreach (var (fishKey, traits) in content.FishTraits)
                {
                    if (!this.fishTraits.TryAdd(fishKey, traits))
                    {
                        this.monitor.Log($"Conflicting fish traits for {fishKey}.", LogLevel.Error);
                    }
                }

                // Fish entries
                this.fishEntries.AddRange(
                    content.FishEntries.Select(
                        entry => this.fishEntryManagerFactory.Create(content.Mod, entry)
                    )
                );

                // Trash entries
                this.trashEntries.AddRange(
                    content.TrashEntries.Select(
                        entry => this.trashEntryManagerFactory.Create(content.Mod, entry)
                    )
                );

                // Treasure entries
                this.treasureEntries.AddRange(
                    content.TreasureEntries.Select(
                        entry => this.treasureEntryManagerFactory.Create(content.Mod, entry)
                    )
                );
            }
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