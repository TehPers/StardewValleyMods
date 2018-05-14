using System.Collections.Generic;
using StardewValley;

namespace TehPers.Core.Saves {
    public interface IStorageObject {
        IList<Item> Inventory { get; }
    }
}
