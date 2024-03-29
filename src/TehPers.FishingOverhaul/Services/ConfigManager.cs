﻿using System;
using Ninject;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Config;

namespace TehPers.FishingOverhaul.Services
{
    internal class ConfigManager<T>
        where T : class, IModConfig, new()
    {
        private readonly IJsonProvider jsonProvider;
        private readonly string path;

        public ConfigManager(IJsonProvider jsonProvider, [Named("path")] string path)
        {
            this.jsonProvider =
                jsonProvider ?? throw new ArgumentNullException(nameof(jsonProvider));
            this.path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public T Load()
        {
            if (this.jsonProvider.ReadJson<T>(this.path) is { } config)
            {
                // Return loaded config
                return config;
            }

            // Create new config
            config = new();
            config.Reset();
            this.jsonProvider.WriteJson(config, this.path);
            return config;
        }

        public void Save(T value)
        {
            this.jsonProvider.WriteJson(value, this.path);
        }
    }
}