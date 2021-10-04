using System.Collections.Generic;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Content source for fishing data. Registering a global binding for this interface will allow
    /// custom fishing content to be added to the game.
    /// </summary>
    public interface IFishingContentSource
    {
        /// <summary>
        /// Reloads the fishing data.
        /// </summary>
        /// <returns>The reloaded fishing data.</returns>
        IEnumerable<FishingContent> Reload();
    }
}