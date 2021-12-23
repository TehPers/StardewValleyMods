using System;
using StardewModdingAPI;

namespace TehPers.Core.Api.Content
{
    /// <summary>
    /// Tracks changes to assets.
    /// </summary>
    public interface IAssetTracker
    {
        /// <summary>
        /// Invoked whenever an asset is loaded or reloaded.
        /// </summary>
        public event EventHandler<IAssetData>? AssetLoading;
    }
}
