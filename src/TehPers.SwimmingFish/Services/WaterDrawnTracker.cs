using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ninject;
using Ninject.Activation;
using StardewValley;
using System;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Setup;
using TehPers.SwimmingFish.Events;

namespace TehPers.SwimmingFish.Services
{
    internal class WaterDrawnTracker : Patcher
    {
        /// <summary>
        /// The singleton instance of this class. 
        /// </summary>
        private static WaterDrawnTracker? Instance { get; set; }

        /// <summary>
        /// Fires before water is drawn.
        /// </summary>
        public event EventHandler<WaterDrawingEventArgs>? WaterDrawing;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        private WaterDrawnTracker(Harmony harmony) : base(harmony)
        {
        }

        /// <summary>
        /// Gets or creates the singleton instance of this class.
        /// </summary>
        public static WaterDrawnTracker Create(IContext context)
        {
            WaterDrawnTracker.Instance ??= new(context.Kernel.Get<Harmony>());
            return WaterDrawnTracker.Instance;
        }

        /// <inheritdoc/>
        public override void Setup()
        {
            // Patch GameLocation.drawWaterTile
            this.Harmony.Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    nameof(GameLocation.drawWater),
                    new[] { typeof(SpriteBatch) }
                ),
                prefix: new(
                    typeof(WaterDrawnTracker),
                    nameof(WaterDrawnTracker.GameLocation_drawWater_Prefix)
                )
            );
        }

        /// <summary>
        /// The prefix for <see cref="GameLocation.drawWater(SpriteBatch)"/>.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony naming convention")]
        private static void GameLocation_drawWater_Prefix(GameLocation __instance, SpriteBatch b)
        {
            WaterDrawnTracker.Instance?.WaterDrawing?.Invoke(__instance, new(__instance, b));
        }
    }
}
