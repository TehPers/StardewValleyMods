namespace TehPers.Core.Api.Setup
{
    /// <summary>
    /// A service which requires setup on game launch.
    /// </summary>
    public interface ISetup
    {
        /// <summary>
        /// Sets up this service.
        /// </summary>
        void Setup();
    }
}