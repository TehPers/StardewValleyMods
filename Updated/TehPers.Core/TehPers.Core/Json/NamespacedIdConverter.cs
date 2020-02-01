using System;
using Newtonsoft.Json;
using TehPers.Core.Api;

namespace TehPers.Core.Json
{
    internal class NamespacedIdJsonConverter : JsonConverter<NamespacedId>
    {
        public override void WriteJson(JsonWriter writer, NamespacedId value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override NamespacedId ReadJson(JsonReader reader, Type objectType, NamespacedId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            return NamespacedId.TryParse(str, out var value) ? value : throw new JsonReaderException($"Failed to parse {str} as a {nameof(NamespacedId)}.");
        }
    }
}