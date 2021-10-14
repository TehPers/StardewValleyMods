using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal sealed class DefaultCustomEvents : ISetup, IDisposable
    {
        private readonly IManifest manifest;
        private readonly IFishingApi fishingApi;

        public DefaultCustomEvents(IManifest manifest, IFishingApi fishingApi)
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
        }

        public void Setup()
        {
            this.fishingApi.CustomEvent += this.OnCustomEvent;
        }

        public void Dispose()
        {
            this.fishingApi.CustomEvent -= this.OnCustomEvent;
        }

        private void OnCustomEvent(object? sender, CustomEvent e)
        {
            var (@namespace, key) = e.EventKey;

            // Check that the event should be handled by this mod
            if (@namespace != this.manifest.UniqueID)
            {
                return;
            }

            // Handle the event based on the key
            switch (key)
            {
                case "LostBook":
                {
                    Game1.showGlobalMessage(
                        "You found a lost book. The library has been expanded."
                    );
                    break;
                }
                case "RandomGoldenWalnut" when Game1.IsMultiplayer:
                {
                    e.Catch.FishingInfo.User.team.RequestLimitedNutDrops(
                        "IslandFishing",
                        e.Catch.FishingInfo.User.currentLocation,
                        (int)e.Catch.FishingInfo.BobberPosition.X,
                        (int)e.Catch.FishingInfo.BobberPosition.Y,
                        5
                    );
                    break;
                }
                case "RandomGoldenWalnut":
                {
                    if (!e.Catch.FishingInfo.User.team.limitedNutDrops.ContainsKey("IslandFishing"))
                    {
                        e.Catch.FishingInfo.User.team.limitedNutDrops["IslandFishing"] = 1;
                    }
                    else
                    {
                        ++e.Catch.FishingInfo.User.team.limitedNutDrops["IslandFishing"];
                    }

                    break;
                }
                case "TidePoolGoldenWalnut"
                    when e.Catch.FishingInfo.User.currentLocation is IslandSouthEast location
                    && !location.fishedWalnut.Value:
                {
                    e.Catch.FishingInfo.User.team.MarkCollectedNut("StardropPool");
                    if (!Game1.IsMultiplayer)
                    {
                        location.fishedWalnut.Value = true;
                    }
                    else
                    {
                        location.fishWalnutEvent.Fire();
                    }

                    break;
                }
            }
        }
    }
}