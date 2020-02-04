using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Introspection;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using TehPers.Core.Api.Conflux;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.DependencyInjection
{
    public abstract class CoreKernel : StandardKernel
    {
        private static readonly MethodInfo ResolveGeneric = typeof(CoreKernel).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(method => method.Name == nameof(CoreKernel.Resolve) && method.IsGenericMethod) ?? throw new InvalidOperationException($"Missing generic method: {nameof(CoreKernel.Resolve)}");

        protected IBindingPrecedenceComparer BindingPrecedenceComparer { get; }

        protected CoreKernel(INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
            this.BindingPrecedenceComparer = this.Components.Get<IBindingPrecedenceComparer>();
        }

        public override bool CanResolve(IRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            return this.CanResolve(request, false);
        }

        public override bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            return this.GetSatisfiedBindings(request).Any(binding => !(ignoreImplicitBindings && binding.Binding.IsImplicit));
        }

        public override IEnumerable<object> Resolve(IRequest request)
        {
            IRequest CreateEnumeratedRequest(Type innerType)
            {
                if (request is { ParentRequest: { } parentRequest, ParentContext: { } parentContext, Target: { } target })
                {
                    var newRequest = parentRequest.CreateChild(innerType, parentContext, target);
                    newRequest.IsOptional = true;
                    return newRequest;
                }

                return this.CreateRequest(innerType, null, request.Parameters.Where(p => p.ShouldInherit), true, false);
            }

            // Request is for T[]
            if (request.Service.IsArray)
            {
                var enumeratedRequest = CreateEnumeratedRequest(request.Service.GetElementType());
                return new[] {this.Resolve(enumeratedRequest, false).ToArray()};
            }

            if (request.Service.IsGenericType)
            {
                var genericTypeDefinition = request.Service.GetGenericTypeDefinition();

                // Request is for IEnumerable<T>
                if (genericTypeDefinition == typeof(IEnumerable<>))
                {
                    var enumeratedRequest = CreateEnumeratedRequest(request.Service.GenericTypeArguments[0]);
                    return new[] {this.Resolve(enumeratedRequest, false)};
                }

                // Request is for ICollection<T>, IReadOnlyList<T>, or IList<T>
                if (genericTypeDefinition == typeof(ICollection<>)
                    || genericTypeDefinition == typeof(IReadOnlyList<>)
                    || genericTypeDefinition == typeof(IList<>))
                {
                    var enumeratedRequest = CreateEnumeratedRequest(request.Service.GenericTypeArguments[0]);
                    return new[] {this.Resolve(enumeratedRequest, false).ToList()};
                }
            }

            // Resolve service normally
            return this.Resolve(request, true);
        }

        private IEnumerable<object> Resolve(IRequest request, bool handleMissingBindings)
        {
            if (CoreKernel.ResolveGeneric.MakeGenericMethod(request.Service).Invoke(this, new object[] {request, handleMissingBindings}) is IEnumerable result)
            {
                return result.Cast<object>();
            }

            return Enumerable.Empty<object>();
        }

        protected virtual IEnumerable<TService> Resolve<TService>(IRequest request, bool handleMissingBindings)
        {
            var satisfiedBindings = this.GetSatisfiedBindings(request).ToArray();

            if (satisfiedBindings.Length == 0)
            {
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

            if (request.IsUnique)
            {
                var firstTwo = satisfiedBindings.Take(2).ToArray();
                if (firstTwo.Length > 1 && this.BindingPrecedenceComparer.Compare(firstTwo[0].Binding, firstTwo[1].Binding) == 0)
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

            if (satisfiedBindings.Any(binding => !binding.Binding.IsImplicit))
            {
                return satisfiedBindings
                    .Where(binding => !binding.Binding.IsImplicit)
                    .Select(binding => binding.Context.Resolve())
                    .Cast<TService>();
            }

            return satisfiedBindings
                .Select(binding => binding.Context.Resolve())
                .Cast<TService>();
        }

        public virtual IEnumerable<ContextualBinding> GetSatisfiedBindings(IRequest request)
        {
            return this.GetBindings(request.Service)
                .Where(this.SatifiesRequest(request))
                .Select(binding => new ContextualBinding(binding, this.CreateContext(request, binding)));
        }

        public class ContextualBinding
        {
            public IBinding Binding { get; }

            public IContext Context { get; }

            public ContextualBinding(IBinding binding, IContext context)
            {
                this.Binding = binding ?? throw new ArgumentNullException(nameof(binding));
                this.Context = context ?? throw new ArgumentNullException(nameof(context));
            }
        }
    }
}