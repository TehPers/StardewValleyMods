using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace TehPers.FishingOverhaul.Patches {

    public class NetAudioPatches {
        public static bool Prefix(string audioName) {
            if (audioName == "FishHit" && Game1.player.CurrentTool is FishingRod rod && !ModEntry.Instance.Overrider.OverridingCatch.Contains(rod)) {
                ModEntry.Instance.Monitor.Log($"Prevented {audioName} cue from playing.");
                return false;
            }

            return true;
        }
    }
}
