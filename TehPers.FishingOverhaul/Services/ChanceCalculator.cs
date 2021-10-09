using System;
using System.Collections.Generic;
using System.Linq;
using ContentPatcher;
using StardewModdingAPI;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Gameplay;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services
{
    internal class ChanceCalculator<T>
        where T : AvailabilityInfo
    {
        private readonly T availabilityInfo;
        private readonly IManagedConditions? managedConditions;

        public ChanceCalculator(
            IContentPatcherAPI contentPatcherApi,
            IManifest fishingManifest,
            IManifest owner,
            T availabilityInfo
        )
        {
            _ = contentPatcherApi ?? throw new ArgumentNullException(nameof(contentPatcherApi));
            _ = owner ?? throw new ArgumentNullException(nameof(owner));
            this.availabilityInfo = availabilityInfo
                ?? throw new ArgumentNullException(nameof(availabilityInfo));

            var version =
                fishingManifest.Dependencies.FirstOrDefault(
                        dependency => dependency.UniqueID == "Pathoschild.ContentPatcher"
                    )
                    ?.MinimumVersion
                ?? throw new ArgumentException(
                    "Fishing overhaul does not depend on Content Patcher",
                    nameof(fishingManifest)
                );

            if (availabilityInfo.When.Any())
            {
                this.managedConditions = contentPatcherApi.ParseConditions(
                    owner,
                    availabilityInfo.When,
                    version,
                    new[] { fishingManifest.UniqueID }
                );
            }
        }

        public double? GetWeightedChance(
            int time,
            Seasons seasons,
            Weathers weathers,
            int fishingLevel,
            IEnumerable<string> locations,
            WaterTypes waterTypes = WaterTypes.All,
            int depth = 4
        )
        {
            return this.availabilityInfo.GetWeightedChance(
                    time,
                    seasons,
                    weathers,
                    fishingLevel,
                    locations,
                    waterTypes,
                    depth
                )
                .Where(
                    _ =>
                    {
                        if (this.managedConditions is not { } conditions)
                        {
                            return true;
                        }

                        if (conditions.IsMutable)
                        {
                            conditions.UpdateContext();
                        }

                        return conditions.IsMatch;
                    }
                );
        }
    }
}