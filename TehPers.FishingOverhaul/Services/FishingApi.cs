using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Api.Weighted;
using TehPers.FishingOverhaul.Config;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul.Services
{
    /// <inheritdoc cref="IFishingApi" />
    public sealed class FishingApi : IFishingApi
    {
        private readonly IMonitor monitor;
        private readonly INamespaceRegistry namespaceRegistry;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;
        private readonly Func<IEnumerable<IFishingContentSource>> contentSourcesFactory;

        private readonly EntryManagerFactory<FishEntry, FishAvailabilityInfo>
            fishEntryManagerFactory;

        private readonly EntryManagerFactory<TrashEntry, AvailabilityInfo> trashEntryManagerFactory;

        private readonly EntryManagerFactory<TreasureEntry, AvailabilityInfo>
            treasureEntryManagerFactory;

        private readonly Dictionary<NamespacedKey, FishTraits> fishTraits;
        private readonly List<EntryManager<FishEntry, FishAvailabilityInfo>> fishEntries;
        private readonly List<EntryManager<TrashEntry, AvailabilityInfo>> trashEntries;
        private readonly List<EntryManager<TreasureEntry, AvailabilityInfo>> treasureEntries;
        private readonly string stateKey;

        private bool reloadRequested;

        public event EventHandler<CatchInfo>? CaughtItem;
        public event EventHandler<List<Item>>? OpenedChest;
        public event EventHandler<CustomEvent>? CustomEvent;

        internal FishingApi(
            IMonitor monitor,
            IManifest manifest,
            INamespaceRegistry namespaceRegistry,
            FishConfig fishConfig,
            TreasureConfig treasureConfig,
            Func<IEnumerable<IFishingContentSource>> contentSourcesFactory,
            EntryManagerFactory<FishEntry, FishAvailabilityInfo> fishEntryManagerFactory,
            EntryManagerFactory<TrashEntry, AvailabilityInfo> trashEntryManagerFactory,
            EntryManagerFactory<TreasureEntry, AvailabilityInfo> treasureEntryManagerFactory
        )
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.namespaceRegistry = namespaceRegistry
                ?? throw new ArgumentNullException(nameof(namespaceRegistry));
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

            this.fishTraits = new();
            this.fishEntries = new();
            this.trashEntries = new();
            this.treasureEntries = new();
            this.stateKey = $"{manifest.UniqueID}/fishing-state";

            this.reloadRequested = true;
        }

        public IEnumerable<IWeightedValue<FishEntry>> GetFishChances(FishingInfo fishingInfo)
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // TODO: redo how legendaries work
            IEnumerable<IWeightedValue<FishEntry>> GetNormalFishChances()
            {
                return this.fishEntries.SelectMany(
                        manager => manager.ChanceCalculator.GetWeightedChance(fishingInfo)
                            .AsEnumerable()
                            .ToWeighted(weight => weight, _ => manager.Entry)
                    )
                    .Where(
                        value => this.fishConfig.ShouldOverrideLegendaries
                            || !this.IsLegendary(value.Value.FishKey)
                    );
            }

            IEnumerable<IWeightedValue<FishEntry>> GetLocationFish(string locationName)
            {
                return Game1.getLocationFromName(locationName) is { } location
                    ? this.GetFishChances(
                        fishingInfo with
                        {
                            Locations = FishingInfo.GetDefaultLocationNames(location)
                        }
                    )
                    : Enumerable.Empty<IWeightedValue<FishEntry>>();
            }

            IEnumerable<IWeightedValue<FishEntry>> GetFarmFishChances(string locationName)
            {
                return locationName switch
                {
                    // Standard: default farm fish
                    "Farm/Standard" => Enumerable.Empty<IWeightedValue<FishEntry>>(),
                    // Riverland: forest + town + default farm fish
                    "Farm/Riverland" => GetLocationFish("Forest").Concat(GetLocationFish("Town")),
                    // Forest: forest + woodskip + default farm fish (woodskip added in content source)
                    "Farm/Forest" => GetLocationFish("Forest"),
                    // Hills: forest + default farm fish
                    "Farm/Hills" => GetLocationFish("Forest"),
                    // Wilderness: mountain + default farm fish
                    "Farm/Mountain" => GetLocationFish("Mountain"),
                    // Four corners: forest + mountain + default farm fish
                    "Farm/FourCorners" => GetLocationFish("Forest")
                        .Concat(GetLocationFish("Mountain")),
                    _ => Enumerable.Empty<IWeightedValue<FishEntry>>()
                };
            }

            var chances = GetNormalFishChances()
                .Concat(fishingInfo.Locations.SelectMany(GetFarmFishChances));

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
            var location = fishingInfo.User.currentLocation;

            // Handle non-overridden legendary fish
            // TODO: remove this - screw vanilla logic
            /*if (!this.fishConfig.ShouldOverrideLegendaries)
            {
                // Setup some values needed to simulate fishing
                var baitValue = rod.attachments[0]?.Price / 10.0 ?? 0.0;
                var bubblyZone = location.fishSplashPoint is { } splashPoint
                    && splashPoint.Value != Point.Zero
                    && new Rectangle(splashPoint.X * 64, splashPoint.Y * 64, 64, 64).Intersects(
                        new((int)rod.bobber.X - 80, (int)rod.bobber.Y - 80, 64, 64)
                    );
                var bobberTile = new Vector2(rod.bobber.X / 64f, rod.bobber.Y / 64f);

                // Create simulated farmer
                var simulatedFarmer = new Farmer
                {
                    currentLocation = farmer.currentLocation,
                    CurrentTool = farmer.CurrentTool,
                    FishingLevel = farmer.FishingLevel,
                    LuckLevel = farmer.LuckLevel,
                };
                simulatedFarmer.mailReceived.CopyFrom(farmer.mailReceived);
                simulatedFarmer.setTileLocation(farmer.getTileLocation());
                simulatedFarmer.secretNotesSeen.CopyFrom(farmer.secretNotesSeen);
                foreach (var item in farmer.fishCaught)
                {
                    simulatedFarmer.fishCaught.Add(item);
                }

                // Simulate fishing
                var oldPlayer = Game1.player;
                Game1.player = simulatedFarmer;
                var normalFish = location.getFish(
                    rod.fishingNibbleAccumulator,
                    rod.attachments[0]?.ParentSheetIndex ?? -1,
                    bobberDepth + (bubblyZone ? 1 : 0),
                    simulatedFarmer,
                    baitValue + (bubblyZone ? 0.4 : 0.0),
                    bobberTile
                );
                Game1.player = oldPlayer;

                // If a legendary fish would be caught, select that fish
                var legendaryFishKey = NamespacedKey.SdvObject(normalFish.ParentSheetIndex);
                if (this.IsLegendary(legendaryFishKey))
                {
                    return new(legendaryFishKey, PossibleCatch.Type.Fish);
                }
            }*/

            // TODO: handle special items (void mayonnaise, etc)

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

            // Secret note
            if (fishingInfo.User.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08)
            {
                if (location.tryToCreateUnseenSecretNote(fishingInfo.User) is
                {
                    ParentSheetIndex: var secretNoteId
                })
                {
                    var secretNoteKey = NamespacedKey.SdvCustom(
                        ItemTypes.Object,
                        $"SecretNote/{secretNoteId}"
                    );
                    return new PossibleCatch.Trash(new(secretNoteKey, new(0.0)));
                }
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

        public IList<Item> GetPossibleTreasure(FishingInfo fishingInfo)
        {
            // Select rewards
            var rewards = new List<Item>();
            var possibleLoot = this.GetTreasureChances(fishingInfo).ToList();
            var streak = this.GetStreak(fishingInfo.User);
            var chance = 1d;
            while (possibleLoot.Any()
                && rewards.Count < this.treasureConfig.MaxTreasureQuantity
                && Game1.random.NextDouble() <= chance)
            {
                var treasure = possibleLoot.Choose(Game1.random);

                // Choose an ID for the treasure
                if (!treasure.Value.TryCreateItem(
                    fishingInfo,
                    this.namespaceRegistry,
                    out var item
                ))
                {
                    this.monitor.Log(
                        "Could not create item for one of the treasure entries! Choosing another.",
                        LogLevel.Warn
                    );
                    possibleLoot.Remove(treasure);
                    continue;
                }

                // Lost books have custom handling
                if (item.Item is SObject { ParentSheetIndex: 102 })
                {
                    if (fishingInfo.User.archaeologyFound is not { } archaeologyFound
                        || !archaeologyFound.TryGetValue(102, out var value)
                        || value[0] >= 21)
                    {
                        // Books already found
                        possibleLoot.Remove(treasure);
                        continue;
                    }

                    Game1.showGlobalMessage(
                        "You found a lost book. The library has been expanded."
                    );
                }

                // Add the reward
                rewards.Add(item.Item);

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

            // Add bait if no rewards were selected.
            if (rewards.Count == 0)
            {
                this.monitor.Log(
                    "Could not find any valid loot for the treasure chest.",
                    LogLevel.Warn
                );
                rewards.Add(new SObject(685, Game1.random.Next(2, 5) * 5));
            }

            return rewards;
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