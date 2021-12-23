using System.IO;
using StardewModdingAPI;

namespace TehPers.Core.Api.Content
{
    public class ContentPackAssetProvider : IAssetProvider
    {
        private readonly IContentPack contentPack;

        public ContentPackAssetProvider(IContentPack contentPack)
        {
            this.contentPack = contentPack;
        }

        public T Load<T>(string path)
        {
            return this.contentPack.LoadAsset<T>(path);
        }

        public Stream Open(string path, FileMode mode)
        {
            var fullPath = Path.Combine(this.contentPack.DirectoryPath, path);
            var createMode =
                mode is FileMode.Create or FileMode.CreateNew or FileMode.OpenOrCreate or FileMode
                    .Append;
            if (createMode && Path.GetDirectoryName(fullPath) is { } dir)
            {
                Directory.CreateDirectory(dir);
            }

            return File.Open(fullPath, mode);
        }
    }
}