using System;

namespace TehPers.Core.Api.Chrono {
    [Flags]
    public enum Weather {
        Sunny = 1,
        Rainy = 2,

        Any = Weather.Sunny | Weather.Rainy
    }
}