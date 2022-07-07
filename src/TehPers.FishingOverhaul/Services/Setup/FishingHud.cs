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

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class FishingHud : IGuiComponent
    {
        private readonly IGuiComponent inner;

        public FishingHud(
            IFishingApi fishingApi,
            IModHelper helper,
            HudConfig hudConfig,
            INamespaceRegistry namespaceRegistry,
            Farmer farmer
        )
        {
            this.inner = FishingHud.CreateComponent(
                fishingApi,
                helper,
                hudConfig,
                namespaceRegistry,
                farmer
            );
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
            var component = GuiComponent.Vertical(
                HorizontalAlignment.Left,
                builder =>
                {
                    // Build header
                    GuiComponent.Vertical(
                            HorizontalAlignment.Left,
                            header =>
                            {
                                // Draw streak chances
                                var streakText = helper.Translation.Get(
                                    "text.streak",
                                    new {streak = fishingApi.GetStreak(farmer)}
                                );
                                GuiComponent.Label(streakText, font: font, color: normalTextColor)
                                    .Aligned(HorizontalAlignment.Left)
                                    .AddTo(header);

                                // Draw treasure chances
                                var treasureText = helper.Translation.Get(
                                    "text.treasure",
                                    new {chance = $"{treasureChance:P2}"}
                                );

                                GuiComponent.Label(treasureText, font: font, color: normalTextColor)
                                    .Aligned(HorizontalAlignment.Left)
                                    .AddTo(header);

                                // Draw trash chances
                                var trashText = helper.Translation.Get(
                                    "text.trash",
                                    new {chance = $"{trashChance:P2}"}
                                );
                                GuiComponent.Label(trashText, font: font, color: normalTextColor)
                                    .Aligned(HorizontalAlignment.Left)
                                    .AddTo(header);
                            }
                        )
                        .WithPadding(64, 64, 64, 0)
                        .AddTo(builder);

                    if (displayedEntries.Any())
                    {
                        // Separator
                        GuiComponent.MenuHorizontalSeparator().AddTo(builder);

                        // Entries
                        GuiComponent.Vertical(
                                HorizontalAlignment.Left,
                                content =>
                                {
                                    // Draw entries
                                    foreach (var displayedEntry in displayedEntries)
                                    {
                                        var (entryKey, textColor) = displayedEntry.Value;
                                        var chance = displayedEntry.Weight;

                                        // Draw fish icon
                                        GuiComponent.Horizontal(
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

                                                    GuiComponent.ItemView(fishItem, sideLength: 32f)
                                                        .AddTo(itemRow);
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
                                                GuiComponent
                                                    .Label(fishText, font: font, color: textColor)
                                                    .Aligned(
                                                        HorizontalAlignment.Left,
                                                        VerticalAlignment.Center
                                                    )
                                                    .AddTo(itemRow);
                                            }
                                        ).AddTo(content);
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
                                        GuiComponent.Label(
                                                moreFishText,
                                                font: font,
                                                normalTextColor
                                            )
                                            .Aligned(HorizontalAlignment.Left)
                                            .AddTo(content);
                                    }
                                }
                            )
                            .WithPadding(64, 64, 0, 64)
                            .AddTo(builder);
                    }
                }
            );

            return component.WithBackground(GuiComponent.MenuBackground());
        }

        public GuiConstraints GetConstraints()
        {
            return this.inner.GetConstraints();
        }

        public void Handle(GuiEvent e, Rectangle bounds)
        {
            this.inner.Handle(e, bounds);
        }
    }
}
