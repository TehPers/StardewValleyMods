using System;
using System.Linq;
using Ninject;
using Ninject.Syntax;

namespace TehPers.Core.DependencyInjection.Api.Extensions
{
    public static class BindingExtensions
    {
        public static IBindingWhenInNamedWithOrOnSyntax<TService> ToService<TService>(this IBindingToSyntax<TService> syntax, Type implementationType)
        {
            return syntax.ToMethod(context => (TService)context.Kernel.Get(implementationType, context.Parameters.ToArray()));
        }

        public static IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToService<TService, TImplementation>(this IBindingToSyntax<TService> syntax) where TImplementation : TService
        {
            return syntax.ToMethod(context => context.Kernel.Get<TImplementation>(context.Parameters.ToArray()));
        }
    }
}
