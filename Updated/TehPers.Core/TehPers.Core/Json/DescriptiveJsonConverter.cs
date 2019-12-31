using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Json;

namespace TehPers.Core.Json {
    internal class DescriptiveJsonConverter : JsonConverter {
        private bool enabled = true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (!(writer is DescriptiveJsonWriter commentableWriter)) {
                this.enabled = false;
                serializer.Serialize(writer, value);
                return;
            }

            // Serialize the object normally
            this.enabled = false;
            var token = JObject.FromObject(value);
            this.enabled = true;

            // Write the object
            this.WriteObject(value, token, commentableWriter, serializer);
        }

        private void WriteObject(object value, JObject token, DescriptiveJsonWriter writer, JsonSerializer serializer) {
            var descriptions = new Dictionary<string, string>();
            var childrenValues = new Dictionary<string, object>();

            // Get all the property descriptions
            foreach (var property in value.GetType().GetProperties()) {
                this.GetMemberData(property, property.GetValue(value), childrenValues, descriptions);
            }

            // Get all the field descriptions
            foreach (var field in value.GetType().GetFields()) {
                this.GetMemberData(field, field.GetValue(value), childrenValues, descriptions);
            }

            // Write the object
            writer.WriteStartObject();
            foreach ((string property, JToken valueToken) in (IDictionary<string, JToken>) token) {
                // Write the property's description
                if (descriptions.TryGetValue(property, out var description)) {
                    writer.WritePropertyComment(description);
                }

                // Write the property's name
                writer.WritePropertyName(property);

                if (childrenValues.TryGetValue(property, out var childValue)) {
                    // Write the child object
                    serializer.Serialize(writer, childValue);
                } else {
                    // Write the value
                    valueToken.WriteTo(writer, serializer.Converters.ToArray());
                }
            }
            writer.WriteEndObject();
        }

        private void GetMemberData<T>(T memberInfo, object value, IDictionary<string, object> values, IDictionary<string, string> descriptions) where T : MemberInfo {
            // Get member name
            var name = memberInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
            name ??= memberInfo.Name;

            // Keep track of object
            values[name] = value;

            // Keep track of description
            var descAttr = memberInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descAttr != null) {
                descriptions[name] = descAttr.Description;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType) {
            return objectType.GetCustomAttribute<JsonDescribeAttribute>() != null;
        }

        public override bool CanRead { get; } = false;
        public override bool CanWrite => this.enabled;
    }
}
