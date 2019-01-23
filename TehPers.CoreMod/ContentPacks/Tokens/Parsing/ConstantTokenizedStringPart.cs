using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.ContentPacks.Tokens.Parsing {
    internal class ConstantTokenizedStringPart : ITokenizedStringPart {
        private readonly string _value;

        public ConstantTokenizedStringPart(string value) {
            this._value = value;
        }

        public TokenValue GetValue(ITokenHelper helper) {
            return new TokenValue(this._value);
        }

        public bool IsValidInContext(IContext context) {
            return true;
        }
    }
}