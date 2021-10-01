using System;
using Ninject;
using Ninject.Syntax;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Setup;

namespace TehPers.FishingOverhaul.Gui
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
            int bobber
        )
        {
            var fishingHelper = this.root.Get<IFishingHelper>();
            if (!fishingHelper.TryGetFishTraits(fishKey, out var fishTraits))
            {
                return null;
            }

            var namespaceRegistry = this.root.Get<INamespaceRegistry>();
            if (!namespaceRegistry.TryGetItemFactory(fishKey, out var fishFactory))
            {
                return null;
            }

            return new CustomBobberBar(
                this.root.Get<IModHelper>(),
                this.root.Get<IFishingHelper>(),
                this.root.Get<FishConfig>(),
                this.root.Get<TreasureConfig>(),
                this.root.Get<FishingRodOverrider>(),
                user,
                fishKey,
                fishTraits,
                fishFactory.Create(),
                fishSizePercent,
                treasure,
                bobber
            );
        }
    }
}