using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace TehPers.Logistics.Machines {
    public interface IMachineInformation {
        /// <summary>The location this machine is located at.</summary>
        GameLocation Location { get; }

        /// <summary>The coordinates of this machine.</summary>
        Vector2 Tile { get; }

        /// <summary>Gets the tracked state of the machine.</summary>
        /// <typeparam name="T">The type of state being tracked.</typeparam>
        /// <returns>The tracked state.</returns>
        T GetState<T>() where T : new();

        /// <summary>Gets the tracked state of the machine.</summary>
        /// <typeparam name="T">The type of state being tracked.</typeparam>
        /// <param name="stateFactory">A factory method to create new state objects.</param>
        /// <returns>The tracked state.</returns>
        T GetState<T>(Func<T> stateFactory);
    }
}