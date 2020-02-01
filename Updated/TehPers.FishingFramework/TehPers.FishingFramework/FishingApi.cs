using System.Collections.Generic;
using TehPers.Core.Api;
using TehPers.FishingFramework.Api;

namespace TehPers.FishingFramework
{
    public class FishingApi : IFishingApi
    {
        public ISet<IFishAvailability> Fish { get; }
        public IDictionary<NamespacedId, IFishTraits> FishTraits { get; }
        public ISet<ITrashAvailability> Trash { get; }
        public ISet<ITreasureAvailability> Treasure { get; }
        public int FishingStreak { get; set; }
        public IFishingChances FishChances { get; }
        public IFishingChances TreasureChances { get; }

        public FishingApi()
        {
            this.Fish = new HashSet<IFishAvailability>();
            this.FishTraits = new Dictionary<NamespacedId, IFishTraits>();
            this.Trash = new HashSet<ITrashAvailability>();
            this.Treasure = new HashSet<ITreasureAvailability>();
        }
    }
}
