namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// The state of the fishing minigame.
    /// </summary>
    /// <param name="IsPerfect">Whether the current catch is still perfect.</param>
    /// <param name="Treasure">The state of the treasure in the minigame.</param>
    public record MinigameState(bool IsPerfect, TreasureState Treasure);
}