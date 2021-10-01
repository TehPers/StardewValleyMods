using System;

namespace TehPers.FishingOverhaul.Extensions
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random rand, double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }
    }
}