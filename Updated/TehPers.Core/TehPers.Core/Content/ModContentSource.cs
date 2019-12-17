using System.IO;
using StardewModdingAPI;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Content
{
    public class ModContentSource : IContentSource
    {
        private readonly IContentHelper contentHelper;
        private readonly string path;

        public ModContentSource(IMod mod) : this(mod.Helper) { }
        public ModContentSource(IModHelper helper)
        {
            this.contentHelper = helper.Content;
            this.path = helper.DirectoryPath;
        }

        public T Load<T>(string path)
        {
            return this.contentHelper.Load<T>(path);
        }

        public Stream Open(string path, FileMode mode)
        {
            return File.Open(Path.Combine(this.path, path), mode);
        }
    }
}
