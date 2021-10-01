using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using TehPers.Core.Api.DependencyInjection.Lifecycle;

namespace TehPers.Core.Api.DependencyInjection.Syntax
{
    /// <summary>
    /// Used to define the target of a binding and automatically register it for events.
    /// </summary>
    /// <remarks>Heavily based off of Ninject's source: https://github.com/ninject/Ninject/blob/3.3.4/src/Ninject/Planning/Bindings/BindingBuilder.cs. </remarks>
    internal abstract class WithEventsBindingToSyntax : IHaveBindingConfiguration, IHaveKernel
    {
        /// <summary>
        /// Gets all the handler types implemented by a particular type.
        /// </summary>
        /// <param name="searchType">The type to search.</param>
        /// <returns>All the event handlers implemented by <paramref name="searchType"/>.</returns>
        protected static IEnumerable<Type> GetHandlerTypes(Type searchType)
        {
            var handlerTypes = new HashSet<Type>();
            var queuedTypes = new Queue<Type>();
            queuedTypes.Enqueue(searchType);
            while (queuedTypes.Any())
            {
                var curType = queuedTypes.Dequeue();
                if (curType == typeof(object))
                {
                    continue;
                }

                // Add current type to set of types if it's an event handler
                if (curType.IsGenericType && curType.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                {
                    handlerTypes.Add(curType);
                }

                // Enqueue parent type
                if (curType.BaseType != null)
                {
                    queuedTypes.Enqueue(curType.BaseType);
                }

                // Enqueue implemented interfaces
                foreach (var type in curType.GetInterfaces())
                {
                    queuedTypes.Enqueue(type);
                }
            }

            return handlerTypes;
        }

        private readonly IProxyBindable root;

        private readonly Type[] services;

        /// <inheritdoc />
        public IBindingConfiguration BindingConfiguration { get; }

        /// <inheritdoc />
        public IKernel Kernel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WithEventsBindingToSyntax"/> class.
        /// </summary>
        /// <param name="root">The root to create bindings in.</param>
        /// <param name="kernel">The mod's kernel.</param>
        /// <param name="bindingConfiguration">The binding configuration of the bindings being created.</param>
        /// <param name="services">The services being bound (excluding event handler services).</param>
        protected WithEventsBindingToSyntax(IProxyBindable root, IKernel kernel, IBindingConfiguration bindingConfiguration, params Type[] services)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
            this.Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            this.BindingConfiguration = bindingConfiguration ?? throw new ArgumentNullException(nameof(bindingConfiguration));
            this.services = services ?? throw new ArgumentNullException(nameof(services));

            if (services.Length == 0)
            {
                throw new ArgumentException("No services are being bound.", nameof(services));
            }
        }

        private IEnumerable<Type> AddEventBindings(Type handler)
        {
            var eventHandlerServices = WithEventsBindingToSyntax.GetHandlerTypes(handler).ToList();
            foreach (var service in eventHandlerServices)
            {
                // This works because the new binding is being resolved in the mod kernel's context
                this.root.GlobalProxyRoot.AddBinding(new Binding(service, this.BindingConfiguration));
            }

            return eventHandlerServices;
        }

        private string GetServiceNames(IEnumerable<Type> handlerTypes)
        {
            var serviceNames = this.services.Select(t => t.Format());
            var handlerNames = handlerTypes.Select(t => $"{t.Format()} (events)");
            return string.Join(", ", serviceNames.Concat(handlerNames));
        }

        /// <summary>
        /// Indicates that the service should be bound to a specific implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type (or a type it can be assigned to).</typeparam>
        /// <param name="implementation">The concrete implementation type.</param>
        /// <returns>The syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalTo<TImplementation>(Type implementation)
        {
            this.BindingConfiguration.ProviderCallback = StandardProvider.GetCreationCallback(implementation);
            this.BindingConfiguration.Target = BindingTarget.Type;

            var handlerTypes = this.AddEventBindings(implementation);
            var serviceNames = this.GetServiceNames(handlerTypes);
            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, serviceNames, this.Kernel ?? throw new InvalidOperationException("Kernel cannot be null"));
        }

        /// <summary>
        /// Indicates that the service should be bound to a specific type of provider.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type (or a type it can be assigned to).</typeparam>
        /// <param name="provider">The provider's type.</param>
        /// <returns>The syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToProvider<TImplementation>(Type provider)
        {
            this.BindingConfiguration.ProviderCallback = _ => this.Kernel.Get(provider) as IProvider;
            this.BindingConfiguration.Target = BindingTarget.Provider;

            var handlerTypes = this.AddEventBindings(typeof(TImplementation));
            var serviceNames = this.GetServiceNames(handlerTypes);
            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, serviceNames, this.Kernel ?? throw new InvalidOperationException("Kernel cannot be null"));
        }

        /// <summary>
        /// Indicates that the service should be bound to a specific type of provider.
        /// </summary>
        /// <typeparam name="TProvider">The provider's type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type (or a type it can be assigned to).</typeparam>
        /// <returns>The syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToProvider<TProvider, TImplementation>()
            where TProvider : IProvider
        {
            this.BindingConfiguration.ProviderCallback = _ => this.Kernel.Get<TProvider>();
            this.BindingConfiguration.Target = BindingTarget.Provider;

            var handlerTypes = this.AddEventBindings(typeof(TImplementation));
            var serviceNames = this.GetServiceNames(handlerTypes);
            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, serviceNames, this.Kernel ?? throw new InvalidOperationException("Kernel cannot be null"));
        }

        /// <summary>
        /// Indicates that the service should be bound to a specific type of provider.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type (or a type it can be assigned to).</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>The syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToProvider<TImplementation>(IProvider<TImplementation> provider)
        {
            this.BindingConfiguration.ProviderCallback = _ => provider;
            this.BindingConfiguration.Target = BindingTarget.Provider;

            var handlerTypes = this.AddEventBindings(typeof(TImplementation));
            var serviceNames = this.GetServiceNames(handlerTypes);
            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, serviceNames, this.Kernel ?? throw new InvalidOperationException("Kernel cannot be null"));
        }

        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToMethod<TImplementation>(Func<IContext, TImplementation> factory)
        {
            this.BindingConfiguration.ProviderCallback = _ => new CallbackProvider<TImplementation>(factory);
            this.BindingConfiguration.Target = BindingTarget.Method;

            var handlerTypes = this.AddEventBindings(typeof(TImplementation));
            var serviceNames = this.GetServiceNames(handlerTypes);
            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, serviceNames, this.Kernel ?? throw new InvalidOperationException("Kernel cannot be null"));
        }

        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToConstant<TImplementation>(TImplementation value)
        {
            this.BindingConfiguration.ProviderCallback = ctx => new ConstantProvider<TImplementation>(value);
            this.BindingConfiguration.Target = BindingTarget.Constant;
            this.BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Singleton;

            var handlerTypes = this.AddEventBindings(typeof(TImplementation));
            var serviceNames = this.GetServiceNames(handlerTypes);
            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, serviceNames, this.Kernel ?? throw new InvalidOperationException("Kernel cannot be null"));
        }

        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToConstructor<TImplementation>(Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
        {
            if (!(newExpression.Body is NewExpression ctorExpression))
            {
                throw new ArgumentException("The expression must be a constructor call.", nameof(newExpression));
            }

            this.BindingConfiguration.ProviderCallback = StandardProvider.GetCreationCallback(ctorExpression.Type, ctorExpression.Constructor);
            this.BindingConfiguration.Target = BindingTarget.Type;
            this.AddConstructorArguments(ctorExpression, newExpression.Parameters[0]);

            var handlerTypes = this.AddEventBindings(ctorExpression.Constructor.DeclaringType);
            var serviceNames = this.GetServiceNames(handlerTypes);
            return new BindingConfigurationBuilder<TImplementation>(this.BindingConfiguration, serviceNames, this.Kernel);
        }

        /// <summary>
        /// Adds the constructor arguments for the specified constructor expression.
        /// </summary>
        /// <param name="ctorExpression">The ctor expression.</param>
        /// <param name="constructorArgumentSyntaxParameterExpression">The constructor argument syntax parameter expression.</param>
        /// <remarks>Source: https://github.com/ninject/Ninject/blob/3.3.4/src/Ninject/Planning/Bindings/BindingBuilder.cs. </remarks>
        private void AddConstructorArguments(NewExpression ctorExpression, ParameterExpression constructorArgumentSyntaxParameterExpression)
        {
            var parameters = ctorExpression.Constructor.GetParameters();

            for (var i = 0; i < ctorExpression.Arguments.Count; i++)
            {
                var argument = ctorExpression.Arguments[i];
                var argumentName = parameters[i].Name;

                this.AddConstructorArgument(argument, argumentName, constructorArgumentSyntaxParameterExpression);
            }
        }

        /// <summary>
        /// Adds a constructor argument for the specified argument expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="constructorArgumentSyntaxParameterExpression">The constructor argument syntax parameter expression.</param>
        /// <remarks>Source (modified): https://github.com/ninject/Ninject/blob/3.3.4/src/Ninject/Planning/Bindings/BindingBuilder.cs. </remarks>
        private void AddConstructorArgument(Expression argument, string argumentName, ParameterExpression constructorArgumentSyntaxParameterExpression)
        {
            if (argument is MethodCallExpression { Method: var method } && method.IsGenericMethod && method.GetGenericMethodDefinition().DeclaringType == typeof(IConstructorArgumentSyntax))
            {
                return;
            }

            var compiledExpression = Expression.Lambda(argument, constructorArgumentSyntaxParameterExpression).Compile();
            this.BindingConfiguration.Parameters.Add(new ConstructorArgument(argumentName, ctx => compiledExpression.DynamicInvoke(new ConstructorArgumentSyntax(ctx))));
        }

        /// <summary>
        /// Passed to ToConstructor to specify that a constructor value is Injected.
        /// </summary>
        /// <remarks>Source (modified): https://github.com/ninject/Ninject/blob/3.3.4/src/Ninject/Planning/Bindings/BindingBuilder.cs. </remarks>
        private class ConstructorArgumentSyntax : IConstructorArgumentSyntax
        {
            /// <summary>
            /// Gets the context.
            /// </summary>
            /// <value>The context.</value>
            public IContext Context { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructorArgumentSyntax"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public ConstructorArgumentSyntax(IContext context)
            {
                this.Context = context;
            }

            /// <summary>
            /// Specifies that the argument is injected.
            /// </summary>
            /// <typeparam name="T1">The type of the parameter</typeparam>
            /// <returns>Not used. This interface has no implementation.</returns>
            public T1 Inject<T1>()
            {
                throw new InvalidOperationException("This method is for declaration that a parameter shall be injected only! Never call it directly.");
            }
        }
    }
}