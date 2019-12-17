using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Json;
using TehPers.Core.Content;

namespace TehPers.Core.Json
{
    internal class CommentedJsonApi : ICommentedJsonApi
    {
        private readonly IMod mod;

        /// <summary>The JSON settings to use when serialising and deserialising files.</summary>
        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
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

        internal CommentedJsonApi(IMod mod)
        {
            this.mod = mod;
            this.AddSmapiConverters(mod.Helper);
        }

        private void AddSmapiConverters(IModHelper helper)
        {
            var smapiJsonHelper = helper.Data.GetType().GetField("JsonHelper", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(helper.Data);
            var jsonSettings = smapiJsonHelper?.GetType().GetProperty("JsonSettings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(smapiJsonHelper);
            if (jsonSettings is JsonSerializerSettings smapiSettings)
            {
                // Add all the converters SMAPI uses to this API's serializer settings
                foreach (var converter in smapiSettings.Converters)
                {
                    this.jsonSettings.Converters.Add(converter);
                }
            }
            else
            {
                this.mod.Monitor.Log("Unable to add SMAPI's JSON converters. Some config settings might be confusing!", LogLevel.Error);
            }
        }

        public void WriteJson<TModel>(string path, TModel model, bool minify = false) where TModel : class => this.WriteJson(path, model, null, minify);
        public void WriteJson<TModel>(string path, TModel model, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class
        {
            var fullPath = Path.Combine(this.mod.Helper.DirectoryPath, path);

            // Validate path
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file path is empty or invalid.", nameof(fullPath));

            // Create directory if needed
            var dir = Path.GetDirectoryName(fullPath);
            if (dir == null)
                throw new ArgumentException("The file path is invalid.", nameof(fullPath));
            Directory.CreateDirectory(dir);

            // Write to file stream directly
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                this.Serialize(model, stream, settings, minify);
            }
        }

        public void Serialize<TModel>(TModel model, Stream outputStream, bool minify = false) where TModel : class => this.Serialize(model, outputStream, null, minify);
        public void Serialize<TModel>(TModel model, Stream outputStream, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class
        {
            // Write to stream directly using the custom JSON writer without closing the stream
            TextWriter textWriter = new StreamWriter(outputStream);
            using (var writer = new DescriptiveJsonWriter(textWriter))
            {
                writer.Minify = minify;

                // Setup JSON Settings
                var jsonSettings = CommentedJsonApi.CloneSettings(this.jsonSettings);
                settings?.Invoke(jsonSettings);

                // Serialize
                JsonSerializer.CreateDefault(jsonSettings).Serialize(writer, model);
                writer.Flush();
            }
        }

        public TModel Deserialze<TModel>(Stream inputStream) where TModel : class => this.Deserialze<TModel>(inputStream, null);
        public TModel Deserialze<TModel>(Stream inputStream, Action<JsonSerializerSettings> settings) where TModel : class
        {
            // Read from stream directly without closing the stream
            var streamReader = new StreamReader(inputStream);
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                // Setup JSON settings
                var jsonSettings = CommentedJsonApi.CloneSettings(this.jsonSettings);
                settings?.Invoke(jsonSettings);

                // Deserialize
                return JsonSerializer.CreateDefault(jsonSettings).Deserialize<TModel>(jsonReader);
            }
        }

        public TModel ReadJson<TModel>(string path) where TModel : class => this.ReadJson<TModel>(path, this.GetModContentSource(), null);
        public TModel ReadJson<TModel>(string path, IContentSource source, Action<JsonSerializerSettings> settings) where TModel : class
        {
            // Validate path
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The file path is empty or invalid.", nameof(path));

            // Read from file stream directly
            try
            {
                using (var stream = source.Open(path, FileMode.Open))
                {
                    return this.Deserialze<TModel>(stream, settings);
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public TModel ReadOrCreate<TModel>(string path, bool minify = false) where TModel : class, new() => this.ReadOrCreate(path, this.GetModContentSource(), null, () => new TModel(), minify);
        public TModel ReadOrCreate<TModel>(string path, IContentSource source, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class, new() => this.ReadOrCreate(path, source, settings, () => new TModel(), minify);
        public TModel ReadOrCreate<TModel>(string path, Func<TModel> modelFactory, bool minify = false) where TModel : class => this.ReadOrCreate(path, this.GetModContentSource(), null, modelFactory, minify);
        public TModel ReadOrCreate<TModel>(string path, IContentSource source, Action<JsonSerializerSettings> settings, Func<TModel> modelFactory, bool minify = false) where TModel : class
        {
            var model = this.ReadJson<TModel>(path, source, settings);
            if (model == null)
            {
                model = modelFactory();
                this.WriteJson(path, model, settings, minify);
            }
            return model;
        }

        private IContentSource GetModContentSource()
        {
            return new ModContentSource(this.mod.Helper);
        }

        private static JsonSerializerSettings CloneSettings(JsonSerializerSettings source)
        {
            return new JsonSerializerSettings
            {
                CheckAdditionalContent = source.CheckAdditionalContent,
                ConstructorHandling = source.ConstructorHandling,
                Context = source.Context,
                ContractResolver = source.ContractResolver,
                Converters = new List<JsonConverter>(source.Converters),
                Culture = source.Culture,
                DateFormatHandling = source.DateFormatHandling,
                DateFormatString = source.DateFormatString,
                DateParseHandling = source.DateParseHandling,
                DateTimeZoneHandling = source.DateTimeZoneHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                Error = source.Error,
                EqualityComparer = source.EqualityComparer,
                Formatting = source.Formatting,
                FloatFormatHandling = source.FloatFormatHandling,
                FloatParseHandling = source.FloatParseHandling,
                MaxDepth = source.MaxDepth,
                MetadataPropertyHandling = source.MetadataPropertyHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                NullValueHandling = source.NullValueHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                PreserveReferencesHandling = source.PreserveReferencesHandling,
                ReferenceLoopHandling = source.ReferenceLoopHandling,
                ReferenceResolverProvider = source.ReferenceResolverProvider,
                SerializationBinder = source.SerializationBinder,
                StringEscapeHandling = source.StringEscapeHandling,
                TypeNameHandling = source.TypeNameHandling,
                TraceWriter = source.TraceWriter,
                TypeNameAssemblyFormatHandling = source.TypeNameAssemblyFormatHandling
            };
        }
    }
}
