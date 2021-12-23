using System;
using System.IO;
using Netcode;
using Newtonsoft.Json;

namespace TehPers.Core.Json
{
    internal class NetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Write the object to a stream and store it in an exposer object
            using var stream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(stream);
            ((AbstractNetSerializable)value).WriteFull(binaryWriter);
            var exposer = new NetExposer(value.GetType(), stream.ToArray());

            serializer.Serialize(writer, exposer);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(AbstractNetSerializable).IsAssignableFrom(objectType);
        }

        public override bool CanRead => false;
    }
}