using System;
using Ninject;
using StardewModdingAPI;

namespace TehPers.Core.DependencyInjection.Api
{
    public interface IDependencyInjectionApi : IDisposable
    {
        IKernel Global { get; }

        IModKernel GetModKernel(IMod mod);
    }
}
