using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.Api.ContentPacks {
    public interface IContentPackValue : IContextSpecific {
        TokenValue GetValue(ITokenHelper helper);
    }
}