using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehCore.Configs;

namespace TehCore.Helpers {
    public class JsonHelper {

        /// <summary>The JSON settings to use when serialising and deserialising files.</summary>
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Replace, // avoid issue where default ICollection<T> values are duplicated each time the config is loaded
            Converters = new List<JsonConverter>
            {
                // Provides descriptions
                new DescriptiveJsonConverter()
            }
        };

        public void AddSMAPIConverters(IModHelper helper) {
            object smapiJsonHelper = helper.GetType().GetField("JsonHelper", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(helper);
            JsonSerializerSettings smapiSettings = smapiJsonHelper?.GetType().GetField("JsonSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(smapiJsonHelper) as JsonSerializerSettings;
            if (smapiSettings != null) {
                foreach (JsonConverter converter in smapiSettings.Converters) {
                    this._jsonSettings.Converters.Add(converter);
                }
            } else {
                ModCore.Instance.Monitor.Log("Unable to add SMAPI's JSON converters. Some config settings might be confusing!", LogLevel.Error);
            }
        }

        public void WriteJson<T>(string path, T model, IModHelper helper, bool minify = false) {
            //string fullPath = Path.Combine(helper.DirectoryPath, PathUtilities.NormalisePathSeparators(path));
            string fullPath = Path.Combine(helper.DirectoryPath, path);

            // Validate path
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file path is empty or invalid.", nameof(fullPath));

            // Create directory if needed
            string dir = Path.GetDirectoryName(fullPath);
            if (dir == null)
                throw new ArgumentException("The file path is invalid.", nameof(fullPath));
            Directory.CreateDirectory(dir);

            // Write to file directly
            using (TextWriter textWriter = new StreamWriter(new FileStream(fullPath, FileMode.Create))) {
                // Create JSON writer
                using (DescriptiveJsonWriter writer = new DescriptiveJsonWriter(textWriter)) {
                    writer.Minify = minify;

                    // Serialize
                    JsonSerializer serializer = JsonSerializer.CreateDefault(this._jsonSettings);
                    serializer.Serialize(writer, model);
                }
            }
        }
    }
}
