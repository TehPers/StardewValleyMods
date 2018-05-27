using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Harmony;
using StardewValley;
using StardewValley.Objects;
using TehPers.Core.Helpers.Static;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items.Managers {
    /// <summary>Manages custom items of a specific type.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class CustomItemManager {
        protected static readonly HarmonyInstance harmonyInstance = HarmonyInstance.Create("TehPers.Core");
        private static readonly Dictionary<int, CustomItemManager> _managers = new Dictionary<int, CustomItemManager>();

        

        #region Static
        static CustomItemManager() {
            CustomItemManager.ApplyPatches<CustomItemManager>();
        }

        protected static void ApplyPatches<TManager>() {
            // Apply patches
            Type originalType = AssortedHelpers.GetSDVType(nameof(StardewValley.Object));
            MethodInfo[] originalMethods = originalType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo replacement in typeof(TManager).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
                MethodInfo original = originalMethods.SingleOrDefault(m => m.Name.Equals(replacement.Name, StringComparison.OrdinalIgnoreCase));
                original = original ?? originalMethods.SingleOrDefault(m => m.Name.Equals($"{replacement.Name}Postfix", StringComparison.OrdinalIgnoreCase));
                if (original != null) {
                    // Postfix
                    Type delegator = typeof(HarmonyDelegator<,>).MakeGenericType(typeof(SObject), original.ReturnType);
                    MethodInfo delegatorMethod = delegator.GetMethod(nameof(HarmonyDelegator<Item, object>.DelegatePostfix), BindingFlags.Static | BindingFlags.Public) ?? throw new Exception($"Couldn't find delegator method for {original.ReturnType.Name}");
                    delegatorMethod.Invoke(null, new object[] { CustomItemManager.harmonyInstance, original, replacement });
                } else {
                    original = originalMethods.SingleOrDefault(m => m.Name.Equals($"{replacement.Name}Prefix", StringComparison.OrdinalIgnoreCase));
                    if (original != null) {
                        // Prefix
                        Type delegator = typeof(HarmonyDelegator<,>).MakeGenericType(typeof(SObject), original.ReturnType);
                        MethodInfo delegatorMethod = delegator.GetMethod(nameof(HarmonyDelegator<Item, object>.DelegatePrefix), BindingFlags.Static | BindingFlags.Public) ?? throw new Exception($"Couldn't find delegator method for {original.ReturnType.Name}");
                        delegatorMethod.Invoke(null, new object[] { CustomItemManager.harmonyInstance, original, replacement });
                    }
                }
            }
        }

        public static void SetManager(int parentSheetIndex, CustomItemManager manager) {
            CustomItemManager._managers[parentSheetIndex] = manager;
        }
        #endregion

        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        protected class HarmonyDelegator<TInstance, TResult> where TInstance : Item {
            private static readonly MethodInfo _prefixDelegator = typeof(HarmonyDelegator<TInstance, TResult>).GetMethod(nameof(HarmonyDelegator<TInstance, TResult>.Prefix), BindingFlags.Public | BindingFlags.Static);
            private static readonly MethodInfo _postfixDelegator = typeof(HarmonyDelegator<TInstance, TResult>).GetMethod(nameof(HarmonyDelegator<TInstance, TResult>.Postfix), BindingFlags.Public | BindingFlags.Static);
            private static readonly Dictionary<MethodInfo, MethodInfo> _prefixes = new Dictionary<MethodInfo, MethodInfo>();
            private static readonly Dictionary<MethodInfo, MethodInfo> _postfixes = new Dictionary<MethodInfo, MethodInfo>();
            private static bool _prefixPatched = false;
            private static bool _postfixPatched = false;

            public static void DelegatePrefix(HarmonyInstance instance, MethodInfo original, MethodInfo prefix) {
                if (original.ReturnType != typeof(TResult))
                    throw new ArgumentException($"Original method must have return type {typeof(TResult).Name}", nameof(original));
                if (prefix.ReturnType != typeof(bool))
                    throw new ArgumentException("Prefix method must return a bool", nameof(prefix));

                ParameterInfo[] parameters = prefix.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(TInstance))
                    throw new ArgumentException($"Prefix must have one argument: {typeof(TInstance).Name}", nameof(prefix));

                HarmonyDelegator<TInstance, TResult>._prefixes[original] = prefix;

                if (!HarmonyDelegator<TInstance, TResult>._prefixPatched) {
                    HarmonyDelegator<TInstance, TResult>._prefixPatched = true;
                    instance.Patch(original, new HarmonyMethod(HarmonyDelegator<TInstance, TResult>._prefixDelegator), null);
                }
            }

            public static void DelegatePostfix(HarmonyInstance instance, MethodInfo original, MethodInfo postfix) {
                if (original.ReturnType != typeof(TResult))
                    throw new ArgumentException($"Original method must have return type {typeof(TResult).Name}", nameof(original));
                if (postfix.ReturnType != typeof(TResult))
                    throw new ArgumentException($"Postfix method must have return type {typeof(TResult).Name}", nameof(postfix));

                ParameterInfo[] parameters = postfix.GetParameters();
                if (parameters.Length != 2 || parameters[0].ParameterType != typeof(TInstance) || parameters[1].ParameterType != typeof(TResult))
                    throw new ArgumentException($"Postfix must have two arguments: {typeof(TInstance).Name}, and {typeof(TResult).Name}", nameof(postfix));

                HarmonyDelegator<TInstance, TResult>._postfixes[original] = postfix;

                if (!HarmonyDelegator<TInstance, TResult>._postfixPatched) {
                    HarmonyDelegator<TInstance, TResult>._postfixPatched = true;
                    instance.Patch(original, null, new HarmonyMethod(HarmonyDelegator<TInstance, TResult>._postfixDelegator));
                    Chest.isPassable
                }
            }

            public static bool Prefix(TInstance __instance, MethodInfo __originalMethod) {
                if (!CustomItemManager._managers.TryGetValue(__instance.ParentSheetIndex, out CustomItemManager manager))
                    return true;

                if (!HarmonyDelegator<TInstance, TResult>._prefixes.TryGetValue(__originalMethod, out MethodInfo prefix))
                    return true;

                object[] arguments = { __instance };
                object ret = prefix.Invoke(manager, arguments);

                // Just to be safe
                if (ret is bool b)
                    return b;

                // Continue execution of original method if the prefix didn't return a bool
                return true;
            }

            public static void Postfix(TInstance __instance, ref TResult __result, MethodInfo __originalMethod) {
                if (!CustomItemManager._managers.TryGetValue(__instance.ParentSheetIndex, out CustomItemManager manager))
                    return;

                if (!HarmonyDelegator<TInstance, TResult>._postfixes.TryGetValue(__originalMethod, out MethodInfo prefix))
                    return;

                object[] arguments = { __instance, __result };
                __result = (TResult) prefix.Invoke(manager, arguments);
            }
        }
    }
}
