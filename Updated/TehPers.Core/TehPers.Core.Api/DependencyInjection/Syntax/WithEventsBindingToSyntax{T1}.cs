using System;
using System.Linq.Expressions;
using Ninject;
using Ninject.Activation;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace TehPers.Core.Api.DependencyInjection.Syntax
{

    /// <inheritdoc cref="WithEventsBindingToSyntax" />
    /// <typeparam name="T1">The service being bound.</typeparam>
    internal class WithEventsBindingToSyntax<T1> : WithEventsBindingToSyntax, IBindingToSyntax<T1>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WithEventsBindingToSyntax{T1}"/> class.
        /// </summary>
        /// <param name="root">The binding root.</param>
        /// <param name="kernel">The mod's kernel.</param>
        /// <param name="bindingConfiguration">The binding configuration of the bindings being created.</param>
        public WithEventsBindingToSyntax(IProxyBindable root, IKernel kernel, IBindingConfiguration bindingConfiguration)
            : base(root, kernel, bindingConfiguration, typeof(T1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WithEventsBindingToSyntax{T1}"/> class.
        /// </summary>
        /// <param name="root">The binding root to create the bindings in.</param>
        /// <param name="kernel">The mod's kernel.</param>
        /// <param name="bindingConfiguration">The binding configuration of the bindings being created.</param>
        /// <param name="services">The services being configured.</param>
        public WithEventsBindingToSyntax(IProxyBindable root, IKernel kernel, IBindingConfiguration bindingConfiguration, params Type[] services)
            : base(root, kernel, bindingConfiguration, services)
        {
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<T1> ToSelf()
        {
            return this.To<T1>();
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T1
        {
            return this.InternalTo<TImplementation>(typeof(TImplementation));
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<T1> To(Type implementation)
        {
            return this.InternalTo<T1>(implementation);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<T1> ToProvider<TProvider>()
            where TProvider : IProvider
        {
            return this.InternalToProvider<TProvider, T1>();
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<T1> ToProvider(Type providerType)
        {
            return this.InternalToProvider<T1>(providerType);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TImplementation>(IProvider<TImplementation> provider)
            where TImplementation : T1
        {
            return this.InternalToProvider(provider);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<T1> ToMethod(Func<IContext, T1> method)
        {
            return this.InternalToMethod(method);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
            where TImplementation : T1
        {
            return this.InternalToMethod(method);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T1
        {
            return this.InternalToConstant(value);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstructor<TImplementation>(Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
            where TImplementation : T1
        {
            return this.InternalToConstructor(newExpression);
        }
    }
}