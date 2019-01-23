using StardewValley;

namespace TehPers.CoreMod.Api.Items {
    public interface IObjectInformation {
        /// <summary>The <see cref="Item.ParentSheetIndex"/> assigned to this type of object. An object might not have an index assigned to it even if it is registered for several reasons: the player might not be in game yet, the host has disabled the object, or not all players have the object registered.</summary>
        int? Index { get; }

        /// <summary>The <see cref="IModObject"/> that handles instances of this type of object.</summary>
        IModObject Manager { get; }

        /// <summary>The key this type of object is registered as.</summary>
        ItemKey Key { get; }
    }
}
