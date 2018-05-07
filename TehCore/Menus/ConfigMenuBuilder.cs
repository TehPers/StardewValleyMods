using System;

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
