using System;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Modules;
using StardewModdingAPI;
using TehPers.Core.Api.DependencyInjection;

namespace TehPers.Core.DependencyInjection
{
    internal class ModKernel : ChildKernel, IModKernel
    {
        public IMod ParentMod { get; }
        public IKernel GlobalKernel { get; }

        public ModKernel(IMod parentMod, IKernel global, INinjectSettings settings, params INinjectModule[] modules) : base(global, settings, modules)
        {
            this.ParentMod = parentMod;
            this.GlobalKernel = global;
        }
    }
}