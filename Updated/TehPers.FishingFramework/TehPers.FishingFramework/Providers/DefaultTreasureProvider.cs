using System;
using System.Collections.Generic;
using System.Linq;
using TehPers.Core.Api;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Providers;

namespace TehPers.FishingFramework.Providers
{
    internal class DefaultTreasureProvider : IDefaultTreasureProvider
    {
        private readonly Lazy<ITreasureAvailability[]> treasure;

        public IEnumerable<ITreasureAvailability> Treasure => this.treasure.Value;

        public DefaultTreasureProvider()
        {
            this.treasure = new Lazy<ITreasureAvailability[]>(() => this.GenerateTreasureAvailabilities().ToArray());
        }

        private IEnumerable<ITreasureAvailability> GenerateTreasureAvailabilities()
        {
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.DressedSpinner).Yield(), 0.025, 1, 1, 6, allowDuplicates: false);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Bait).Yield(), 0.25, 2, 4);

            // Archaeology
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.LostBook).Yield(), 0.025, allowDuplicates: false);
            yield return new TreasureAvailability(Enumerable.Range(585, 4).Select(NamespacedId.FromObjectIndex), 0.0625); // Archaeology, part 1
            yield return new TreasureAvailability(Enumerable.Range(96, 32).Select(NamespacedId.FromObjectIndex), 0.125); // Archaeology, part 2

            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Geode).Yield(), 0.2, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.FrozenGeode).Yield(), 0.125, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.MagmaGeode).Yield(), 0.125, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.OmniGeode).Yield(), 0.0625, 1, 3);

            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.IridiumOre).Yield(), 0.0075, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.GoldOre).Yield(), 0.15, 3, 10);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.IronOre).Yield(), 0.15, 3, 10);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.CopperOre).Yield(), 0.15, 3, 10);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Coal).Yield(), 0.3, 3, 10);

            // Junk
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Wood).Yield(), 0.25, 10, 25);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Stone).Yield(), 0.25, 10, 25);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.MixedSeeds).Yield(), 0.5, 3, 5, maxLevel: 1);

            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.TreasureChest).Yield(), 0.005, allowDuplicates: false);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.PrismaticShard).Yield(), 0.00025, allowDuplicates: false);

            // Gems
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Diamond).Yield(), 0.01, allowDuplicates: false);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.FireQuartz).Yield(), 0.025, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Emerald).Yield(), 0.025, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Ruby).Yield(), 0.025, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.FrozenTear).Yield(), 0.025, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Jade).Yield(), 0.025, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Aquamarine).Yield(), 0.025, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.EarthCrystal).Yield(), 0.025, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Amethyst).Yield(), 0.025, 1, 3);
            yield return new TreasureAvailability(NamespacedId.FromObjectIndex(ObjectsReference.Topaz).Yield(), 0.025, 1, 3);

            // Weapons
            var swords = new[] {ObjectsReference.NeptuneGlaive, ObjectsReference.BrokenTrident};
            yield return new TreasureAvailability(swords.Select(NamespacedId.FromSwordIndex), 0.001, allowDuplicates: false);

            // Boots
            yield return new TreasureAvailability(Enumerable.Range(504, 10).Select(NamespacedId.FromBootsIndex), 0.005, allowDuplicates: false); // Boots

            // Rings
            var rings = Enumerable.Range(516, 4).Concat(Enumerable.Range(529, 6)).Append(ObjectsReference.IridiumBand);
            yield return new TreasureAvailability(rings.Select(NamespacedId.FromRingIndex), 0.005, allowDuplicates: false); // Rings
        }
    }
}