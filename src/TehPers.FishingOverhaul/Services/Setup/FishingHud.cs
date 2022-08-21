using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Linq;
using TehPers.Core.Api.Items;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Config;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class FishingHud : IGuiComponent
    {
        private readonly IGuiComponent inner;

        public IGuiBuilder GuiBuilder { get; }

        public FishingHud(
            IFishingApi fishingApi,
            IModHelper helper,
            HudConfig hudConfig,
            INamespaceRegistry namespaceRegistry,
            IGuiBuilder guiBuilder,
            Farmer farmer
        )
        {
            this.GuiBuilder = guiBuilder;
            this.inner = this.CreateComponent(
                fishingApi,
                helper,
                hudConfig,
                namespaceRegistry,
                farmer
            );
        }

        private IGuiComponent CreateComponent(
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
            var component = this.GuiBuilder.VerticalLayout(
                builder =>
                {
                    builder = builder.Aligned(horizontal: HorizontalAlignment.Left);

                    // Build header
                    this.GuiBuilder.VerticalLayout(
                            header =>
                            {
                                header = header.Aligned(horizontal: HorizontalAlignment.Left);

                                // Draw streak chances
                                var streakText = helper.Translation.Get(
                                    "text.streak",
                                    new {streak = fishingApi.GetStreak(farmer)}
                                );
                                this.GuiBuilder.Label(streakText)
                                    .WithFont(font)
                                    .WithColor(normalTextColor)
                                    .Aligned(HorizontalAlignment.Left)
                                    .AddTo(header);

                                // Draw treasure chances
                                var treasureText = helper.Translation.Get(
                                    "text.treasure",
                                    new {chance = $"{treasureChance:P2}"}
                                );

                                this.GuiBuilder.Label(treasureText)
                                    .WithFont(font)
                                    .WithColor(normalTextColor)
                                    .Aligned(HorizontalAlignment.Left)
                                    .AddTo(header);

                                // Draw trash chances
                                var trashText = helper.Translation.Get(
                                    "text.trash",
                                    new {chance = $"{trashChance:P2}"}
                                );
                                this.GuiBuilder.Label(trashText)
                                    .WithFont(font)
                                    .WithColor(normalTextColor)
                                    .Aligned(HorizontalAlignment.Left)
                                    .AddTo(header);
                            }
                        )
                        .WithPadding(64, 64, 64, 0)
                        .AddTo(builder);

                    if (displayedEntries.Any())
                    {
                        // Separator
                        this.GuiBuilder.MenuHorizontalSeparator().AddTo(builder);

                        // Entries
                        this.GuiBuilder.VerticalLayout(
                                content =>
                                {
                                    content = content.Aligned(horizontal: HorizontalAlignment.Left);

                                    // Draw entries
                                    foreach (var displayedEntry in displayedEntries)
                                    {
                                        var (entryKey, textColor) = displayedEntry.Value;
                                        var chance = displayedEntry.Weight;

                                        // Draw fish icon
                                        this.GuiBuilder.HorizontalLayout(
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

                                                        this.GuiBuilder.ItemView(fishItem)
                                                            .WithSideLength(32f)
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
                                                    this.GuiBuilder.Label(fishText)
                                                        .WithFont(font)
                                                        .WithColor(textColor)
                                                        .Aligned(
                                                            HorizontalAlignment.Left,
                                                            VerticalAlignment.Center
                                                        )
                                                        .AddTo(itemRow);
                                                }
                                            )
                                            .AddTo(content);
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
                                        this.GuiBuilder.Label(moreFishText)
                                            .WithFont(font)
                                            .WithColor(normalTextColor)
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

            return component.WithBackground(this.GuiBuilder.MenuBackground());
        }

        /// <inheritdoc />
        public IGuiConstraints GetConstraints()
        {
            return this.inner.GetConstraints();
        }

        /// <inheritdoc />
        public void Handle(IGuiEvent e, Rectangle bounds)
        {
            this.inner.Handle(e, bounds);
        }
    }
}
