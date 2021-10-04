using System;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// Type of water that fish can be caught in. Each location handles these values differently.
    /// </summary>
    [Flags]
    public enum WaterTypes
    {
        River = 0b1,
        PondOrOcean = 0b10, // Pond and ocean use the same ID
        Freshwater = 0b100,

        None = 0,
        All = WaterTypes.River | WaterTypes.PondOrOcean | WaterTypes.Freshwater
    }
}