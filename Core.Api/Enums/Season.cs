using System;

namespace TehPers.Core.Api.Enums {
    [Flags]
    public enum Season {
        Spring = 1,
        Summer = 2,
        Fall = 4,
        Winter = 8,
    }
}