using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Extensions;

namespace TehPers.CoreMod.Drawing {
    internal class DrawingInfo : IDrawingInfo {
        private readonly DrawingDelegator.NativeDraw _nativeDraw;

        public Texture2D Texture { get; private set; }
        public Rectangle? SourceRectangle { get; private set; }
        public Color Tint { get; private set; }
        public bool Cancelled { get; private set; }
        public Vector2 Scale { get; private set; }
        public bool Modified { get; private set; }
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

        public void SetSource(Texture2D texture, Rectangle? sourceRectangle) {
            Rectangle originalBounds = this.SourceRectangle ?? this.Texture.Bounds;
            Rectangle newBounds = sourceRectangle ?? texture.Bounds;

            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Scale = new Vector2(this.Scale.X * originalBounds.Width / newBounds.Width, this.Scale.Y * originalBounds.Height / newBounds.Height);
            this.Modified = true;
        }

        public void SetTint(Color tint) {
            this.Tint = tint;
            this.Modified = true;
        }

        public void AddTint(Color tint) {
            this.SetTint(this.Tint.Multiply(tint));
        }

        public void SetScale(float scale) => this.SetScale(new Vector2(scale, scale));
        public void SetScale(Vector2 scale) {
            this.Scale = scale;
            this.Modified = true;
        }

        public void StopPropagating() {
            this.Propagate = false;
            this.Modified = true;
        }

        public void Cancel() {
            this.StopPropagating();
            this.Cancelled = true;
        }

        public void DrawAndCancel() {
            this.Cancel();
            this._nativeDraw(this);
        }
    }
}