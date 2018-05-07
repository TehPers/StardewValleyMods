using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace TehCore.Saves {
    public interface IStorageObject {
        IList<Item> Inventory { get; }
    }
}
