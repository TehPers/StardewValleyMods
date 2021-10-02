using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using Ninject;
using Ninject.Activation;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Extensions;
using TehPers.FishingOverhaul.Extensions.Drawing;
using TehPers.FishingOverhaul.Services;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul.Setup
{
    [SuppressMessage(
        "ReSharper",
        "InconsistentNaming",
        Justification = "Harmony patches have a specific naming convention."
    )]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Intentionally non-standard naming convention.")]
    internal class FishingRodPatcher : ISetup, IDisposable
    {
        private static FishingRodPatcher? Instance { get; set; }

        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly Harmony harmony;
        private readonly FishingTracker fishingTracker;
        private readonly IFishingHelper fishingHelper;
        private readonly ICustomBobberBarFactory customBobberBarFactory;
        private readonly FishConfig fishConfig;
        private readonly INamespaceRegistry namespaceRegistry;

        private bool initialized;
        private MethodInfo? updatePatch;
        private MethodInfo? doFunctionPatch;
        private MethodInfo? drawPatch;

        private FishingRodPatcher(
            IModHelper helper,
            IMonitor monitor,
            Harmony harmony,
            FishingTracker fishingTracker,
            IFishingHelper fishingHelper,
            ICustomBobberBarFactory customBobberBarFactory,
            FishConfig fishConfig,
            INamespaceRegistry namespaceRegistry
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.harmony = harmony ?? throw new ArgumentNullException(nameof(harmony));
            this.fishingTracker = fishingTracker ?? throw new ArgumentNullException(nameof(fishingTracker));
            this.fishingHelper = fishingHelper ?? throw new ArgumentNullException(nameof(fishingHelper));
            this.customBobberBarFactory = customBobberBarFactory
                ?? throw new ArgumentNullException(nameof(customBobberBarFactory));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.namespaceRegistry = namespaceRegistry ?? throw new ArgumentNullException(nameof(namespaceRegistry));

            this.initialized = false;
            this.updatePatch = null;
            this.doFunctionPatch = null;
            this.drawPatch = null;
        }

        public static FishingRodPatcher Create(IContext context)
        {
            FishingRodPatcher.Instance ??= new(
                context.Kernel.Get<IModHelper>(),
                context.Kernel.Get<IMonitor>(),
                context.Kernel.Get<Harmony>(),
                context.Kernel.Get<FishingTracker>(),
                context.Kernel.Get<IFishingHelper>(),
                context.Kernel.Get<ICustomBobberBarFactory>(),
                context.Kernel.Get<FishConfig>(),
                context.Kernel.Get<INamespaceRegistry>()
            );
            return FishingRodPatcher.Instance;
        }

        public void Setup()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;

            // Apply patches
            this.updatePatch = this.harmony.Patch(
                AccessTools.Method(typeof(FishingRod), nameof(FishingRod.tickUpdate)),
                prefix: new(AccessTools.Method(typeof(FishingRodPatcher), nameof(FishingRodPatcher.update_Prefix)))
            );
            this.doFunctionPatch = this.harmony.Patch(
                AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
                prefix: new(AccessTools.Method(typeof(FishingRodPatcher), nameof(FishingRodPatcher.DoFunction_Prefix)))
            );
            this.drawPatch = this.harmony.Patch(
                AccessTools.Method(typeof(FishingRod), nameof(FishingRod.draw)),
                prefix: new(AccessTools.Method(typeof(FishingRodPatcher), nameof(FishingRodPatcher.draw_Prefix)))
            );
        }

        public void Dispose()
        {
            if (!this.initialized)
            {
                return;
            }

            this.initialized = false;

            // Remove patches
            if (this.updatePatch is { } updatePatch)
            {
                this.harmony.Unpatch(
                    AccessTools.Method(typeof(FishingRod), nameof(FishingRod.tickUpdate)),
                    updatePatch
                );
            }

            if (this.doFunctionPatch is { } doFunctionPatch)
            {
                this.harmony.Unpatch(
                    AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
                    doFunctionPatch
                );
            }

            if (this.drawPatch is { } drawPatch)
            {
                this.harmony.Unpatch(
                    AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
                    drawPatch
                );
            }
        }

        private void StartFishingMinigame(
            Farmer user,
            FishingRod rod,
            NamespacedKey fishKey,
            bool fromFishPond,
            int bobberDepth
        )
        {
            // Update user
            this.fishingTracker.ActiveFisherData[user] = new(rod, new FishingState.Fishing(fishKey));
            var beginReelingEvent = this.helper.Reflection.GetField<NetEvent0>(rod, "beginReelingEvent").GetValue();
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
            var treasure = !Game1.isFestival()
                && user.fishCaught?.Count() > 1
                && Game1.random.NextDouble() < this.fishingHelper.GetChanceForTreasure(user);
            var customBobber = this.customBobberBarFactory.Create(
                user,
                fishKey,
                fishSizePercent,
                treasure,
                rod.attachments[1]?.ParentSheetIndex ?? -1,
                fromFishPond
            );
            if (customBobber is not null)
            {
                customBobber.LostFish += (_, _) =>
                {
                    var initialStreak = this.fishingHelper.GetStreak(user);
                    if (initialStreak >= this.fishConfig.StreakForIncreasedQuality)
                    {
                        Game1.showGlobalMessage(
                            this.helper.Translation.Get("text.streak.lost", new { streak = initialStreak })
                        );
                    }

                    this.fishingHelper.SetStreak(user, 0);
                };

                customBobber.CatchFish += (_, info) =>
                {
                    var initialStreak = this.fishingHelper.GetStreak(user);
                    this.fishingHelper.SetStreak(user, initialStreak + 1);
                    this.CatchItem(user, rod, info);
                };

                Game1.activeClickableMenu = customBobber;
            }
            else
            {
                this.monitor.Log("Error creating fishing minigame GUI", LogLevel.Error);
            }
        }

        private void CatchItem(Farmer user, FishingRod rod, CatchInfo info)
        {
            // Track fishing state
            var newState = new FishingState.Caught(info);
            this.fishingTracker.ActiveFisherData[user] = new(rod, newState);
            this.monitor.Log($"{user.Name} caught {info.ItemKey}.");

            var (_, item, fromFishPond) = info;
            if (info is CatchInfo.FishCatch (_, _, var fishSize, var isLegendary, var fishQuality, var fishDifficulty,
                var wasTreasureCaught, var wasPerfectCatch, _, var caughtDouble))
            {
                // Update caught item
                if (item is SObject obj)
                {
                    obj.Quality = fishQuality;

                    if (caughtDouble)
                    {
                        obj.Stack = 2;
                    }
                }

                // Update fishing rod
                rod.treasureCaught = wasTreasureCaught;
                this.helper.Reflection.GetField<int>(rod, "fishSize").SetValue(fishSize);
                this.helper.Reflection.GetField<int>(rod, "fishQuality").SetValue(Math.Max(fishQuality, 0));
                this.helper.Reflection.GetField<int>(rod, "whichFish").SetValue(0);
                rod.fromFishPond = fromFishPond;
                rod.caughtDoubleFish = caughtDouble;
                this.helper.Reflection.GetField<string>(rod, "itemCategory").SetValue("Object");

                // Give the user experience
                if (!Game1.isFestival() && user.IsLocalPlayer && !fromFishPond)
                {
                    rod.bossFish = isLegendary;
                    var experience = Math.Max(1, (fishQuality + 1) * 3 + fishDifficulty / 3)
                        * (wasTreasureCaught ? 2.2 : 1)
                        * (wasPerfectCatch ? 2.4 : 1)
                        * (rod.bossFish ? 5.0 : 1);
                    user.gainExperience(1, (int)experience);
                }
            }
            else
            {
                // Update fishing rod
                rod.treasureCaught = false;
                this.helper.Reflection.GetField<int>(rod, "fishSize").SetValue(-1);
                this.helper.Reflection.GetField<int>(rod, "fishQuality").SetValue(-1);
                this.helper.Reflection.GetField<int>(rod, "whichFish").SetValue(0);
                rod.fromFishPond = fromFishPond;
                rod.caughtDoubleFish = false;
                this.helper.Reflection.GetField<string>(rod, "itemCategory").SetValue("Object");
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
            sourceRect = new(228, 408, 16, 16);
            // }

            // Create animation
            float animationInterval;
            if (user.FacingDirection is 1 or 3)
            {
                var distToBobber = Vector2.Distance(rod.bobber, user.Position);
                const float y1 = 1f / 1000f;
                var num6 = 128.0f - (user.Position.Y - rod.bobber.Y + 10.0f);
                const double a1 = 4.0 * Math.PI / 11.0;
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
                    new(
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
                        motion = new((user.FacingDirection == 3 ? -1f : 1f) * -num7, -f1),
                        acceleration = new(0.0f, y1),
                        timeBasedMotion = true,
                        endFunction = _ => this.FinishFishing(user, rod, info),
                        endSound = "tinyWhip"
                    }
                );
                if (info is CatchInfo.FishCatch { CaughtDouble: true })
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
                        new(
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
                            motion = new((user.FacingDirection == 3 ? -1f : 1f) * -num10, -f2),
                            acceleration = new(0.0f, y2),
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

                const float y3 = 3f / 1000f;
                var num13 = (float)Math.Sqrt(2.0 * y3 * num12);
                animationInterval = (float)(Math.Sqrt(2.0 * (num12 - (double)num11) / y3)
                    + num13 / (double)y3);
                var x1 = 0.0f;
                if (animationInterval != 0.0)
                {
                    x1 = (user.Position.X - rod.bobber.X) / animationInterval;
                }

                rod.animations.Add(
                    new(
                        textureName,
                        sourceRect,
                        animationInterval,
                        1,
                        0,
                        new(rod.bobber.X, rod.bobber.Y),
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
                        motion = new(x1, -num13),
                        acceleration = new(0.0f, y3),
                        timeBasedMotion = true,
                        endFunction = _ => this.FinishFishing(user, rod, info),
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

                    const float y4 = 0.004f;
                    var num16 = (float)Math.Sqrt(2.0 * y4 * num15);
                    animationInterval = (float)(Math.Sqrt(2.0 * (num15 - (double)num14) / y4)
                        + num16 / (double)y4);
                    var x2 = 0.0f;
                    if (animationInterval != 0.0)
                    {
                        x2 = (user.Position.X - rod.bobber.X) / animationInterval;
                    }

                    rod.animations.Add(
                        new(
                            textureName,
                            sourceRect,
                            animationInterval,
                            1,
                            0,
                            new(rod.bobber.X, rod.bobber.Y),
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
                            motion = new(x2, -num16),
                            acceleration = new(0.0f, y4),
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

        private void FinishFishing(Farmer user, FishingRod rod, CatchInfo info)
        {
            user.Halt();
            user.armOffset = Vector2.Zero;
            rod.castedButBobberStillInAir = false;
            rod.fishCaught = true;
            rod.isReeling = false;
            rod.isFishing = false;
            rod.pullingOutOfWater = false;
            user.canReleaseTool = false;
            if (!user.IsLocalPlayer)
            {
                return;
            }

            if (info is CatchInfo.FishCatch
            {
                Item : SObject { ParentSheetIndex: var parentSheetIndex, Stack: var stack },
                FishSize: var fishSize,
                FromFishPond: var fromFishPond
            })
            {
                if (!Game1.isFestival())
                {
                    rod.recordSize = user.caughtFish(parentSheetIndex, fishSize, fromFishPond, stack);
                    user.faceDirection(2);
                }
                else
                {
                    Game1.currentLocation.currentEvent.caughtFish(parentSheetIndex, fishSize, user);
                    rod.fishCaught = false;
                    rod.doneFishing(user);
                }
            }

            if (info is CatchInfo.FishCatch { IsLegendary: true })
            {
                Game1.showGlobalMessage(Game1.content.LoadString(@"Strings\StringsFromCSFiles:FishingRod.cs.14068"));
                string str = info.Item.DisplayName;
                var multiplayer = (Multiplayer)typeof(Game1).GetField(
                    "multiplayer",
                    BindingFlags.NonPublic | BindingFlags.Static
                )!.GetValue(null)!;
                multiplayer.globalChatInfoMessage("CaughtLegendaryFish", Game1.player.Name, str);
            }
            else if (rod.recordSize)
            {
                rod.sparklingText = new(
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

        private void OpenTreasureMenuEndFunction(
            Farmer user,
            FishingRod rod,
            IList<Item> treasure,
            int bobberDepth
        )
        {
            // Finish fishing
            user.gainExperience(5, 10 * (bobberDepth + 1));
            user.UsingTool = false;
            user.completelyStopAnimatingOrDoingAction();
            rod.doneFishing(user, true);

            // Show menu
            var menu = new ItemGrabMenu(treasure, rod) { source = 3, }.setEssential(true);
            Game1.activeClickableMenu = menu;
            user.completelyStopAnimatingOrDoingAction();

            // Track fishing state
            this.fishingTracker.ActiveFisherData[user] = new(rod, new FishingState.NotFishing());
        }

        public static bool DoFunction_Prefix(
            GameLocation location,
            int x,
            int y,
            Farmer who,
            FishingRod __instance,
            int ___clearWaterDistance,
            ref bool ___lastCatchWasJunk
        )
        {
            // Get patcher instance
            if (FishingRodPatcher.Instance is not { } patcher)
            {
                return true;
            }

            // Get active fisher data
            if (!patcher.fishingTracker.ActiveFisherData.TryGetValue(who, out var activeFisher))
            {
                activeFisher = new(__instance, FishingState.Start());
                patcher.fishingTracker.ActiveFisherData[who] = activeFisher;
            }

            // Ensure the correct rod is being tracked
            if (activeFisher.Rod != __instance)
            {
                activeFisher = new(__instance, FishingState.Start());
                patcher.fishingTracker.ActiveFisherData[who] = activeFisher;
            }

            // Transition
            switch (activeFisher.State)
            {
                // Start fishing
                case FishingState.NotFishing:
                {
                    patcher.fishingTracker.ActiveFisherData[who] = new(__instance, new FishingState.WaitingForBite());
                    return true;
                }

                // Pull line from water
                case FishingState.WaitingForBite:
                {
                    who.FarmerSprite.PauseForSingleAnimation = false;
                    who.FarmerSprite.animateBackwardsOnce(
                        who.FacingDirection switch
                        {
                            0 => 299,
                            1 => 300,
                            2 => 301,
                            3 => 302,
                            // TODO: what should go here?
                            _ => 301,
                        },
                        35f
                    );

                    // Check if fish is nibbling
                    if (!__instance.isNibbling)
                    {
                        return true;
                    }

                    // Check if fishing from fish pond
                    var bobberTile = patcher.helper.Reflection
                        .GetMethod(__instance, "calculateBobberTile")
                        .Invoke<Vector2>();
                    var fromFishPond = location.isTileBuildingFishable((int)bobberTile.X, (int)bobberTile.Y);
                    if (patcher.fishingHelper.GetFishPondFish(who, bobberTile, true) is { } fishKey)
                    {
                        if (patcher.namespaceRegistry.TryGetItemFactory(fishKey, out var factory))
                        {
                            patcher.CatchItem(
                                who,
                                __instance,
                                new CatchInfo.FishCatch(
                                    fishKey,
                                    factory.Create(),
                                    -1,
                                    false,
                                    0,
                                    0,
                                    false,
                                    false,
                                    true
                                )
                            );
                            return false;
                        }
                        else
                        {
                            patcher.monitor.Log(
                                $"No provider for {fishKey} (from fish pond)! Defaulting to normal fishing behavior.",
                                LogLevel.Error
                            );
                        }
                    }

                    // Select an item to catch
                    var (itemKey, catchType) = patcher.fishingHelper.GetPossibleCatch(
                        who,
                        __instance,
                        ___clearWaterDistance
                    );

                    // Catch the item
                    switch (catchType)
                    {
                        // Begin fishing minigame
                        case PossibleCatch.Type.Fish:
                        {
                            ___lastCatchWasJunk = false;
                            if (__instance.hit || !who.IsLocalPlayer)
                            {
                                return false;
                            }

                            __instance.hit = true;
                            Game1.screenOverlayTempSprites.Add(
                                new(
                                    "LooseSprites\\Cursors",
                                    new(612, 1913, 74, 30),
                                    1500f,
                                    1,
                                    0,
                                    Game1.GlobalToLocal(
                                        Game1.viewport,
                                        __instance.bobber + new Vector2(-140f, -160f)
                                    ),
                                    false,
                                    false,
                                    1f,
                                    0.005f,
                                    Color.White,
                                    4f,
                                    0.075f,
                                    0.0f,
                                    0.0f,
                                    true
                                )
                                {
                                    scaleChangeChange = -0.005f,
                                    motion = new(0.0f, -0.1f),
                                    endFunction = _ => patcher.StartFishingMinigame(
                                        who,
                                        __instance,
                                        itemKey,
                                        fromFishPond,
                                        ___clearWaterDistance
                                    ),
                                    id = 9.876543E+08f
                                }
                            );
                            location.localSound("FishHit");
                            break;
                        }

                        // Pull trash from the water
                        case PossibleCatch.Type.Trash:
                        {
                            ___lastCatchWasJunk = true;
                            if (patcher.namespaceRegistry.TryGetItemFactory(itemKey, out var factory))
                            {
                                patcher.CatchItem(
                                    who,
                                    __instance,
                                    new CatchInfo.TrashCatch(itemKey, factory.Create(), fromFishPond)
                                );
                            }
                            else
                            {
                                patcher.monitor.Log(
                                    $"Could not create {itemKey} because no provider exists for it.",
                                    LogLevel.Error
                                );
                                patcher.CatchItem(
                                    who,
                                    __instance,
                                    new CatchInfo.TrashCatch(itemKey, new SObject(0, 1), fromFishPond)
                                );
                            }

                            break;
                        }

                        // Pull special item from the water
                        case PossibleCatch.Type.Special:
                        {
                            ___lastCatchWasJunk = true;
                            if (patcher.namespaceRegistry.TryGetItemFactory(itemKey, out var factory))
                            {
                                patcher.CatchItem(
                                    who,
                                    __instance,
                                    new CatchInfo.SpecialCatch(itemKey, factory.Create(), fromFishPond)
                                );
                            }
                            else
                            {
                                patcher.monitor.Log(
                                    $"Could not create {itemKey} because no provider exists for it.",
                                    LogLevel.Error
                                );
                                patcher.CatchItem(
                                    who,
                                    __instance,
                                    new CatchInfo.SpecialCatch(itemKey, new SObject(0, 1), fromFishPond)
                                );
                            }

                            break;
                        }

                        default:
                            throw new($"Unknown catch type: {catchType}");
                    }

                    return false;
                }

                // No actions available
                default:
                    return false;
            }
        }

        public static bool update_Prefix(FishingRod __instance, ref int ___recastTimerMs, int ___clearWaterDistance)
        {
            if (FishingRodPatcher.Instance is not { } patcher)
            {
                return true;
            }

            // Get last user
            if (__instance.getLastFarmerToUse() is not { } user)
            {
                return true;
            }

            // Ensure the user is using this rod
            if (user.CurrentTool != __instance)
            {
                return true;
            }

            // Get/init user's fishing state
            if (!patcher.fishingTracker.ActiveFisherData.TryGetValue(user, out var fisherData))
            {
                fisherData = new(__instance, FishingState.Start());
                patcher.fishingTracker.ActiveFisherData[user] = fisherData;
            }

            // Reset state if not fishing
            if (!__instance.inUse())
            {
                patcher.fishingTracker.ActiveFisherData[user] =
                    new(__instance, FishingState.Start());
            }

            // Transition state
            switch (fisherData.State)
            {
                case FishingState.Caught(var catchInfo):
                {
                    // Check if user is holding the fish now
                    if (!__instance.bobber.Value.Equals(Vector2.Zero)
                        && (__instance.isFishing
                            || __instance.pullingOutOfWater
                            || __instance.castedButBobberStillInAir)
                        && user.FarmerSprite.CurrentFrame is not 57
                        && (user.FacingDirection is not 0 || !__instance.pullingOutOfWater)
                        || !__instance.fishCaught)
                    {
                        return true;
                    }

                    // Transition state
                    patcher.fishingTracker.ActiveFisherData[user] = new(
                        __instance,
                        new FishingState.Holding(catchInfo)
                    );
                    return false;
                }

                case FishingState.Holding(var (itemKey, item, _)):
                {
                    // Give the user the item they caught if they are the local player
                    if (!user.IsLocalPlayer
                        || Game1.input.GetMouseState().LeftButton != ButtonState.Pressed
                        && !Game1.didPlayerJustClickAtAll()
                        && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
                    {
                        return true;
                    }

                    // Create caught item
                    if (item is SObject caughtObj)
                    {
                        // Quest items
                        if (object.Equals(itemKey, NamespacedKey.SdvObject(GameLocation.CAROLINES_NECKLACE_ITEM)))
                        {
                            caughtObj.questItem.Value = true;
                        }
                        else if (object.Equals(itemKey, NamespacedKey.SdvObject(79))
                            || object.Equals(itemKey, NamespacedKey.SdvObject(842)))
                        {
                            item = user.currentLocation.tryToCreateUnseenSecretNote(user);
                            if (item == null)
                            {
                                return false;
                            }
                        }
                    }

                    user.currentLocation.localSound("coin");
                    var fromFishPond = __instance.fromFishPond;
                    if (!Game1.isFestival() && !fromFishPond && Game1.player.team.specialOrders is { } specialOrders)
                    {
                        foreach (SpecialOrder specialOrder in specialOrders)
                        {
                            specialOrder.onFishCaught?.Invoke(Game1.player, item);
                        }
                    }

                    if (!__instance.treasureCaught)
                    {
                        ___recastTimerMs = 200;
                        user.completelyStopAnimatingOrDoingAction();
                        __instance.doneFishing(user, !fromFishPond);
                        if (Game1.isFestival() || user.addItemToInventoryBool(item))
                        {
                            // Transition fishing state and prevent rod from being used this frame
                            patcher.fishingTracker.ActiveFisherData[user] = new(
                                __instance,
                                new FishingState.NotFishing()
                            );
                            __instance.isFishing = true;
                            return false;
                        }

                        Game1.activeClickableMenu =
                            new ItemGrabMenu(new List<Item> { item }, __instance).setEssential(true);
                    }
                    else
                    {
                        __instance.fishCaught = false;
                        __instance.showingTreasure = true;
                        user.UsingTool = true;
                        var treasure = patcher.fishingHelper.GetPossibleTreasure(user);
                        if (!user.addItemToInventoryBool(item))
                        {
                            treasure.Add(item);
                        }

                        __instance.animations.Add(
                            new(
                                "LooseSprites\\Cursors",
                                new(64, 1920, 32, 32),
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
                                motion = new(0.0f, -0.128f),
                                timeBasedMotion = true,
                                endFunction = _ =>
                                {
                                    user.currentLocation.localSound("openChest");
                                    __instance.sparklingText = null;
                                    __instance.animations.Add(
                                        new(
                                            "LooseSprites\\Cursors",
                                            new(64, 1920, 32, 32),
                                            200f,
                                            4,
                                            0,
                                            user.Position + new Vector2(-32f, -228f),
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
                                            endFunction = _ => patcher.OpenTreasureMenuEndFunction(
                                                user,
                                                __instance,
                                                treasure,
                                                ___clearWaterDistance
                                            )
                                        }
                                    );
                                },
                                alpha = 0.0f,
                                alphaFade = -1f / 500f
                            }
                        );
                    }

                    // Transition fishing state
                    patcher.fishingTracker.ActiveFisherData[user] = new(__instance, new FishingState.OpeningTreasure());
                    return false;
                }
            }

            return true;
        }

        public static bool draw_Prefix(SpriteBatch b, FishingRod __instance)
        {
            if (FishingRodPatcher.Instance is not { } patcher)
            {
                return true;
            }

            // Get last user
            if (__instance.getLastFarmerToUse() is not { } user)
            {
                return true;
            }

            // Ensure the user is using this rod
            if (user.CurrentTool != __instance)
            {
                return true;
            }

            // Get user's fishing state
            if (!patcher.fishingTracker.ActiveFisherData.TryGetValue(user, out var fisherData))
            {
                return true;
            }

            // Render each fisher
            switch (fisherData.State)
            {
                case FishingState.Holding(var info):
                {
                    var y = (float)(4.0
                        * Math.Round(
                            Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0),
                            2
                        ));

                    // Draw bubble
                    var layerDepth = user.getStandingY() / 10000.0f + 0.06f;
                    b.Draw(
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
                    info.Item.DrawInMenuCorrected(
                        b,
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
                    var count = info.Item is SObject { Stack: var stack } ? stack : 1;
                    count = Math.Min(1, count);
                    foreach (var fishIndex in Enumerable.Range(0, count))
                    {
                        // TODO: some kind of jagged pattern with all the fish
                        // Maybe:
                        //  - X offset in range [-8, 8]
                        //  - Y offset in range [-8, 8]
                        var offset = new Vector2(0f, 0f);
                        info.Item.DrawInMenuCorrected(
                            b,
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

                    // Draw item name
                    var isLegendary = info is CatchInfo.FishCatch { IsLegendary: true };
                    b.DrawString(
                        Game1.smallFont,
                        info.Item.DisplayName,
                        Game1.GlobalToLocal(
                            Game1.viewport,
                            user.Position
                            + new Vector2(
                                (float)(26.0 - Game1.smallFont.MeasureString(info.Item.DisplayName).X / 2.0),
                                y - 278f
                            )
                        ),
                        isLegendary ? new(126, 61, 237) : Game1.textColor,
                        0.0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f
                    );

                    // Draw fish specific labels
                    if (info is CatchInfo.FishCatch { FishSize: var fishSize })
                    {
                        // Draw fish length label
                        b.DrawString(
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
                        b.DrawString(
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
                                            )
                                            .X
                                        / 2.0),
                                    y - 179f
                                )
                            ),
                            __instance.recordSize
                                ? Color.Blue * Math.Min(1f, (float)(y / 8.0 + 1.5))
                                : Game1.textColor,
                            0.0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f
                        );
                    }

                    return false;
                }
            }

            return true;
        }
    }
}