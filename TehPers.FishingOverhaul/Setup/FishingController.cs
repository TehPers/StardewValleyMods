using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Api.Messages;
using TehPers.FishingOverhaul.Api.Weighted;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Extensions;
using TehPers.FishingOverhaul.Extensions.Drawing;
using TehPers.FishingOverhaul.Gui;

namespace TehPers.FishingOverhaul.Setup
{
    internal class FishingController : ISetup, IDisposable
    {
        private static readonly FieldInfo beginReelingEvent =
            typeof(FishingRod).GetField("beginReelingEvent", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new Exception($"Missing info for {nameof(FishingController.beginReelingEvent)}.");

        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly IFishingHelper fishingHelper;
        private readonly ICustomBobberBarFactory customBobberBarFactory;
        private readonly FishingTracker fishingTracker;
        private readonly FishingRodOverrider overrider;
        private readonly FishConfig fishConfig;

        public FishingController(
            IModHelper helper,
            IMonitor monitor,
            IFishingHelper fishingHelper,
            ICustomBobberBarFactory customBobberBarFactory,
            FishingTracker fishingTracker,
            FishingRodOverrider overrider,
            FishConfig fishConfig
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.fishingHelper = fishingHelper ?? throw new ArgumentNullException(nameof(fishingHelper));
            this.customBobberBarFactory = customBobberBarFactory
                ?? throw new ArgumentNullException(nameof(customBobberBarFactory));
            this.fishingTracker = fishingTracker ?? throw new ArgumentNullException(nameof(fishingTracker));
            this.overrider = overrider ?? throw new ArgumentNullException(nameof(overrider));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
        }

        public void Setup()
        {
            // SMAPI events
            this.helper.Events.GameLoop.UpdateTicking += this.PreUpdateFishingRods;
            this.helper.Events.GameLoop.UpdateTicked += this.PostUpdateFishingRods;
            this.helper.Events.Display.RenderedWorld += this.RenderFishing;
            this.helper.Events.Multiplayer.PeerDisconnected += this.ResetPeer;

            // Fishing events
            this.overrider.PullFish += this.PullFish;
        }

        public void Dispose()
        {
            // SMAPI events
            this.helper.Events.GameLoop.UpdateTicking -= this.PreUpdateFishingRods;
            this.helper.Events.GameLoop.UpdateTicked -= this.PostUpdateFishingRods;
            this.helper.Events.Display.RenderedWorld -= this.RenderFishing;
            this.helper.Events.Multiplayer.PeerDisconnected -= this.ResetPeer;

            // Fishing events
            this.overrider.PullFish -= this.PullFish;
        }

        private void PreUpdateFishingRods(object? sender, UpdateTickingEventArgs e)
        {
            foreach (var (user, (rod, state)) in this.fishingTracker.ActiveFisherData.ToList())
            {
                // Check if user is still fishing
                if (user.CurrentTool != rod)
                {
                    this.fishingTracker.ActiveFisherData.Remove(user);
                    continue;
                }

                // Update user's fishing state
                switch (state)
                {
                    case FishingState.CaughtFish(var fishKey, var fish, var fishSize, var isLegendary):
                    {
                        // Check if user is holding the fish now
                        if ((rod.bobber.Value.Equals(Vector2.Zero)
                                || !rod.isFishing && !rod.pullingOutOfWater && !rod.castedButBobberStillInAir
                                || user.FarmerSprite.CurrentFrame is 57
                                || user.FacingDirection is 0 && rod.pullingOutOfWater)
                            && rod.fishCaught)
                        {
                            // Disable the holding animation for the base fishing rod
                            rod.fishCaught = false;

                            // Transition state
                            this.fishingTracker.ActiveFisherData[user] = new FishingTracker.ActiveFisher(
                                rod,
                                new FishingState.HoldingFish(fishKey, fish, fishSize, isLegendary)
                            );
                        }

                        break;
                    }

                    case FishingState.HoldingFish(var fishKey, var fish, _, _):
                    {
                        if (!Game1.isFestival())
                        {
                            user.faceDirection(2);
                            user.FarmerSprite.setCurrentFrame(84);
                        }

                        if (Game1.random.NextDouble() < 0.025)
                        {
                            user.currentLocation.temporarySprites.Add(
                                new TemporaryAnimatedSprite(
                                    "LooseSprites\\Cursors",
                                    new Rectangle(653, 858, 1, 1),
                                    9999f,
                                    1,
                                    1,
                                    user.Position + new Vector2(Game1.random.Next(-3, 2) * 4, -32f),
                                    false,
                                    false,
                                    (float)(user.getStandingY() / 10000.0 + 1.0 / 500.0),
                                    0.04f,
                                    Color.LightBlue,
                                    5f,
                                    0.0f,
                                    0.0f,
                                    0.0f
                                )
                                {
                                    acceleration = new Vector2(0.0f, 0.25f)
                                }
                            );
                        }

                        // Give the user the fish they caught if they are the local player
                        if (!user.IsLocalPlayer
                            || Game1.input.GetMouseState().LeftButton != ButtonState.Pressed
                            && !Game1.didPlayerJustClickAtAll()
                            && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
                        {
                            continue;
                        }

                        // Create caught item
                        var caughtItem = fish;
                        if (caughtItem is StardewValley.Object caughtObj)
                        {
                            // Quest items
                            if (object.Equals(fishKey, NamespacedKey.SdvObject(GameLocation.CAROLINES_NECKLACE_ITEM)))
                            {
                                caughtObj.questItem.Value = true;
                            }
                            else if (object.Equals(fishKey, NamespacedKey.SdvObject(79))
                                || object.Equals(fishKey, NamespacedKey.SdvObject(842)))
                            {
                                caughtItem = user.currentLocation.tryToCreateUnseenSecretNote(user);
                                if (caughtItem == null)
                                {
                                    continue;
                                }
                            }
                        }

                        user.currentLocation.localSound("coin");
                        var fromFishPond = rod.fromFishPond;
                        if (!Game1.isFestival()
                            && !fromFishPond
                            // TODO: && this.itemCategory == "Object"
                            && Game1.player.team.specialOrders is { } specialOrders)
                        {
                            foreach (SpecialOrder specialOrder in specialOrders)
                            {
                                specialOrder.onFishCaught?.Invoke(Game1.player, caughtItem);
                            }
                        }

                        if (!rod.treasureCaught)
                        {
                            this.helper.Reflection.GetField<int>(rod, "recastTimerMs").SetValue(200);

                            user.completelyStopAnimatingOrDoingAction();
                            rod.doneFishing(user, !fromFishPond);

                            if (Game1.isFestival() || user.addItemToInventoryBool(caughtItem))
                            {
                                // Transition fishing state and prevent rod from being used this frame
                                this.fishingTracker.ActiveFisherData[user] =
                                    new FishingTracker.ActiveFisher(rod, new FishingState.DoneFishing());
                                rod.isFishing = true;

                                continue;
                            }

                            Game1.activeClickableMenu =
                                new ItemGrabMenu(new List<Item> { caughtItem }, this).setEssential(true);
                        }
                        else
                        {
                            // TODO: fix what happens when user's inv is too full for fish and caught treasure
                            rod.fishCaught = false;
                            rod.showingTreasure = true;
                            user.UsingTool = true;
                            var wasAddedToInventory = user.addItemToInventoryBool(caughtItem);
                            rod.animations.Add(
                                new TemporaryAnimatedSprite(
                                    "LooseSprites\\Cursors",
                                    new Rectangle(64, 1920, 32, 32),
                                    500f,
                                    1,
                                    0,
                                    user.Position + new Vector2(-32f, -160f),
                                    false,
                                    false,
                                    user.getStandingY() / 10000.0f + 1.0f / 1000.0f,
                                    0.0f,
                                    Color.White,
                                    4f,
                                    0.0f,
                                    0.0f,
                                    0.0f
                                )
                                {
                                    motion = new Vector2(0.0f, -0.128f),
                                    timeBasedMotion = true,
                                    endFunction = rod.openChestEndFunction,
                                    extraInfoForEndBehavior = wasAddedToInventory ? 0 : 1,
                                    alpha = 0.0f,
                                    alphaFade = -1f / 500f
                                }
                            );
                        }

                        // Transition fishing state and prevent rod from being used this frame
                        this.fishingTracker.ActiveFisherData[user] =
                            new FishingTracker.ActiveFisher(rod, new FishingState.DoneFishing());
                        rod.fishCaught = true;

                        break;
                    }
                }
            }
        }

        private void PostUpdateFishingRods(object? sender, UpdateTickedEventArgs e)
        {
            foreach (var (user, (rod, state)) in this.fishingTracker.ActiveFisherData.ToList())
            {
                // Check if user is still fishing
                if (user.CurrentTool != rod)
                {
                    this.fishingTracker.ActiveFisherData.Remove(user);
                    continue;
                }

                // Update user's fishing state
                switch (state)
                {
                    case FishingState.DoneFishing:
                    {
                        // Transition fishing state and allow rod to be used again
                        this.fishingTracker.ActiveFisherData[user] =
                            new FishingTracker.ActiveFisher(rod, new FishingState.NotFishing());
                        rod.isFishing = false;

                        break;
                    }
                }
            }
        }

        private void RenderFishing(object? sender, RenderedWorldEventArgs e)
        {
            // Render each fisher
            foreach (var (user, (rod, state)) in this.fishingTracker.ActiveFisherData)
            {
                switch (state)
                {
                    case FishingState.HoldingFish(_, var fish, var fishSize, var isLegendary):
                    {
                        var y = (float)(4.0
                            * Math.Round(
                                Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0),
                                2
                            ));

                        // Draw bubble
                        var layerDepth = user.getStandingY() / 10000.0f + 0.06f;
                        e.SpriteBatch.Draw(
                            Game1.mouseCursors,
                            Game1.GlobalToLocal(
                                Game1.viewport,
                                user.Position + new Vector2(-120f, y - 288f)
                            ),
                            new Rectangle(31, 1870, 73, 49),
                            Color.White * 0.8f,
                            0.0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            layerDepth
                        );

                        // Draw item in bubble
                        fish.DrawInMenuCorrected(
                            e.SpriteBatch,
                            Game1.GlobalToLocal(
                                Game1.viewport,
                                user.Position + new Vector2(-124f, y - 284f) + new Vector2(44f, 68f)
                            ),
                            1f,
                            1f,
                            layerDepth + 0.0001f,
                            StackDrawType.Draw,
                            Color.White,
                            false,
                            new TopLeftDrawOrigin()
                        );

                        // Draw item in hand
                        var count = fish is StardewValley.Object { Stack: var stack } ? stack : 1;
                        count = Math.Min(1, count);
                        foreach (var fishIndex in Enumerable.Range(0, count))
                        {
                            // TODO: some kind of jagged pattern with all the fish
                            // Maybe:
                            //  - X offset in range [-8, 8]
                            //  - Y offset in range [-8, 8]
                            var offset = new Vector2(0f, 0f);
                            fish.DrawInMenuCorrected(
                                e.SpriteBatch,
                                Game1.GlobalToLocal(
                                    Game1.viewport,
                                    user.Position + new Vector2(0.0f, -56f) + offset
                                ),
                                3f / 4f,
                                1f,
                                user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f,
                                StackDrawType.Hide,
                                Color.White,
                                false,
                                new CenterDrawOrigin()
                            );
                        }

                        // Draw fish name
                        e.SpriteBatch.DrawString(
                            Game1.smallFont,
                            fish.DisplayName,
                            Game1.GlobalToLocal(
                                Game1.viewport,
                                user.Position
                                + new Vector2(
                                    (float)(26.0 - Game1.smallFont.MeasureString(fish.DisplayName).X / 2.0),
                                    y - 278f
                                )
                            ),
                            isLegendary ? new Color(126, 61, 237) : Game1.textColor,
                            0.0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f
                        );

                        if (fishSize is not -1)
                        {
                            // Draw fish length label
                            e.SpriteBatch.DrawString(
                                Game1.smallFont,
                                Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14082"),
                                Game1.GlobalToLocal(
                                    Game1.viewport,
                                    user.Position + new Vector2(20f, y - 214f)
                                ),
                                Game1.textColor,
                                0.0f,
                                Vector2.Zero,
                                1f,
                                SpriteEffects.None,
                                user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f
                            );

                            // Draw fish length
                            e.SpriteBatch.DrawString(
                                Game1.smallFont,
                                Game1.content.LoadString(
                                    "Strings\\StringsFromCSFiles:FishingRod.cs.14083",
                                    LocalizedContentManager.CurrentLanguageCode
                                    != LocalizedContentManager.LanguageCode.en
                                        ? Math.Round(fishSize * 2.54)
                                        : fishSize
                                ),
                                Game1.GlobalToLocal(
                                    Game1.viewport,
                                    user.Position
                                    + new Vector2(
                                        (float)(85.0
                                            - Game1.smallFont.MeasureString(
                                                Game1.content.LoadString(
                                                    "Strings\\StringsFromCSFiles:FishingRod.cs.14083",
                                                    LocalizedContentManager.CurrentLanguageCode
                                                    != LocalizedContentManager.LanguageCode.en
                                                        ? Math.Round(fishSize * 2.54)
                                                        : fishSize
                                                )
                                            ).X
                                            / 2.0),
                                        y - 179f
                                    )
                                ),
                                rod.recordSize
                                    ? Color.Blue * Math.Min(1f, (float)(y / 8.0 + 1.5))
                                    : Game1.textColor,
                                0.0f,
                                Vector2.Zero,
                                1f,
                                SpriteEffects.None,
                                user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f
                            );
                        }

                        break;
                    }
                }
            }
        }

        private void ResetPeer(object? sender, PeerDisconnectedEventArgs e)
        {
            // Find user
            var user =
                this.fishingTracker.ActiveFisherData.Keys.FirstOrDefault(
                    farmer => farmer.UniqueMultiplayerID == e.Peer.PlayerID
                );

            // Stop tracking them
            if (user is not null)
            {
                this.fishingTracker.ActiveFisherData.Remove(user);
            }
        }

        private void PullFish(object? sender, FishingRodOverrider.PullFishEvent e)
        {
            // Update user's state
            this.fishingTracker.ActiveFisherData[e.User] =
                new FishingTracker.ActiveFisher(e.Rod, new FishingState.Fishing());

            // Get some information about the rod
            var bobberDepth = this.helper.Reflection.GetField<int>(e.Rod, "clearWaterDistance").GetValue();
            var location = e.User.currentLocation;

            NamespacedKey? fish = null;

            // Handle non-overridden legendary fish
            if (!this.fishConfig.ShouldOverrideVanillaLegendaries)
            {
                // Check if a legendary would be caught
                var baitValue = e.Rod.attachments[0]?.Price / 10.0 ?? 0.0;
                var bubblyZone = location.fishSplashPoint is { } splashPoint
                    && splashPoint.Value != Point.Zero
                    && new Rectangle(splashPoint.X * 64, splashPoint.Y * 64, 64, 64).Intersects(
                        new Rectangle((int)e.Rod.bobber.X - 80, (int)e.Rod.bobber.Y - 80, 64, 64)
                    );
                var bobberTile = new Vector2(e.Rod.bobber.X / 64f, e.Rod.bobber.Y / 64f);
                var normalFish = location.getFish(
                    e.Rod.fishingNibbleAccumulator,
                    e.Rod.attachments[0]?.ParentSheetIndex ?? -1,
                    bobberDepth + (bubblyZone ? 1 : 0),
                    e.User,
                    baitValue + (bubblyZone ? 0.4 : 0.0),
                    bobberTile
                );

                // If so, select that fish
                var normalFishKey = NamespacedKey.SdvObject(normalFish.ParentSheetIndex);
                if (this.fishingHelper.IsLegendary(normalFishKey))
                {
                    fish = normalFishKey;
                }
            }

            // TODO: handle special items (void mayonnaise, fish ponds, etc)

            // Choose a random fish if one hasn't been chosen yet
            var fishChance = this.fishingHelper.GetChanceForFish(e.User);
            fish ??= this.fishingHelper.GetFishChances(e.User, bobberDepth)
                .ToWeighted(val => val.Weight, val => (NamespacedKey?)val.Value)
                .Normalize(fishChance)
                .Append(new WeightedValue<NamespacedKey?>(null, 1 - fishChance))
                .ChooseOrDefault(Game1.random)
                ?.Value;

            // Start fishing minigame if a fish was chosen
            if (fish is { } fishKey)
            {
                this.helper.Multiplayer.SendMessage(
                    new StartFishingMessage(e.User.UniqueMultiplayerID, fishKey),
                    FishingMessageTypes.StartFishing
                );
                this.StartFishingMinigame(e.User, e.Rod, fishKey, bobberDepth);
                return;
            }

            // Secret note
            if (e.User.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08)
            {
                if (location.tryToCreateUnseenSecretNote(e.User) is { ParentSheetIndex: var secretNoteId })
                {
                    var secretNoteKey = NamespacedKey.SdvCustom(ItemTypes.Object, $"SecretNote/{secretNoteId}");
                    this.helper.Multiplayer.SendMessage(
                        new SpecialCaughtMessage(e.User.UniqueMultiplayerID, secretNoteKey),
                        FishingMessageTypes.SpecialCaught
                    );
                    this.PullItemFromWater(e.User, e.Rod, secretNoteKey);
                    // TODO: rod.pullFishFromWater(noteId, -1, 0, 0, false, false, false);
                    return;
                }
            }

            // Trash
            var trash = this.fishingHelper.GetTrashChances(e.User).ChooseOrDefault(Game1.random)?.Value;
            if (trash is { } trashKey)
            {
                this.helper.Multiplayer.SendMessage(
                    new TrashCaughtMessage(e.User.UniqueMultiplayerID, trashKey),
                    FishingMessageTypes.TrashCaught
                );
                this.PullItemFromWater(e.User, e.Rod, trashKey);
                return;
            }

            // Default trash item
            this.monitor.Log("No valid trash, selecting a default item.", LogLevel.Warn);
            var defaultTrashKey = NamespacedKey.SdvObject(168);
            this.helper.Multiplayer.SendMessage(
                new TrashCaughtMessage(e.User.UniqueMultiplayerID, defaultTrashKey),
                FishingMessageTypes.TrashCaught
            );
            this.PullItemFromWater(e.User, e.Rod, defaultTrashKey);
        }

        public void StartFishingMinigame(
            Farmer user,
            FishingRod rod,
            NamespacedKey fishKey,
            float bobberDepth
        )
        {
            if (FishingController.beginReelingEvent.GetValue(rod) is not NetEvent0 beginReelingEvent)
            {
                this.monitor.Log($"Could not get '{nameof(beginReelingEvent)}' for fishing rod.", LogLevel.Warn);
                return;
            }

            // Update user
            beginReelingEvent.Fire();
            rod.isReeling = true;
            rod.hit = false;
            switch (user.FacingDirection)
            {
                case 1:
                    user.FarmerSprite.setCurrentSingleFrame(48);
                    break;
                case 3:
                    user.FarmerSprite.setCurrentSingleFrame(48, flip: true);
                    break;
            }

            // Open fishing minigame
            var sizeDepthFactor = 1f * (bobberDepth / 5f);
            var sizeLevelFactor = 1 + user.FishingLevel / 2;
            var sizeFactor = sizeDepthFactor * Game1.random.Next(sizeLevelFactor, Math.Max(6, sizeLevelFactor)) / 5f;
            if (rod.favBait)
            {
                sizeFactor *= 1.2f;
            }

            var fishSizePercent = Math.Clamp(sizeFactor * (1.0f + Game1.random.Next(-10, 11) / 100.0f), 0.0f, 1.0f);
            var treasure = Game1.isFestival()
                && user.fishCaught?.FieldDict?.Count is > 1
                && Game1.random.NextDouble() < this.fishingHelper.GetChanceForTreasure(user);
            var customBobber = this.customBobberBarFactory.Create(
                user,
                fishKey,
                fishSizePercent,
                treasure,
                rod.attachments[1]?.ParentSheetIndex ?? -1
            );
            if (customBobber is not null)
            {
                Game1.activeClickableMenu = customBobber;
            }
            else
            {
                this.monitor.Log("Error creating fishing minigame GUI", LogLevel.Warn);
            }
        }

        public void PullItemFromWater(Farmer user, FishingRod rod, NamespacedKey itemKey)
        {
            // TODO
        }
    }
}