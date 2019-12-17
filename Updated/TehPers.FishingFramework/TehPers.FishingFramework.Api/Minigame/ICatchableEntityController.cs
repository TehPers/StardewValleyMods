using System;

namespace TehPers.FishingFramework.Api.Minigame
{
    /// <summary>
    /// Controller for the state of an item in the fishing minigame.
    /// </summary>
    /// <typeparam name="TItem">The type of item being controlled.</typeparam>
    public interface ICatchableEntityController<TItem>
        where TItem : BobberBarCatchableItem
    {
        /// <summary>
        /// Updates the state of an item in the fishing minigame.
        /// </summary>
        /// <param name="itemState">The state of the item being updated.</param>
        /// <param name="controllerState">The current state of the controller.</param>
        /// <param name="fishingState">The state of the fishing minigame.</param>
        /// <param name="delta">The amount of time that has elapsed since the last update tick.</param>
        void Update(ref TItem itemState, ref object controllerState, IFishingState fishingState, TimeSpan delta);
    }

    /// <summary>
    /// Generic motion controller for fish which determines how a fish behaves in the bobber bar.
    /// </summary>
    /// <typeparam name="TItem">The type of item being controlled.</typeparam>
    /// <typeparam name="TState">The state of the motion controller.</typeparam>
    public interface ICatchableEntityController<TItem, TState> : ICatchableEntityController<TItem>
        where TItem : BobberBarCatchableItem
    {
        /// <summary>
        /// Updates the state of an item in the fishing minigame.
        /// </summary>
        /// <param name="itemState">The state of the item being updated.</param>
        /// <param name="controllerState">The current state of the controller.</param>
        /// <param name="fishingState">The state of the fishing minigame.</param>
        /// <param name="delta">The amount of time that has elapsed since the last update tick.</param>
        void Update(ref TItem itemState, ref TState controllerState, IFishingState fishingState, TimeSpan delta);
    }
}