using System;
using System.Globalization;
using StardewModdingAPI;

namespace VariableFishing {
    public class ModEntry : Mod, IAssetEditor {
        public ConfigMain MainConfig { get; private set; }

        public override void Entry(IModHelper helper) {
            this.MainConfig = this.Helper.ReadConfig<ConfigMain>();

            // Reload Data\Fish.xnb
            this.Helper.Content.InvalidateCache(@"Data\Fish.xnb");
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals(@"Data\Fish");
        }

        public void Edit<T>(IAssetData asset) {
            asset.AsDictionary<int, string>()
                .Set((fish, rawData) => {
                    string[] data = rawData.Split('/');
                    if (!int.TryParse(data[1], out int difficulty))
                        return rawData;
                    data[1] = (difficulty * this.MainConfig.Difficulty).ToString(CultureInfo.InvariantCulture);
                    return string.Join("/", data);
                });
        }
    }
}
