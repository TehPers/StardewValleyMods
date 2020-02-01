using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Extensions.ContextPreservation;
using Ninject.Infrastructure;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.Api.Configuration;
using TehPers.Core.Api.Conflux;
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
        /// Binds an event handler.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of arguments for the event.</typeparam>
        /// <param name="root">The mod's binding root.</param>
        /// <returns>The syntax that can be used to configure the binding.</returns>
        public static IBindingToSyntax<IEventHandler<TEventArgs>> BindEventHandler<TEventArgs>(this IModBindingRoot root)
            where TEventArgs : EventArgs
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));
            return root.GlobalRoot.Bind<IEventHandler<TEventArgs>>();
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

            return root.BindEventHandler<TEventArgs>()
                .ToConstant(new SimpleEventHandler<TEventArgs>(handler))
                .InSingletonScope();
        }

        /// <summary>
        /// Registers a service as a handler for all the events it can handle. This does not bind the service.
        /// </summary>
        /// <typeparam name="TService">The type of service being bound as an event handler. This service should be registered to your mod's kernel separately.</typeparam>
        /// <param name="root">The mod's binding root.</param>
        /// <returns>The mod kernel for chaining.</returns>
        public static IModBindingRoot AddEventHandler<TService>(this IModBindingRoot root)
            where TService : class
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));

            var handlerTypes = new HashSet<Type>();
            var queuedTypes = new Queue<Type>();
            queuedTypes.Enqueue(typeof(TService));
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

            if (!handlerTypes.Any())
            {
                throw new ArgumentException($"Type does not implement {typeof(IEventHandler<>).FullName} so it cannot be bound as an event handler.");
            }

            var bindHandler = typeof(BindingExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(method => method.Name == nameof(BindingExtensions.AddEventHandler) && method.GetGenericArguments().Length == 2);

            if (bindHandler == null)
            {
                throw new Exception("An error occurred while binding an event handler: the handler could not be retrieved with reflection.");
            }

            foreach (var handlerType in handlerTypes)
            {
                bindHandler.MakeGenericMethod(typeof(TService), handlerType.GenericTypeArguments[0]).Invoke(null, new object[] {root});
            }

            return root;
        }

        /// <summary>
        /// Binds a service as a handler for a particular type of event.
        /// </summary>
        /// <param name="root">The mod's kernel.</param>
        /// <typeparam name="TService">The type of service being bound as an event handler. This service should be injectible by your mod's kernel.</typeparam>
        /// <typeparam name="TEventArgs">The type of arguments propagated by the event this service handles.</typeparam>
        /// <returns>The mod kernel for chaining.</returns>
        public static IModBindingRoot AddEventHandler<TService, TEventArgs>(this IModBindingRoot root)
            where TService : IEventHandler<TEventArgs>
            where TEventArgs : EventArgs
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));

            root.GlobalRoot.Bind<IEventHandler<TEventArgs>>()
                .ToMethod(context => context.Kernel.Get<TService>(context.GetChildParameters()))
                .InTransientScope();

            return root;
        }

        /// <summary>
        /// Exposes a service in a mod's kernel to the global kernel.
        /// </summary>
        /// <param name="kernel">The mod's kernel.</param>
        /// <typeparam name="TService">The service being exposed globally.</typeparam>
        /// <returns>The syntax that can be used to configure the service that was exposed.</returns>
        public static IBindingOnSyntax<TService> ExposeService<TService>(this IModBindingRoot kernel)
        {
            return kernel.ExposeService<TService, TService>();
        }

        /// <summary>
        /// Exposes a service in a mod's kernel to the global kernel.
        /// </summary>
        /// <param name="kernel">The mod's kernel.</param>
        /// <typeparam name="TGlobalService">The type of service that is visible globally and will be injected by the global kernel. Generally, this would be your type's interface or base class.</typeparam>
        /// <typeparam name="TModService">The type of service that is visible within your mod. This is generally the concrete type of your service, although it could be a base class or interface as well.</typeparam>
        /// <returns>The syntax that can be used to configure the service that was exposed.</returns>
        public static IBindingOnSyntax<TModService> ExposeService<TGlobalService, TModService>(this IModBindingRoot kernel)
            where TModService : TGlobalService
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));

            return kernel.GlobalRoot.Bind<TGlobalService>()
                .ToMethod(context => context.Kernel.Get<TModService>(context.Parameters.Append(new GlobalParameter()).ToArray()))
                .InTransientScope();
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