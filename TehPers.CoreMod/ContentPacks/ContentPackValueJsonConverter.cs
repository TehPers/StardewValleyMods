using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI.Events;
using TehPers.CoreMod.Api.Conflux.Collections;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.ContentPacks;
using TehPers.CoreMod.Api.ContentPacks.Tokens;
using TehPers.CoreMod.ContentPacks.Tokens.Parsing;

namespace TehPers.CoreMod.ContentPacks {
    internal class ContentPackValueJsonConverter : JsonConverter<IContentPackValue> {
        private readonly TokenParser _tokenParser;

        public ContentPackValueJsonConverter(TokenParser tokenParser) {
            this._tokenParser = tokenParser;
        }

        public override void WriteJson(JsonWriter writer, IContentPackValue value, JsonSerializer serializer) {
            throw new NotSupportedException();
        }

        public override IContentPackValue ReadJson(JsonReader reader, Type objectType, IContentPackValue existingValue, bool hasExistingValue, JsonSerializer serializer) {
            object value = reader.Value;
            throw new NotImplementedException();

            if (reader.TokenType == JsonToken.StartArray) {

            } else if (reader.TokenType == JsonToken.String) {
                return new TokenizedContentPackValue((string) reader.Value, this._tokenParser);
            } else {
                throw new InvalidOperationException($"Unexpected token: {reader.TokenType}");
            }
        }

        private IContentPackValue ReadArray(JsonReader reader) {
            JArray arr = JArray.Load(reader);

            if (arr.Any(token => token.Type != JTokenType.Object)) {
                throw new InvalidOperationException("Conditional values must be arrays of objects.");
            }

            foreach (JObject caseToken in arr.Cast<JObject>()) {
                if (!caseToken.TryGetValue("Value", StringComparison.OrdinalIgnoreCase, out JToken valueToken)) {
                    throw new InvalidOperationException("Case must have a value.");
                }

                TokenValue value = valueToken.Type.Match<JTokenType, TokenValue>()
                    .When(JTokenType.Null, () => new TokenValue())
                    .When(JTokenType.String, () => new TokenValue((string) valueToken))
                    .When(JTokenType.Integer, () => new TokenValue((int) valueToken))
                    .When(JTokenType.Boolean, () => new TokenValue((bool) valueToken))
                    .When(JTokenType.Float, () => new TokenValue((double) valueToken))
                    .ElseThrow(type => new InvalidOperationException($"Unexpected token: {type}"));

                // TODO
                object condition;
            }

            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
