using System;
using System.Linq;
using System.Reflection;
using StardewValley;
using TehPers.Core.Helpers.Static;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items.Managers {
    public abstract class CustomObjectManager : CustomItemManager {

        public virtual string GetDescription(SObject item, string vanillaResult) => vanillaResult;

        #region Static
        static CustomObjectManager() {
            CustomItemManager.ApplyPatches<CustomObjectManager>();
        }
        #endregion
    }
}