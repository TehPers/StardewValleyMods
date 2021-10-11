using System;
using System.Linq;
using ContentPatcher;
using StardewModdingAPI;
using TehPers.Core.Api.Extensions;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class ChanceCalculator<T>
        where T : AvailabilityInfo
    {
        private readonly T availabilityInfo;
        private readonly IManagedConditions? managedConditions;

        public ChanceCalculator(
            IMonitor monitor,
            IContentPatcherAPI contentPatcherApi,
            IManifest fishingManifest,
            IManifest owner,
            T availabilityInfo
        )
        {
            _ = monitor ?? throw new ArgumentNullException(nameof(monitor));
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

                if (!this.managedConditions.IsValid)
                {
                    monitor.Log(
                        $"Validation error in CP conditions: {this.managedConditions.ValidationError}",
                        LogLevel.Error
                    );
                }
            }
        }

        public double? GetWeightedChance(FishingInfo fishingInfo)
        {
            return this.availabilityInfo.GetWeightedChance(fishingInfo)
                .Where(
                    _ =>
                    {
                        if (this.managedConditions is not { } conditions)
                        {
                            return true;
                        }

                        conditions.UpdateContext();
                        return conditions.IsMatch;
                    }
                );
        }
    }
}