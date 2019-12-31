using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Json;
using TehPers.Core.Content;

namespace TehPers.Core.Json
{
    internal class CommentedJsonProvider : IJsonProvider
    {
        private readonly IModHelper helper;
        private readonly JsonSerializerSettings jsonSettings;

        public CommentedJsonProvider(IModHelper helper, IEnumerable<JsonConverter> jsonConverters)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));

            this.jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ObjectCreationHandling = ObjectCreationHandling.Replace, // avoid issue where default ICollection<T> values are duplicated each time the config is loaded
                Converters = jsonConverters.ToList(),
            };
        }

        public void WriteJson<TModel>(TModel model, string path, IAssetProvider assetProvider, bool minify = false) where TModel : class => this.WriteJson(model, path, assetProvider, null, minify);

        public void WriteJson<TModel>(TModel model, string path, IAssetProvider assetProvider, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class
        {
            _ = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));

            // Write to stream directly
            using var stream = assetProvider.Open(path, FileMode.OpenOrCreate);
            using var writer = new StreamWriter(stream);
            this.Serialize(model, writer, settings, minify);
        }

        public void Serialize<TModel>(TModel model, StreamWriter outputStream, bool minify = false) where TModel : class => this.Serialize(model, outputStream, null, minify);

        public void Serialize<TModel>(TModel model, StreamWriter outputStream, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class
        {
            // Write to stream directly using the custom JSON writer without closing the stream
            using var writer = new DescriptiveJsonWriter(outputStream)
            {
                Minify = minify
            };

            // Setup JSON Settings
            var clonedSettings = CommentedJsonProvider.CloneSettings(this.jsonSettings);
            settings?.Invoke(clonedSettings);

            // Serialize
            JsonSerializer.CreateDefault(clonedSettings).Serialize(writer, model);
            writer.Flush();
        }

        public TModel Deserialze<TModel>(StreamReader inputStream) where TModel : class => this.Deserialze<TModel>(inputStream, null);

        public TModel Deserialze<TModel>(StreamReader inputStream, Action<JsonSerializerSettings> settings) where TModel : class
        {
            // Read from stream directly without closing the stream
            using var jsonReader = new JsonTextReader(inputStream);
            var clonedSettings = CommentedJsonProvider.CloneSettings(this.jsonSettings);
            settings?.Invoke(clonedSettings);

            // Deserialize
            return JsonSerializer.CreateDefault(clonedSettings).Deserialize<TModel>(jsonReader);
        }

        public TModel ReadJson<TModel>(string path) where TModel : class => this.ReadJson<TModel>(path, this.GetModContentSource(), null);

        public TModel ReadJson<TModel>(string path, IAssetProvider assetProvider, Action<JsonSerializerSettings> settings) where TModel : class
        {
            // Validate path
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("The file path is empty or invalid.", nameof(path));
            }

            // Read from file stream directly
            try
            {
                using var stream = assetProvider.Open(path, FileMode.Open);
                return this.Deserialze<TModel>(new StreamReader(stream), settings);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public TModel ReadOrCreate<TModel>(string path, bool minify = false) where TModel : class, new() => this.ReadOrCreate(path, this.GetModContentSource(), null, () => new TModel(), minify);
        public TModel ReadOrCreate<TModel>(string path, IAssetProvider assetProvider, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class, new() => this.ReadOrCreate(path, assetProvider, settings, () => new TModel(), minify);
        public TModel ReadOrCreate<TModel>(string path, Func<TModel> modelFactory, bool minify = false) where TModel : class => this.ReadOrCreate(path, this.GetModContentSource(), null, modelFactory, minify);

        public TModel ReadOrCreate<TModel>(string path, IAssetProvider assetProvider, Action<JsonSerializerSettings> settings, Func<TModel> modelFactory, bool minify = false) where TModel : class
        {
            var model = this.ReadJson<TModel>(path, assetProvider, settings);
            if (model == null)
            {
                model = modelFactory();
                this.WriteJson(model, path, assetProvider, settings, minify);
            }

            return model;
        }

        private IAssetProvider GetModContentSource()
        {
            return new ModAssetProvider(this.helper);
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