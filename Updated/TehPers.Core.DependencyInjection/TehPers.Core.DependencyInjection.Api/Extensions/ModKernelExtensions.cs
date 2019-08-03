using Ninject.Syntax;
using StardewModdingAPI;

namespace TehPers.Core.DependencyInjection.Api.Extensions
{
    public static class ModKernelExtensions
    {
        public static IModKernel BindMonitor(this IModKernel modKernel)
        {
            modKernel.Bind<IMonitor>().ToConstant(modKernel.ParentMod.Monitor).InSingletonScope();
            return modKernel;
        }

        public static IModKernel BindMod(this IModKernel modKernel)
        {
            modKernel.Bind<IMod>().ToConstant(modKernel.ParentMod).InSingletonScope();
            return modKernel;
        }

        public static IModKernel BindManifest(this IModKernel modKernel)
        {
            modKernel.Bind<IManifest>().ToConstant(modKernel.ParentMod.ModManifest).InSingletonScope();
            return modKernel;
        }

        public static IModKernel BindHelper(this IModKernel modKernel)
        {
            modKernel.Bind<IModHelper>().ToConstant(modKernel.ParentMod.Helper).InSingletonScope();
            return modKernel;
        }

        public static IModKernel BindModTypes(this IModKernel modKernel)
        {
            return modKernel
                .BindMod()
                .BindMonitor()
                .BindManifest()
                .BindHelper();
        }

        public static IBindingWhenInNamedWithOrOnSyntax<T> BindCustomModApi<T>(this IModKernel modKernel, string modId)
            where T : class
        {
            return modKernel.Bind<T>().ToMethod(_ => modKernel.ParentMod.Helper.ModRegistry.GetApi<T>(modId));
        }
    }
}
