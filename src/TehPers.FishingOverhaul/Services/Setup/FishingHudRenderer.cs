using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;
using TehPers.Core.Api.Gui;
using TehPers.Core.Api.Gui.Layouts;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Extensions;
using TehPers.FishingOverhaul.Extensions.Drawing;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal sealed class FishingHudRenderer : ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly IFishingApi fishingApi;
        private readonly HudConfig hudConfig;
        private readonly INamespaceRegistry namespaceRegistry;

        public FishingHudRenderer(
            IModHelper helper,
            IFishingApi fishingApi,
            HudConfig hudConfig,
            INamespaceRegistry namespaceRegistry
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
            this.hudConfig = hudConfig ?? throw new ArgumentNullException(nameof(hudConfig));
            this.namespaceRegistry = namespaceRegistry;
        }

        public void Setup()
        {
            this.helper.Events.Display.RenderedHud += this.RenderFishingHud;

            // TODO: remove this
            this.helper.Events.Input.ButtonPressed += (sender, args) =>
            {
                if (args.Button != SButton.Y)
                {
                    return;
                }

                var component = VerticalLayout.Of(
                        new Label("Hello, world!", Game1.smallFont) {Color = Color.Black}
                            .Aligned(HorizontalAlignment.Center)
                            .WithPadding(32, 32, 32, 0)
                            .Wrapped(),
                        HorizontalLayout.Of(
                                new Label("This is some text", Game1.smallFont)
                                        {
                                            Color = Color.Black
                                        }
                                    .Aligned(HorizontalAlignment.Center, VerticalAlignment.Center)
                                    .WithPadding(32, 0)
                                    .Wrapped(),
                                new MenuVerticalSeparator(MenuSeparatorConnector.Separator)
                                    .Wrapped(),
                                new Label("This is some more text", Game1.smallFont)
                                        {
                                            Color = Color.Black
                                        }
                                    .Aligned(HorizontalAlignment.Center, VerticalAlignment.Center)
                                    .WithPadding(32, 0)
                                    .Wrapped()
                            )
                            .WithBackground(
                                VerticalLayout.Of(
                                    new MenuHorizontalSeparator().Wrapped(),
                                    new EmptySpace().Wrapped(),
                                    new MenuHorizontalSeparator().Wrapped()
                                )
                            )
                            .Wrapped(),
                        new Label(
                                "This is some text inside of an inner menu I guess...",
                                Game1.smallFont
                            ) {Color = Color.Black}
                            .Aligned(HorizontalAlignment.Center, VerticalAlignment.Center)
                            .WithPadding(32)
                            .WithBackground(new MenuBackground())
                            .WithPadding(32)
                            .Wrapped()
                    )
                    .WithBackground(new MenuBackground());
                Game1.activeClickableMenu = component.ToMenu();
            };
        }

        public void Dispose()
        {
            this.helper.Events.Display.RenderedHud -= this.RenderFishingHud;
        }

        private void RenderFishingHud(object? sender, RenderedHudEventArgs e)
        {
            var c = VerticalLayout.Build(
                builder =>
                {
                    builder.Add(new EmptySpace());
                    builder.Add(
                        HorizontalLayout.Build(
                                builder =>
                                {
                                    builder.Add(
                                        new Label("start", Game1.smallFont) {Color = Color.Black}
                                            .WithPadding(96)
                                    );
                                    builder.Add(
                                        new MenuVerticalSeparator(MenuSeparatorConnector.MenuBorder)
                                    );
                                    builder.Add(
                                        new Label("item 1", Game1.smallFont) {Color = Color.Black}
                                            .WithPadding(64)
                                            .Aligned(VerticalAlignment.Center)
                                    );
                                    builder.Add(
                                        new Label("item 2", Game1.smallFont) {Color = Color.Black}
                                            .WithPadding(64)
                                            .Aligned(VerticalAlignment.Center)
                                    );
                                    builder.Add(
                                        new Label("item 3", Game1.smallFont) {Color = Color.Black}
                                            .WithPadding(64)
                                            .Aligned(VerticalAlignment.Center)
                                    );
                                    builder.Add(
                                        new StretchedTexture(Game1.objectSpriteSheet)
                                            {
                                                MinScale = new(0, 0)
                                            }.WithPadding(64)
                                            .Aligned(VerticalAlignment.Center)
                                    );
                                    builder.Add(new EmptySpace());
                                }
                            )
                            .WithBackground(new MenuBackground())
                    );
                }
            );
            c.Draw(
                e.SpriteBatch,
                c.Initialize(new(0, 0, Game1.viewport.Width, Game1.viewport.Height))
            );

            // Check if HUD should be rendered
            var farmer = Game1.player;
            if (!this.hudConfig.ShowFishingHud
                || Game1.eventUp
                || farmer.CurrentTool is not FishingRod)
            {
                return;
            }

            // Draw the fishing GUI to the screen
            var normalTextColor = Color.Black;
            var fishTextColor = Color.Black;
            var trashTextColor = Color.Gray;
            var font = Game1.smallFont;
            var fishingInfo = this.fishingApi.CreateDefaultFishingInfo(farmer);
            var fishChances = this.fishingApi.GetFishChances(fishingInfo)
                .ToWeighted(value => value.Weight, value => value.Value.FishKey)
                .Condense()
                .OrderByDescending(x => x.Weight)
                .ToList();
            var trashChances = this.fishingApi.GetTrashChances(fishingInfo)
                .ToWeighted(value => value.Weight, value => value.Value.ItemKey)
                .Condense()
                .OrderByDescending(x => x.Weight)
                .ToList();
            var treasureChance = this.fishingApi.GetChanceForTreasure(fishingInfo);
            var chanceForFish = this.fishingApi.GetChanceForFish(fishingInfo);
            var trashChance = fishChances.Count == 0 ? 1.0 : 1.0 - chanceForFish;

            // Setup the sprite batch
            var content = VerticalLayout.Build(
                builder =>
                {
                    var mapped = builder.Select(c => c.Aligned(HorizontalAlignment.Left));
                    mapped.Add(new Label("Hello, world!", Game1.smallFont) {Color = Color.Black});

                    // Build header
                    builder.VerticalLayout(
                        header =>
                        {
                            // Draw streak chances
                            var streakText = this.helper.Translation.Get(
                                "text.streak",
                                new {streak = this.fishingApi.GetStreak(farmer)}
                            );
                            header.Add(
                                new Label(streakText, font) {Color = normalTextColor}
                                    .Aligned(HorizontalAlignment.Left)
                                    .Wrapped()
                            );

                            // Draw treasure chances
                            var treasureText = this.helper.Translation.Get(
                                "text.treasure",
                                new {chance = $"{treasureChance:P2}"}
                            );
                            header.Add(
                                new Label(treasureText, font) {Color = normalTextColor}
                                    .Aligned(HorizontalAlignment.Left)
                                    .Wrapped()
                            );

                            // Draw trash chances
                            var trashText = this.helper.Translation.Get(
                                "text.trash",
                                new {chance = $"{trashChance:P2}"}
                            );
                            header.Add(
                                new Label(trashText, font) {Color = normalTextColor}
                                    .Aligned(HorizontalAlignment.Left)
                                    .Wrapped()
                            );
                        }
                    );

                    // Separator
                    builder.Add(new MenuHorizontalSeparator());

                    // Entries
                    builder.VerticalLayout(
                        content =>
                        {
                            // Draw entries
                            var maxDisplayedFish = this.hudConfig.MaxFishTypes;
                            var displayedEntries = fishChances.ToWeighted(
                                x => x.Weight,
                                x => (entry: x.Value, textColor: fishTextColor)
                            );
                            if (this.hudConfig.ShowTrash)
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
                                .OrderByDescending(x => x.Weight);

                            foreach (var displayedEntry in displayedEntries.Take(maxDisplayedFish))
                            {
                                var (entryKey, textColor) = displayedEntry.Value;
                                var chance = displayedEntry.Weight;

                                // Draw fish icon
                                var itemRow = new List<WrappedComponent>();
                                var fishName = this.helper.Translation.Get(
                                        "text.fish.unknownName",
                                        new {key = entryKey.ToString()}
                                    )
                                    .ToString();
                                if (this.namespaceRegistry.TryGetItemFactory(
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
                                                MinSize = new(iconSize, iconSize),
                                                MaxSize = new(iconSize, iconSize)
                                            },
                                            (batch, bounds) => fishItem.DrawInMenuCorrected(
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
                                        ).Wrapped()
                                    );
                                }

                                // Draw chance
                                var fishText = this.helper.Translation.Get(
                                    "text.fish",
                                    new
                                    {
                                        name = fishName,
                                        chance = $"{chance * 100.0:F2}"
                                    }
                                );
                                itemRow.Add(
                                    new Label(fishText, font) {Color = textColor}.Aligned(
                                            HorizontalAlignment.Left,
                                            VerticalAlignment.Center
                                        )
                                        .Wrapped()
                                );
                                content.Add(HorizontalLayout.Of(itemRow).Wrapped());
                            }

                            // Draw 'more fish' text
                            if (fishChances.Count > maxDisplayedFish)
                            {
                                var moreFishText = this.helper.Translation.Get(
                                        "text.fish.more",
                                        new {quantity = fishChances.Count - maxDisplayedFish}
                                    )
                                    .ToString();
                                content.Add(
                                    new Label(moreFishText, font) {Color = normalTextColor}
                                        .Aligned(HorizontalAlignment.Left)
                                        .Wrapped()
                                );
                            }
                        }
                    );
                }
            );

            // Draw the component
            var component = content.WithBackground(new MenuBackground());
            var constraints = component.GetConstraints();
            var state = component.Initialize(
                new(
                    this.hudConfig.TopLeftX,
                    this.hudConfig.TopLeftY,
                    (int)Math.Ceiling(constraints.MinSize.Width),
                    (int)Math.Ceiling(constraints.MinSize.Height)
                )
            );
            component.Draw(e.SpriteBatch, state);
        }
    }
}
