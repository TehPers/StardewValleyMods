using System.IO;
using Microsoft.Xna.Framework.Content;
using Ninject;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.Api.Content
{
    /// <summary>
    /// A provider for game assets.
    /// You may use <see cref="ContentSourceAttribute"/> to specify which content source you want to use.<br />
    /// <br />
    /// For custom content sources, it is recommended that you create a class that extends <see cref="ConstraintAttribute"/> to identify which one you want to inject into your service.
    /// </summary>
    public interface IAssetProvider
    {
        /// <summary>
        /// Loads an asset from this content source.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="path">The path to the asset relative to this content source.</param>
        /// <returns>The loaded asset.</returns>
        /// <exception cref="ContentLoadException">The asset failed to load.</exception>
        T Load<T>(string path);

        /// <summary>
        /// Opens a file in this content source.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="mode">The mode to open the file in.</param>
        /// <returns>The file's stream.</returns>
        Stream Open(string path, FileMode mode);
    }
}
