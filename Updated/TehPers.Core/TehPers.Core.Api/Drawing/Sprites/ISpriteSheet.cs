namespace TehPers.Core.Api.Drawing.Sprites {
    /// <summary>
    /// A texture containing several sprites.
    /// </summary>
    public interface ISpriteSheet : ITextureEvents {
        /// <summary>
        /// Gets a reference to the tracked texture used by this sprite sheet. It is used to draw objects from the sprite sheet.
        /// </summary>
        ITrackedTexture TrackedTexture { get; }

        /// <summary>
        /// Tries to get a reference to a specific sprite on this sprite sheet.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <param name="sprite">The sprite, if a sprite with the given index exists.</param>
        /// <returns>True if a sprite was found, false if the index isn't associated with any sprites on this sprite sheet.</returns>
        bool TryGetSprite(int index, out ISprite sprite);

        /// <summary>
        /// Gets the index of the sprite containing a particular point on the sprite sheet. This index may be outside the range of the sprite sheet, so make sure to check that the index is valid.
        /// </summary>
        /// <param name="u">The x-coordinate of the point.</param>
        /// <param name="v">The y-coordinate of the point.</param>
        /// <returns>The sprite's index.</returns>
        int GetIndex(int u, int v);
    }
}