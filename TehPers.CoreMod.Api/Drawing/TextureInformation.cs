using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace TehPers.CoreMod.Api.Drawing {
    public class TextureInformation {
        public Texture2D Texture { get; }
        public Rectangle? SourceRectangle { get; }
        public Color Tint { get; }

        public TextureInformation(Texture2D texture) : this(texture, null, Color.White) { }
        public TextureInformation(Texture2D texture, Rectangle? sourceRectangle) : this(texture, sourceRectangle, Color.White) { }
        public TextureInformation(Texture2D texture, Rectangle? sourceRectangle, Color tint) {
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Tint = tint;
        }

        public static TextureInformation FromAssetFile(IContentHelper helper, string path) => TextureInformation.FromAssetFile(helper, path, null, Color.White);
        public static TextureInformation FromAssetFile(IContentHelper helper, string path, Rectangle? sourceRectangle) => TextureInformation.FromAssetFile(helper, path, sourceRectangle, Color.White);
        public static TextureInformation FromAssetFile(IContentHelper helper, string path, Rectangle? sourceRectangle, Color tint) {
            Texture2D texture = helper.Load<Texture2D>(path);
            return new TextureInformation(texture, sourceRectangle, tint);
        }
    }
}