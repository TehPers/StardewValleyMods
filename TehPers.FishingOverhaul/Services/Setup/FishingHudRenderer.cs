using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
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

        private readonly Texture2D whitePixel;

        private readonly Texture2D mask;
        private readonly RenderTarget2D lightBuffer;

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
            this.whitePixel = new(Game1.graphics.GraphicsDevice, 1, 1);
            this.whitePixel.SetData(new[] { Color.White });

            this.mask = this.helper.Content.Load<Texture2D>("assets/mask.png");
            this.lightBuffer = new(
                Game1.graphics.GraphicsDevice,
                Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24
            );
        }

        public void Setup()
        {
            this.helper.Events.Display.RenderedHud += this.RenderFishingHud;
            this.helper.Events.Display.RenderedWorld += this.RenderMask;
        }

        public void Dispose()
        {
            this.helper.Events.Display.RenderedHud -= this.RenderFishingHud;
            this.helper.Events.Display.RenderedWorld -= this.RenderMask;
        }

        private void RenderMask(object? sender, RenderedWorldEventArgs e)
        {
            // Draw the lighting
            e.SpriteBatch.End();
            var previousTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();
            Game1.graphics.GraphicsDevice.SetRenderTarget(this.lightBuffer);
            Game1.graphics.GraphicsDevice.Clear(new(new Vector4(0.1f, 0.1f, 0.1f, 1.0f)));
            e.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            var centerX = this.lightBuffer.Width / 2;
            var centerY = this.lightBuffer.Height / 2;
            e.SpriteBatch.Draw(this.mask, new Rectangle(centerX - 300, centerY - 300, 600, 600), Color.White);
            e.SpriteBatch.Draw(this.mask, new Rectangle(100, 300, 200, 200), Color.Blue);
            e.SpriteBatch.Draw(this.mask, new Rectangle(400, 0, 300, 300), Color.Red);
            e.SpriteBatch.Draw(this.mask, new Rectangle(750, 300, 500, 500), Color.Green);

            // Draw the alpha mask
            e.SpriteBatch.End();
            Game1.graphics.GraphicsDevice.SetRenderTargets(previousTargets);
            var blendState = new BlendState
            {
                AlphaBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero,
            };
            e.SpriteBatch.Begin(SpriteSortMode.Immediate, blendState);
            e.SpriteBatch.Draw(this.lightBuffer, Vector2.Zero, Color.White);

            // Reset sprite batch
            e.SpriteBatch.End();
            e.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise
            );
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

            // Draw the fishing GUI to the screen
            var textColor = Color.White;
            var font = Game1.smallFont;
            var boxWidth = 0f;
            var lineHeight = (float)font.LineSpacing;
            var boxTopLeft = new Vector2(this.hudConfig.TopLeftX, this.hudConfig.TopLeftY);
            var boxBottomLeft = boxTopLeft;
            var fishChances = this.fishingApi
                .GetFishChances(this.fishingApi.CreateDefaultFishingInfo(farmer))
                .ToWeighted(value => value.Weight, value => value.Value.FishKey)
                .Condense()
                .Normalize()
                .OrderByDescending(fishChance => fishChance.Weight)
                .ToArray();

            // Setup the sprite batch
            e.SpriteBatch.End();
            e.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw streak chances
            var streakText = this.helper.Translation.Get(
                "text.streak",
                new { streak = this.fishingApi.GetStreak(farmer) }
            );
            e.SpriteBatch.DrawStringWithShadow(font, streakText, boxBottomLeft, textColor, 1F);
            boxWidth = Math.Max(boxWidth, font.MeasureString(streakText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw treasure chances
            var treasureChance = this.fishingApi.GetChanceForTreasure(farmer);
            var treasureText = this.helper.Translation.Get(
                "text.treasure",
                new { chance = $"{treasureChance:P2}" }
            );
            e.SpriteBatch.DrawStringWithShadow(font, treasureText, boxBottomLeft, textColor, 1F);
            boxWidth = Math.Max(boxWidth, font.MeasureString(treasureText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw trash chances
            var trashChance = fishChances.Length == 0
                ? 1.0
                : 1.0 - this.fishingApi.GetChanceForFish(farmer);
            var trashText = this.helper.Translation.Get(
                "text.trash",
                new { chance = $"{trashChance:P2}" }
            );
            e.SpriteBatch.DrawStringWithShadow(font, trashText, boxBottomLeft, textColor, 1F);
            boxWidth = Math.Max(boxWidth, font.MeasureString(trashText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw fish chances
            var maxDisplayedFish = this.hudConfig.MaxFishTypes;
            foreach (var fishChance in fishChances.Take(maxDisplayedFish))
            {
                var fishKey = fishChance.Value;
                var chance = fishChance.Weight;

                // Draw fish icon
                var lineWidth = 0f;
                var fishName = this.helper.Translation.Get(
                        "text.fish.unknownName",
                        new { key = fishKey.ToString() }
                    )
                    .ToString();
                if (this.namespaceRegistry.TryGetItemFactory(fishKey, out var factory))
                {
                    var fishItem = factory.Create();
                    fishName = fishItem.DisplayName;

                    const float iconScale = 0.5f;
                    const float iconSize = 64f * iconScale;
                    fishItem.DrawInMenuCorrected(
                        e.SpriteBatch,
                        boxBottomLeft,
                        iconScale,
                        1F,
                        0.9F,
                        StackDrawType.Hide,
                        Color.White,
                        false,
                        new TopLeftDrawOrigin()
                    );

                    lineWidth += iconSize;
                    lineHeight = Math.Max(lineHeight, iconSize);
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
                e.SpriteBatch.DrawStringWithShadow(
                    font,
                    fishText,
                    boxBottomLeft + new Vector2(lineWidth, 0),
                    textColor,
                    1F
                );
                var (textWidth, textHeight) = font.MeasureString(fishText);
                lineWidth += textWidth;
                lineHeight = Math.Max(lineHeight, textHeight);

                // Update background box
                boxWidth = Math.Max(boxWidth, lineWidth);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            // Draw 'more fish' text
            if (fishChances.Length > maxDisplayedFish)
            {
                var moreFishText = this.helper.Translation.Get(
                        "text.fish.more",
                        new { quantity = fishChances.Length - maxDisplayedFish }
                    )
                    .ToString();
                e.SpriteBatch.DrawStringWithShadow(
                    font,
                    moreFishText,
                    boxBottomLeft,
                    textColor,
                    1F
                );
                boxWidth = Math.Max(boxWidth, font.MeasureString(moreFishText).X);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            // Draw the background rectangle
            // TODO: use a nicer background
            e.SpriteBatch.Draw(
                this.whitePixel,
                new((int)boxTopLeft.X, (int)boxTopLeft.Y, (int)boxWidth, (int)boxBottomLeft.Y),
                null,
                new(0, 0, 0, 0.25F),
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.85F
            );
        }
    }
}