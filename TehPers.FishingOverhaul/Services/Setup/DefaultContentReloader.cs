﻿using System;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal sealed class DefaultContentReloader : ISetup, IDisposable
    {
        private readonly IFishingApi fishingApi;
        private readonly INamespaceRegistry namespaceRegistry;

        public DefaultContentReloader(IFishingApi fishingApi, INamespaceRegistry namespaceRegistry)
        {
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
            this.namespaceRegistry = namespaceRegistry ?? throw new ArgumentNullException(nameof(namespaceRegistry));
        }

        public void Setup()
        {
            this.namespaceRegistry.OnReload += this.ReloadFishingData;
        }

        public void Dispose()
        {
            this.namespaceRegistry.OnReload -= this.ReloadFishingData;
        }

        private void ReloadFishingData(object? sender, EventArgs e)
        {
            this.fishingApi.RequestReload();
        }
    }
}