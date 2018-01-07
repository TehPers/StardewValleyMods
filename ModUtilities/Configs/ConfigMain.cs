using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Menus;
using Newtonsoft.Json;

namespace ModUtilities.Configs {
    public class ConfigMain : IAutoConfig {
        public Keys ModConfigKey { get; set; }
        public int ScrollSpeed { get; set; } = 100;

        [JsonIgnore]
        public float RealScrollbarSpeed => 1F / this.ScrollSpeed;

        public void BuildConfig(ModConfigMenu menu) {
            menu.AddItem(() => this.ScrollSpeed, "Scroll Speed");
            menu.AddItem(() => this.ModConfigKey, "Open Mod Config");

            byte test = 0;
            menu.AddItem(() => test, "Test item");

            menu.SettingChanged += (sender, args) => ModUtilities.Instance.Helper.WriteConfig(this);
        }
    }
}
