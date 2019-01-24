using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley.Menus;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Conflux.Collections;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Structs;
using TehPers.CoreMod.ContentPacks.Data;
using TehPers.CoreMod.ContentPacks.Data.Converters;
using TehPers.CoreMod.Items;

namespace TehPers.CoreMod.ContentPacks {
    internal class ContentPackLoader {
        private readonly ICoreApi _api;

        public ContentPackLoader(ICoreApi api) {
            this._api = api;
        }

        public void LoadContentPacks() {
            // Get all content packs
            IContentPack[] contentPacks = this._api.Owner.Helper.ContentPacks.GetOwned().ToArray();
            this._api.Owner.Monitor.Log($"Found {contentPacks.Length} content pack{(contentPacks.Length == 1 ? "s" : "")}", LogLevel.Trace);

            // If there aren't any, then just skip this step
            if (!contentPacks.Any()) {
                return;
            }

            this._api.Owner.Monitor.Log($"Loading {contentPacks.Length} content pack(s):", LogLevel.Info);
            foreach (IContentPack contentPack in contentPacks) {
                try {
                    this.LoadContentPack(contentPack);
                    this._api.Owner.Monitor.Log($" - {contentPack.Manifest.UniqueID} loaded successfully", LogLevel.Info);
                } catch (Exception ex) {
                    this._api.Owner.Monitor.Log($" - {contentPack.Manifest.UniqueID} failed to load: {ex}", LogLevel.Error);
                }
            }
        }

        private void LoadContentPack(IContentPack contentPack) {
            // Create the content source for this pack
            ContentPackContentSource contentSource = new ContentPackContentSource(contentPack);

            // Load all content files
            ContentSource[] data = this.LoadData(contentSource).ToArray();

            // Load all the items from the content pack
            this.LoadItems(contentPack, contentSource, data);
        }

        private IEnumerable<ContentSource> LoadData(IContentSource contentSource) => this.LoadData(contentSource, new HashSet<string>(), "", "content.json");
        private IEnumerable<ContentSource> LoadData(IContentSource contentSource, ISet<string> loadedPaths, string configDir, string configName) {
            // Combine the directory and file name
            string fullPath = Path.Combine(configDir, configName);

            // Check if this data has already been loaded
            if (!loadedPaths.Add(fullPath)) {
                return Enumerable.Empty<ContentSource>();
            }

            // Set up all the custom converters
            void SetSerializerSettings(JsonSerializerSettings settings) {
                settings.Converters.Add(new PossibleConfigValuesDataJsonConverter());
                // settings.Converters.Add(new ContentPackValueJsonConverter());
                settings.Converters.Add(new SColorJsonConverter());
            }

            // Check if this path points to proper content pack data
            if (!(this._api.Json.ReadJson<ContentPackData>(fullPath, contentSource, SetSerializerSettings) is ContentPackData curData)) {
                throw new Exception($"Invalid content file: {fullPath}");
            }

            // Get all child include paths
            IEnumerable<ContentSource> childIncludes = from relativeChildPath in curData.Include
                                                       let fullChildPath = Path.Combine(configDir, relativeChildPath)
                                                       let childDir = Path.GetDirectoryName(fullChildPath)
                                                       let childName = Path.GetFileName(fullChildPath)
                                                       from child in this.LoadData(contentSource, loadedPaths, childDir, childName)
                                                       select child;

            // Return all the paths
            return new ContentSource(fullPath, curData).Yield().Concat(childIncludes);
        }

        private Dictionary<string, PossibleConfigValuesData> GetConfigStructure(IEnumerable<ContentSource> sources) {
            var configPairs = (from source in sources
                               from kv in source.Content.Config
                               select new { Key = kv.Key, PossibleValues = kv.Value, Source = source }).ToArray();

            // Create exceptions for conflicting config keys
            Exception[] exceptions = (from pairGroup in this.GetDuplicateGroups(configPairs, pair => pair.Key)
                                      select new Exception($"Config option '{pairGroup.Key}' is being registered by multiple content files: {string.Join(", ", pairGroup.Select(pair => $"'{pair.Source}'"))}")).ToArray();

            // Throw the exceptions
            if (exceptions.Any()) {
                if (exceptions.Length > 1) {
                    throw new AggregateException(exceptions);
                }

                throw exceptions.First();
            }

            return configPairs.ToDictionary(pair => pair.Key, pair => pair.PossibleValues);
        }

        private void LoadItems(IContentPack contentPack, IContentSource contentSource, IEnumerable<ContentSource> sources) {
            var items = (from source in sources
                         from entry in source.Content.Objects
                         select new { Source = source, Name = entry.Key, Data = entry.Value }).ToArray();

            // Create exceptions for conflicting item names
            Exception[] exceptions = (from itemGroup in this.GetDuplicateGroups(items, item => item.Name)
                                      select new Exception($"Item '{itemGroup.Key}' is being registered by multiple content files: {string.Join(", ", itemGroup.Select(item => $"'{item.Source.Path}'"))}")).ToArray();

            // Throw the exceptions
            if (exceptions.Any()) {
                if (exceptions.Length > 1) {
                    throw new AggregateException(exceptions);
                }

                throw exceptions.First();
            }

            // Create each object
            foreach (var item in items) {
                ItemKey key = new ItemKey(contentPack.Manifest, item.Name);

                // Create the sprite for the object
                // TODO: Create each possible sprite (in the case of tokens and conditional values)
                ISprite sprite = this.CreateSprite(contentSource, item.Name, item.Data);

                // Create the object's manager
                ModObject objectManager = new ModObject(this._api, sprite, key.LocalKey, item.Data.Cost, new Category(item.Data.CategoryNumber, item.Data.CategoryName), item.Data.Edibility) {
                    Tint = item.Data.Tint
                };

                // Register the object
                this._api.Items.DefaultItemProviders.ObjectProvider.Register(key, objectManager);
            }
        }

        private ISprite CreateSprite(IContentSource contentSource, string key, SObjectData objectData) {
            // Try to get the sprite location
            if (!(objectData.Sprite is string spriteLocation)) {
                throw new Exception($"{key} must have a valid sprite location.");
            }

            // Try to load the sprite
            if (!(contentSource.Load<Texture2D>(spriteLocation) is Texture2D spriteTexture)) {
                throw new Exception($"{key}'s sprite location is invalid: {spriteLocation}");
            }

            return this._api.Items.CreateSprite(spriteTexture);
        }

        private IEnumerable<IGrouping<TKey, T>> GetDuplicateGroups<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector) {
            return from item in source // For each item
                   group item by keySelector(item) into g // Group the source paths by the name of the item
                   where g.Skip(1).Any() // Where there's a conflict
                   select g;
        }

        private class ContentSource {
            public string Path { get; }
            public ContentPackData Content { get; }

            public ContentSource(string path, ContentPackData content) {
                this.Path = path;
                this.Content = content;
            }
        }
    }
}
