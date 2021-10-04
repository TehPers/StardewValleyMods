using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;
using TehPers.FishingOverhaul.Services.Data;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class FishingTracker
    {
        public Dictionary<Farmer, FisherData> ActiveFisherData { get; } = new();

        public record FisherData(FishingRod Rod, FishingState State);

        public void Transition(Farmer user, FishingRod rod, Func<FishingState, FishingState> transition)
        {
            if (!this.ActiveFisherData.TryGetValue(user, out var data) || data.Rod != rod)
            {
                return;
            }

            this.ActiveFisherData[user] = data with { State = transition(data.State) };
        }

        public FisherData TransitionOr(
            Farmer user,
            FishingRod rod,
            Func<FishingState, FishingState> transition,
            Func<FishingState> @default
        )
        {
            var newData = this.ActiveFisherData.TryGetValue(user, out var data) switch
            {
                true when data!.Rod == rod => data with { State = transition(data.State) },
                _ => new(rod, @default())
            };

            this.ActiveFisherData[user] = newData;
            return newData;
        }
    }
}