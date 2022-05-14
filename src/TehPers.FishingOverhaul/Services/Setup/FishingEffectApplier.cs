using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class FishingEffectApplier : ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly FishingApi fishingApi;

        public FishingEffectApplier(IModHelper helper, FishingApi fishingApi)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
        }

        public void Setup()
        {
            this.helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
        }

        public void Dispose()
        {
            this.helper.Events.GameLoop.UpdateTicking -= this.OnUpdateTicking;
        }

        private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
        {
            foreach (var manager in this.fishingApi.fishingEffectManagers)
            {
                var info = new FishingInfo(Game1.player);
                switch (manager.UpdateEnabled(info))
                {
                    case true:
                        manager.Effect.Apply(info);
                        break;
                    case false:
                        manager.Effect.Unapply(info);
                        break;
                }
            }
        }
    }
}
