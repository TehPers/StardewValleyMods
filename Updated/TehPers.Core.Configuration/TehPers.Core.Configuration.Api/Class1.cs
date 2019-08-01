using System;
using System.Runtime.CompilerServices;
using Ninject.Activation;
using Ninject.Syntax;
using TehPers.Core.DependencyInjection.Api;

namespace TehPers.Core.Configuration.Api
{
    public static class KernelExtensions
    {
        public static IBindingWhenInNamedWithOrOnSyntax<T> BindConfig<T>(this IModKernel kernel) where T : class, new()
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));

            return kernel.Bind<T>().ToMethod(context => kernel.ParentMod.Helper.ReadConfig<T>());
        }

        public static IBindingWhenInNamedWithOrOnSyntax<T> BindConfig<T>(this IModKernel kernel, string path) where T : class, new()
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _ = path ?? throw new ArgumentNullException(nameof(path));

            return kernel.Bind<T>().ToMethod(context => kernel.ParentMod.Helper.Data.ReadJsonFile<T>(path) ?? new T());
        }

        public static IBindingWhenInNamedWithOrOnSyntax<T> BindConfig<T>(this IModKernel kernel, string path, Func<IContext, T> configFactory) where T : class
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _ = path ?? throw new ArgumentNullException(nameof(path));
            _ = configFactory ?? throw new ArgumentNullException(nameof(configFactory));

            return kernel.Bind<T>().ToMethod(context => kernel.ParentMod.Helper.Data.ReadJsonFile<T>(path) ?? configFactory(context));
        }
    }
}
