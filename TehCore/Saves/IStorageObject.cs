using System.Collections.Generic;
using StardewValley;

namespace TehCore.Saves {
    public interface IStorageObject {
        IList<Item> Inventory { get; }
    }
}
