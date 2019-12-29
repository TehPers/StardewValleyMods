using System;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// Enumeration of types of water that can be found in a map.
    /// </summary>
    [Flags]
    public enum WaterTypes
    {
        /// <summary>
        /// Water that is part of a lake. For example, the lake in the forest map is classified as lake water.
        /// </summary>
        Lake = 0b1,

        /// <summary>
        /// Water that is part of a river. For example, the river in the forest map is classified as river water.
        /// </summary>
        River = 0b10,

        /// <summary>
        /// Water of any type.
        /// </summary>
        Any = WaterTypes.Lake | WaterTypes.River,
    }
}
