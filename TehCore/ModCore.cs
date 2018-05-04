using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using TehCore.Helpers;

namespace TehCore {
    public class ModCore : Mod {
        public static ModCore Instance { get; private set; }
        
        public JsonHelper Json { get; } = new JsonHelper();

        public Texture2D WhitePixel { get; private set; }

        public override void Entry(IModHelper helper) {
            ModCore.Instance = this;

            // White texture
            this.WhitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.WhitePixel.SetData(new[] { Color.White});

            // Json writer
            this.Json.AddSmapiConverters(helper);
        }
    }
}
