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
        private string text = "Click me!";
        private int count = 0;

        protected override IGuiComponent CreateRoot()
        {
            return VerticalLayout.Build(
                    builder =>
                    {
                        builder.Add(
                            new Label(this.text, Game1.smallFont).Aligned(
                                    HorizontalAlignment.Center
                                )
                                .OnClick(
                                    clickType =>
                                    {
                                        this.text += $" <{clickType}>";
                                        this.count += 1;
                                    }
                                )
                        );
                        builder.Horizontal(
                            row =>
                            {
                                row.Add(new Label("You have clicked ", Game1.smallFont));
                                row.Add(
                                    new Label(
                                        this.count.ToString("G"),
                                        Game1.smallFont,
                                        color: Color.DarkGreen
                                    )
                                );
                                row.Add(new Label(" times!", Game1.smallFont));
                            }
                        );
                    },
                    HorizontalAlignment.Center
                )
                .Aligned(HorizontalAlignment.Center, VerticalAlignment.Center)
                .WithPadding(64)
                .WithBackground(new MenuBackground());
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

                Game1.activeClickableMenu = new TestMenu();
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
