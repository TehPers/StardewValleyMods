using System;

namespace TehPers.Core.Api.Chrono
{
    /// <summary>A vanilla weather in Stardew Valley.</summary>
    [Flags]
    public enum Weathers
    {
        /// <summary>Sunny (or snowy) weather.</summary>
        Sunny = 1,

        /// <summary>Rainy weather.</summary>
        Rainy = 2,

        /// <summary>Any weather.</summary>
        Any = Weathers.Sunny | Weathers.Rainy,
    }
}