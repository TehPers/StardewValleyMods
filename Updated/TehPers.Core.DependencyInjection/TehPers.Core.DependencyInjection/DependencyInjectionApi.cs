using System.Collections.Generic;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.DependencyInjection.Api;

namespace TehPers.Core.DependencyInjection
{
    public sealed class DependencyInjectionApi : IDependencyInjectionApi
    {
        private readonly Dictionary<string, IModKernel> _modApis = new Dictionary<string, IModKernel>();

        public IKernel Global { get; }

        internal DependencyInjectionApi() {
            this.Global = new StandardKernel();
        }

        public IModKernel GetModKernel(IMod mod)
        {
            if (this._modApis.TryGetValue(mod.ModManifest.UniqueID, out IModKernel modApi))
            {
                return modApi;
            }

            mod.Monitor.Log($"[DI] Creating kernel for '{mod.ModManifest.UniqueID}'", LogLevel.Debug);
            modApi = new ModKernel(mod, this.Global, new NinjectSettings
            {
                LoadExtensions = false
            });

            this._modApis.Add(mod.ModManifest.UniqueID, modApi);
            return modApi;
        }

        public void Dispose()
        {
            foreach (IModKernel modKernel in this._modApis.Values)
            {
                modKernel.Dispose();
            }

            this.Global.Dispose();
            this._modApis.Clear();
        }
    }
}