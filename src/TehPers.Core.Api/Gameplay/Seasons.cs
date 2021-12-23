using System;

namespace TehPers.Core.Api.Gameplay
{
    [Flags]
    public enum Seasons
    {
        Spring = 0b1,
        Summer = 0b10,
        Fall = 0b100,
        Winter = 0b1000,

        None = 0,
        All = Seasons.Spring | Seasons.Summer | Seasons.Fall | Seasons.Winter,
    }
}