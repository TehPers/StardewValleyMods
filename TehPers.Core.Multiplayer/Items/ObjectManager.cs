using System.Diagnostics.CodeAnalysis;
using StardewValley;
using SObject = StardewValley.Object;

namespace TehPers.Core.Multiplayer.Items {
    public abstract class ObjectManager : ItemManager {
        public virtual string GetDescription(SObject item, string original) => original;

        #region Delegators
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used by Harmony as a patch")]
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Parameter names required by Harmony")]
        [DelegatorFor(nameof(Object), nameof(SObject.getDescription))]
        private static void GetDescription(SObject __instance, ref string __result) {
            if (ItemDelegator.GetManagerFor(__instance.ParentSheetIndex) is ObjectManager manager) {
                __result = manager.GetDescription(__instance, __result);
            }
        }
        #endregion
    }
}
