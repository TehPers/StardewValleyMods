using System;

namespace TehPers.Core.Api.Chrono
{
    /// <summary>A season in Stardew Valley.</summary>
    [Flags]
    public enum Seasons
    {
        /// <summary>The spring season.</summary>
        Spring = 1,

        /// <summary>The summer season.</summary>
        Summer = 2,

        /// <summary>The fall season.</summary>
        Fall = 4,

        /// <summary>The winter season.</summary>
        Winter = 8,

        /// <summary>No seasons.</summary>
        None = 0,

        /// <summary>Any season.</summary>
        Any = Seasons.Spring | Seasons.Summer | Seasons.Fall | Seasons.Winter,
    }
}