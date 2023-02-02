using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Extensions.Drawing;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Setup;
using TehPers.SwimmingFish.Events;

namespace TehPers.SwimmingFish.Services
{
    internal sealed class FishRenderer : ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly WaterDrawnTracker waterDrawnTracker;
        private readonly FishTracker fishTracker;
        private readonly Dictionary<NamespacedKey, RenderTarget2D> fishBuffers;

        public FishRenderer(
            IModHelper helper,
            WaterDrawnTracker waterDrawnTracker,
            FishTracker fishTracker
        )
        {
            this.helper = helper;
            this.waterDrawnTracker = waterDrawnTracker;
            this.fishTracker = fishTracker;
            this.fishBuffers = new();
        }

        /// <inheritdoc/>
        public void Setup()
        {
            this.helper.Events.Display.RenderingWorld += this.PrepareFish;
            this.waterDrawnTracker.WaterDrawing += this.DrawFish;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.helper.Events.Display.RenderingWorld -= this.PrepareFish;
            this.waterDrawnTracker.WaterDrawing -= this.DrawFish;

            // Dispose of all the buffers
            foreach (var buffer in this.fishBuffers.Values)
            {
                buffer.Dispose();
            }

            this.fishBuffers.Clear();
        }

        /// <summary>
        /// Prepares the fish to be drawn by rendering them to a buffer.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        private void PrepareFish(object? sender, RenderingWorldEventArgs e)
        {
            // End the current batch
            e.SpriteBatch.End();
            var oldTargets = e.SpriteBatch.GraphicsDevice.GetRenderTargets();

            // Draw all the fish to a buffer
            var drawn = new HashSet<NamespacedKey>();
            foreach (var fish in this.fishTracker.GetFish())
            {
                // Skip if the fish has already been drawn
                if (!drawn.Add(fish.ItemKey))
                {
                    continue;
                }

                // Get source sprite size
                if (fish.Item.GetDefaultDrawingProperties() is not {SourceSize: var sourceSize})
                {
                    continue;
                }

                // Get the buffer
                var size = sourceSize;
                var buffer = this.fishBuffers.GetOrAdd(
                    fish.ItemKey,
                    () => new(e.SpriteBatch.GraphicsDevice, (int)size.X, (int)size.Y)
                );

                // Verify the size of the buffer
                if (buffer.Width != (int)size.X || buffer.Height != (int)size.Y)
                {
                    buffer.Dispose();
                    buffer = new(e.SpriteBatch.GraphicsDevice, (int)size.X, (int)size.Y);
                    this.fishBuffers[fish.ItemKey] = buffer;
                }

                // Draw the fish to the buffer
                e.SpriteBatch.GraphicsDevice.SetRenderTarget(buffer);
                e.SpriteBatch.GraphicsDevice.Clear(Color.Transparent);
                e.SpriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp
                );
                fish.Item.DrawInMenuCorrected(
                    e.SpriteBatch,
                    Vector2.Zero,
                    0.25f,
                    1.0f,
                    1.0f,
                    StackDrawType.Hide,
                    Color.White,
                    false,
                    new TopLeftDrawOrigin()
                );
                e.SpriteBatch.End();
            }

            // Reset the batch
            e.SpriteBatch.GraphicsDevice.SetRenderTargets(oldTargets);
            e.SpriteBatch.Begin(
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp
            );
        }

        /// <summary>
        /// Draws the fish at a tile.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        public void DrawFish(object? sender, WaterDrawingEventArgs e)
        {
            // Draw each fish
            foreach (var fish in this.fishTracker.GetFish())
            {
                if (!this.fishBuffers.TryGetValue(fish.ItemKey, out var buffer))
                {
                    continue;
                }

                var position = Game1.GlobalToLocal(Game1.viewport, fish.Position);
                var localCenter = new Vector2(buffer.Width / 2.0f, buffer.Height / 2.0f);
                var finalScale = (fish.SpawnTicksRemaining, fish.TicksRemaining) switch
                {
                    // Spawning
                    (> 0, _) => fish.Scale * (1 - fish.SpawnTicksRemaining / 60.0f),
                    // Normal
                    (_, > 0) => fish.Scale,
                    // Despawning
                    _ => fish.Scale * fish.DespawnTicksRemaining / 60.0f,
                };
                var rotationDeg = fish.IsFish ? fish.Velocity.X > 0 ? 45.0f : -45.0f : 0.0f;
                var spriteEffects = fish.Velocity.X > 0
                    ? SpriteEffects.None
                    : SpriteEffects.FlipHorizontally;
                e.Batch.Draw(
                    buffer,
                    position + fish.Scale * localCenter,
                    null,
                    Color.White,
                    rotationDeg * (float)Math.PI / 180.0f,
                    localCenter,
                    finalScale,
                    spriteEffects,
                    0.55f
                );
            }
        }
    }
}
