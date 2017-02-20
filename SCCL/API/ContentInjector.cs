using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.API {
    public class ContentInjector {
        private static MethodInfo registerAssetMethod = typeof(ContentInjector).GetMethods().Where(m => m.Name == "RegisterAsset" && m.IsGenericMethod).First();

        internal Dictionary<string, HashSet<object>> ModContent { get; } = new Dictionary<string, HashSet<object>>();

        private string _name;
        public string Name => _name;

        private bool _enabled;
        public bool Enabled {
            get {
                return _enabled;
            }
            set {
                if (_enabled != value) {
                    foreach (string asset in ModContent.Keys)
                        this.RefreshAsset(asset);
                }
                _enabled = value;
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
        public bool RegisterAsset<T>(string assetName, T asset) {
            assetName = assetName.Replace('/', '\\');

            if (!ContentAPI.TryCreateDelegate<T>(assetName))
                return false;

            if (!ModContent.ContainsKey(assetName))
                ModContent[assetName] = new HashSet<object>();

            ModContent[assetName].Add(asset);
            this.RefreshAsset(assetName);

            ModEntry.INSTANCE.Monitor.Log(string.Format("[{2}] Registered {0} ({1})", assetName, typeof(T).ToString(), Name));

            return true;
        }

        /**
         * <summary>Registers the given asset with the given asset name</summary>
         * <param name="assetName">The name of the asset to merge</param>
         * <param name="asset">The asset to merge</param>
         * <returns>Whether the asset was registered successfully. If false, then the asset was probably the wrong type</returns>
         **/
        public bool RegisterAsset(string assetName, object asset) {
            return (bool) registerAssetMethod.MakeGenericMethod(asset.GetType()).Invoke(this, new object[] { assetName, asset });
        }


        /**
         * <summary>Removes the given asset with the given asset name</summary>
         * <param name="assetName">The name of the asset to remove</param>
         * <param name="asset">The asset to remove</param>
         * <returns>Whether the asset was removed successfully</returns>
         **/
        public bool UnregisterAsset(string assetName, object asset) {
            assetName = assetName.Replace('/', '\\');

            if (ModContent.ContainsKey(assetName) && ModContent[assetName].Remove(asset)) {
                this.RefreshAsset(assetName);
                ModEntry.INSTANCE.Monitor.Log(string.Format("[{2}] Unregistered {0} ({1})", assetName, asset.GetType().ToString(), Name));
                return true;
            }
            return false;
        }

        /**
         * <summary>Mark the given asset to be regenerated</summary>
         * <param name="assetName">The name of the asset to regenerate</param>
         * <returns>Whether the asset needs to be marked</returns>
         **/
        public bool RefreshAsset(string assetName) {
            bool b = ModEntry.INSTANCE.merger.Cache.Remove(assetName);
            if (b) ModEntry.INSTANCE.reloadContent();
            return b;
        }
    }
}
