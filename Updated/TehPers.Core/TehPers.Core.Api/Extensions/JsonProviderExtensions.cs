using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TehPers.Core.Api.Json;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IJsonProvider"/>.
    /// </summary>
    public static class JsonProviderExtensions
    {
        /// <summary>
        /// Serializes data to JSON.
        /// </summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="json">The JSON provider.</param>
        /// <param name="data">The object being written.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The serialized data.</returns>
        public static string Serialize<TModel>(this IJsonProvider json, TModel data, Action<JsonSerializerSettings> settings = null, bool minify = false)
            where TModel : class
        {
            _ = json ?? throw new ArgumentNullException(nameof(json));

            using var buffer = new MemoryStream();

            // This writer cannot be disposed before the stream is read
            using var writer = new StreamWriter(buffer, Encoding.UTF8);
            json.Serialize(data, writer, settings, minify);

            buffer.Position = 0;
            using var reader = new StreamReader(buffer, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Deserializes JSON text.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="json">The JSON provider.</param>
        /// <param name="text">The text to read from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deserialized model.</returns>
        public static TModel Deserialize<TModel>(this IJsonProvider json, string text, Action<JsonSerializerSettings> settings = null)
            where TModel : class
        {
            _ = text ?? throw new ArgumentNullException(nameof(text));
            _ = json ?? throw new ArgumentNullException(nameof(json));

            using var buffer = new MemoryStream();

            // This writer cannot be disposed before the stream is read
            using var writer = new StreamWriter(buffer, Encoding.UTF8);
            writer.Write(text);

            buffer.Position = 0;
            using var reader = new StreamReader(buffer, Encoding.UTF8);
            return json.Deserialize<TModel>(reader, settings);
        }
    }
}