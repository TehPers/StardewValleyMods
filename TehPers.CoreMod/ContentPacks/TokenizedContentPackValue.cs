using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sprache;
using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;
using TehPers.CoreMod.ContentPacks.Tokens.Parsing;

namespace TehPers.CoreMod.ContentPacks {
    internal class TokenizedContentPackValue : IContentPackValue {
        private readonly string _rawString;
        private readonly ITokenizedStringPart[] _parts;
        private readonly TokenParser _tokenParser;

        public TokenizedContentPackValue(string rawString, TokenParser tokenParser) {
            this._rawString = rawString;
            this._tokenParser = tokenParser;

            this._parts = tokenParser.ParseRawValue(rawString).ToArray();
        }

        public TokenValue GetValue(ITokenHelper helper) {
            // No parts, return an empty string
            if (!this._parts.Any()) {
                return new TokenValue(string.Empty);
            }

            // Single part, so just return its value
            if (this._parts.Length == 1) {
                return this._parts[0].GetValue(helper);
            }

            // Multiple parts, so concatenate them all into a single string
            return new TokenValue(string.Concat(this._parts.Select(p => p.GetValue(helper))));
        }

        public bool IsValidInContext(IContext context) {
            return this._parts.All(p => p.IsValidInContext(context));
        }
    }
}