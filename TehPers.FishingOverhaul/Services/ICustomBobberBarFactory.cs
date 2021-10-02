using StardewValley;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Gui;

namespace TehPers.FishingOverhaul.Services
{
    internal interface ICustomBobberBarFactory
    {
        CustomBobberBar? Create(
            Farmer user,
            NamespacedKey fishKey,
            float fishSizePercent,
            bool treasure,
            int bobber,
            bool fromFishPond
        );
    }
}