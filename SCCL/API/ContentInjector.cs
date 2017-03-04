using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TehPers.Stardew.SCCL.Configs;

namespace TehPers.Stardew.SCCL.API {
    public class ContentInjector {
        private static MethodInfo registerAssetMethod = typeof(ContentInjector).GetMethods().Where(m => m.Name == "RegisterAsset" && m.IsGenericMethod).First();

        internal Dictionary<string, HashSet<object>> ModContent { get; } = new Dictionary<string, HashSet<object>>();

        private string _name;
        public virtual string Name => _name;

        public virtual bool Enabled {
            get {
                return !ModEntry.INSTANCE.config.DisabledMods.Contains(this.Name);
            }
            set {
                ModConfig config = ModEntry.INSTANCE.config;
                bool changed = value != this.Enabled;

                if (value)
                    config.DisabledMods.Remove(this.Name);
                else
                    config.DisabledMods.Add(this.Name);

                if (changed) {
                    ModEntry.INSTANCE.Helper.WriteConfig(config);
                    foreach (string asset in ModContent.Keys)
                        this.RefreshAsset(asset);
                }
            }
        }

        internal ContentInjector(string name) {
            this._name = name;
        }

        /**
         * <summary>Registers the given asset with the given type and asset name</summary>
         * <param name="assetName">The name of the asset to merge</param>
         * <param name="asset">The asset to merge</param>
         * <typeparam name="T">The type of the asset to merge. If unknown, use non-generic <seealso cref="RegisterAsset(string, object)"/> instead</typeparam>
         * <returns>Whether the asset was registered successfully. If false, then T was probably incompatible with the asset</returns>
         **/
        public virtual bool RegisterAsset<T>(string assetName, T asset) {
            assetName = assetName.Replace('/', '\\');

            if (!ModContent.ContainsKey(assetName))
                ModContent[assetName] = new HashSet<object>();

            ModContent[assetName].Add(asset);
            this.RefreshAsset(assetName);

            ModEntry.INSTANCE.Monitor.Log(string.Format("[{2}] Registered {0} ({1})", assetName, typeof(T).ToString(), Name), LogLevel.Trace);

            return true;
        }

        /**
         * <summary>Registers the given asset with the given asset name</summary>
         * <param name="assetName">The name of the asset to merge</param>
         * <param name="asset">The asset to merge</param>
         * <returns>Whether the asset was registered successfully. If false, then the asset was probably the wrong type</returns>
         **/
        public virtual bool RegisterAsset(string assetName, object asset) {
            return (bool) registerAssetMethod.MakeGenericMethod(asset.GetType()).Invoke(this, new object[] { assetName, asset });
        }


        /**
         * <summary>Removes the given asset with the given asset name</summary>
         * <param name="assetName">The name of the asset to remove</param>
         * <param name="asset">The asset to remove</param>
         * <returns>Whether the asset was removed successfully</returns>
         **/
        public virtual bool UnregisterAsset(string assetName, object asset) {
            assetName = assetName.Replace('/', '\\');

            if (ModContent.ContainsKey(assetName) && ModContent[assetName].Remove(asset)) {
                ModEntry.INSTANCE.Monitor.Log(string.Format("[{2}] Unregistered {0} ({1})", assetName, asset.GetType().ToString(), Name), LogLevel.Trace);
                this.RefreshAsset(assetName);
                return true;
            }
            return false;
        }

        /**
         * <summary>Mark the given asset to be regenerated</summary>
         * <param name="assetName">The name of the asset to regenerate</param>
         * <returns>Whether the asset needs to be marked</returns>
         **/
        public virtual bool RefreshAsset(string assetName) {
            return ModEntry.INSTANCE.merger.Dirty.Add(assetName);
        }
    }
}
