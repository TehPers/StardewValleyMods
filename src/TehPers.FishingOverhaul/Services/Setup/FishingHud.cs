using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Linq;
using TehPers.Core.Api.Gui;
using TehPers.Core.Api.Gui.Layouts;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Extensions;
using TehPers.FishingOverhaul.Extensions.Drawing;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class FishingHud : WrapperComponent
    {
        public FishingHud(
            IFishingApi fishingApi,
            IModHelper helper,
            HudConfig hudConfig,
            INamespaceRegistry namespaceRegistry,
            Farmer farmer
        )
            : base(
                FishingHud.CreateComponent(fishingApi, helper, hudConfig, namespaceRegistry, farmer)
            )
        {
        }

        private static IGuiComponent CreateComponent(
            IFishingApi fishingApi,
            IModHelper helper,
            HudConfig hudConfig,
            INamespaceRegistry namespaceRegistry,
            Farmer farmer
        )
        {
            var normalTextColor = Color.Black;
            var fishTextColor = Color.Black;
            var trashTextColor = Color.Gray;
            var font = Game1.smallFont;
            var fishingInfo = fishingApi.CreateDefaultFishingInfo(farmer);
            var fishChances = fishingApi.GetFishChances(fishingInfo)
                .ToWeighted(value => value.Weight, value => value.Value.FishKey)
                .Condense()
                .OrderByDescending(x => x.Weight)
                .ToList();
            var trashChances = fishingApi.GetTrashChances(fishingInfo)
                .ToWeighted(value => value.Weight, value => value.Value.ItemKey)
                .Condense()
                .OrderByDescending(x => x.Weight)
                .ToList();
            var treasureChance = fishingApi.GetChanceForTreasure(fishingInfo);
            var chanceForFish = fishingApi.GetChanceForFish(fishingInfo);
            var trashChance = fishChances.Count == 0 ? 1.0 : 1.0 - chanceForFish;

            // Get displayed entries
            var maxDisplayedFish = hudConfig.MaxFishTypes;
            var displayedEntries = fishChances.ToWeighted(
                x => x.Weight,
                x => (entry: x.Value, textColor: fishTextColor)
            );
            if (hudConfig.ShowTrash)
            {
                displayedEntries = displayedEntries.Normalize(chanceForFish)
                    .Concat(
                        trashChances.ToWeighted(
                                x => x.Weight,
                                x => (entry: x.Value, textColor: trashTextColor)
                            )
                            .Normalize(1 - chanceForFish)
                    );
            }

            displayedEntries = displayedEntries.Normalize()
                .Where(x => x.Weight > 0d)
                .OrderByDescending(x => x.Weight)
                .Take(maxDisplayedFish)
                .ToList();

            // Setup the sprite batch
            var component = VerticalLayout.Build(
                builder =>
                {
                    // Build header
                    builder.Add(
                        VerticalLayout.Build(
                                header =>
                                {
                                    // Draw streak chances
                                    var streakText = helper.Translation.Get(
                                        "text.streak",
                                        new {streak = fishingApi.GetStreak(farmer)}
                                    );
                                    header.Add(
                                        new Label(streakText, font) {Color = normalTextColor}
                                            .Aligned(
                                                HorizontalAlignment.Left
                                            )
                                    );

                                    // Draw treasure chances
                                    var treasureText = helper.Translation.Get(
                                        "text.treasure",
                                        new {chance = $"{treasureChance:P2}"}
                                    );
                                    header.Add(
                                        new Label(treasureText, font) {Color = normalTextColor}
                                            .Aligned(
                                                HorizontalAlignment.Left
                                            )
                                    );

                                    // Draw trash chances
                                    var trashText = helper.Translation.Get(
                                        "text.trash",
                                        new {chance = $"{trashChance:P2}"}
                                    );
                                    header.Add(
                                        new Label(trashText, font) {Color = normalTextColor}
                                            .Aligned(
                                                HorizontalAlignment.Left
                                            )
                                    );
                                }
                            )
                            .WithPadding(64, 64, 64, 0)
                    );

                    if (displayedEntries.Any())
                    {
                        // Separator
                        builder.Add(new MenuHorizontalSeparator());

                        // Entries
                        builder.Add(
                            VerticalLayout.Build(
                                    content =>
                                    {
                                        // Draw entries
                                        foreach (var displayedEntry in displayedEntries)
                                        {
                                            var (entryKey, textColor) = displayedEntry.Value;
                                            var chance = displayedEntry.Weight;

                                            // Draw fish icon
                                            content.Horizontal(
                                                itemRow =>
                                                {
                                                    var fishName = helper.Translation.Get(
                                                            "text.fish.unknownName",
                                                            new {key = entryKey.ToString()}
                                                        )
                                                        .ToString();
                                                    if (namespaceRegistry.TryGetItemFactory(
                                                            entryKey,
                                                            out var factory
                                                        ))
                                                    {
                                                        var fishItem = factory.Create();
                                                        fishName = fishItem.DisplayName;

                                                        const float iconScale = 0.5f;
                                                        const float iconSize = 64f * iconScale;
                                                        itemRow.Add(
                                                            new SimpleComponent(
                                                                new()
                                                                {
                                                                    MinSize = new(
                                                                        iconSize,
                                                                        iconSize
                                                                    ),
                                                                    MaxSize = new(
                                                                        iconSize,
                                                                        iconSize
                                                                    )
                                                                },
                                                                (batch, bounds) =>
                                                                    fishItem.DrawInMenuCorrected(
                                                                        batch,
                                                                        new(bounds.X, bounds.Y),
                                                                        iconScale,
                                                                        1F,
                                                                        0.9F,
                                                                        StackDrawType.Hide,
                                                                        Color.White,
                                                                        false,
                                                                        new TopLeftDrawOrigin()
                                                                    )
                                                            )
                                                        );
                                                    }

                                                    // Draw chance
                                                    var fishText = helper.Translation.Get(
                                                        "text.fish",
                                                        new
                                                        {
                                                            name = fishName,
                                                            chance = $"{chance * 100.0:F2}"
                                                        }
                                                    );
                                                    itemRow.Add(
                                                        new Label(fishText, font)
                                                                {
                                                                    Color = textColor
                                                                }
                                                            .Aligned(
                                                                HorizontalAlignment.Left,
                                                                VerticalAlignment.Center
                                                            )
                                                    );
                                                }
                                            );
                                        }

                                        // Draw 'more fish' text
                                        if (fishChances.Count > maxDisplayedFish)
                                        {
                                            var moreFishText = helper.Translation.Get(
                                                    "text.fish.more",
                                                    new
                                                        {
                                                            quantity = fishChances.Count
                                                                - maxDisplayedFish
                                                        }
                                                )
                                                .ToString();
                                            content.Add(
                                                new Label(moreFishText, font)
                                                        {
                                                            Color = normalTextColor
                                                        }
                                                    .Aligned(HorizontalAlignment.Left)
                                            );
                                        }
                                    }
                                )
                                .WithPadding(64, 64, 0, 64)
                        );
                    }
                }
            );

            return component.WithBackground(new MenuBackground());
        }
    }
}
