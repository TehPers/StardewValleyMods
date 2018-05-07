using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehCore.Saves {
    public interface ICustomItem<TModel> {
        /// <summary>Stores all the info about this item onto a single object.</summary>
        /// <returns>An object containing the info about this item that needs to be saved.</returns>
        TModel Save();

        /// <summary>Loads save data into this object.</summary>
        /// <param name="model">The object containing the data to load.</param>
        void Load(TModel model);
    }
}
