﻿using System;

namespace TehPers.Core.Api.Gameplay
{
    /// <summary>
    /// Seasons within the game. Because this is a flags-style enum, multiple seasons can be
    /// combined.
    /// </summary>
    [Flags]
    public enum Seasons
    {
        /// <summary>
        /// The spring season.
        /// </summary>
        Spring = 0b1,

        /// <summary>
        /// The summer season.
        /// </summary>
        Summer = 0b10,

        /// <summary>
        /// The fall season.
        /// </summary>
        Fall = 0b100,

        /// <summary>
        /// The winter season.
        /// </summary>
        Winter = 0b1000,

        /// <summary>
        /// No seasons.
        /// </summary>
        None = 0,

        /// <summary>
        /// All seasons in the game.
        /// </summary>
        All = Seasons.Spring | Seasons.Summer | Seasons.Fall | Seasons.Winter,
    }
}