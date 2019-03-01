using System;

namespace TehPers.CoreMod.Api.Environment {
    /// <summary>The different possible types of bodies of water.</summary>
    [Flags]
    public enum WaterTypes : byte {
        /// <summary>Game ID is 1</summary>
        Lake = 1,

        /// <summary>Game ID is 0</summary>
        River = 2,

        /// <summary>Game ID is -1</summary>
        Any = WaterTypes.Lake | WaterTypes.River,

        [Obsolete("Use " + nameof(Weather.Any) + " instead")]
        All = WaterTypes.Any
    }
}
