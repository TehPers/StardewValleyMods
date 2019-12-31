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

            this.playLocalMethodInfo = this.GetMethod(typeof(NetAudio), nameof(NetAudio.PlayLocal));
            this.playLocalPrefixMethodInfo = this.GetMethod(typeof(NetAudioPatchingService), nameof(NetAudioPatchingService.PlayLocalPrefix));
        }

        protected override void ApplyPatchesInternal()
        {
            this.harmony.Patch(this.playLocalMethodInfo, prefix: new HarmonyMethod(this.playLocalPrefixMethodInfo));
        }

        protected override void RemovePatchesInternal()
        {
            this.harmony.Unpatch(this.playLocalMethodInfo, this.playLocalPrefixMethodInfo);
        }

        public static bool PlayLocalPrefix(string audioName)
        {
            if (audioName != "FishHit")
            {
                return true;
            }

            if (!(Game1.player.CurrentTool is FishingRod rod))
            {
                return true;
            }

            if (PatchingService<NetAudioPatchingService>.Instance.overrideService.IsRodBeingProcessed(rod))
            {
                return true;
            }

            PatchingService<NetAudioPatchingService>.Instance.monitor.Log($"Prevented {audioName} cue from playing.");
            return false;

        }
    }
}
