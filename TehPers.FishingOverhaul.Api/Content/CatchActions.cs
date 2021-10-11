using System.Collections.Immutable;
using System.ComponentModel;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Actions to be executed on catch
    /// </summary>
    [JsonDescribe]
    public record CatchActions
    {
        [Description(
            "Raise custom events with this name to notify SMAPI mods that this was caught. Event "
            + "key format is '<namespace>:<key>' (for example "
            + "'TehPers.FishingOverhaul:GoldenWalnut')."
        )]
        public ImmutableArray<NamespacedKey> CustomEvents { get; init; } =
            ImmutableArray<NamespacedKey>.Empty;

        [Description("Sets one or more mail flags.")]
        public ImmutableArray<string> SetFlags { get; init; } = ImmutableArray<string>.Empty;

        [Description("Sets one or more quests as active.")]
        public ImmutableArray<int> StartQuests { get; init; } = ImmutableArray<int>.Empty;

        public void OnCatch(IFishingApi fishingApi, CatchInfo catchInfo)
        {
            // Custom events
            foreach (var customEvent in this.CustomEvents)
            {
                fishingApi.RaiseCustomEvent(new(catchInfo, customEvent));
            }

            // Mail flags
            foreach (var flag in this.SetFlags)
            {
                catchInfo.FishingInfo.User.mailReceived.Add(flag);
            }

            // Quests
            foreach (var questId in this.StartQuests)
            {
                catchInfo.FishingInfo.User.addQuest(questId);
            }
        }
    }
}