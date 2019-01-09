using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.CoreMod.Api.Drawing {
    public interface IReadonlyDrawingInfo {
        /// <summary>The amount to scale the source by when drawing the texture.</summary>
        Vector2 Scale { get; }

        /// <summary>The rectangle on the source texture representing the area to be drawn, or <c>null</c> if the whole source texture should be drawn.</summary>
        Rectangle? SourceRectangle { get; }

        /// <summary>The source texture to draw from.</summary>
        Texture2D Texture { get; }

        /// <summary>The color being applied to the texture while drawing. The formula used depends on the state of the <see cref="SpriteBatch"/>, but usually this color is multiplied by the source to calculate the destination color.</summary>
        Color Tint { get; }
    }

    public interface IDrawingInfo : IReadonlyDrawingInfo {
        /// <summary>True if the original drawing call has been cancelled.</summary>
        bool Cancelled { get; }

        /// <summary>True if this drawing call has been modified by the current overrider.</summary>
        bool Modified { get; }

        /// <summary>True if the drawing information should be propagated to the next overrider afterwards.</summary>
        bool Propagate { get; }

        /// <summary>Adds a tint by multiplying it with the current tint.</summary>
        /// <param name="tint">The tint to add.</param>
        void AddTint(Color tint);

        /// <summary>Prevents this drawing information from being drawn.</summary>
        void Cancel();

        /// <summary>Immediately draws the texture and prevents the <see cref="Core.Drawing.DrawingDelegator"/> from drawing it.</summary>
        void DrawAndCancel();

        /// <summary>Sets the scaling of the source image.</summary>
        /// <param name="scale">The amount to scale the source by when drawing.</param>
        void SetScale(float scale);

        /// <summary>Sets the scaling of the source image.</summary>
        /// <param name="scale">The amount to scale the source by when drawing.</param>
        void SetScale(Vector2 scale);

        /// <summary>Sets the source texture and rectangle.</summary>
        /// <param name="texture">The new source texture.</param>
        /// <param name="sourceRectangle">The new source rectangle.</param>
        void SetSource(Texture2D texture, Rectangle? sourceRectangle);

        /// <summary>Sets the tint color.</summary>
        /// <param name="tint">The new tint color.</param>
        void SetTint(Color tint);

        /// <summary>Prevents any other drawing overriders from handling this draw call afterwards.</summary>
        void StopPropagating();
    }
}