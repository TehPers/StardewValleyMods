using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items.ItemProviders;

namespace TehPers.CoreMod.Items.ItemProviders {
    internal class DefaultItemProviders : IDefaultItemProviders {
        public IObjectProvider ObjectProvider { get; }

        public DefaultItemProviders(IApiHelper apiHelper, ItemDelegator2 itemDelegator) {
            ObjectProvider objectProvider = new ObjectProvider(apiHelper, itemDelegator);
            this.ObjectProvider = objectProvider;
            itemDelegator.AddProvider(_ => this.ObjectProvider);
            apiHelper.Owner.Helper.Content.AssetEditors.Add(objectProvider);
        }
    }
}