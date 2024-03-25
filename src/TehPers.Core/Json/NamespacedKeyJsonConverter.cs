using System;
using Newtonsoft.Json;
using TehPers.Core.Api.Items;

namespace TehPers.Core.Json
{
    internal class NamespacedKeyJsonConverter : JsonConverter<NamespacedKey>
    {
        public override void WriteJson(
            JsonWriter writer,
            NamespacedKey value,
            JsonSerializer serializer
        )
        {
            writer.WriteValue(value.ToString());
        }

        public override NamespacedKey ReadJson(
            JsonReader reader,
            Type objectType,
            NamespacedKey existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            if (reader.Value is not string raw || !NamespacedKey.TryParse(raw, out var key))
            {
                throw new JsonException(
                    "Expected colon-delimited string in the format 'namespace:key'."
                );
            }

            return key;
        }
    }
}
