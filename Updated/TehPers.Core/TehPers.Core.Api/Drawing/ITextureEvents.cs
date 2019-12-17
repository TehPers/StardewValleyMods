using System;

namespace TehPers.Core.Api.Drawing {
    public interface ITextureEvents {
        /// <summary>Raised when a texture is being drawn to the screen.</summary>
        event EventHandler<IDrawingInfo> Drawing;

        /// <summary>Raised after a texture is drawn to the screen.</summary>
        event EventHandler<IReadonlyDrawingInfo> Drawn;
    }
}