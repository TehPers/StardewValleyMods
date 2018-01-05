using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace ModUtilities.Configs {
    public class ConfigMain {
        public Keys? ModConfigKey { get; set; }
        public int ScrollSpeed { get; set; } = 100;

        [JsonIgnore]
        public float RealScrollbarSpeed => 1F / this.ScrollSpeed;
    }
}
