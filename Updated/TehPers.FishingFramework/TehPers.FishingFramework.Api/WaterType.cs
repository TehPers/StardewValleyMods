using System;

namespace TehPers.FishingFramework.Api
{
    [Flags]
    public enum WaterType
    {
        Lake = 0b1,
        River = 0b10,

        Any = Lake | River
    }
}
