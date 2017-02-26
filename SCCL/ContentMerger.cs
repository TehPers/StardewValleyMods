using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TehPers.Stardew.Framework;

namespace TehPers.Stardew.SCCL {
    public class ContentMerger {
        public HashSet<string> Dirty { get; } = new HashSet<string>();
        public Dictionary<string, object> Cache { get; } = new Dictionary<string, object>();

        internal ContentMerger() { }

        public object Merge(string assetName, object obj) {
            return null;
        }

        public void RefreshAssets() {
            foreach (string assetName in Dirty) {
                object orig = Game1.content.Load<object>(assetName);
                
            }
        }

        private void RefreshAsset(string assetName, object orig) {
            this.Merge(assetName, orig).CopyAllFields(this.Cache[assetName]);
        }

        public void AssetLoading(object sender, IContentEventHelper e) {
            if (!Dirty.Contains(e.AssetName)) {
                Cache[e.AssetName] = e.Data;
            }
        }
    }
}
