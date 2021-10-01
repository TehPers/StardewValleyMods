using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api.Items;

namespace TehPers.Core.Items
{
    /// <inheritdoc cref="INamespaceRegistry"/>
    public class NamespaceRegistry : INamespaceRegistry
    {
        private readonly IMonitor monitor;
        private readonly Dictionary<string, INamespaceProvider> namespaceProviders;

        public NamespaceRegistry(IMonitor monitor, IEnumerable<INamespaceProvider> namespaceProviders)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.namespaceProviders = namespaceProviders.ToDictionary(provider => provider.Name);

            monitor.Log($"Loaded {this.namespaceProviders.Count} namespaces:", LogLevel.Info);
            foreach (var name in this.namespaceProviders.Keys)
            {
                monitor.Log($" - {name}", LogLevel.Info);
            }
        }

        public bool TryGetItemFactory(NamespacedKey key, [NotNullWhen(true)] out IItemFactory? factory)
        {
            factory = default;
            return this.namespaceProviders.TryGetValue(key.Namespace, out var provider)
                   && provider.TryGetItemFactory(key.Key, out factory);
        }

        public IEnumerable<NamespacedKey> GetKnownItemKeys()
        {
            return this.namespaceProviders.Values.SelectMany(
                provider => provider.GetKnownItemKeys(),
                (provider, itemKey) => new NamespacedKey(provider.Name, itemKey)
            );
        }

        public void Reload()
        {
            foreach (var provider in this.namespaceProviders.Values)
            {
                provider.Reload();
            }

            this.OnReload?.Invoke(this, EventArgs.Empty);

            this.monitor.Log("Namespaces reloaded.", LogLevel.Info);
            this.monitor.Log($"There are {this.GetKnownItemKeys().Count()} known item keys.", LogLevel.Info);
        }

        public event EventHandler? OnReload;
    }
}