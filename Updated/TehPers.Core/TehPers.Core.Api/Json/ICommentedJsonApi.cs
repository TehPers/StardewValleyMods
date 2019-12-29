using System;
using System.IO;
using Newtonsoft.Json;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Api.Json
{
    /// <summary>API for reading and writing commented JSON files.</summary>
    public interface ICommentedJsonApi
    {
        /// <summary>Writes a commented JSON file to a specified path.</summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="path">The path to the output file.</param>
        /// <param name="model">The object being written.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        void WriteJson<TModel>(string path, TModel model, bool minify = false)
            where TModel : class;

        /// <summary>Writes a commented JSON file to a specified path.</summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="path">The path to the output file.</param>
        /// <param name="model">The object being written.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        void WriteJson<TModel>(string path, TModel model, Action<JsonSerializerSettings> settings, bool minify = false)
            where TModel : class;

        /// <summary>Serializes commented JSON to a <see cref="Stream"/>.</summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="model">The object being written.</param>
        /// <param name="outputStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        void Serialize<TModel>(TModel model, Stream outputStream, bool minify = false)
            where TModel : class;

        /// <summary>Serializes commented JSON to a <see cref="Stream"/>.</summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="model">The object being written.</param>
        /// <param name="outputStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        void Serialize<TModel>(TModel model, Stream outputStream, Action<JsonSerializerSettings> settings, bool minify = false)
            where TModel : class;

        /// <summary>Deserializes commented JSON from a <see cref="Stream"/>.</summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="inputStream">The <see cref="Stream"/> to read from.</param>
        /// <returns>The deserialized model.</returns>
        TModel Deserialze<TModel>(Stream inputStream)
            where TModel : class;

        /// <summary>Deserializes commented JSON from a <see cref="Stream"/>.</summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="inputStream">The <see cref="Stream"/> to read from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deserialized model.</returns>
        TModel Deserialze<TModel>(Stream inputStream, Action<JsonSerializerSettings> settings)
            where TModel : class;

        /// <summary>Reads commented JSON from a file.</summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadJson<TModel>(string path)
            where TModel : class;

        /// <summary>Reads commented JSON from a file.</summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="assetProvider">The <see cref="IAssetProvider"/> to read the file from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadJson<TModel>(string path, IAssetProvider assetProvider, Action<JsonSerializerSettings> settings)
            where TModel : class;

        /// <summary>Reads commented JSON from a file, creating the file if it doesn't exist.</summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="minify">Whether to minify the output if the file is created. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadOrCreate<TModel>(string path, bool minify = false)
            where TModel : class, new();

        /// <summary>Reads commented JSON from a file, creating the file if it doesn't exist.</summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="assetProvider">The <see cref="IAssetProvider"/> to read the file from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="minify">Whether to minify the output if the file is created. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadOrCreate<TModel>(string path, IAssetProvider assetProvider, Action<JsonSerializerSettings> settings, bool minify = false)
            where TModel : class, new();

        /// <summary>Reads commented JSON from a file, creating the file if it doesn't exist.</summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="modelFactory">Factory method which creates an instance of model.</param>
        /// <param name="minify">Whether to minify the output if the file is created. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadOrCreate<TModel>(string path, Func<TModel> modelFactory, bool minify = false)
            where TModel : class;

        /// <summary>Reads commented JSON from a file, creating the file if it doesn't exist.</summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="assetProvider">The <see cref="IAssetProvider"/> to read the file from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="modelFactory">Factory method which creates an instance of model.</param>
        /// <param name="minify">Whether to minify the output if the file is created. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadOrCreate<TModel>(string path, IAssetProvider assetProvider, Action<JsonSerializerSettings> settings, Func<TModel> modelFactory, bool minify = false)
            where TModel : class;
    }
}