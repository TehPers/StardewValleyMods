using StardewValley;
using System;

namespace TehPers.Stardew.DataInjector {
    /**<summary>Wrapper class for the ContentMerger class. Only used until Ento's SmartContentManager loads in.</summary>**/
    public class CustomContentManager : LocalizedContentManager {

        public CustomContentManager(string rootDir, IServiceProvider service) : base(service, rootDir) {

        }

        public override T Load<T>(string assetName) {
            return ModEntry.INSTANCE.merger.Inject(base.Load<T>, assetName);
        }
    }
}
