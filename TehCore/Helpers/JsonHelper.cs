using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehCore.Helpers.Json;

namespace TehCore.Helpers {
    public class JsonHelper {

        /// <summary>The JSON settings to use when serialising and deserialising files.</summary>
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Replace, // avoid issue where default ICollection<T> values are duplicated each time the config is loaded
            Converters = new List<JsonConverter>
            {
                // Properly converts Net* objects
                new NetConverter(),

                // Provides descriptions
                new DescriptiveJsonConverter()
            }
        };

        public void AddSmapiConverters(IModHelper helper) {
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

        public void WriteJson<TModel>(string path, TModel model, IModHelper helper, bool minify = false) where TModel : class => this.WriteJson(path, model, helper, s => { }, minify);
        public void WriteJson<TModel>(string path, TModel model, IModHelper helper, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class {
            //string fullPath = Path.Combine(helper.DirectoryPath, PathUtilities.NormalisePathSeparators(path));
            string fullPath = helper != null ? Path.Combine(helper.DirectoryPath, path) : path;

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
                    JsonSerializerSettings jsonSettings = this._jsonSettings.Clone();
                    settings(jsonSettings);
                    JsonSerializer serializer = JsonSerializer.CreateDefault(jsonSettings);
                    serializer.Serialize(writer, model);
                }
            }
        }

        public TModel ReadJson<TModel>(string path, IModHelper helper, Action<JsonSerializerSettings> settings) where TModel : class {
            //string fullPath = Path.Combine(helper.DirectoryPath, PathUtilities.NormalisePathSeparators(path));
            string fullPath = helper != null ? Path.Combine(helper.DirectoryPath, path) : path;

            // Validate path
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file path is empty or invalid.", nameof(fullPath));
            if (!File.Exists(fullPath))
                return null;

            // Setup serializer settings
            JsonSerializerSettings jsonSettings = this._jsonSettings.Clone();
            settings(jsonSettings);

            // Deserialize
            return JsonConvert.DeserializeObject<TModel>(File.ReadAllText(fullPath), jsonSettings);
        }
    }
}
