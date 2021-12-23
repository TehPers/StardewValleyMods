using System.IO;
using StardewModdingAPI;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Content
{
    public class ModAssetProvider : IAssetProvider
    {
        private readonly IContentHelper contentHelper;
        private readonly string modPath;

        public ModAssetProvider(IModHelper helper)
        {
            this.contentHelper = helper.Content;
            this.modPath = helper.DirectoryPath;
        }

        public T Load<T>(string path)
        {
            return this.contentHelper.Load<T>(path);
        }

        public Stream Open(string path, FileMode mode)
        {
            var fullPath = Path.Combine(this.modPath, path);
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