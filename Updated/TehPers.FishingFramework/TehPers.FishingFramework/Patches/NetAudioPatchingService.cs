using System;
using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Tools;
using TehPers.Core.Api.Harmony;

namespace TehPers.FishingFramework.Patches
{
    internal sealed class NetAudioPatchingService : PatchingService<NetAudioPatchingService>
    {
        private readonly IMonitor monitor;
        private readonly FishingOverrideService overrideService;
        private readonly HarmonyInstance harmony;
        private readonly MethodInfo playLocalMethodInfo;
        private readonly MethodInfo playLocalPrefixMethodInfo;

        public NetAudioPatchingService(IMonitor monitor, FishingOverrideService overrideService, HarmonyInstance harmony)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.overrideService = overrideService ?? throw new ArgumentNullException(nameof(overrideService));
            this.harmony = harmony ?? throw new ArgumentNullException(nameof(harmony));

            playLocalMethodInfo = GetMethod(typeof(NetAudio), nameof(NetAudio.PlayLocal));
            playLocalPrefixMethodInfo = GetMethod(typeof(NetAudioPatchingService), nameof(NetAudioPatchingService.PlayLocalPrefix));
        }

        protected override void ApplyPatchesInternal()
        {
            harmony.Patch(playLocalMethodInfo, prefix: new HarmonyMethod(playLocalPrefixMethodInfo));
        }

        protected override void RemovePatchesInternal()
        {
            harmony.Unpatch(playLocalMethodInfo, playLocalPrefixMethodInfo);
        }

        public static bool PlayLocalPrefix(string audioName)
        {
            if (audioName == "FishHit" && Game1.player.CurrentTool is FishingRod rod && !Instance.overrideService.OverridingCatch.Contains(rod))
            {
                Instance.monitor.Log($"Prevented {audioName} cue from playing.", LogLevel.Trace);
                return false;
            }

            return true;
        }
    }
}
