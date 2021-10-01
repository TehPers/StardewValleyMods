using System;
using System.Linq.Expressions;
using Ninject;
using Ninject.Activation;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace TehPers.Core.Api.DependencyInjection.Syntax
{
    /// <inheritdoc cref="WithEventsBindingToSyntax" />
    /// <typeparam name="T1">The first service being bound.</typeparam>
    /// <typeparam name="T2">The second service being bound.</typeparam>
    internal class WithEventsBindingToSyntax<T1, T2> : WithEventsBindingToSyntax, IBindingToSyntax<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WithEventsBindingToSyntax{T1, T2}"/> class.
        /// </summary>
        /// <param name="root">The binding root.</param>
        /// <param name="kernel">The mod's kernel.</param>
        /// <param name="bindingConfiguration">The binding configuration of the bindings being created.</param>
        public WithEventsBindingToSyntax(IProxyBindable root, IKernel kernel, IBindingConfiguration bindingConfiguration)
            : base(root, kernel, bindingConfiguration, typeof(T1), typeof(T2))
        {
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T1, T2
        {
            return this.InternalTo<TImplementation>(typeof(TImplementation));
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<object> To(Type implementation)
        {
            return this.InternalTo<object>(implementation);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<object> ToProvider<TProvider>()
            where TProvider : IProvider
        {
            throw new InvalidOperationException("The event handler types cannot be determined with this overload. Use ToProvider<TProvider, TImplementation>() instead if possible.");
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TProvider, TImplementation>()
            where TProvider : IProvider<TImplementation>
            where TImplementation : T1, T2
        {
            return this.InternalToProvider<TProvider, TImplementation>();
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<object> ToProvider(Type providerType)
        {
            throw new InvalidOperationException("The event handler types cannot be determined with this overload. Use ToProvider<TProvider, TImplementation>() instead if possible.");
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TImplementation>(IProvider<TImplementation> provider)
            where TImplementation : T1, T2
        {
            return this.InternalToProvider(provider);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
            where TImplementation : T1, T2
        {
            return this.InternalToMethod(method);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T1, T2
        {
            return this.InternalToConstant(value);
        }

        /// <inheritdoc/>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstructor<TImplementation>(Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
            where TImplementation : T1, T2
        {
            return this.InternalToConstructor(newExpression);
        }
    }
}