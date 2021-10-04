using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gameplay;
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
    internal class FishingApi : IFishingApi
    {
        private readonly IMonitor monitor;
        private readonly INamespaceRegistry namespaceRegistry;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;
        private readonly Func<IEnumerable<IFishingContentSource>> contentSourcesFactory;

        private readonly Dictionary<NamespacedKey, FishTraits> fishTraits;
        private readonly List<FishEntry> fishEntries;
        private readonly List<TrashEntry> trashEntries;
        private readonly List<TreasureEntry> treasureEntries;
        private readonly string stateKey;

        private bool reloadRequested;

        public FishingApi(
            IMonitor monitor,
            IManifest manifest,
            INamespaceRegistry namespaceRegistry,
            FishConfig fishConfig,
            TreasureConfig treasureConfig,
            Func<IEnumerable<IFishingContentSource>> contentSourcesFactory
        )
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.namespaceRegistry = namespaceRegistry
                ?? throw new ArgumentNullException(nameof(namespaceRegistry));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.treasureConfig =
                treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
            this.contentSourcesFactory =
                contentSourcesFactory
                ?? throw new ArgumentNullException(nameof(contentSourcesFactory));

            this.fishTraits = new();
            this.fishEntries = new();
            this.trashEntries = new();
            this.treasureEntries = new();
            this.stateKey = $"{manifest.UniqueID}/fishing-state";

            this.reloadRequested = true;
        }

        public IEnumerable<IWeightedValue<NamespacedKey>> GetFishChances(
            GameLocation location,
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel,
            double dailyLuck,
            int depth = 4
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // Get location names
            var locationNames = this.GetLocationNames(location);

            IEnumerable<IWeightedValue<NamespacedKey>> GetNormalFishChances()
            {
                // TODO: curiosity lure
                return this.fishEntries.SelectMany(
                        entry => entry.Availability.GetWeightedChance(
                                time,
                                seasons,
                                weathers,
                                fishingLevel,
                                locationNames,
                                waterTypes,
                                depth
                            )
                            .AsEnumerable()
                            .ToWeighted(weightedChance => weightedChance, _ => entry.FishKey)
                    )
                    .GroupBy(weightedValue => weightedValue.Value)
                    .ToWeighted(
                        group => group.Sum(weightedValue => weightedValue.Weight),
                        group => group.Key
                    );
            }

            // Farms
            if (location.Name is "Farm")
            {
                return this.GetFarmFishChances(
                        seasons,
                        weathers,
                        waterTypes,
                        time,
                        fishingLevel,
                        dailyLuck,
                        depth
                    )
                    .Concat(
                        this.fishConfig.AllowFishOnAllFarms
                            ? GetNormalFishChances()
                            : Enumerable.Empty<IWeightedValue<NamespacedKey>>()
                    );
            }

            return GetNormalFishChances();
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

        public IEnumerable<IWeightedValue<TrashEntry>> GetTrashChances(
            GameLocation location,
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // Get location names
            var locationNames = this.GetLocationNames(location);

            return this.trashEntries
                .SelectMany(
                    entry => entry.Availability.GetWeightedChance(
                            time,
                            seasons,
                            weathers,
                            fishingLevel,
                            locationNames,
                            waterTypes
                        )
                        .Map(weight => (entry, weight))
                        .AsEnumerable()
                )
                .ToWeighted(item => item.weight, item => item.entry);
        }

        public IEnumerable<IWeightedValue<TreasureEntry>> GetTreasureChances(
            GameLocation location,
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel
        )
        {
            // Reload data if necessary
            this.ReloadIfRequested();

            // Get location names
            var locationNames = this.GetLocationNames(location);

            return this.treasureEntries
                .SelectMany(
                    entry => entry.Availability.GetWeightedChance(
                            time,
                            seasons,
                            weathers,
                            fishingLevel,
                            locationNames,
                            waterTypes
                        )
                        .Map(weight => (entry, weight))
                        .AsEnumerable()
                )
                .ToWeighted(item => item.weight, item => item.entry);
        }

        private IEnumerable<string> GetLocationNames(GameLocation location)
        {
            return location switch
            {
                MineShaft mine => new[] { mine.Name, $"{mine.Name}/{mine.mineLevel}" },
                _ => new[] { location.Name },
            };
        }

        private IEnumerable<IWeightedValue<NamespacedKey>> GetFarmFishChances(
            Seasons seasons,
            Weathers weathers,
            WaterTypes waterTypes,
            int time,
            int fishingLevel,
            double dailyLuck,
            int depth = 4
        )
        {
            IEnumerable<IWeightedValue<NamespacedKey>> GetLocationFish(string locationName) =>
                Game1.getLocationFromName(locationName) is { } location
                    ? this.GetFishChances(
                        location,
                        seasons,
                        weathers,
                        waterTypes,
                        time,
                        fishingLevel,
                        dailyLuck,
                        depth
                    )
                    : Enumerable.Empty<IWeightedValue<NamespacedKey>>();

            return Game1.whichFarm switch
            {
                // Standard: default farm fish
                0 => Enumerable.Empty<IWeightedValue<NamespacedKey>>(),
                // Riverland: forest + town + default farm fish
                1 => GetLocationFish("Forest")
                    .Concat(GetLocationFish("Town")),
                // Forest: forest + woodskip + default farm fish
                2 => GetLocationFish("Forest")
                    .Append(
                        new WeightedValue<NamespacedKey>(
                            NamespacedKey.SdvObject(734),
                            0.05 + dailyLuck
                        )
                    ),
                // Hills: forest + default farm fish
                3 => GetLocationFish("Forest"),
                // Wilderness: mountain + default farm fish
                4 => GetLocationFish("Mountain"),
                // Four corners: forest + mountain + default farm fish
                5 => GetLocationFish("Forest")
                    .Concat(GetLocationFish("Mountain")),
                _ => Enumerable.Empty<IWeightedValue<NamespacedKey>>()
            };
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

        public PossibleCatch GetPossibleCatch(Farmer farmer, FishingRod rod, int bobberDepth)
        {
            var location = farmer.currentLocation;

            // Handle non-overridden legendary fish
            if (!this.fishConfig.ShouldOverrideVanillaLegendaries)
            {
                // Check if a legendary would be caught
                var baitValue = rod.attachments[0]?.Price / 10.0 ?? 0.0;
                var bubblyZone = location.fishSplashPoint is { } splashPoint
                    && splashPoint.Value != Point.Zero
                    && new Rectangle(splashPoint.X * 64, splashPoint.Y * 64, 64, 64).Intersects(
                        new((int)rod.bobber.X - 80, (int)rod.bobber.Y - 80, 64, 64)
                    );
                var bobberTile = new Vector2(rod.bobber.X / 64f, rod.bobber.Y / 64f);
                var normalFish = location.getFish(
                    rod.fishingNibbleAccumulator,
                    rod.attachments[0]?.ParentSheetIndex ?? -1,
                    bobberDepth + (bubblyZone ? 1 : 0),
                    farmer,
                    baitValue + (bubblyZone ? 0.4 : 0.0),
                    bobberTile
                );

                // If so, select that fish
                var legendaryFishKey = NamespacedKey.SdvObject(normalFish.ParentSheetIndex);
                if (this.IsLegendary(legendaryFishKey))
                {
                    return new(legendaryFishKey, PossibleCatch.Type.Fish);
                }
            }

            // TODO: handle special items (void mayonnaise, etc)

            // Choose a random fish if one hasn't been chosen yet
            var fishChance = this.GetChanceForFish(farmer);
            var fish = ((IFishingApi)this).GetFishChances(farmer, bobberDepth)
                .ToWeighted(val => val.Weight, val => (NamespacedKey?)val.Value)
                .Normalize(fishChance)
                .Append(new WeightedValue<NamespacedKey?>(null, 1 - fishChance))
                .ChooseOrDefault(Game1.random)
                ?.Value;

            // Return if a fish was chosen
            if (fish is { } fishKey)
            {
                return new(fishKey, PossibleCatch.Type.Fish);
            }

            // Secret note
            if (farmer.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08)
            {
                if (location.tryToCreateUnseenSecretNote(farmer) is
                    { ParentSheetIndex: var secretNoteId })
                {
                    var secretNoteKey = NamespacedKey.SdvCustom(
                        ItemTypes.Object,
                        $"SecretNote/{secretNoteId}"
                    );
                    return new(secretNoteKey, PossibleCatch.Type.Special);
                }
            }

            // Trash
            var trash = ((IFishingApi)this).GetTrashChances(farmer)
                .ChooseOrDefault(Game1.random)
                ?.Value;
            if (trash is { ItemKey: var trashKey })
            {
                return new(trashKey, PossibleCatch.Type.Trash);
            }

            // Default trash item
            this.monitor.Log("No valid trash, selecting a default item.", LogLevel.Warn);
            var defaultTrashKey = NamespacedKey.SdvObject(168);
            return new(defaultTrashKey, PossibleCatch.Type.Trash);
        }

        public IList<Item> GetPossibleTreasure(Farmer farmer)
        {
            // Select rewards
            var rewards = new List<Item>();
            var possibleLoot = ((IFishingApi)this).GetTreasureChances(farmer).ToList();
            var streak = this.GetStreak(farmer);
            var chance = 1d;
            while (possibleLoot.Any()
                && rewards.Count < this.treasureConfig.MaxTreasureQuantity
                && Game1.random.NextDouble() <= chance)
            {
                var treasure = possibleLoot.Choose(Game1.random);

                // Choose an ID for the treasure
                if (treasure.Value.ItemKeys.ToWeighted(_ => 1).ChooseOrDefault(Game1.random) is not
                    { Value: var key })
                {
                    this.monitor.Log(
                        "No possible treasure in one of the entries! Choosing another.",
                        LogLevel.Warn
                    );
                    possibleLoot.Remove(treasure);
                    continue;
                }

                // Lost books have custom handling
                if (key.Equals(NamespacedKey.SdvObject(102)))
                {
                    if (farmer.archaeologyFound == null
                        || !farmer.archaeologyFound.TryGetValue(102, out var value)
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

                // Create reward item
                if (this.namespaceRegistry.TryGetItemFactory(key, out var factory))
                {
                    var reward = factory.Create();
                    if (reward is SObject obj)
                    {
                        // Random quantity
                        obj.Stack = Game1.random.Next(
                            treasure.Value.MinQuantity,
                            treasure.Value.MaxQuantity
                        );
                    }

                    // Add the reward
                    rewards.Add(reward);
                }
                else
                {
                    this.monitor.Log(
                        $"No provider for treasure item {key}. Skipping it.",
                        LogLevel.Error
                    );
                }

                // Check if this reward shouldn't be duplicated
                if (!this.treasureConfig.AllowDuplicateLoot || !treasure.Value.AllowDuplicates)
                {
                    possibleLoot.Remove(treasure);
                }

                // Update chance
                chance *= this.treasureConfig.AdditionalLootChances.GetChance(farmer, streak);
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
                foreach (var (fishKey, traits) in content.Traits)
                {
                    if (!this.fishTraits.TryAdd(fishKey, traits))
                    {
                        this.monitor.Log($"Conflicting fish traits for {fishKey}.", LogLevel.Error);
                    }
                }

                // Fish entries
                this.fishEntries.AddRange(content.FishEntries);

                // Trash entries
                this.trashEntries.AddRange(content.TrashEntries);

                // Treasure entries
                this.treasureEntries.AddRange(content.TreasureEntries);
            }
        }
    }
}