using Ninject;
using Ninject.Modules;

namespace TehPers.Core.DependencyInjection
{
    public class GlobalKernel : CoreKernel
    {
        public GlobalKernel(params INinjectModule[] modules)
            : this(new NinjectSettings(), modules)
        {
        }

        public GlobalKernel(INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
        }
    }
}