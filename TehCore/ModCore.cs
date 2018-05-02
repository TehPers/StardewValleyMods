using System;
using System.IO;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehCore.Helpers;

namespace TehCore {
    public class ModCore : Mod {
        public static ModCore Instance { get; private set; }
        
        public JsonHelper Json { get; } = new JsonHelper();

        public override void Entry(IModHelper helper) {
            ModCore.Instance = this;

            this.Json.AddSMAPIConverters(helper);
        }
    }
}
