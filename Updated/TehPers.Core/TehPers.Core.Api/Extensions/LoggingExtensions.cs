using System;
using System.Runtime.CompilerServices;
using StardewModdingAPI;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>
    /// Extension methods for logging purposes.
    /// </summary>
    public static class LoggingExtensions
    {

        /// <summary>
        /// Logs a message with information about where the logging message was created. The caller information is automatically generated but may be overridden if desired.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The message's severity.</param>
        /// <param name="callerPath">The file path referenced in the logging message. This will be automatically generated but may be overridden.</param>
        /// <param name="callerMember">The member referenced in the logging message. This will be automatically generated but may be overridden.</param>
        /// <param name="callerLine">The line number referenced in the logging message. This will be automatically generated but may be overridden.</param>
        public static void LogWithLocation(
            this IMonitor monitor,
            string message,
            LogLevel level = LogLevel.Trace,
            [CallerFilePath] string callerPath = "",
            [CallerMemberName] string callerMember = "",
            [CallerLineNumber] long callerLine = 0)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));
            _ = monitor ?? throw new ArgumentNullException(nameof(monitor));

            monitor.Log($"[{callerPath}:{callerMember}:{callerLine}] {message}", level);
        }
    }
}
