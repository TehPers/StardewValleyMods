namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// A reservation for an index.
    /// </summary>
    public interface IIndexReservation
    {
        /// <summary>
        /// Tries to get the assigned index. If no index has been assigned yet (i.e. in the title screen), then this method returns <see langword="false"/>.
        /// </summary>
        /// <param name="index">The assigned item index.</param>
        /// <returns><see langword="true"/> if an index has been assigned, or <see langword="false"/> otherwise.</returns>
        bool TryGetIndex(out int index);
    }
}