using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Static.Extensions;

namespace TehPers.CoreMod.Drawing {
    internal class DrawingInfo : IDrawingInfo {
        private readonly DrawingDelegator.NativeDraw _nativeDraw;

        /// <inheritdoc />
        public Texture2D Texture { get; private set; }

        /// <inheritdoc />
        public Rectangle? SourceRectangle { get; private set; }

        /// <inheritdoc />
        public Color Tint { get; private set; }

        /// <inheritdoc />
        public bool Cancelled { get; private set; }

        /// <inheritdoc />
        public Vector2 Scale { get; private set; }

        /// <inheritdoc />
        public bool Modified { get; private set; }

        /// <inheritdoc />
        public bool Propagate { get; private set; }

        internal DrawingInfo(Texture2D texture, in Rectangle? sourceRectangle, in Color tint, in Vector2 scale, DrawingDelegator.NativeDraw nativeDraw, Action<Action> resetSignal) {
            this._nativeDraw = nativeDraw;
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Tint = tint;
            this.Scale = scale;

            this.Cancelled = false;
            this.Modified = false;
            this.Propagate = true;

            resetSignal(() => this.Modified = false);
        }

        /// <inheritdoc />
        public void SetSource(Texture2D texture, Rectangle? sourceRectangle) {
            Rectangle originalBounds = this.SourceRectangle ?? this.Texture.Bounds;
            Rectangle newBounds = sourceRectangle ?? texture.Bounds;

            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Scale = new Vector2(this.Scale.X * originalBounds.Width / newBounds.Width, this.Scale.Y * originalBounds.Height / newBounds.Height);
            this.Modified = true;
        }

        /// <inheritdoc />
        public void SetTint(Color tint) {
            this.Tint = tint;
            this.Modified = true;
        }

        /// <inheritdoc />
        public void AddTint(Color tint) {
            this.SetTint(this.Tint.Multiply(tint));
        }
        
        /// <inheritdoc />
        public void SetScale(float scale) => this.SetScale(new Vector2(scale, scale));

        /// <inheritdoc />
        public void SetScale(Vector2 scale) {
            this.Scale = scale;
            this.Modified = true;
        }

        /// <inheritdoc />
        public void StopPropagating() {
            this.Propagate = false;
            this.Modified = true;
        }

        /// <inheritdoc />
        public void Cancel() {
            this.StopPropagating();
            this.Cancelled = true;
        }

        /// <inheritdoc />
        public void DrawAndCancel() {
            this.Cancel();
            this._nativeDraw(this);
        }
    }
}