using System;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.CoreMod.Api.Drawing {
    public interface IDrawingApi {

        /// <summary>A texture containing a single white pixel.</summary>
        Texture2D WhitePixel { get; }

        /// <summary>Adds a drawing overrider which can override drawing calls made to <see cref="SpriteBatch"/>.</summary>
        /// <param name="overrider">The callback that may override drawing calls.</param>
        void AddOverride(Action<IDrawingInfo> overrider);

        /// <summary>Removes a drawing overrider.</summary>
        /// <param name="overrider">The overrider to remove.</param>
        /// <returns>True if removed, false if not found.</returns>
        bool RemoveOverride(Action<IDrawingInfo> overrider);
    }
}