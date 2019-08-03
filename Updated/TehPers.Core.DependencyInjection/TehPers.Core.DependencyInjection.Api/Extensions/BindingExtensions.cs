using System;
using System.Linq;
using Ninject;
using Ninject.Parameters;
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

        public static object ToFirst<TService>(this IBindingToSyntax<TService> syntax, params Type[] implementationTypes)
        {
            return syntax.ToMethod(context =>
            {
                IParameter[] parameters = context.Parameters.ToArray();
                foreach (Type implementationType in implementationTypes)
                {
                    if (context.Kernel.TryGet(implementationType, parameters) is TService result)
                    {
                        return result;
                    }
                }
                throw new ActivationException("None of the implementations could be activated");
            });
        }

        public static object ToFirst<TService, T1>(this IBindingToSyntax<TService> syntax)
            where T1 : TService
        {
            return syntax.ToFirst(typeof(T1));
        }

        public static object ToFirst<TService, T1, T2>(this IBindingToSyntax<TService> syntax)
            where T1 : TService
            where T2 : TService
        {
            return syntax.ToFirst(typeof(T1), typeof(T2));
        }

        public static object ToFirst<TService, T1, T2, T3>(this IBindingToSyntax<TService> syntax)
            where T1 : TService
            where T2 : TService
            where T3 : TService
        {
            return syntax.ToFirst(typeof(T1), typeof(T2), typeof(T3));
        }
    }
}
