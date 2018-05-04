using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehCore.Menus {
    public class ConfigMenuBuilder {

        public Menu BuildConfigMenu(IGuiConfig config) {
            throw new NotImplementedException();
        }

    }

    public interface IGuiConfig {
        void BuildGui(object menu);
    }
}
