using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.ContentPacks.Tokens.Parsing {
    internal interface ITokenizedStringPart : IContextSpecific {
        TokenValue GetValue(ITokenHelper helper);
    }
}