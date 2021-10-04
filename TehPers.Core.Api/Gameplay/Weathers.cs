using System;

namespace TehPers.Core.Api.Gameplay
{
    [Flags]
    public enum Weathers
    {
        Sunny = 0b1,
        Rainy = 0b10,
     
        None = 0,
        All = Weathers.Sunny | Weathers.Rainy,
    }
}