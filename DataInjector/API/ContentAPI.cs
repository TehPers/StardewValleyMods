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
        private static Dictionary<string, Delegate> injectorDelegates = new Dictionary<string, Delegate>();
        private static MethodInfo delegateCreator = typeof(ContentAPI).GetMethod("CreateDelegate", BindingFlags.Static | BindingFlags.NonPublic);
        private static MethodInfo injector = typeof(ContentMerger).GetMethod("Inject", BindingFlags.Public | BindingFlags.Instance);
        private static MethodInfo registerHandler = typeof(IContentRegistry).GetMethod("RegisterHandler");

        private ContentAPI() { }

        /**
         * <summary>Returns the injector with the given name, or a new injector if none exists</summary>
         * <param name="name">The name of the injector. This can be your mod's name.</param>
         **/
        public static ContentInjector GetInjector(string name) {
            if (!mods.ContainsKey(name)) mods[name] = new ContentInjector(name);
            return mods[name];
        }

        internal static bool TryCreateDelegate<T>(string assetName) {
            Type assetType = typeof(T);

            // Check that T is compatible with any existing delegate
            if (injectorDelegates.ContainsKey(assetName)) {
                Delegate curDelegate = injectorDelegates[assetName];
                if (curDelegate.Method.IsGenericMethod && curDelegate.Method.GetGenericArguments()[0] == assetType)
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

            // Check if a delegate is already registered (mod conflict)
            IContentRegistry registry = EntoFramework.GetContentRegistry();
            IReflectionHelper reflection = ModEntry.INSTANCE.Helper.Reflection;
            LocalizedContentManager manager = reflection.GetPrivateValue<LocalizedContentManager>(registry, "SmartManager");
            if (manager != null) {
                IPrivateMethod m = reflection.GetPrivateMethod(manager, "CanRegister");
                if (m != null && !m.Invoke<bool>(assetName)) {
                    ModEntry.INSTANCE.Monitor.Log(string.Format("Could not register delegate for {0} because one already exists (mod conflict)", assetName));
                    return false;
                }
            }

            // Register delegate for T
            ModEntry.INSTANCE.Monitor.Log(string.Format("Creating delegate for {0} (Type: {1})", assetName, assetType.ToString()));
            FileLoadMethod<T> injectorDelegate = CreateDelegate<T>(injector.MakeGenericMethod(assetType));
            registry.RegisterHandler(assetName, injectorDelegate);
            return true;
        }

        private static FileLoadMethod<T> CreateDelegate<T>(MethodInfo method) {
            return (loader, asset) => {
                return (T) method.Invoke(ModEntry.INSTANCE.merger, new object[] { loader, asset });
            };
        }
    }
}
