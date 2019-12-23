using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// Provides all items that can be provided. This type is merely for convenience. Item providers should be registered through your mod's <see cref="IModKernel"/>.
    /// </summary>
    public interface IGlobalItemProvider : IItemProvider
    {
    }
}
