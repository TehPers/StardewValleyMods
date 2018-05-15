using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using TehPers.Core.Api.Enums;
using TehPers.Core.Api.Weighted;
using TehPers.Core.Helpers;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Configs;

namespace TehPers.FishingOverhaul {
    public class FishingApi : IFishingApi {
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
        private readonly Dictionary<int, string> _fishNames = new Dictionary<int, string>();
        private readonly Dictionary<Farmer, int> _streaks = new Dictionary<Farmer, int>();
        private readonly HashSet<int> _hidden = new HashSet<int>();

        private bool _obsoleteWarning = false;

        public void SetFishChance(float? chance) {
            this._fishChance = chance;
        }

        public float GetFishChance(Farmer who) {
            return this._fishChance ?? FishHelper.GetRawFishChance(who);
        }

        public void SetTreasureChance(float? chance) {
            this._treasureChance = chance;
        }

        public float GetTreasureChance(Farmer who, FishingRod rod) {
            return this._treasureChance ?? FishHelper.GetRawTreasureChance(who, rod);
        }

        public void SetUnawareChance(float? chance) {
            this._unawareChance = chance;
        }

        public float GetUnawareChance(Farmer who, int fish) {
            if (FishHelper.IsLegendary(fish))
                return 0F;

            return this._unawareChance ?? FishHelper.GetRawUnawareChance(who);
        }

        public void SetFarmFishing(bool? allowFish) {
            this._farmFishing = allowFish;
        }

        public bool GetFarmFishing() {
            return this._farmFishing ?? ModFishing.Instance.MainConfig.GlobalFishSettings.AllowFishOnAllFarms;
        }

        public void SetFishableFarmFishing(bool? allowFish) {
            this._fishableFarmFishing = allowFish;
        }

        public bool GetFishableFarmFishing() {
            return this._fishableFarmFishing ?? true;
        }

        public IReadOnlyDictionary<int, IFishData> GetFishData(string location) {
            IEnumerable<KeyValuePair<int, IFishData>> result = Enumerable.Empty<KeyValuePair<int, IFishData>>();

            // Get mod location data
            if (ModFishing.Instance.FishConfig.PossibleFish.TryGetValue(location, out Dictionary<int, FishData> modLocData)) {
                result = result.Concat(modLocData.Select(kv => new KeyValuePair<int, IFishData>(kv.Key, kv.Value)));
            }

            // Get overridden location data
            if (this._fishDataOverrides.TryGetValue(location, out Dictionary<int, IFishData> overrideLocData)) {
                // Removed data
                result = result.Except(overrideLocData.Where(kv => kv.Value == null));

                // Added data
                result = result.Concat(overrideLocData.Where(kv => kv.Value != null));
            }

            // Format it as a dictionary
            return result.ToDictionary();
        }

        public IFishData GetFishData(string location, int fish) {
            // Overriden fish data
            if (this._fishDataOverrides.TryGetValue(location, out Dictionary<int, IFishData> overrideLocData) && overrideLocData.TryGetValue(fish, out IFishData overrideData))
                return overrideData;

            // Mod fish data
            if (ModFishing.Instance.FishConfig.PossibleFish.TryGetValue(location, out Dictionary<int, FishData> modLocData) && modLocData.TryGetValue(fish, out FishData modData))
                return modData;

            // None found
            return null;
        }

        public void SetFishData(string location, int fish, IFishData data) {
            // Get location data dictionary
            if (!this._fishDataOverrides.TryGetValue(location, out Dictionary<int, IFishData> overrideLocData)) {
                overrideLocData = new Dictionary<int, IFishData>();
                this._fishDataOverrides.Add(location, overrideLocData);
            }

            // Set the fish data for that location
            overrideLocData[fish] = data;
        }

        public void ResetFishData() {
            this._fishDataOverrides.Clear();
        }

        public bool ResetFishData(string location) {
            return this._fishDataOverrides.Remove(location);
        }

        public bool ResetFishData(string location, int fish) {
            return this._fishDataOverrides.TryGetValue(location, out Dictionary<int, IFishData> overrideLocData) && overrideLocData.Remove(fish);
        }

        public void SetFishTraits(int fish, IFishTraits traits) {
            if (traits == null) {
                this._fishTraitsOverrides.Remove(fish);
            } else {
                this._fishTraitsOverrides[fish] = traits;
            }
        }

        public IFishTraits GetFishTraits(int fish) {
            return this._fishTraitsOverrides.TryGetValue(fish, out IFishTraits traits) ? traits : null;
        }

        public bool AddTreasureData(ITreasureData data) {
            return this._added.Add(data);
        }

        public bool RemoveTreasureData(ITreasureData data) {
            if (data is TreasureData td) {
                return this._removed.Add(td);
            } else {
                return this._added.Remove(data);
            }
        }

        public IEnumerable<ITreasureData> GetTreasureData() {
            return ModFishing.Instance.TreasureConfig.PossibleLoot.Except(this._removed).Concat(this._added);
        }

        [Obsolete]
        public void SetTrashWeight(int id, double weight) {
            this.WarnObsolete();
            ITrashData data = this._trash.FirstOrDefault(t => t.PossibleIds.Contains(id));
            if (data is SpecificTrashData specificData) {
                this.RemoveTrashData(specificData);
                this.AddTrashData(new SpecificTrashData(specificData.PossibleIds, weight, specificData.Location, specificData.WaterType, specificData.Season, specificData.Weather, specificData.FishingLevel, specificData.MineLevel));
            } else if (data != null) {
                throw new NotSupportedException();
            } else {
                this.AddTrashData(new SpecificTrashData(new[] { id }, weight, null));
            }
        }

        [Obsolete]
        public bool RemoveTrash(int id) {
            this.WarnObsolete();
            ITrashData data = this._trash.FirstOrDefault(t => t.PossibleIds.Contains(id));
            return data != null && this.RemoveTrashData(data);
        }

        [Obsolete]
        public IEnumerable<IWeightedElement<int>> GetPossibleTrash() {
            this.WarnObsolete();
            return this.GetTrashData().SelectMany(t => t.PossibleIds.ToWeighted(id => t.GetWeight()));
        }

        public bool AddTrashData(ITrashData data) {
            return this._trash.Add(data);
        }

        public bool RemoveTrashData(ITrashData data) {
            return this._trash.Remove(data);
        }

        public IEnumerable<ITrashData> GetTrashData() {
            return this._trash;
        }

        public IEnumerable<ITrashData> GetTrashData(Farmer who) {
            Season s = SDVHelpers.ToSeason(Game1.currentSeason) ?? Season.Spring | Season.Summer | Season.Fall | Season.Winter;
            WaterType w = SDVHelpers.ToWaterType(who.currentLocation?.getFishingLocation(who.getTileLocation()) ?? -1) ?? WaterType.Both;
            int mineLevel = who.currentLocation is MineShaft mine ? mine.mineLevel : -1;
            return this.GetTrashData(who, who.currentLocation?.Name ?? "", w, s, Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, Game1.player.FishingLevel, mineLevel);
        }

        public IEnumerable<ITrashData> GetTrashData(Farmer who, string locationName, WaterType waterType, Season season, Weather weather, int time, int fishingLevel, int? mineLevel) {
            return this._trash.Where(t => t.MeetsCriteria(who, locationName, waterType, season, weather, time, fishingLevel, mineLevel));
        }

        public IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who) {
            Season s = SDVHelpers.ToSeason(Game1.currentSeason) ?? Season.Spring | Season.Summer | Season.Fall | Season.Winter;
            WaterType w = SDVHelpers.ToWaterType(who.currentLocation?.getFishingLocation(who.getTileLocation()) ?? -1) ?? WaterType.Both;
            int mineLevel = who.currentLocation is MineShaft mine ? mine.mineLevel : -1;
            return this.GetPossibleFish(who, who.currentLocation?.Name ?? "", w, s, Game1.isRaining ? Weather.Rainy : Weather.Sunny, Game1.timeOfDay, Game1.player.FishingLevel, mineLevel);
        }

        public IEnumerable<IWeightedElement<int?>> GetPossibleFish(Farmer who, string locationName, WaterType water, Season season, Weather weather, int time, int fishLevel, int? mineLevel = null) {
            // Custom handling for farm maps
            if (locationName == "Farm" && this.GetFishableFarmFishing()) {
                switch (Game1.whichFarm) {
                    case 1: {
                            // Forest fish + town fish
                            IEnumerable<IWeightedElement<int?>> forestFish = this.GetPossibleFish(who, "Forest", water, season, weather, time, fishLevel, mineLevel).NormalizeTo(0.3);
                            IEnumerable<IWeightedElement<int?>> townFish = this.GetPossibleFish(who, "Town", water, season, weather, time, fishLevel, mineLevel).NormalizeTo(0.7);
                            return forestFish.Concat(townFish);
                        }
                    case 2: {
                            // Forest fish + woodskip
                            float scale = 0.05F + (float) Game1.dailyLuck;
                            IEnumerable<IWeightedElement<int?>> forestFish = this.GetPossibleFish(who, "Forest", water, season, weather, time, fishLevel, mineLevel).NormalizeTo(1 - scale);
                            IWeightedElement<int?>[] woodSkip = { new WeightedElement<int?>(734, scale) };
                            return forestFish.Concat(woodSkip);
                        }
                    case 3: {
                            // Forest fish + default farm fish
                            IEnumerable<IWeightedElement<int?>> forestFish = this.GetPossibleFish(who, "Forest", water, season, weather, time, fishLevel, mineLevel);
                            IEnumerable<IWeightedElement<int?>> farmFish = this.GetPossibleFishWithoutFarm(who, locationName, water, season, weather, time, fishLevel, mineLevel);
                            return forestFish.Concat(farmFish);
                        }
                    case 4: {
                            // Mountain fish + default farm fish
                            IEnumerable<IWeightedElement<int?>> forestFish = this.GetPossibleFish(who, "Mountain", water, season, weather, time, fishLevel, mineLevel).NormalizeTo(0.35);
                            IEnumerable<IWeightedElement<int?>> farmFish = this.GetPossibleFishWithoutFarm(who, locationName, water, season, weather, time, fishLevel, mineLevel).NormalizeTo(0.65);
                            return forestFish.Concat(farmFish);
                        }
                }
            }

            return this.GetPossibleFishWithoutFarm(who, locationName, water, season, weather, time, fishLevel, mineLevel);
        }

        private IEnumerable<IWeightedElement<int?>> GetPossibleFishWithoutFarm(Farmer who, string locationName, WaterType water, Season season, Weather weather, int time, int fishLevel, int? mineLevel = null) {
            // Check if this location has fish data
            if (!ModFishing.Instance.FishConfig.PossibleFish.ContainsKey(locationName))
                return new[] { new WeightedElement<int?>(null, 1) };

            // Check if this is the farm
            if (locationName == "Farm" && !this.GetFarmFishing())
                return new[] { new WeightedElement<int?>(null, 1) };

            // Get chance for fish
            float fishChance = this.GetFishChance(who);

            // Filter all the fish that can be caught at that location
            IEnumerable<IWeightedElement<int?>> fish = this.GetFishData(locationName).Where(f => {
                // Legendary fish criteria
                if (FishHelper.IsLegendary(f.Key)) {
                    // If custom legendaries is disabled, then don't include legendary fish. They are handled in CustomFishingRod
                    if (!ModFishing.Instance.MainConfig.CustomLegendaries) {
                        return false;
                    }

                    // If recatchable legendaries is disabled, then make sure this fish hasn't been caught yet
                    if (!ModFishing.Instance.MainConfig.RecatchableLegendaries && who.fishCaught.ContainsKey(f.Key)) {
                        return false;
                    }
                }

                // Normal criteria check
                return f.Value.MeetsCriteria(water, season, weather, time, fishLevel, mineLevel);
            }).ToWeighted(kv => kv.Value.GetWeight(who), kv => (int?) kv.Key);

            // Include trash
            IWeightedElement<int?>[] trash = { new WeightedElement<int?>(null, 1) };

            // Combine fish with trash
            return fish.NormalizeTo(fishChance).Concat(trash.NormalizeTo(1 - fishChance));
        }

        public string GetFishName(int fish) {
            // Check if fish names have been loaded in yet
            if (!this._fishNames.Any()) {
                // Get raw content data for fish
                Dictionary<int, string> fishContent = ModFishing.Instance.Helper.Content.Load<Dictionary<int, string>>(@"Data\Fish.xnb", ContentSource.GameContent);

                // Store all the names
                foreach (KeyValuePair<int, string> fishData in fishContent) {
                    string[] contentData = fishContent[fishData.Key].Split('/');
                    this._fishNames[fishData.Key] = contentData.Length > 13 ? contentData[13] : contentData[0];
                }
            }

            return this._fishNames.TryGetValue(fish, out string name) ? name : null;
        }

        public void SetFishName(int fish, string name) {
            this._fishNames[fish] = name ?? string.Empty;
        }

        public int GetStreak(Farmer who) {
            return this._streaks.TryGetValue(who, out int streak) ? streak : 0;
        }

        public void SetStreak(Farmer who, int streak) {
            this._streaks[who] = streak;
        }

        public void HideFish(int fish) {
            this._hidden.Add(fish);
        }

        public bool RevealFish(int fish) {
            return this._hidden.Remove(fish);
        }

        public bool IsHidden(int fish) {
            return this._hidden.Contains(fish);
        }

        private void WarnObsolete() {
            if (this._obsoleteWarning)
                return;

            ModFishing.Instance.Monitor.Log("A mod is using an obsolete trash command! The Nuget package cannot be updated right now, so go to https://github.com/TehPers/StardewValleyMods/tree/2.0-dev/FishingOverhaulApi for the latest version of the API.");
            this._obsoleteWarning = true;
        }
    }
}
