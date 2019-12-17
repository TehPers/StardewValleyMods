namespace TehPers.FishingFramework.Api.Config
{
    public interface IGlobalConfiguration
    {
        /// <summary>Whether vanilla legendary fish should have overridden catching behavior.</summary>
        bool ShouldOverrideVanillaLegendaries { get; }
    }
}