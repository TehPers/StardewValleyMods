using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning.Bindings;

namespace TehPers.Core.DependencyInjection
{
    public class ProxyBindingConfiguration : IBindingConfiguration
    {
        private readonly IBindingConfiguration parentConfiguration;

        public IBindingMetadata Metadata => this.parentConfiguration.Metadata;

        public BindingTarget Target { get; set; }

        public bool IsImplicit { get; set; }

        public bool IsConditional => this.Condition != null;

        public Func<IRequest, bool> Condition { get; set; }

        public Func<IContext, IProvider> ProviderCallback { get; set; }

        public Func<IContext, object> ScopeCallback { get; set; }

        public ICollection<IParameter> Parameters { get; }

        public ICollection<Action<IContext, object>> ActivationActions { get; }

        public ICollection<Action<IContext, object>> DeactivationActions { get; }

        public ProxyBindingConfiguration(IBindingConfiguration parentConfiguration)
        {
            this.parentConfiguration = parentConfiguration;
            this.Parameters = new List<IParameter>();
            this.ActivationActions = new List<Action<IContext, object>>();
            this.DeactivationActions = new List<Action<IContext, object>>();
            this.ScopeCallback = StandardScopeCallbacks.Transient;
        }

        public IProvider GetProvider(IContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            if (this.ProviderCallback == null)
            {
                throw new ActivationException(ExceptionFormatter.ProviderCallbackIsNull(context));
            }

            return this.ProviderCallback(context);
        }

        public object GetScope(IContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            return this.ScopeCallback(context);
        }

        public bool Matches(IRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            return this.Condition?.Invoke(request) ?? true;
        }
    }
}