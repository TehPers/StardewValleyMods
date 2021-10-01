using StardewValley;
using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Gui
{
    internal interface ICustomBobberBarFactory
    {
        CustomBobberBar? Create(Farmer user, NamespacedKey fishKey, float fishSizePercent, bool treasure, int bobber);
    }
}