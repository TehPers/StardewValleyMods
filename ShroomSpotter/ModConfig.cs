using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace ShroomSpotter {
    public class ModConfig {
        public bool ModEnabled { get; set; } = true;
        public Keys GetShroomLevels { get; set; } = Keys.NumPad5;
    }
}
