using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Tools;

namespace FishingOverhaul.Patches {
    [HarmonyPatch(typeof(NetAudio))]
    [HarmonyPatch(nameof(NetAudio.PlayLocal))]
    public class NetAudioPatches {
        public static bool Prefix(string audioName) {
            if (audioName == "FishHit" && Game1.player.CurrentTool is FishingRod rod && !ModFishing.Instance.Overrider.OverridingCatch.Contains(rod)) {
                ModFishing.Instance.Monitor.Log($"Prevented {audioName} cue from playing.");
                return false;
            }

            return true;
        }
    }
}
