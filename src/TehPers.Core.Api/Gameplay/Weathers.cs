using System;

namespace TehPers.Core.Api.Gameplay
{
    /// <summary>
    /// Weathers within the game. Because this is a flags-style enum, multiple weathers can be
    /// combined.
    /// </summary>
    [Flags]
    public enum Weathers
    {
        /// <summary>
        /// Sunny weather.
        /// </summary>
        Sunny = 0b1,

        /// <summary>
        /// Rainy weather.
        /// </summary>
        Rainy = 0b10,

        /// <summary>
        /// No weathers.
        /// </summary>
        None = 0,

        /// <summary>
        /// All weathers in the game.
        /// </summary>
        All = Weathers.Sunny | Weathers.Rainy,
    }
}