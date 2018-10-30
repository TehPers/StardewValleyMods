using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.FestiveSlimes.Items {
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

        public static TextureInformation FromAssetFile(string relativePath) => TextureInformation.FromAssetFile(relativePath, null, Color.White);
        public static TextureInformation FromAssetFile(string relativePath, Rectangle? sourceRectangle) => TextureInformation.FromAssetFile(relativePath, sourceRectangle, Color.White);
        public static TextureInformation FromAssetFile(string relativePath, Rectangle? sourceRectangle, Color tint) {
            Texture2D texture = ModFestiveSlimes.Instance.Helper.Content.Load<Texture2D>($"assets/{relativePath}");
            return new TextureInformation(texture, sourceRectangle, tint);
        }
    }
}