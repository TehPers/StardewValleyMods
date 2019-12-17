using System;
using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Config;
using TehPers.FishingFramework.Api.Events;

namespace TehPers.FishingFramework
{
    public class FishingApi : IFishingApi
    {
        private readonly IMonitor monitor;
        private readonly IGlobalConfiguration localGlobalConfig;

        public ISet<IFishAvailability> Fish { get; }
        public IDictionary<int, IFishTraits> FishTraits { get; }
        public ISet<ITrashAvailability> Trash { get; }
        public ISet<ITreasureAvailability> Treasure { get; }
        public IGlobalConfiguration GlobalConfig { get; }
        public IPersonalConfiguration PersonalConfig { get; }
        public int FishingStreak { get; set; }
        public IFishingChances FishChances { get; }
        public IFishingChances TreasureChances { get; }

        public event EventHandler<FishCatchingEventArgs> FishCatching;
        public event EventHandler<FishCaughtEventArgs> FishCaught;
        public event EventHandler<FishLostEventArgs> FishLost;
        public event EventHandler<TreasureOpeningEventArgs> TreasureOpening;
        public event EventHandler<TreasureOpenedEventArgs> TreasureOpened;
        public event EventHandler<TrashCatchingEventArgs> TrashCatching;
        public event EventHandler<TrashCaughtEventArgs> TrashCaught;

        public FishingApi(IMonitor monitor, IGlobalConfiguration localGlobalConfig, IPersonalConfiguration personalConfig)
        {
            this.monitor = monitor;
            this.localGlobalConfig = localGlobalConfig;

            this.Fish = new HashSet<IFishAvailability>();
            this.FishTraits = new Dictionary<int, IFishTraits>();
            this.Trash = new HashSet<ITrashAvailability>();
            this.Treasure = new HashSet<ITreasureAvailability>();
            this.GlobalConfig = localGlobalConfig;
            this.PersonalConfig = personalConfig;
        }

        public virtual void OnFishCatching(FishCatchingEventArgs e)
        {
            this.monitor.Log($"Invoking {nameof(FishingApi.FishCatching)}");
            this.FishCatching?.Invoke(this, e);
        }

        public virtual void OnFishCaught(FishCaughtEventArgs e)
        {
            this.monitor.Log($"Invoking {nameof(FishingApi.FishCaught)}");
            this.FishCaught?.Invoke(this, e);
        }

        protected virtual void OnFishLost(FishLostEventArgs e)
        {
            this.monitor.Log($"Invoking {nameof(FishingApi.FishLost)}");
            this.FishLost?.Invoke(this, e);
        }

        public virtual void OnTreasureOpening(TreasureOpeningEventArgs e)
        {
            this.monitor.Log($"Invoking {nameof(FishingApi.TreasureOpening)}");
            this.TreasureOpening?.Invoke(this, e);
        }

        public virtual void OnTreasureOpened(TreasureOpenedEventArgs e)
        {
            this.monitor.Log($"Invoking {nameof(FishingApi.TreasureOpened)}");
            this.TreasureOpened?.Invoke(this, e);
        }

        public virtual void OnTrashCatching(TrashCatchingEventArgs e)
        {
            this.monitor.Log($"Invoking {nameof(FishingApi.TrashCatching)}");
            this.TrashCatching?.Invoke(this, e);
        }

        public virtual void OnTrashCaught(TrashCaughtEventArgs e)
        {
            this.monitor.Log($"Invoking {nameof(FishingApi.TrashCaught)}");
            this.TrashCaught?.Invoke(this, e);
        }
    }
}
