using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModUtilities.Menus {
    public interface IAutoConfig {
        /// <summary>Adds this config's options to a <see cref="ModConfigMenu"/></summary>
        /// <param name="menu">The menu to add options to</param>
        void BuildConfig(ModConfigMenu menu);
    }
}
