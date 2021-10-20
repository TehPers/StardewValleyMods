using System;
using Ninject;
using Ninject.Syntax;
using StardewModdingAPI;
using StardewValley;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Gui;

namespace TehPers.FishingOverhaul.Services
{
    internal class CustomBobberBarFactory : ICustomBobberBarFactory
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly IFishingApi fishingApi;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;

        public CustomBobberBarFactory(
            IModHelper helper,
            IMonitor monitor,
            IFishingApi fishingApi,
            FishConfig fishConfig,
            TreasureConfig treasureConfig
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.treasureConfig =
                treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
        }

        public CustomBobberBar? Create(
            FishingInfo fishingInfo,
            FishEntry fishEntry,
            Item fishItem,
            float fishSizePercent,
            bool treasure,
            int bobber,
            bool fromFishPond
        )
        {
            if (!this.fishingApi.TryGetFishTraits(fishEntry.FishKey, out var fishTraits))
            {
                this.monitor.Log($"Missing fish traits for {fishEntry.FishKey}.", LogLevel.Error);
                return null;
            }

            return new(
                this.helper,
                this.fishConfig,
                this.treasureConfig,
                fishingInfo,
                fishEntry,
                fishTraits,
                fishItem,
                fishSizePercent,
                treasure,
                bobber,
                fromFishPond
            );
        }
    }
}