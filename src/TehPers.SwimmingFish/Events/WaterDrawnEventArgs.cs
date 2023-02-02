using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace TehPers.SwimmingFish.Events
{
    internal class WaterDrawingEventArgs : EventArgs
    {
        /// <summary>
        /// The location being drawn at.
        /// </summary>
        public GameLocation Location { get; }

        /// <summary>
        /// The sprite batch being used for drawing.
        /// </summary>
        public SpriteBatch Batch { get; }

        public WaterDrawingEventArgs(GameLocation location, SpriteBatch batch)
        {
            this.Location = location;
            this.Batch = batch;
        }
    }
}
