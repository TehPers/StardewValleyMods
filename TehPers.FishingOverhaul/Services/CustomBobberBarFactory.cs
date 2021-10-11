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
        private readonly IResolutionRoot root;

        public CustomBobberBarFactory(IResolutionRoot root)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
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
            var fishingHelper = this.root.Get<IFishingApi>();
            if (!fishingHelper.TryGetFishTraits(fishEntry.FishKey, out var fishTraits))
            {
                return null;
            }

            return new(
                this.root.Get<IModHelper>(),
                this.root.Get<FishConfig>(),
                this.root.Get<TreasureConfig>(),
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