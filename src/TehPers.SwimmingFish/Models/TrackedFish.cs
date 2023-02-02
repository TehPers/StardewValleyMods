using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.Core.Api.Items;

namespace TehPers.SwimmingFish.Models
{
    /// <summary>
    /// A swimming fish that is being tracked.
    /// </summary>
    internal class TrackedFish
    {
        public NamespacedKey ItemKey { get; }
        public Item Item { get; }
        public bool IsFish { get; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Scale { get; set; }
        public int SpawnTicksRemaining { get; set; }
        public int TicksRemaining { get; set; }
        public int DespawnTicksRemaining { get; set; }

        public TrackedFish(
            NamespacedKey itemKey,
            Item item,
            bool isFish,
            Vector2 position,
            Vector2 velocity,
            float scale,
            int spawnTicksRemaining,
            int ticksRemaining,
            int despawnTickRemaining
        )
        {
            this.ItemKey = itemKey;
            this.Item = item;
            this.IsFish = isFish;
            this.Position = position;
            this.Velocity = velocity;
            this.Scale = scale;
            this.TicksRemaining = ticksRemaining;
        }
    }
}
