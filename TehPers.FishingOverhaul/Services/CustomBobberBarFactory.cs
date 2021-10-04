using System;
using Ninject;
using Ninject.Syntax;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
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
            Farmer user,
            NamespacedKey fishKey,
            float fishSizePercent,
            bool treasure,
            int bobber,
            bool fromFishPond
        )
        {
            var fishingHelper = this.root.Get<IFishingApi>();
            if (!fishingHelper.TryGetFishTraits(fishKey, out var fishTraits))
            {
                return null;
            }

            var namespaceRegistry = this.root.Get<INamespaceRegistry>();
            if (!namespaceRegistry.TryGetItemFactory(fishKey, out var fishFactory))
            {
                return null;
            }

            return new(
                this.root.Get<IModHelper>(),
                this.root.Get<IFishingApi>(),
                this.root.Get<FishConfig>(),
                this.root.Get<TreasureConfig>(),
                user,
                fishKey,
                fishTraits,
                fishFactory.Create(),
                fishSizePercent,
                treasure,
                bobber,
                fromFishPond
            );
        }
    }
}