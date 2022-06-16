using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core.Api.Gui;
using TehPers.Core.Api.Gui.Layouts;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Config;

namespace TehPers.FishingOverhaul.Services.Setup
{
    // TODO: remove this
    internal class TestMenu : ManagedMenu
    {
        private string text = "Clicks: ";

        protected override IGuiComponent<Unit> CreateRoot()
        {
            return VerticalLayout.Build(
                    builder =>
                    {
                        // TODO: remove responses?
                        var button = new Button(
                            new Label("Click me!", Game1.smallFont) {Color = Color.Black}.Aligned(
                                HorizontalAlignment.Center
                            )
                        );
                        builder.Add(button);
                    }
                )
                .Aligned(HorizontalAlignment.Center, VerticalAlignment.Center)
                .WithBackground(new MenuBackground())
                .IgnoreResponse();
        }
    }

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

                var component = VerticalLayout.Build(
                        builder =>
                        {
                            builder.Add(
                                new Label("Hello, world!", Game1.smallFont) {Color = Color.Black}
                                    .Aligned(HorizontalAlignment.Center)
                                    .WithPadding(32, 32, 32, 0)
                            );
                            builder.Add(
                                HorizontalLayout.Build(
                                        builder =>
                                        {
                                            builder.Add(
                                                new Label("This is some text", Game1.smallFont)
                                                    {
                                                        Color = Color.Black
                                                    }.Aligned(
                                                        HorizontalAlignment.Center,
                                                        VerticalAlignment.Center
                                                    )
                                                    .WithPadding(32, 0)
                                            );
                                            builder.Add(
                                                new MenuVerticalSeparator(
                                                    MenuSeparatorConnector.Separator
                                                )
                                            );
                                            builder.Add(
                                                new Label("This is some more text", Game1.smallFont)
                                                    {
                                                        Color = Color.Black
                                                    }.Aligned(
                                                        HorizontalAlignment.Center,
                                                        VerticalAlignment.Center
                                                    )
                                                    .WithPadding(32, 0)
                                            );
                                        }
                                    )
                                    .WithBackground(
                                        VerticalLayout.Build(
                                            builder =>
                                            {
                                                builder.Add(new MenuHorizontalSeparator());
                                                builder.Add(new EmptySpace());
                                                builder.Add(new MenuHorizontalSeparator());
                                            }
                                        )
                                    )
                            );
                            builder.Add(
                                new Label(
                                        "This is some text inside of an inner menu I guess...",
                                        Game1.smallFont
                                    ) {Color = Color.Black}
                                    .Aligned(HorizontalAlignment.Center, VerticalAlignment.Center)
                                    .WithPadding(32)
                                    .WithBackground(new MenuBackground())
                                    .WithPadding(32)
                            );
                        }
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
            // Check if HUD should be rendered
            var farmer = Game1.player;
            if (!this.hudConfig.ShowFishingHud
                || Game1.eventUp
                || farmer.CurrentTool is not FishingRod)
            {
                return;
            }

            // Draw the fishing HUD
            var component = new FishingHud(
                this.fishingApi,
                this.helper,
                this.hudConfig,
                this.namespaceRegistry,
                farmer
            );
            var constraints = component.GetConstraints();
            component.Handle(
                new GuiEvent.Draw(e.SpriteBatch),
                new(
                    this.hudConfig.TopLeftX,
                    this.hudConfig.TopLeftY,
                    (int)Math.Ceiling(constraints.MinSize.Width),
                    (int)Math.Ceiling(constraints.MinSize.Height)
                )
            );
        }
    }
}
