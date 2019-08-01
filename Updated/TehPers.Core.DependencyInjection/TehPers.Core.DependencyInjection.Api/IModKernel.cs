using System;
using Ninject;
using StardewModdingAPI;

namespace TehPers.Core.DependencyInjection.Api
{
    public interface IModKernel : IKernel
    {
        IMod ParentMod { get; }
        IKernel Global { get; }

        void RegisterEvents<T>();
        void RegisterEvents(Type implementationType);
    }
}