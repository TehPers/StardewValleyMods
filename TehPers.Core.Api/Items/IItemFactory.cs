using StardewValley;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// A factory which can create instances of an <see cref="Item"/>.
    /// </summary>
    public interface IItemFactory
    {
        /// <summary>
        /// Gets the type of item this factory creates.
        /// </summary>
        string ItemType { get; }

        /// <summary>
        /// Creates an instance of this item. If the item can be stacked, then the stack size
        /// should be 1.
        /// </summary>
        /// <returns>An instance of this item.</returns>
        Item Create();
    }
}