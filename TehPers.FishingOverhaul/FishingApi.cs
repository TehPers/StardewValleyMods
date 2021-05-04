using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using TehPers.Core.Api.Enums;
using TehPers.Core.Api.Weighted;
using TehPers.Core.Helpers.Static;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Configs;

namespace TehPers.FishingOverhaul {
    public class FishingApi : IFishingApi {
        private static readonly Regex _mineLocRegex = new Regex(@"^UndergroundMine\d+$", RegexOptions.IgnoreCase);

        private float? _fishChance;
        private float? _treasureChance;
        private float? _unawareChance;
        private bool? _farmFishing;
        private bool? _fishableFarmFishing;
        private readonly Dictionary<string, Dictionary<int, IFishData>> _fishDataOverrides = new Dictionary<string, Dictionary<int, IFishData>>();
        private readonly Dictionary<int, IFishTraits> _fishTraitsOverrides = new Dictionary<int, IFishTraits>();
        private readonly HashSet<ITreasureData> _added = new HashSet<ITreasureData>();
        private readonly HashSet<TreasureData> _removed = new HashSet<TreasureData>();
        private readonly HashSet<ITrashData> _trash = new HashSet<ITrashData>();
        private readonly Dictionary<int, string> _fishNameOverrides = new Dictionary<int, string>();
        private readonly Dictionary<Farmer, int> _streaks = new Dictionary<Farmer, int>();
        private readonly HashSet<int> _hidden = new HashSet<int>();
        private readonly int[] _defaultLegendary = { 159, 160, 163, 682, 775 };
        private readonly Dictionary<int, bool> _legendaryOverrides = new Dictionary<int, bool>();
        private bool _obsoleteWarning;

        internal FishingApi() { }

        /// <inheritdoc />
        public event EventHandler<FishingEventArgs> BeforeFishCatching;

        /// <inheritdoc />
        public event EventHandler<FishingEventArgs> FishCaught;

        /// <inheritdoc />
        public event EventHandler<FishingEventArgs> TrashCaught;

        /// <inheritdoc />
        public void SetFishChance(float? chance) {
            _fishChance = chance;
        }

        /// <inheritdoc />
        public float GetFishChance(Farmer who) {
            return (_fishChance ?? FishHelper.GetRawFishChance(who))
                .Clamp(ModFishing.Instance.MainConfig.GlobalFishSettings.MinFishChance, ModFishing.Instance.MainConfig.GlobalFishSettings.MaxFishChance)
                .Clamp(0F, 1F); // Prevent invalid chances
        }

        /// <inheritdoc />
        public void SetTreasureChance(float? chance) {
            _treasureChance = chance;
        }

        /// <inheritdoc />
        public float GetTreasureChance(Farmer who, FishingRod rod) {
            return _treasureChance ?? FishHelper.GetRawTreasureChance(who, rod);
        }

        /// <inheritdoc />
        public void SetUnawareChance(float? chance) {
            _unawareChance = chance;
        }

        /// <inheritdoc />
        public float GetUnawareChance(Farmer who, int fish) {
            if (IsLegendary(fish))
                return 0F;

            return _unawareChance ?? FishHelper.GetRawUnawareChance(who);
        }

        /// <inheritdoc />
        public void SetFarmFishing(bool? allowFish) {
            _farmFishing = allowFish;
        }

        /// <inheritdoc />
        public bool GetFarmFishing() {
            return _farmFishing ?? ModFishing.Instance.MainConfig.GlobalFishSettings.AllowFishOnAllFarms;
        }

        /// <inheritdoc />
        public void SetFishableFarmFishing(bool? allowFish) {
            _fishableFarmFishing = allowFish;
        }

        /// <inheritdoc />
        public bool GetFishableFarmFishing() {
            return _fishableFarmFishing ?? true;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<int, IFishData> GetFishData(string location) {
            var result = new Dictionary<int, IFishData>();

            // Get mod location data
            if (ModFishing.Instance.FishConfig.PossibleFish.TryGetValue(location, out var modLocData)) {
                foreach (var modData in modLocData) {
                    // Mod data
                    result[modData.Key] = modData.Value;
                }
            }

            // Get overridden location data
            if (!_fishDataOverrides.TryGetValue(location, out var overrideLocData)) return result.ToDictionary();
            foreach (var overrideData in overrideLocData) {
                if (overrideData.Value == null) {
                    // Removed data
                    result.Remove(overrideData.Key);
                } else {
                    // Added data
                    result[overrideData.Key] = overrideData.Value;
                }
            }

            // Format it as a dictionary
            return result.ToDictionary();
        }

        /// <inheritdoc />
        public IFishData GetFishData(string location, int fish) {
            // Overriden fish data
            if (_fishDataOverrides.TryGetValue(location, out var overrideLocData) && overrideLocData.TryGetValue(fish, out var overrideData))
                return overrideData;

            // Mod fish data
            if (ModFishing.Instance.FishConfig.PossibleFish.TryGetValue(location, out var modLocData) && modLocData.TryGetValue(fish, out var modData))
                return modData;

            // None found
            return null;
        }

        /// <inheritdoc />
        public void SetFishData(string location, int fish, IFishData data) {
            // Get location data dictionary
            if (!_fishDataOverrides.TryGetValue(location, out var overrideLocData)) {
                overrideLocData = new Dictionary<int, IFishData>();
                _fishDataOverrides.Add(location, overrideLocData);
            }

            // Set the fish data for that location
            overrideLocData[fish] = data;
        }

        /// <inheritdoc />
        public void ResetFishData() {
            _fishDataOverrides.Clear();
        }

        /// <inheritdoc />
        public bool ResetFishData(string location) {
            return _fishDataOverrides.Remove(location);
        }

        /// <inheritdoc />
        public bool ResetFishData(string location, int fish) {
            return _fishDataOverrides.TryGetValue(location, out var overrideLocData) && overrideLocData.Remove(fish);
        }

        /// <inheritdoc />
        public void SetFishTraits(int fish, IFishTraits traits) {
            if (traits == null) {
                _fishTraitsOverrides.Remove(fish);
            } else {
                _fishTraitsOverrides[fish] = traits;
            }
        }

        /// <inheritdoc />
        public IFishTraits GetFishTraits(int fish) {
            return _fishTraitsOverrides.TryGetValue(fish, out var traits) ? traits : null;
        }

        /// <inheritdoc />
        public bool AddTreasureData(ITreasureData data) {
            return _added.Add(data);
        }

        /// <inheritdoc />
        public bool RemoveTreasureData(ITreasureData data)
        {
            if (data is TreasureData td) {
                return _removed.Add(td);
            }

            return _added.Remove(data);
        }

        /// <inheritdoc />
        public IEnumerable<ITreasureData> GetTreasureData() {
            return ModFishing.Instance.TreasureConfig.PossibleLoot.Except(_removed).Concat(_added);
        }

        /// <inheritdoc />
        public bool AddTrashData(ITrashData data) {
            return _trash.Add(data);
        }

        /// <inheritdoc />
        public bool RemoveTrashData(ITrashData data) {
            return _trash.Remove(data);
        }

        /// <inheritdoc />
        public IEnumerable<ITrashData> GetTrashData() {
            return _trash;
        }

        /// <inheritdoc />
        public IEnumerable<ITrashData> GetTrashData(Farmer who) {
            var w = SDVHelpers.ToWaterType(who.currentLocation?.getFishingLocation(who.getTileLocation()) ?? -1) ?? WaterType.Both;
            var mineLevel = who.currentLocation is MineShaft mine ? mine.mineLevel : -1;
            return GetTrashData(who, who.currentLocation?.Name ?? "", w, SDate.Now(), Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, Game1.player.FishingLevel, mineLevel);
        }

        /// <inheritdoc />
        public IEnumerable<ITrashData> GetTrashData(Farmer who, string locationName, WaterType waterType, SDate date, Weather weather, int time, int fishingLevel, int? mineLevel) {
            return _trash.Where(t => t.MeetsCriteria(who, locationName, waterType, date, weather, time, fishingLevel, mineLevel));
        }

        /// <inheritdoc />
        public IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who) {
            var w = SDVHelpers.ToWaterType(who.currentLocation?.getFishingLocation(who.getTileLocation()) ?? -1) ?? WaterType.Both;
            var mineLevel = who.currentLocation is MineShaft mine ? mine.mineLevel : -1;
            return GetPossibleFish(who, who.currentLocation?.Name ?? "", w, SDate.Now(), Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, Game1.player.FishingLevel, mineLevel);
        }

        /// <inheritdoc />
        public IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who, string locationName, WaterType water, SDate date, Weather weather, int time, int fishLevel, int? mineLevel = null) {
            if(string.Equals(locationName, "BeachNightMarket", StringComparison.OrdinalIgnoreCase)) {
                locationName = "Beach";
            }
            if (!string.Equals(locationName, "Farm", StringComparison.OrdinalIgnoreCase) || !GetFishableFarmFishing())
                return GetPossibleFishInternal(who, locationName, GetFarmFishing(), water, date, weather, time, fishLevel, mineLevel);

            // Custom handling for farm maps
            switch (Game1.whichFarm) {
                case 0: {
                    // Standard: default farm fish
                    break;
                }
                case 1: {
                    // Riverland: forest fish + town fish + default farm fish
                    var forestFish = GetPossibleFish(who, "Forest", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.3);
                    var townFish = GetPossibleFish(who, "Town", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.7);
                    var farmFish = GetPossibleFishInternal(who, locationName, true, water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.65);
                    return forestFish.Concat(townFish).Concat(farmFish);
                }
                case 2: {
                    // Forest: forest fish + woodskip + default farm fish
                    var scale = 0.05F + (float) Game1.player.DailyLuck;
                    var forestFish = GetPossibleFish(who, "Forest", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(1 - scale);
                    IWeightedElement<int?>[] woodSkip = { new WeightedElement<int?>(734, scale) };
                    var farmFish = GetPossibleFishInternal(who, locationName, true, water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.65);
                    return forestFish.Concat(woodSkip).Concat(farmFish);
                }
                case 3: {
                    // Hills: forest fish + default farm fish
                    var forestFish = GetPossibleFish(who, "Forest", water, date, weather, time, fishLevel, mineLevel);
                    var farmFish = GetPossibleFishInternal(who, locationName, true, water, date, weather, time, fishLevel, mineLevel);
                    return forestFish.Concat(farmFish);
                }
                case 4: {
                    // Wilderness: mountain fish + default farm fish
                    var forestFish = GetPossibleFish(who, "Mountain", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.35);
                    var farmFish = GetPossibleFishInternal(who, locationName, true, water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.65);
                    return forestFish.Concat(farmFish);
                }
                case 5: {
                    // Four Corners: mountain fish + forest fish?
                    var forestFish = GetPossibleFish(who, "Forest", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.3);
                    var mountainFish = GetPossibleFish(who, "Mountain", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.7);
                    var farmFish = GetPossibleFishInternal(who, locationName, true, water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.65);
                    return forestFish.Concat(mountainFish).Concat(farmFish);
                }
                case 6: {
                    // Beach Farm: Ocean fish
                    var beachFish = GetPossibleFish(who, "Beach", water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.35);
                    var farmFish = GetPossibleFishInternal(who, locationName, true, water, date, weather, time, fishLevel, mineLevel).NormalizeTo(0.65);
                    return beachFish.Concat(farmFish);
                }
                default:
                    return GetPossibleFishInternal(who, locationName, GetFarmFishing(), water, date, weather, time, fishLevel, mineLevel);
                
            }

            return GetPossibleFishInternal(who, locationName, GetFarmFishing(), water, date, weather, time, fishLevel, mineLevel);
        }

        private IEnumerable<IWeightedElement<int?>> GetPossibleFishInternal(Farmer who, string locationName, bool allowFarmFish, WaterType water, SDate date, Weather weather, int time, int fishLevel, int? mineLevel = null) {
            // Check if this is the farm
            if (locationName == "Farm" && !allowFarmFish)
                return new[] { new WeightedElement<int?>(null, 1) };

            // Get chance for fish
            var fishChance = GetFishChance(who);

            // Get fish data for location
            IEnumerable<KeyValuePair<int, IFishData>> locFish = GetFishData(locationName);
            if (_mineLocRegex.IsMatch(locationName)) {
                locFish = locFish.Concat(GetFishData("UndergroundMine"));
            }

            // Filter all the fish that can be caught at that location
            var fish = locFish.Where(f => {
                // Legendary fish criteria
                if (!IsLegendary(f.Key))
                    return f.Value.MeetsCriteria(f.Key, water, date, weather, time, fishLevel, mineLevel);
                // If custom legendaries is disabled, then don't include legendary fish. They are handled in CustomFishingRod
                if (!ModFishing.Instance.MainConfig.CustomLegendaries) {
                    return false;
                }

                // If recatchable legendaries is disabled, then make sure this fish hasn't been caught yet
                if (!ModFishing.Instance.MainConfig.RecatchableLegendaries && who.fishCaught.ContainsKey(f.Key)) {
                    return false;
                }

                // Normal criteria check
                return f.Value.MeetsCriteria(f.Key, water, date, weather, time, fishLevel, mineLevel);
            }).ToWeighted(kv => kv.Value.GetWeight(who), kv => (int?) kv.Key);

            // Include trash
            IEnumerable<IWeightedElement<int?>> trash = new WeightedElement<int?>(null, 1).Yield();

            // Combine fish with trash
            return fish.NormalizeTo(fishChance).Concat(trash.NormalizeTo(1 - fishChance));
        }

        /// <inheritdoc />
        public string GetFishName(int fish) {
            // Search registered fish names
            if (_fishNameOverrides.TryGetValue(fish, out var name)) {
                return name;
            }

            // Fallback to Data/ObjectInformation
            return Game1.objectInformation.TryGetValue(fish, out var objectData) ? objectData.Split('/')[4] : string.Empty;

            // Not found
        }

        /// <inheritdoc />
        public void SetFishName(int fish, string name) {
            _fishNameOverrides[fish] = name ?? string.Empty;
        }

        /// <inheritdoc />
        public bool ResetFishName(int fish) {
            return _fishNameOverrides.Remove(fish);
        }

        /// <inheritdoc />
        public int GetStreak(Farmer who) {
            return _streaks.TryGetValue(who, out var streak) ? streak : 0;
        }

        /// <inheritdoc />
        public void SetStreak(Farmer who, int streak) {
            _streaks[who] = streak;
        }

        /// <inheritdoc />
        public void HideFish(int fish) {
            _hidden.Add(fish);
        }

        /// <inheritdoc />
        public bool RevealFish(int fish) {
            return _hidden.Remove(fish);
        }

        /// <inheritdoc />
        public bool IsHidden(int fish) {
            return _hidden.Contains(fish);
        }

        /// <inheritdoc />
        public bool IsLegendary(int fish) {
            return _legendaryOverrides.TryGetValue(fish, out var legendary) ? legendary : _defaultLegendary.Contains(fish);
        }

        /// <inheritdoc />
        public void SetLegendary(int fish, bool? legendary) {
            if (legendary.HasValue) {
                _legendaryOverrides[fish] = legendary.Value;
            } else {
                _legendaryOverrides.Remove(fish);
            }
        }

        private void WarnObsolete() {
            if (_obsoleteWarning)
                return;

            ModFishing.Instance.Monitor.Log("A mod is using an obsolete trash command! The Nuget package cannot be updated right now, so go to https://github.com/TehPers/StardewValleyMods/tree/2.0-dev/FishingOverhaulApi for the latest version of the API.");
            _obsoleteWarning = true;
        }

        internal void OnBeforeFishCatching(FishingEventArgs e) {
            BeforeFishCatching?.Invoke(this, e);
        }

        internal virtual void OnFishCaught(FishingEventArgs e) {
            FishCaught?.Invoke(this, e);
        }

        internal virtual void OnTrashCaught(FishingEventArgs e) {
            TrashCaught?.Invoke(this, e);
        }
    }
}
