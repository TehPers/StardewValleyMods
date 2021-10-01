using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.ContextPreservation;
using Ninject.Infrastructure;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.Api.Configuration;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.DependencyInjection.Lifecycle;
using TehPers.Core.Api.Json;

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

        public static object ToFirst<TService>(this IBindingToSyntax<TService> syntax, params Type[] implementationTypes)
        {
            _ = implementationTypes ?? throw new ArgumentNullException(nameof(implementationTypes));
            _ = syntax ?? throw new ArgumentNullException(nameof(syntax));
            return syntax.ToMethod(context =>
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

        /// <summary>
        /// Binds an API exposed by another mod to your mod's kernel.
        /// </summary>
        /// <typeparam name="TApi">The type the mod's API returns, or an interface which matches part of (or all of) its signature.</typeparam>
        /// <param name="root">The mod's binding root.</param>
        /// <param name="modId">The foreign mod's API.</param>
        /// <returns>The syntax that can be used to configure the binding.</returns>
        public static IBindingWhenInNamedWithOrOnSyntax<TApi> BindForeignModApi<TApi>(this IModBindingRoot root, string modId)
            where TApi : class
        {
            _ = modId ?? throw new ArgumentNullException(nameof(modId));
            _ = root ?? throw new ArgumentNullException(nameof(root));

            return root.Bind<TApi>()
                .ToMethod(_ => root.ParentMod.Helper.ModRegistry.GetApi<TApi>(modId));
        }

        /// <summary>
        /// Binds a simple event handler. This is the easiest way to create an event handler, however they cannot have dependencies injected via DI.
        /// </summary>
        /// <param name="root">The mod's binding root.</param>
        /// <param name="handler">The event handler.</param>
        /// <typeparam name="TEventArgs">The type of arguments for the event.</typeparam>
        /// <returns>The syntax that can be used to configure the binding.</returns>
        public static IBindingOnSyntax<SimpleEventHandler<TEventArgs>> BindSimpleEventHandler<TEventArgs>(this IModBindingRoot root, EventHandler<TEventArgs> handler)
            where TEventArgs : EventArgs
        {
            _ = handler ?? throw new ArgumentNullException(nameof(handler));
            _ = root ?? throw new ArgumentNullException(nameof(root));

            return root.GlobalProxyRoot.Bind<IEventHandler<TEventArgs>>()
                .ToConstant(new SimpleEventHandler<TEventArgs>(handler))
                .InSingletonScope();
        }

        /// <summary>
        /// Indicates that services should be bound to their event handler types as well.
        /// </summary>
        /// <param name="root">The binding root.</param>
        /// <returns>A new binding root which automatically binds services to event handlers.</returns>
        public static IProxyBindable WithEvents(this IProxyBindable root)
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));
            return new WithEventsModBindingRoot(root);
        }

        /// <summary>
        /// Binds a configuration that is loaded from a file.
        /// </summary>
        /// <typeparam name="T">The type of configuration to bind. The file's contents are deserialized into this type.</typeparam>
        /// <param name="root">The mod's binding root.</param>
        /// <param name="path">The path to the configuration file.</param>
        /// <param name="source">The source folder for that configuration file.</param>
        /// <param name="saveOnChange">Whether the configuration should be saved when its value changes.</param>
        /// <returns>The syntax that can be used to configure the configuration.</returns>
        public static IBindingWhenInNamedWithOrOnSyntax<IConfiguration<T>> BindConfiguration<T>(this IModBindingRoot root, string path, ContentSource source = ContentSource.ModFolder, bool saveOnChange = false)
            where T : class, new()
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));
            var binding = root.Bind<IConfiguration<T>>().ToMethod(context =>
            {
                var assetProvider = context.ContextPreservingGet<IAssetProvider>(new ContentSourceAttribute(source).Matches);
                var jsonProvider = context.ContextPreservingGet<IJsonProvider>();
                var configValue = jsonProvider.ReadOrCreate<T>(path, assetProvider, null);

                IConfiguration<T> config = new SimpleConfiguration<T>(configValue);
                if (saveOnChange)
                {
                    config.Changed += (sender, args) => jsonProvider.WriteJson(args.NewValue, path, assetProvider);
                }

                return config;
            });

            // Default scope is singleton
            binding.BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Singleton;
            return binding;
        }

        /// <summary>
        /// Loads modules into every <see cref="IModBindingRoot"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of module to load.</typeparam>
        /// <param name="kernelFactory">The mod kernel factory.</param>
        public static void LoadIntoModKernels<TModule>(this IModKernelFactory kernelFactory)
            where TModule : INinjectModule, new()
        {
            _ = kernelFactory ?? throw new ArgumentNullException(nameof(kernelFactory));
            kernelFactory.LoadIntoModKernels(_ => new TModule());
        }
    }
}