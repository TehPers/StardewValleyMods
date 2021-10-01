using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Messages;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Gui;

namespace TehPers.FishingOverhaul.Setup
{
    internal sealed class FishingRodOverrider : ISetup, IDisposable
    {
        private static readonly FieldInfo pullFishFromWaterEvent =
            typeof(FishingRod).GetField("pullFishFromWaterEvent", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new Exception($"Missing info for {nameof(FishingRodOverrider.pullFishFromWaterEvent)}.");

        private static readonly FieldInfo netEventOnEvent =
            typeof(AbstractNetEvent1<byte[]>).GetField("onEvent", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new Exception($"Missing info for {nameof(FishingRodOverrider.netEventOnEvent)}.");

        private static readonly MethodInfo doPullFishFromWater =
            typeof(FishingRod).GetMethod("doPullFishFromWater", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new Exception($"Missing info for {nameof(FishingRodOverrider.doPullFishFromWater)}.");

        private readonly IModHelper helper;
        private readonly IFishingHelper fishingHelper;
        private readonly IMonitor monitor;
        private readonly INamespaceRegistry namespaceRegistry;
        private readonly ICustomBobberBarFactory customBobberBarFactory;
        private readonly FishConfig fishConfig;
        private readonly FishingTracker fishingTracker;
        private readonly HashSet<FishingRod> overriddenRods = new();

        public event EventHandler<PullFishEvent>? PullFish;
        public event EventHandler<OpenTreasureEvent>? OpenTreasure;

        public FishingRodOverrider(
            IModHelper helper,
            IFishingHelper fishingHelper,
            IMonitor monitor,
            INamespaceRegistry namespaceRegistry,
            ICustomBobberBarFactory customBobberBarFactory,
            FishConfig fishConfig,
            FishingTracker fishingTracker
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.fishingHelper = fishingHelper ?? throw new ArgumentNullException(nameof(fishingHelper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.namespaceRegistry = namespaceRegistry ?? throw new ArgumentNullException(nameof(namespaceRegistry));
            this.customBobberBarFactory = customBobberBarFactory
                ?? throw new ArgumentNullException(nameof(customBobberBarFactory));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.fishingTracker = fishingTracker ?? throw new ArgumentNullException(nameof(fishingTracker));
        }

        public void Setup()
        {
            this.helper.Events.GameLoop.UpdateTicked += this.OverrideFishingEvents;
        }

        public void Dispose()
        {
            this.helper.Events.GameLoop.UpdateTicked -= this.OverrideFishingEvents;
        }

        private void OverrideFishingEvents(object? sender, UpdateTickedEventArgs e)
        {
            // TODO: check all players?
            if (Game1.player.CurrentTool is not FishingRod rod)
            {
                return;
            }

            // Check if rod needs to have its events overridden
            if (this.overriddenRods.Add(rod))
            {
                this.OverridePullFishEvent(rod);
            }

            // Remove all non-overridden catching animations
            var catchingAnimations = Game1.screenOverlayTempSprites.RemoveAll(
                animation => animation.endFunction == rod.startMinigameEndFunction
            );

            // Replace catch if any animations were removed
            if (catchingAnimations > 0)
            {
                this.OnPullFromNibble(rod.getLastFarmerToUse(), rod);
            }

            if (Game1.IsMultiplayer && !Game1.IsServer)
            {
                return;
            }

            // Replace the end function for custom treasure
            var openTreasureAnims =
                rod.animations.Where(animation => animation.endFunction == rod.openTreasureMenuEndFunction);
            foreach (var animation in openTreasureAnims)
            {
                animation.endFunction = extra => this.OpenTreasureEndFunction(rod, rod.getLastFarmerToUse(), extra);
            }
        }

        private void OverridePullFishEvent(FishingRod rod)
        {
            // Get the pull fish event
            if (FishingRodOverrider.pullFishFromWaterEvent.GetValue(rod) is not NetEventBinary pullFishEvent)
            {
                return;
            }

            // Get the backing field for the event
            if (FishingRodOverrider.netEventOnEvent.GetValue(pullFishEvent) is not AbstractNetEvent1<byte[]>.Event
                eventField)
            {
                return;
            }

            // Replace all the handlers from the event
            foreach (var method in eventField.GetInvocationList())
            {
                // Remove existing handler
                if (method is AbstractNetEvent1<byte[]>.Event handler)
                {
                    pullFishEvent.onEvent -= handler;
                }
                else
                {
                    this.monitor.Log(
                        $"Failed to remove {method} ({method.Method.Name}) from {nameof(AbstractNetEvent1<byte[]>.onEvent)}.",
                        LogLevel.Warn
                    );
                }
            }

            // Set a new handler
            pullFishEvent.AddReaderHandler(
                reader => this.DoPullFishFromWater(rod, rod.getLastFarmerToUse(), reader)
            );
            this.monitor.Log("Events overridden for fishing rod.");
        }

        private void DoPullFishFromWater(FishingRod rod, Farmer user, BinaryReader reader)
        {
            var isTracked = this.fishingTracker.ActiveFisherData.TryGetValue(user, out var fisherData);
            switch (isTracked, fisherData)
            {
                case (true, { Rod: var trackedRod }) when rod != trackedRod:
                    // Wrong fishing rod is tracked
                    break;
                case (true, { State: not FishingState.NotFishing }):
                    // Don't override, user is already fishing
                    this.monitor.Log("Calling vanilla fishing logic.", LogLevel.Warn);
                    FishingRodOverrider.doPullFishFromWater.Invoke(rod, new object[] { reader });
                    return;
                case (false, _):
                    // User's fishing state not tracked
                    break;
            }

            // Start tracking the user (or reset their state if wrong fishing rod is tracked)
            fisherData = new FishingTracker.ActiveFisher(rod, new FishingState.Fishing());
            this.fishingTracker.ActiveFisherData[user] = fisherData;

            // Override this catch (should be trash)
            this.OnPullFromNibble(user, rod);
        }

        private void OnPullFromNibble(Farmer user, FishingRod rod)
        {
            _ = rod ?? throw new ArgumentNullException(nameof(rod), "Fishing rod is null. Please report this.");
            _ = user ?? throw new ArgumentNullException(nameof(user), "User is null. Please report this.");

            if (!rod.isFishing)
            {
                return;
            }

            // Notify that the user is fishing
            this.monitor.Log("Overridding vanilla catch.");
            this.OnPullFish(new PullFishEvent(user, rod));
        }

        public void CatchFish(FishCaughtMessage message)
        {
            if (Game1.getFarmer(message.FarmerId) is not { } user)
            {
                this.monitor.Log($"Unknown farmer {message.FarmerId}", LogLevel.Warn);
                return;
            }

            if (user.CurrentTool is not FishingRod rod)
            {
                this.monitor.Log($"Farmer {message.FarmerId} is not holding a fishing rod!", LogLevel.Warn);
                return;
            }

            // Track fishing state
            this.fishingTracker.ActiveFisherData[user] =
                new FishingTracker.ActiveFisher(rod, new FishingState.Fishing());

            // TODO: proper error handling
            this.monitor.Log($"{message.FarmerId} caught {message.FishKey}.");
            var item = this.namespaceRegistry.TryGetItemFactory(message.FishKey, out var factory)
                ? factory.Create()
                : new StardewValley.Object(0, 1);
            if (message.CaughtDouble && item is StardewValley.Object obj)
            {
                obj.Stack = 2;
                obj.Quality = message.FishQuality;
            }

            var newState = new FishingState.CaughtFish(
                message.FishKey,
                item,
                message.FishSize,
                this.fishingHelper.IsLegendary(message.FishKey)
            );
            this.fishingTracker.ActiveFisherData[user] = new FishingTracker.ActiveFisher(rod, newState);

            var fishKey = message.FishKey;
            var fishSize = message.FishSize;
            var fishQuality = message.FishQuality;
            var fishDifficulty = message.FishDifficulty;
            var treasureCaught = message.WasTreasureCaught;
            var wasPerfectCatch = message.WasPerfectCatch;
            var fromFishPond = message.FromFishPond;
            var caughtDouble = message.CaughtDouble;
            rod.treasureCaught = treasureCaught;
            this.helper.Reflection.GetField<int>(rod, "fishSize").SetValue(fishSize);
            this.helper.Reflection.GetField<int>(rod, "fishQuality").SetValue(Math.Max(fishQuality, 0));
            this.helper.Reflection.GetField<int>(rod, "whichFish").SetValue(0);
            rod.fromFishPond = fromFishPond;
            rod.caughtDoubleFish = caughtDouble;
            this.helper.Reflection.GetField<string>(rod, "itemCategory").SetValue("Object");

            // Give the user experience
            if (!Game1.isFestival() && user.IsLocalPlayer && !fromFishPond)
            {
                rod.bossFish = this.fishingHelper.IsLegendary(fishKey);
                var experience = Math.Max(1, (fishQuality + 1) * 3 + fishDifficulty / 3)
                    * (treasureCaught ? 2.2 : 1)
                    * (wasPerfectCatch ? 2.4 : 1)
                    * (rod.bossFish ? 5.0 : 1);
                user.gainExperience(1, (int)experience);
            }

            // TODO: draw the correct item when pulling fish from water
            // Get particle sprite
            Rectangle sourceRect;
            string textureName;
            // if (itemCategory == "Object")
            // {
            //     textureName = "Maps\\springobjects";
            //     sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, fishKey, 16, 16);
            // }
            // else
            // {
            textureName = "LooseSprites\\Cursors";
            sourceRect = new Rectangle(228, 408, 16, 16);
            // }

            // Create animation
            float animationInterval;
            if (user.FacingDirection is 1 or 3)
            {
                var distToBobber = Vector2.Distance(rod.bobber, user.Position);
                var y1 = 1f / 1000f;
                var num6 = 128.0f - (user.Position.Y - rod.bobber.Y + 10.0f);
                var a1 = 4.0 * Math.PI / 11.0;
                var f1 = (float)(distToBobber
                    * y1
                    * Math.Tan(a1)
                    / Math.Sqrt(2.0 * distToBobber * y1 * Math.Tan(a1) - 2.0 * y1 * num6));
                if (float.IsNaN(f1))
                {
                    f1 = 0.6f;
                }

                var num7 = f1 * (float)(1.0 / Math.Tan(a1));
                animationInterval = distToBobber / num7;
                rod.animations.Add(
                    new TemporaryAnimatedSprite(
                        textureName,
                        sourceRect,
                        animationInterval,
                        1,
                        0,
                        rod.bobber,
                        false,
                        false,
                        rod.bobber.Y / 10000f,
                        0.0f,
                        Color.White,
                        4f,
                        0.0f,
                        0.0f,
                        0.0f
                    )
                    {
                        motion = new Vector2((user.FacingDirection == 3 ? -1f : 1f) * -num7, -f1),
                        acceleration = new Vector2(0.0f, y1),
                        timeBasedMotion = true,
                        endFunction = _ => this.PlayerCaughtFishEndFunction(user, rod, fishKey),
                        endSound = "tinyWhip"
                    }
                );
                if (caughtDouble)
                {
                    var y2 = 0.0008f;
                    var f2 = (float)(distToBobber
                        * (double)y2
                        * Math.Tan(a1)
                        / Math.Sqrt(2.0 * distToBobber * y2 * Math.Tan(a1) - 2.0 * y2 * num6));
                    if (float.IsNaN(f2))
                    {
                        f2 = 0.6f;
                    }

                    var num10 = f2 * (float)(1.0 / Math.Tan(a1));
                    animationInterval = distToBobber / num10;
                    rod.animations.Add(
                        new TemporaryAnimatedSprite(
                            textureName,
                            sourceRect,
                            animationInterval,
                            1,
                            0,
                            rod.bobber,
                            false,
                            false,
                            rod.bobber.Y / 10000f,
                            0.0f,
                            Color.White,
                            4f,
                            0.0f,
                            0.0f,
                            0.0f
                        )
                        {
                            motion = new Vector2((user.FacingDirection == 3 ? -1f : 1f) * -num10, -f2),
                            acceleration = new Vector2(0.0f, y2),
                            timeBasedMotion = true,
                            endSound = "fishSlap",
                            Parent = user.currentLocation
                        }
                    );
                }
            }
            else
            {
                var num11 = rod.bobber.Y - (user.getStandingY() - 64);
                var num12 = Math.Abs((float)(num11 + 256.0 + 32.0));
                if (user.FacingDirection == 0)
                {
                    num12 += 96f;
                }

                var y3 = 3f / 1000f;
                var num13 = (float)Math.Sqrt(2.0 * y3 * num12);
                animationInterval = (float)(Math.Sqrt(2.0 * (num12 - (double)num11) / y3)
                    + num13 / (double)y3);
                var x1 = 0.0f;
                if (animationInterval != 0.0)
                {
                    x1 = (user.Position.X - rod.bobber.X) / animationInterval;
                }

                rod.animations.Add(
                    new TemporaryAnimatedSprite(
                        textureName,
                        sourceRect,
                        animationInterval,
                        1,
                        0,
                        new Vector2(rod.bobber.X, rod.bobber.Y),
                        false,
                        false,
                        rod.bobber.Y / 10000f,
                        0.0f,
                        Color.White,
                        4f,
                        0.0f,
                        0.0f,
                        0.0f
                    )
                    {
                        motion = new Vector2(x1, -num13),
                        acceleration = new Vector2(0.0f, y3),
                        timeBasedMotion = true,
                        endFunction = _ => this.PlayerCaughtFishEndFunction(user, rod, fishKey),
                        endSound = "tinyWhip"
                    }
                );
                if (rod.caughtDoubleFish)
                {
                    var num14 = rod.bobber.Y - (user.getStandingY() - 64);
                    var num15 = Math.Abs((float)(num14 + 256.0 + 32.0));
                    if (user.FacingDirection == 0)
                    {
                        num15 += 96f;
                    }

                    var y4 = 0.004f;
                    var num16 = (float)Math.Sqrt(2.0 * y4 * num15);
                    animationInterval = (float)(Math.Sqrt(2.0 * (num15 - (double)num14) / y4)
                        + num16 / (double)y4);
                    var x2 = 0.0f;
                    if (animationInterval != 0.0)
                    {
                        x2 = (user.Position.X - rod.bobber.X) / animationInterval;
                    }

                    rod.animations.Add(
                        new TemporaryAnimatedSprite(
                            textureName,
                            sourceRect,
                            animationInterval,
                            1,
                            0,
                            new Vector2(rod.bobber.X, rod.bobber.Y),
                            false,
                            false,
                            rod.bobber.Y / 10000f,
                            0.0f,
                            Color.White,
                            4f,
                            0.0f,
                            0.0f,
                            0.0f
                        )
                        {
                            motion = new Vector2(x2, -num16),
                            acceleration = new Vector2(0.0f, y4),
                            timeBasedMotion = true,
                            endSound = "fishSlap",
                            Parent = user.currentLocation
                        }
                    );
                }
            }

            if (user.IsLocalPlayer)
            {
                user.currentLocation.playSound("pullItemFromWater");
                user.currentLocation.playSound("dwop");
            }

            rod.castedButBobberStillInAir = false;
            rod.pullingOutOfWater = true;
            rod.isFishing = false;
            rod.isReeling = false;
            user.FarmerSprite.PauseForSingleAnimation = false;
            var animation = user.FacingDirection switch
            {
                0 => 299,
                1 => 300,
                2 => 301,
                3 => 302,
                _ => 299,
            };
            user.FarmerSprite.animateBackwardsOnce(animation, animationInterval);
        }

        private void PlayerCaughtFishEndFunction(Farmer user, FishingRod rod, NamespacedKey fishKey)
        {
            // rod.playerCaughtFishEndFunction(fishKey);
            user.Halt();
            user.armOffset = Vector2.Zero;
            rod.castedButBobberStillInAir = false;
            // TODO: Don't set this so the rod doesn't add the item to the player's inventory
            rod.fishCaught = true;
            rod.isReeling = false;
            rod.isFishing = false;
            rod.pullingOutOfWater = false;
            user.canReleaseTool = false;
            if (!user.IsLocalPlayer)
            {
                return;
            }

            // TODO
            if (!Game1.isFestival())
            {
                // TODO
                // rod.recordSize = user.caughtFish(fishKey, fishSize, fromFishPond, caughtDouble ? 2 : 1);
                user.faceDirection(2);
            }
            else
            {
                // TODO
                // Game1.currentLocation.currentEvent.caughtFish(fishKey, fishSize, user);
                rod.fishCaught = false;
                rod.doneFishing(user);
            }

            if (this.fishingHelper.IsLegendary(fishKey))
            {
                Game1.showGlobalMessage(Game1.content.LoadString(@"Strings\StringsFromCSFiles:FishingRod.cs.14068"));
                // TODO
                // string str = Game1.objectInformation[fishKey].Split('/')[4];
                // Game1.multiplayer.globalChatInfoMessage("CaughtLegendaryFish", Game1.player.Name, str);
            }
            else if (rod.recordSize)
            {
                rod.sparklingText = new SparklingText(
                    Game1.dialogueFont,
                    Game1.content.LoadString(@"Strings\StringsFromCSFiles:FishingRod.cs.14069"),
                    Color.LimeGreen,
                    Color.Azure
                );
                user.currentLocation.localSound("newRecord");
            }
            else
            {
                user.currentLocation.localSound("fishSlap");
            }
        }

        private void OpenTreasureEndFunction(FishingRod rod, Farmer user, int includeFish)
        {
            this.OnOpenTreasure(new OpenTreasureEvent(user, rod));

            // TODO
            rod.doneFishing(user, true);
            user.completelyStopAnimatingOrDoingAction();
        }

        private void OnPullFish(PullFishEvent e)
        {
            this.PullFish?.Invoke(this, e);
        }

        private void OnOpenTreasure(OpenTreasureEvent e)
        {
            this.OpenTreasure?.Invoke(this, e);
        }

        internal record PullFishEvent(Farmer User, FishingRod Rod);

        internal record OpenTreasureEvent(Farmer User, FishingRod Rod);
    }
}