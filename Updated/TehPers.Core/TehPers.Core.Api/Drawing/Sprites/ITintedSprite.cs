namespace TehPers.Core.Api.Drawing.Sprites {
    public interface ITintedSprite : ISprite {
        /// <summary>The sprite, which contains a reference to the texture it is contained in and the rectangle on that texture it is located in.</summary>
        ISprite Sprite { get; }

        /// <summary>The color to tint the sprite when it's drawn.</summary>
        SColor Tint { get; }
    }
}