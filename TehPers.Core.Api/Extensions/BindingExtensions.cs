using System;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Syntax;
using TehPers.Core.Api.DI;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>
    /// Extension methods for binding services.
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Gets the inherited parameters from a context.
        /// </summary>
        /// <param name="context">The parent context.</param>
        /// <returns>The inherited parameters.</returns>
        public static IParameter[] GetChildParameters(this IContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            return context.Parameters.Where(parameter => parameter.ShouldInherit).ToArray();
        }

        public static object ToFirst<TService>(
            this IBindingToSyntax<TService> syntax,
            params Type[] implementationTypes
        )
        {
            _ = implementationTypes ?? throw new ArgumentNullException(nameof(implementationTypes));
            _ = syntax ?? throw new ArgumentNullException(nameof(syntax));
            return syntax.ToMethod(
                context =>
                {
                    var parameters = context.GetChildParameters();
                    foreach (var implementationType in implementationTypes)
                    {
                        if (context.Kernel.TryGet(implementationType, parameters) is TService result)
                        {
                            return result;
                        }
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("None of the services could be activated. The following services were attempted:");
                    foreach (var type in implementationTypes)
                    {
                        sb.Append(" - ");
                        sb.AppendLine(type.FullName);
                    }

                    sb.AppendLine("Ensure that at least one of the requested services can be activated.");
                    throw new ActivationException(sb.ToString());
                }
            );
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

        /// <summary>
        /// Binds an API exposed by another mod through SMAPI to your mod's kernel.
        /// </summary>
        /// <typeparam name="TApi">The type the mod's API returns, or an interface which matches part of (or all of) its signature.</typeparam>
        /// <param name="root">The mod's binding root.</param>
        /// <param name="modId">The foreign mod's ID.</param>
        /// <returns>The syntax that can be used to configure the binding.</returns>
        public static IBindingInNamedWithOrOnSyntax<TApi> BindForeignModApi<TApi>(
            this IModBindingRoot root,
            string modId
        )
            where TApi : class
        {
            _ = modId ?? throw new ArgumentNullException(nameof(modId));
            _ = root ?? throw new ArgumentNullException(nameof(root));

            // TODO: should not create binding if the mod doesn't exist
            return root.Bind<TApi>()
                .ToMethod(_ => root.ParentMod.Helper.ModRegistry.GetApi<TApi>(modId))
                .When(_ => root.ParentMod.Helper.ModRegistry.IsLoaded(modId));
        }
    }
}