using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace TehPers.ShroomSpotter {
    public class ModConfig {
        public bool ModEnabled { get; set; } = true;
        public SButton GetShroomLevels { get; set; } = SButton.NumPad5;
        public Shroom ShroomIcon { get; set; } = Shroom.Common;

        public enum Shroom {
            Morel = 257,
            Chanterelle = 281,
            Common = 404,
            Red = 420,
            Purple = 422,
            Truffle = 430
        }
    }
}