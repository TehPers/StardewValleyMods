using System;
using System.IO;
using StardewModdingAPI;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Content
{
    public class GameAssetProvider : IAssetProvider
    {
        private readonly IModHelper helper;

        public GameAssetProvider(IModHelper helper)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public T Load<T>(string path)
        {
            return this.helper.Content.Load<T>(path, ContentSource.GameContent);
        }

        public Stream Open(string path, FileMode mode)
        {
            var fullPath = Path.Combine(Constants.DataPath, path);
            if (Path.GetDirectoryName(fullPath) is { } dir)
            {
                Directory.CreateDirectory(dir);
            }

            return File.OpenRead(fullPath);
        }
    }
}