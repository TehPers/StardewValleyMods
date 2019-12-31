using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Infrastructure;
using Ninject.Modules;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection;
using Context = Ninject.Activation.Context;

namespace TehPers.Core.DependencyInjection
{
    internal class ModKernel : StandardKernel, IModKernel
    {
        private readonly KernelBase globalKernel;
        
        public IMod ParentMod { get; }
        public IKernel GlobalKernel => this.globalKernel;
        public IModKernelFactory ParentFactory { get; }

        public ModKernel(IMod parentMod, KernelBase globalKernel, IModKernelFactory parentFactory, INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
            this.ParentMod = parentMod;
            this.ParentFactory = parentFactory;
            this.globalKernel = globalKernel;
        }

        public override IEnumerable<IBinding> GetBindings(Type service)
        {
            var bindings = base.GetBindings(service).ToArray();
            if (bindings.Any())
            {
                return bindings;
            }

            var planner = this.globalKernel.Components.Get<IPlanner>();
            var cache = this.globalKernel.Components.Get<ICache>();
            var pipeline = this.globalKernel.Components.Get<IPipeline>();
            return this.GlobalKernel.GetBindings(service)
                .Where(binding => !binding.IsImplicit)
                .Select(binding =>
                {
                    var child = new Binding(binding.Service)
                    {
                        ScopeCallback = StandardScopeCallbacks.Transient,
                    };

                    if (child.IsConditional)
                    {
                        child.Condition = binding.Condition;
                    }

                    if (binding.ProviderCallback is { } providerCallback)
                    {
                        child.ProviderCallback = context =>
                        {
                            var request = new Request(context.Request.Service, context.Request.Constraint, context.Request.Parameters, context.Request.GetScope, context.Request.IsOptional, context.Request.IsUnique);
                            return providerCallback(new Context(this.GlobalKernel, request, binding, cache, planner, pipeline));
                        };
                    }

                    return child;
                });
        }
    }
}