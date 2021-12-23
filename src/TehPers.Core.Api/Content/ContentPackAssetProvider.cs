using System.IO;
using StardewModdingAPI;

namespace TehPers.Core.Api.Content
{
    /// <summary>
    /// Asset provider for a content pack.
    /// </summary>
    /// <inheritdoc cref="IAssetProvider"/>
    public class ContentPackAssetProvider : IAssetProvider
    {
        private readonly IContentPack contentPack;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPackAssetProvider"/> class.
        /// </summary>
        /// <param name="contentPack">The content pack to provide assets for.</param>
        public ContentPackAssetProvider(IContentPack contentPack)
        {
            this.contentPack = contentPack;
        }

        /// <inheritdoc/>
        public T Load<T>(string path)
        {
            return this.contentPack.LoadAsset<T>(path);
        }

        /// <inheritdoc/>
        public Stream Open(string path, FileMode mode)
        {
            var fullPath = Path.Combine(this.contentPack.DirectoryPath, path);
            var createMode = mode is FileMode.Create or FileMode.CreateNew or FileMode.OpenOrCreate
                or FileMode.Append;
            if (createMode && Path.GetDirectoryName(fullPath) is { } dir)
            {
                Directory.CreateDirectory(dir);
            }

            return File.Open(fullPath, mode);
        }
    }
}