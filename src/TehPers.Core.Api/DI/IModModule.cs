using Ninject.Modules;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// A loadable unit that defines bindings for your mod.
    /// </summary>
    public interface IModModule : INinjectModule, IModBindingRoot, IHaveModKernel
    {
    }
}