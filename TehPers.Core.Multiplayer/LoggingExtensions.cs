using StardewModdingAPI;

namespace TehPers.Core.Multiplayer {
    internal static class LoggingExtensions {

        internal static void PrefixedLog(this IMonitor monitor, string message, LogLevel level = LogLevel.Debug) {
            monitor.Log($"[TehPers.Core.Multiplayer] {message}", level);
        }
    }
}