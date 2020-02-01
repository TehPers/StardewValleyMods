using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Api;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Providers;

namespace TehPers.FishingFramework.Providers
{
    internal class DefaultTrashProvider : IDefaultTrashProvider
    {
        private readonly Lazy<ITrashAvailability[]> trash;

        public IEnumerable<ITrashAvailability> Trash => this.trash.Value;

        public DefaultTrashProvider()
        {
            this.trash = new Lazy<ITrashAvailability[]>(() => DefaultTrashProvider.GenerateTrashAvailabilities().ToArray());
        }

        private static IEnumerable<ITrashAvailability> GenerateTrashAvailabilities()
        {
            for (var i = 167; i < 173; i++)
            {
                yield return new GlobalTrashAvailability(NamespacedId.FromObjectIndex(i));
            }

            yield return new SpecificTrashAvailability(NamespacedId.FromObjectIndex(152), "Beach"); // Seaweed
            yield return new SpecificTrashAvailability(NamespacedId.FromObjectIndex(153), "Farm", invertLocations: true); // Green Algae
            yield return new SpecificTrashAvailability(NamespacedId.FromObjectIndex(157), "BugLand"); // White Algae
            yield return new SpecificTrashAvailability(NamespacedId.FromObjectIndex(157), "Sewers"); // White Algae
            yield return new SpecificTrashAvailability(NamespacedId.FromObjectIndex(157), "WitchSwamp"); // White Algae
            yield return new SpecificTrashAvailability(NamespacedId.FromObjectIndex(157), "UndergroundMines"); // White Algae
            yield return new SpecificTrashAvailability(NamespacedId.FromObjectIndex(797), "Submarine", 0.01D); // Pearl
            yield return new SpecificTrashAvailability(NamespacedId.FromObjectIndex(152), "Submarine", 0.99D); // Seaweed
        }
    }
}