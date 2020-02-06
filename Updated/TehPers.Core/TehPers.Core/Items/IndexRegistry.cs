using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TehPers.Core.Api;
using TehPers.Core.Api.DependencyInjection.Lifecycle;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.Core.Items
{
    public abstract class IndexRegistry : IIndexRegistry, IEventHandler<SaveLoadedEventArgs>, IEventHandler<ReturnedToTitleEventArgs>, IEventHandler<SavingEventArgs>
    {
        private readonly IJsonProvider json;
        private readonly IModHelper helper;
        private readonly string registryKey;
        private readonly int randomOffset;
        private readonly HashSet<int> fixedIndexes;
        private readonly Dictionary<NamespacedId, int> indexAssignments;
        private readonly Dictionary<NamespacedId, FixedReservation> fixedReservations;
        private readonly Dictionary<NamespacedId, RandomReservation> randomReservations;

        protected IndexRegistry(IJsonProvider json, IModHelper helper, string registryKey, int randomOffset = 0)
        {
            this.json = json ?? throw new ArgumentNullException(nameof(json));
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.registryKey = registryKey ?? throw new ArgumentNullException(nameof(registryKey));
            this.randomOffset = randomOffset;
            this.fixedIndexes = new HashSet<int>();
            this.indexAssignments = new Dictionary<NamespacedId, int>();
            this.fixedReservations = new Dictionary<NamespacedId, FixedReservation>();
            this.randomReservations = new Dictionary<NamespacedId, RandomReservation>();
        }

        public IIndexReservation Reserve(NamespacedId id)
        {
            return this.fixedReservations.TryGetValue(id, out var reservation) ? (IIndexReservation)reservation : this.randomReservations.GetOrAdd(id, () => new RandomReservation(this, id));
        }

        public IIndexReservation Reserve(NamespacedId id, int index)
        {
            if (this.fixedReservations.ContainsKey(id))
            {
                throw new InvalidOperationException($"A fixed reservation already exists for \"{id}\".");
            }

            if (!this.fixedIndexes.Add(index))
            {
                throw new InvalidOperationException($"The index {index} has already been reserved.");
            }

            var reservation = new FixedReservation(index);
            this.fixedReservations[id] = reservation;
            return reservation;
        }

        public IReadOnlyDictionary<NamespacedId, int> GetAll()
        {
            return this.indexAssignments;
        }

        private void AssignIndexes()
        {
            var assigned = new HashSet<int>();

            // Fixed index reservations
            foreach (var (id, reservation) in this.fixedReservations)
            {
                this.indexAssignments.Add(id, reservation.ReservedIndex);
                assigned.Add(reservation.ReservedIndex);
            }

            // Load index reservations
            var serialized = this.helper.Data.ReadSaveData<string>($"indexes.{this.registryKey}");
            var data = this.json.Deserialize<IndexRegistryData>(serialized);
            var loadedIndexes = data.Version switch
            {
                "1.0" => data.Indexes,
                _ => throw new InvalidOperationException($"Unknown format version: \"{data.Version}\"")
            };

            // Random index reservations
            var index = this.randomOffset;
            foreach (var id in this.randomReservations.Keys)
            {
                if (this.indexAssignments.ContainsKey(id))
                {
                    continue;
                }

                if (loadedIndexes.TryGetValue(id, out var loaded))
                {
                    this.indexAssignments.Add(id, loaded);
                    continue;
                }

                // Find next available index
                while (this.fixedIndexes.Contains(index) || !assigned.Add(index))
                {
                    // Prevent overflow
                    index = checked(index + 1);
                }

                this.indexAssignments.Add(id, index);
            }
        }

        private void SaveIndexes()
        {
            var data = new IndexRegistryData
            {
                Indexes = this.indexAssignments.Where(kv => this.randomReservations.ContainsKey(kv.Key)).ToDictionary(),
            };

            var serialized = this.json.Serialize(data, minify: true);
            this.helper.Data.WriteSaveData($"indexes.{this.registryKey}", serialized);
        }

        void IEventHandler<ReturnedToTitleEventArgs>.HandleEvent(object sender, ReturnedToTitleEventArgs args)
        {
            this.indexAssignments.Clear();
        }

        void IEventHandler<SaveLoadedEventArgs>.HandleEvent(object sender, SaveLoadedEventArgs args)
        {
            this.AssignIndexes();
        }

        void IEventHandler<SavingEventArgs>.HandleEvent(object sender, SavingEventArgs args)
        {
            this.SaveIndexes();
        }

        private class FixedReservation : IIndexReservation
        {
            public int ReservedIndex { get; }

            public FixedReservation(int reservedIndex)
            {
                this.ReservedIndex = reservedIndex;
            }

            public bool TryGetIndex(out int index)
            {
                index = this.ReservedIndex;
                return true;
            }
        }

        private class RandomReservation : IIndexReservation
        {
            private readonly IndexRegistry registry;
            private readonly NamespacedId id;

            public RandomReservation(IndexRegistry registry, NamespacedId id)
            {
                this.registry = registry;
                this.id = id;
            }

            public bool TryGetIndex(out int index)
            {
                return this.registry.indexAssignments.TryGetValue(this.id, out index);
            }
        }
    }
}