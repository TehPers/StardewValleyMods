using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;
using StardewValley;

namespace TehPers.Core.Multiplayer.Items {
    public abstract class ItemManager {
        public virtual int AttachmentSlots(Item item, int result) => result;

        #region Delegators
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used by Harmony as a patch")]
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Parameter names required by Harmony")]
        [DelegatorFor(nameof(Item), nameof(Item.attachmentSlots))]
        private static void GetDescription(Item __instance, ref int __result) {
            ItemManager manager = ItemDelegator.GetManagerFor(__instance.ParentSheetIndex);
            __result = manager.AttachmentSlots(__instance, __result);
        }
        #endregion

        // Used by the delegator to keep track of which mod owns which manager
        internal IMod Owner { get; set; }
    }
}