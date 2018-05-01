using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModUtilities.Menus;
using StardewConfigFramework;
using StardewModdingAPI;

namespace ModUtilities.Helpers {
    public static class SCMHelper {
        private const string SCMNamespace = "StardewConfigFramework";
        private static readonly Dictionary<string, Type> SCMTypes = new Dictionary<string, Type>();
        private static Dictionary<Mod, object> ModOptions { get; } = new Dictionary<Mod, object>();
        private static ulong _curID = 0;

        private static readonly Assembly SCM;
        private static readonly Action<object> AddModOptions;
        private static readonly MethodInfo AddModOption;

        public static bool SCMInstalled => SCMHelper.SCM != null;

        static SCMHelper() {
            SCMHelper.SCM = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("StardewConfigFramework"));
            Type t = SCMHelper.GetSCMType("ModOptions");
            if (t != null) {
                SCMHelper.AddModOption = t.GetMethod("AddModOption", BindingFlags.Public | BindingFlags.Instance);
            }

            t = SCMHelper.GetSCMType("IModSettingsFramework");
            if (t != null) {
                PropertyInfo instanceProperty = t.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                object instance = instanceProperty?.GetValue(null);
                if (instance != null) {
                    MethodInfo method = t.GetMethod("AddModOptions", BindingFlags.Public | BindingFlags.Instance);
                    SCMHelper.AddModOptions = options => method?.Invoke(instance, new[] { options });
                }
            }
        }

        public static object GetModOptions(Mod mod) {
            if (!SCMHelper.SCMInstalled)
                return null;

            if (SCMHelper.ModOptions.TryGetValue(mod, out object options))
                return options;

            options = Activator.CreateInstance(SCMHelper.GetSCMType("ModOptions"), mod);
            SCMHelper.ModOptions.Add(mod, options);
            SCMHelper.AddModOptions(options);
            return options;
        }

        public static void AddDefaultModOptions(ModConfigMenu menu) {
            ModOptions options = (ModOptions) SCMHelper.GetModOptions(menu.ParentMod);
            ModOptionTrigger button = new ModOptionTrigger("openConfig", "Open Config Menu", OptionActionType.SET);
            button.ActionTriggered += id => ModUtilities.Instance.ShowMenu(menu);

            options.AddModOption(button);
            //IModSettingsFramework.Instance.AddModOptions(options);
        }

        public static Type GetSCMType(string typeName) {
            if (SCMHelper.SCMTypes.TryGetValue(typeName, out Type t))
                return t;

            t = SCMHelper.SCM.GetType($@"{SCMHelper.SCMNamespace}.{typeName}");
            SCMHelper.SCMTypes.Add(typeName, t);
            return t;
        }
    }
}
