using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewValley;

namespace TehPers.FishingOverhaul.Services.Tokens
{
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Used by Content Patcher."
    )]
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Used by Content Patcher."
    )]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by Content Patcher.")]
    internal class BooksFoundToken
    {
        public bool AllowsInput()
        {
            return true;
        }

        public bool CanHaveMultipleValues(string? input = null)
        {
            return false;
        }

        public bool IsReady()
        {
            return Game1.player is not null;
        }

        public IEnumerable<string> GetValues(string? id)
        {
            if (id == null )
            {
                return Enumerable.Empty<string>();
            }

            // Get player's archaeology
            if (Game1.player is not { archaeologyFound: { } archaeologyFound })
            {
                return Enumerable.Empty<string>();
            }

            // Get the stats for this ID
            if (!archaeologyFound.TryGetValue(id, out var value))
            {
                return Enumerable.Empty<string>();
            }

            return new[] { value[0].ToString("G") };
        }
    }
}
