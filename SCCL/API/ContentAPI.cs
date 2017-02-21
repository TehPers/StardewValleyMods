using Entoarox.Framework;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.API {
    public class ContentAPI {
        //internal static ContentAPI INSTANCE { get; } = new ContentAPI();
        internal static Dictionary<string, ContentInjector> mods = new Dictionary<string, ContentInjector>();
        private static Dictionary<string, Type> injectorDelegateTypes = new Dictionary<string, Type>();
        private static MethodInfo delegateCreator = typeof(ContentAPI).GetMethod("CreateDelegate", BindingFlags.Static | BindingFlags.NonPublic);
        private static MethodInfo injector = typeof(ContentMerger).GetMethod("Inject", BindingFlags.Public | BindingFlags.Instance);
        private static MethodInfo registerHandler = typeof(IContentRegistry).GetMethod("RegisterHandler");

        internal static OriginalInjector originalInjector = new OriginalInjector();

        private ContentAPI() { }

        /**
         * <summary>Returns the injector with the given name, or a new injector if none exists</summary>
         * <param name="name">The name of the injector. This can be your mod's name.</param>
         * <returns>The injector with the given name, or a new one if needed</returns>
         **/
        public static ContentInjector GetInjector(string name) {
            if (!mods.ContainsKey(name)) mods[name] = new ContentInjector(name);
            return mods[name];
        }

        /**
         * <summary>Returns a list of all registered injectors</summary>
         * <returns>A string[] containing the names of all injectors</returns>
         **/
        public static string[] GetAllInjectors() {
            return mods.Keys.ToArray();
        }

        /// <summary>Returns whether the specified injector has been created yet</summary>
        /// <param name="name">The name of the injector</param>
        /// <returns>True if the injector exists already</returns>
        public static bool InjectorExists(string name) {
            return mods.ContainsKey(name);
        }

        internal static bool TryCreateDelegate<T>(string assetName) {
            Type assetType = typeof(T);

            // Check that T is compatible with any existing delegate
            if (injectorDelegateTypes.ContainsKey(assetName)) {
                Type curDelegateType = injectorDelegateTypes[assetName];
                if (curDelegateType == assetType)
                    return true; // Only return true if the delegates are equal. This could have been more flexible if Ento's framework didn't do the same thing x_x
                return false;
            }

            // If creating a delegate for a game asset, make sure T is the correct type
            try {
                object asset = Game1.content.Load<object>(assetName);
                if (!typeof(T).IsAssignableFrom(asset.GetType()))
                    return false; // Asset cannot be cast to T
                assetType = asset.GetType();
            } catch (ContentLoadException) { } // Asset doesn't exist

            // Check if a delegate is already registered (mod conflict) -- Nevermind, Ento's framework already warns for conflicts
            IContentRegistry registry = EntoFramework.GetContentRegistry();

            // Register delegate for T
            ModEntry.INSTANCE.Monitor.Log(string.Format("Creating delegate for {0} (Type: {1})", assetName, assetType.ToString()), LogLevel.Trace);
            FileLoadMethod<T> injectorDelegate = CreateDelegate<T>(injector.MakeGenericMethod(assetType));
            registry.RegisterHandler(assetName, injectorDelegate);
            injectorDelegateTypes[assetName] = assetType;
            return true;
        }

        private static FileLoadMethod<T> CreateDelegate<T>(MethodInfo method) {
            return (loader, asset) => {
                return (T) method.Invoke(ModEntry.INSTANCE.merger, new object[] { loader, asset });
            };
        }
    }
}
