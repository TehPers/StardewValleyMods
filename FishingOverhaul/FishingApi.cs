using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishingOverhaul.Api;
using FishingOverhaul.Configs;
using StardewValley;
using StardewValley.Tools;
using TehCore.Api.Weighted;
using TehCore.Helpers;

namespace FishingOverhaul {
    public class FishingApi : IFishingApi {
        private const int MIN_TRASH_ID = 167;
        private const int MAX_TRASH_ID = 173;

        private float? _fishChance;
        private float? _treasureChance;
        private float? _unawareChance;
        private bool? _farmFishing;
        private bool? _fishableFarmFishing;
        private readonly Dictionary<string, Dictionary<int, IFishData>> _fishDataOverrides = new Dictionary<string, Dictionary<int, IFishData>>();
        private readonly HashSet<ITreasureData> _added = new HashSet<ITreasureData>();
        private readonly HashSet<TreasureData> _removed = new HashSet<TreasureData>();
        private readonly Dictionary<int, double> _trash = Enumerable.Range(FishingApi.MIN_TRASH_ID, FishingApi.MAX_TRASH_ID - FishingApi.MIN_TRASH_ID).ToDictionary(id => id, id => 1D);

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

        public void SetTrashWeight(int id, double weight) {
            this._trash[id] = weight;
        }

        public bool RemoveTrash(int id) {
            return this._trash.Remove(id);
        }

        public IEnumerable<IWeightedElement<int>> GetPossibleTrash() {
            return this._trash.ToWeighted();
        }
    }
}
