using System;

namespace TehPers.CoreMod.Api.Environment {
    [Flags]
    public enum Weather {
        Sunny = 1,
        Rainy = 2,

        Any = Weather.Sunny | Weather.Rainy,

        [Obsolete("Use " + nameof(Weather.Any) + " instead")]
        All = Weather.Any
    }
}