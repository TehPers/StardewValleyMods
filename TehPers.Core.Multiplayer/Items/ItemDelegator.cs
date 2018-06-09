using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace TehPers.Core.Multiplayer.Items {

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class ItemDelegator {
        private static readonly Dictionary<int, ItemManager> _managers = new Dictionary<int, ItemManager>();
        private static readonly HashSet<IMod> _conflictedMods = new HashSet<IMod>();
        private static readonly HashSet<int> _conflictedKeys = new HashSet<int>();
        private static readonly HashSet<MethodInfo> _prefixMethods = new HashSet<MethodInfo>();
        private static readonly HashSet<MethodInfo> _postfixMethods = new HashSet<MethodInfo>();
        private static bool _patched;

        internal static ItemManager GetManagerFor(int parentSheetIndex) {
            return ItemDelegator._managers.TryGetValue(parentSheetIndex, out ItemManager manager) ? manager : null;
        }

        internal static void SetManagerFor(int parentSheetIndex, ItemManager manager, TehMultiplayerApi api) {
            if (ItemDelegator._conflictedMods.Contains(api.Mod)) {
                // Don't register managers for conflicted mods
                api.Mod.Monitor.PrefixedLog($"Skipped manager for {parentSheetIndex}", LogLevel.Trace);
            } else if (ItemDelegator._conflictedKeys.Contains(parentSheetIndex)) {
                // Conflict found
                api.Mod.Monitor.PrefixedLog($"Conflict found with item {parentSheetIndex}. Removing all items from conflicting mods...", LogLevel.Warn);
                ItemDelegator.BlacklistMod(manager.Owner);
            } else if (ItemDelegator._managers.TryGetValue(parentSheetIndex, out ItemManager conflict)) {
                // Conflict found
                api.Mod.Monitor.PrefixedLog($"Conflict found with item {parentSheetIndex}. Removing all items from conflicting mods...", LogLevel.Warn);
                ItemDelegator.BlacklistMod(manager.Owner);
                if (manager.Owner != conflict.Owner) {
                    ItemDelegator.BlacklistMod(conflict.Owner);
                }
            } else {
                ItemDelegator._managers[parentSheetIndex] = manager;
            }

            ItemDelegator.ApplyPatches();
        }

        private static void BlacklistMod(IMod mod) {
            // Make sure this mod can't register anymore managers
            ItemDelegator._conflictedMods.Add(mod);
            mod.Monitor.PrefixedLog("Removing all item managers...", LogLevel.Warn);

            // Get all managers registered by this mod
            HashSet<int> removed = new HashSet<int>();
            foreach (KeyValuePair<int, ItemManager> managerPair in ItemDelegator._managers) {
                if (managerPair.Value.Owner != mod)
                    continue;

                mod.Monitor.PrefixedLog($"Removing {managerPair.Key} from {mod.ModManifest.UniqueID}", LogLevel.Warn);
                removed.Add(managerPair.Key);
                ItemDelegator._conflictedKeys.Add(managerPair.Key);
            }

            // Remove each manager found
            foreach (int key in removed) {
                ItemDelegator._managers.Remove(key);
            }
        }

        internal static void ApplyPatches() {
            if (ItemDelegator._patched)
                return;

            ItemDelegator._patched = true;

            // Get a harmony instance
            HarmonyInstance instance = HarmonyInstance.Create("TehPers.Core.Multiplayer");

            // Get every delegator function
            var delegators = from type in Assembly.GetExecutingAssembly().GetTypes()
                             from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                             from attribute in method.GetCustomAttributes<DelegatorForAttribute>()
                             select new { Attribute = attribute, Method = method };

            // Apply patches for them all
            foreach (var delegator in delegators) {
                Type returnType = delegator.Method.ReturnType;

                if (typeof(IEnumerable<CodeInstruction>).IsAssignableFrom(returnType)) {
                    // Transpiler
                    throw new NotSupportedException("Transpilers can't be delegated");
                } else if (delegator.Method.ReturnType == typeof(void)) {
                    // Postfix
                    if (!ItemDelegator._postfixMethods.Add(delegator.Attribute.TargetMethod))
                        throw new InvalidOperationException("Tried to apply multiple postfixes to the same method");

                    instance.Patch(delegator.Attribute.TargetMethod, null, new HarmonyMethod(delegator.Method));
                } else if (delegator.Method.ReturnType == typeof(bool)) {
                    // Prefix
                    if (!ItemDelegator._prefixMethods.Add(delegator.Attribute.TargetMethod))
                        throw new InvalidOperationException("Tried to apply multiple prefixes to the same method");

                    instance.Patch(delegator.Attribute.TargetMethod, new HarmonyMethod(delegator.Method), null);
                } else {
                    throw new InvalidOperationException("Unrecognized patch type");
                }
            }
        }
    }
}
