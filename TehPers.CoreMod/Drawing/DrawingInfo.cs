using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Extensions;

namespace TehPers.CoreMod.Drawing {
    internal class DrawingInfo : IDrawingInfo, IDrawingInfoInternal {
        public Texture2D Texture { get; private set; }
        public Rectangle? SourceRectangle { get; private set; }
        public Rectangle Destination { get; private set; }
        public Color Tint { get; private set; }
        public SpriteBatch Batch { get; }
        public Vector2 Origin { get; private set; }
        public float Rotation { get; private set; }
        public SpriteEffects Effects { get; private set; }
        public float Depth { get; private set; }

        public bool Cancelled { get; private set; }
        public bool Modified { get; private set; }
        public bool Propagate { get; private set; }

        private DrawingInfo() {
            this.Cancelled = false;
            this.Modified = false;
            this.Propagate = true;
        }

        public DrawingInfo(SpriteBatch batch, Texture2D texture, Rectangle? sourceRectangle, Rectangle destination, Color tint, Vector2 origin, float rotation, SpriteEffects effects, float depth) : this() {
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Destination = destination;
            this.Tint = tint;
            this.Batch = batch;
            this.Origin = origin;
            this.Rotation = rotation;
            this.Effects = effects;
            this.Depth = depth;
        }

        public DrawingInfo(SpriteBatch batch, Texture2D texture, Rectangle? sourceRectangle, Vector2 destination, Color tint, Vector2 origin, float rotation, SpriteEffects effects, float depth) : this() {
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.Tint = tint;
            this.Batch = batch;
            this.Origin = origin;
            this.Rotation = rotation;
            this.Effects = effects;
            this.Depth = depth;

            Rectangle sourceBounds = this.SourceRectangle ?? this.Texture.Bounds;
            this.Destination = new Rectangle((int) destination.X, (int) destination.Y, sourceBounds.Width, sourceBounds.Height);
        }

        public void SetTint(Color tint) {
            this.Tint = tint;
            this.Modified = true;
        }

        public void SetEffects(SpriteEffects effects) {
            this.Effects = effects;
        }

        public void SetDepth(float depth) {
            this.Depth = depth;
        }

        public void AddTint(Color tint) {
            this.SetTint(this.Tint.Multiply(tint));
        }

        public void Cancel() {
            this.StopPropagating();
            this.Cancelled = true;
        }

        public void DrawAndCancel() {
            this.Cancel();
            this.Draw();
        }

        public void SetScale(float scale) => this.SetScale(new Vector2(scale, scale));
        public void SetScale(Vector2 scale) {
            Rectangle source = this.SourceRectangle ?? this.Texture.Bounds;

            // Calculate new size for the rectangle
            float newWidth = scale.X * source.Width;
            float newHeight = scale.Y * source.Height;

            // Calculate new location for the rectangle
            float xOffsetScale = this.Origin.X / source.Width;
            float yOffsetScale = this.Origin.Y / source.Height;
            float destOriginX = this.Destination.X + this.Destination.Width * xOffsetScale;
            float destOriginY = this.Destination.Y + this.Destination.Height * yOffsetScale;
            float newX = destOriginX - this.Destination.Width * xOffsetScale;
            float newY = destOriginY - this.Destination.Height * yOffsetScale;

            // Set the destination rectangle
            this.Destination = new Rectangle((int) newX, (int) newY, (int) newWidth, (int) newHeight);
        }

        public Vector2 GetScale() {
            Rectangle source = this.SourceRectangle ?? this.Texture.Bounds;
            return new Vector2((float) this.Destination.Width / source.Width, (float) this.Destination.Height / source.Height);
        }

        public void SetSource(Texture2D texture, Rectangle? sourceRectangle) {
            Rectangle prevSource = this.SourceRectangle ?? this.Texture.Bounds;
            Rectangle newSource = sourceRectangle ?? texture.Bounds;

            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            this.SetScale(this.GetScale() * new Vector2((float) newSource.Width / prevSource.Width, (float) newSource.Height / prevSource.Height));
            this.Modified = true;
        }

        public void SetDestination(Rectangle destination) {
            this.Destination = destination;
            this.Modified = true;
        }

        public void SetOrigin(Vector2 origin) {
            this.Origin = origin;
            this.Modified = true;
        }

        public void SetRotation(float rotation) {
            this.Rotation = rotation;
            this.Modified = true;
        }

        public void StopPropagating() {
            this.Propagate = false;
            this.Modified = true;
        }

        public void Reset() {
            this.Modified = false;
        }

        public void Draw() {
            this.Batch.Draw(this.Texture, this.Destination, this.SourceRectangle, this.Tint, this.Rotation, this.Origin, this.Effects, this.Depth);
        }
    }
}