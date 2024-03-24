using System;
using System.IO;
using StardewModdingAPI;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Content
{
    public class GameAssetProvider : IAssetProvider
    {
        private readonly IGameContentHelper contentHelper;

        public GameAssetProvider(IModHelper helper)
        {
            this.contentHelper = helper.GameContent;
        }

        public T Load<T>(string path) where T : notnull
        {
            return this.contentHelper.Load<T>(path);
        }

        public Stream Open(string path, FileMode mode)
        {
            if (mode != FileMode.Open)
            {
                throw new ArgumentException("Game assets can only be read from.", nameof(mode));
            }

            var fullPath = Path.Combine(Constants.DataPath, path);
            return File.OpenRead(fullPath);
        }
    }
}
