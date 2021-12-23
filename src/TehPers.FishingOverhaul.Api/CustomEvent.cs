using TehPers.Core.Api.Items;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// A custom fishing event.
    /// </summary>
    /// <param name="Catch">The catch info.</param>
    /// <param name="EventKey">The key for the event.</param>
    public record CustomEvent(CatchInfo Catch, NamespacedKey EventKey);
}