namespace TehPers.FishingFramework.Api.Minigame
{
    /// <inheritdoc />
    /// <typeparam name="TItem">The type of item being caught.</typeparam>
    public class BobberBarCatchableItem<TItem> : BobberBarCatchableItem
    {
        /// <summary>
        /// Gets or sets the item being caught.
        /// </summary>
        public TItem Item { get; set; }
    }
}