using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Api.Internal;

/// <summary>
/// Internal use only APIs.
/// </summary>
public interface IInternalApi
{
    /// <summary>
    /// Creates a default component provider.
    /// </summary>
    /// <returns>A new default component provider.</returns>
    IDefaultComponentProvider CreateDefaultComponentProvider();
}
