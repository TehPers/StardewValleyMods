using System;
using System.Linq;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Modules;
using StardewModdingAPI;
using TehPers.Core.DependencyInjection.Api;
using TehPers.Core.DependencyInjection.Api.Extensions;
using TehPers.Core.DependencyInjection.Api.Lifecycle.GameLoop;
using TehPers.Core.DependencyInjection.Lifecycle;

namespace TehPers.Core.DependencyInjection
{
    internal class ModKernel : ChildKernel, IModKernel
    {
        private readonly Type[] _eventHandlerTypes;

        public IMod ParentMod { get; }
        public IKernel Global { get; }

        public ModKernel(IMod parentMod, IKernel global, params INinjectModule[] modules) : base(global, modules)
        {
            this.ParentMod = parentMod;
            this.Global = global;

            this._eventHandlerTypes = ModKernel.GetLifecycleHandlerTypes();
        }

        public ModKernel(IMod parentMod, IKernel global, INinjectSettings settings, params INinjectModule[] modules) : base(global, settings, modules)
        {
            this.ParentMod = parentMod;
            this.Global = global;
        }

        private static Type[] GetLifecycleHandlerTypes()
        {
            return new[] {
                typeof(IUpdateTickingHandler),
                typeof(IUpdateTickedHandler)
            };
        }

        public void RegisterEvents<T>()
        {
            this.RegisterEvents(typeof(T));
        }

        public void RegisterEvents(Type implementationType)
        {
            _ = implementationType ?? throw new ArgumentNullException(nameof(implementationType));

            if (!implementationType.IsClass || implementationType.IsAbstract)
            {
                throw new ArgumentException($"{implementationType.FullName} must be a non-abstract class to be registered as an event handler.", nameof(implementationType));
            }

            // Add binding for each of the lifecycle events it handles
            foreach (Type eventHandlerType in LifecycleManager.LifecycleInterfaces)
            {
                if (eventHandlerType.IsAssignableFrom(implementationType))
                {
                    this.Global.Bind(eventHandlerType).ToMethod(context => this.Get(implementationType, context.Parameters.ToArray())).InTransientScope();
                }
            }
        }
    }
}