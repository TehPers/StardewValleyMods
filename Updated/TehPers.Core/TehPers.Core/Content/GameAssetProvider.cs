using System.IO;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Content
{
    public class GameAssetProvider : IAssetProvider
    {
        public T Load<T>(string path)
        {
            return Game1.content.Load<T>(path);
        }

        public Stream Read(string path, FileMode mode)
        {
            return File.OpenRead(Path.Combine(Constants.DataPath, path));
        }
    }
}