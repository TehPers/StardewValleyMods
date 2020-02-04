using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Introspection;
using Ninject.Modules;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.Api;
using TehPers.Core.Api.Conflux;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.DependencyInjection
{
    internal class ModKernel : CoreKernel, IModKernel
    {
        private readonly GlobalKernel globalKernel;

        public IMod ParentMod { get; }

        public IKernel GlobalKernel => this.globalKernel;

        public IBindingRoot GlobalProxyRoot { get; }

        public IModKernelFactory ParentFactory { get; }

        public ModKernel(IMod parentMod, GlobalKernel globalKernel, IModKernelFactory parentFactory, INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
            this.ParentMod = parentMod;
            this.ParentFactory = parentFactory;
            this.globalKernel = globalKernel;
            this.GlobalProxyRoot = new ProxiedBindingRoot(globalKernel, this);
        }

        public override bool CanResolve(IRequest request)
        {
            if (request.Parameters.OfType<GlobalParameter>().Any())
            {
                return this.globalKernel.CanResolve(request);
            }

            return base.CanResolve(request);
        }

        public override bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            if (request.Parameters.OfType<GlobalParameter>().Any())
            {
                return this.globalKernel.CanResolve(request, ignoreImplicitBindings);
            }

            return base.CanResolve(request, ignoreImplicitBindings);
        }

        public override IEnumerable<object> Resolve(IRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            if (ModKernel.ShouldInherit(request))
            {
                return this.globalKernel.Resolve(request);
            }

            return base.Resolve(request);
        }

        protected override IEnumerable<TService> Resolve<TService>(IRequest request, bool handleMissingBindings)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var modBindings = this.GetSatisfiedBindings(request).ToArray();
            var bindingGroups = modBindings.Where(binding => !binding.Binding.IsImplicit).ToList().Yield()
                .Append(this.globalKernel.GetSatisfiedBindings(request).ToList())
                .Append(modBindings.Where(binding => binding.Binding.IsImplicit).ToList())
                .ToList();

            var resolvedServices = new List<TService>();
            foreach (var satisfiedBindings in bindingGroups)
            {
                if (satisfiedBindings.Count == 0)
                {
                    continue;
                }

                // Implicit bindings should only be added if there are no other matching bindings
                if (resolvedServices.Any() && satisfiedBindings.Any(binding => binding.Binding.IsImplicit))
                {
                    break;
                }

                if (request.IsUnique)
                {
                    var firstTwo = satisfiedBindings.Take(2).ToList();
                    if (firstTwo.Count > 1 && this.BindingPrecedenceComparer.Compare(firstTwo[0].Binding, firstTwo[1].Binding) == 0)
                    {
                        if (request.IsOptional && !request.ForceUnique)
                        {
                            return Enumerable.Empty<TService>();
                        }

                        var formattedBindings = satisfiedBindings
                            .Select(binding => binding.Binding.Format(binding.Context))
                            .ToArray();

                        throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(request, formattedBindings));
                    }

                    return firstTwo[0].Context.Resolve()
                        .Forward(x => (TService)x)
                        .Yield();
                }

                resolvedServices.AddRange(satisfiedBindings
                    .Select(binding => binding.Context.Resolve())
                    .Cast<TService>());
            }

            if (resolvedServices.Any())
            {
                return resolvedServices;
            }

            if (handleMissingBindings && this.HandleMissingBinding(request))
            {
                return this.Resolve<TService>(request, false);
            }

            if (request.IsOptional)
            {
                return Enumerable.Empty<TService>();
            }

            throw new ActivationException(ExceptionFormatter.CouldNotResolveBinding(request));
        }

        private static bool ShouldInherit(IRequest request)
        {
            return request.Parameters.OfType<GlobalParameter>().Any()
                   || (request.Target?.Member.GetAttributes<GlobalAttribute>().Any() ?? false);
        }
    }
}