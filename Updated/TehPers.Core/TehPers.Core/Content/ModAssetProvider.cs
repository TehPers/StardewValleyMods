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

        public Stream Read(string path, FileMode mode)
        {
            return File.Open(Path.Combine(this.modPath, path), mode);
        }
    }
}
