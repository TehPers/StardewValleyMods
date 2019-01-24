namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface IDefaultItemProviders {
        /// <summary>Item provider for simple objects, like the ones that can be found in "Maps/springobjects".</summary>
        IObjectProvider ObjectProvider { get; }
        // TODO: object BigCraftableProvider { get; }
    }
}