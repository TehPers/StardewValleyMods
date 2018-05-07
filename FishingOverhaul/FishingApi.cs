using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishingOverhaul.Api;
using FishingOverhaul.Configs;
using StardewValley;
using TehCore.Helpers;

namespace FishingOverhaul {
    public class FishingApi : IFishingApi {
        private float? _fishChance;
        private readonly Dictionary<string, Dictionary<int, IFishData>> _fishDataOverrides = new Dictionary<string, Dictionary<int, IFishData>>();
        private readonly HashSet<ITreasureData> _added = new HashSet<ITreasureData>();
        private readonly HashSet<TreasureData> _removed = new HashSet<TreasureData>();

        public float GetFishChance(Farmer who) {
            return this._fishChance ?? FishHelper.GetRawFishChance(who);
        }

        public void SetFishChance(float? chance) {
            this._fishChance = chance;
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
    }
}
