using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using TehPers.Core.Api;
using TehPers.Core.Api.Configuration;
using TehPers.Core.Api.DependencyInjection;
using TehPers.Core.Api.DependencyInjection.Lifecycle;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.FishingFramework.Api.Events;
using TehPers.FishingFramework.Api.Extensions;
using TehPers.FishingFramework.Config;
using SObject = StardewValley.Object;

namespace TehPers.FishingFramework
{
    internal class FishingOverrideService : IEventHandler<UpdateTickedEventArgs>
    {
        private readonly IMonitor monitor;
        private readonly IModHelper helper;
        private readonly FishingApi api;
        private readonly IGlobalItemProvider globalItemProvider;
        private readonly FishingEventService fishingEvents;
        private readonly IConfiguration<FishConfiguration> fishConfig;
        private readonly IConfiguration<TreasureConfiguration> treasureConfig;
        private readonly IDataStore<ConditionalWeakTable<FishingRod, FishingRodData>> rodData;

        public FishingOverrideService(
            IMonitor monitor,
            IModHelper helper,
            FishingApi api,
            IGlobalItemProvider globalItemProvider,
            FishingEventService fishingEvents,
            IConfiguration<FishConfiguration> fishConfig,
            IConfiguration<TreasureConfiguration> treasureConfig,
            IDataStore<ConditionalWeakTable<FishingRod, FishingRodData>> rodData)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.globalItemProvider = globalItemProvider ?? throw new ArgumentNullException(nameof(globalItemProvider));
            this.fishingEvents = fishingEvents ?? throw new ArgumentNullException(nameof(fishingEvents));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.treasureConfig = treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
            this.rodData = rodData ?? throw new ArgumentNullException(nameof(rodData));
        }

        private T AccessRodData<T>(FishingRod rod, Func<FishingRodData, T> callback)
        {
            return this.rodData.Access(table => callback(table.GetValue(rod, _ => new FishingRodData())));
        }

        public bool IsRodBeingProcessed(FishingRod rod)
        {
            return this.AccessRodData(rod, data => data.IsProcessing);
        }

        void IEventHandler<UpdateTickedEventArgs>.HandleEvent(object sender, UpdateTickedEventArgs args)
        {
            // Get the client's farmer
            var currentFarmer = Game1.player;

            // Make sure the farmer has a fishing rod equipped
            if (!(currentFarmer.CurrentTool is FishingRod rod))
            {
                return;
            }

            // Override the pullFishFromWater event (for trash)
            var eventsOverridden = this.AccessRodData(rod, data =>
            {
                var prev = data.IsPullEventOverridden;
                data.IsPullEventOverridden = true;
                return prev;
            });

            if (!eventsOverridden)
            {
                this.OverridePullFishEvent(rod);
            }

            if (!rod.isFishing)
            {
                this.AccessRodData(rod, data => data.IsProcessing = false);
            }

            // Check if the rod hit a fish and replace the catch
            var removed = Game1.screenOverlayTempSprites.RemoveAll(animation => animation.endFunction == rod.startMinigameEndFunction);
            if (removed > 0)
            {
                this.OnPullFromNibble(rod, Game1.player);
            }

            // Replace the end function for the custom treasure
            foreach (var anim in rod.animations)
            {
                if (anim.endFunction == rod.openTreasureMenuEndFunction)
                {
                    anim.endFunction = extra => this.OpenTreasureEndFunction(rod, Game1.player, extra);
                }
            }
        }

        private void OverridePullFishEvent(FishingRod rod)
        {
            // Remove all the handlers from the pull fish event
            var pullFishEvent = this.helper.Reflection.GetField<NetEventBinary>(rod, "pullFishFromWaterEvent").GetValue();
            var eventField = this.helper.Reflection.GetField<AbstractNetEvent1<byte[]>.Event>(pullFishEvent, "onEvent").GetValue();
            foreach (var method in eventField.GetInvocationList())
            {
                if (method is AbstractNetEvent1<byte[]>.Event handler)
                {
                    pullFishEvent.onEvent -= handler;
                }
                else
                {
                    this.monitor.Log($"Failed to remove {method} ({method.Method.Name}) from {nameof(AbstractNetEvent1<byte[]>.onEvent)}.", LogLevel.Warn);
                }
            }

            // Add a new event handler
            pullFishEvent.AddReaderHandler(reader => this.DoPullFishFromWater(rod, this.helper.Reflection.GetField<Farmer>(rod, "lastUser").GetValue(), reader));
            this.monitor.Log("Events overridden for fishing rod");
        }

        private void DoPullFishFromWater(FishingRod rod, Farmer user, BinaryReader reader)
        {
            // Check if this should be overridden
            var wasProcessing = this.AccessRodData(rod, data =>
            {
                var prev = data.IsProcessing;
                data.IsProcessing = false;
                return prev;
            });

            if (wasProcessing)
            {
                // Call normal handler
                this.monitor.Log("Calling vanilla fishing logic");
                this.helper.Reflection.GetMethod(rod, "doPullFishFromWater").Invoke(reader);
            }
            else
            {
                // Override this catch (should be trash or something)
                this.OnPullFromNibble(rod, user);
            }
        }

        private void OnPullFromNibble(FishingRod rod, Farmer user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            _ = rod ?? throw new ArgumentNullException(nameof(rod));

            // Make sure this rod doesn't get the pullFishFromWater event overridden when it shouldn't be
            this.AccessRodData(rod, data => data.IsProcessing = true);

            // Get some info about the rod
            var clearWaterDistance = this.helper.Reflection.GetField<int>(rod, "clearWaterDistance").GetValue();
            var location = user.currentLocation;
            var bobberTile = new Vector2(rod.bobber.X / 64F, rod.bobber.Y / 64F);
            var bubblyZone = false;
            if (location.fishSplashPoint is { Value: var fishSplashPoint } && fishSplashPoint != Point.Zero)
            {
                var splashBounds = new Rectangle(fishSplashPoint.X * 64, fishSplashPoint.Y * 64, 64, 64);
                var bobberBounds = new Rectangle((int)rod.bobber.X - 80, (int)rod.bobber.Y - 80, 64, 64);
                bubblyZone = splashBounds.Intersects(bobberBounds);
            }

            if (!TryCatchFish() && !TryCatchSpecial() && !TryCatchTrash())
            {
                // No valid trash was found
                this.monitor.LogWithLocation($"No possible trash found for {location.Name}, using stone instead. This is probably caused by another mod removing trash data.", LogLevel.Warn);
                rod.pullFishFromWater(ObjectsReference.Stone, -1, 0, 0, false, false, false);
            }

            bool TryCatchFish()
            {
                // Check if legendary fish are being overridden as well
                if (!this.fishConfig.Value.ShouldOverrideVanillaLegendaries)
                {
                    // Check if a legendary would be caught
                    var baitValue = rod.attachments[0]?.Price / 10.0 ?? 0.0;
                    var normalFish = location.getFish(rod.fishingNibbleAccumulator, rod.attachments[0]?.ParentSheetIndex ?? -1, clearWaterDistance + (bubblyZone ? 1 : 0), user, baitValue + (bubblyZone ? 0.4 : 0.0), bobberTile, user.currentLocation.Name);
                    var normalFishId = NamespacedId.FromItem(normalFish);

                    // If so, select that fish
                    if (this.api.IsLegendaryFish(normalFishId))
                    {
                        return DoCatch(normalFishId);
                    }
                }

                // Choose a random fish if one hasn't been chosen yet
                if (!(Game1.random.NextDouble() < this.api.FishChances.GetChance(user, this.api.FishingStreak)))
                {
                    return false;
                }

                return this.api.Fish.WhereAvailable(user).ChooseOrDefault(Game1.random) is { Value: { FishId: var fishId } } && DoCatch(fishId);
            }

            bool DoCatch(NamespacedId fishId)
            {
                // Invoke event
                var catchingArgs = new FishCatchingEventArgs(user, rod, fishId);
                this.fishingEvents.OnFishCatching(this, catchingArgs);

                // Check if favBait should be set to true (still not sure what this does...)
                if (!this.globalItemProvider.TryCreate(catchingArgs.FishId, out var fishItem))
                {
                    this.monitor.LogWithLocation($"Unable to create instance of {catchingArgs.FishId} while creating fish. Reverting to trash.", LogLevel.Warn);
                    return false;
                }

                if (fishItem is SObject { Scale: { X: var xScale } } && Math.Abs(xScale - 1.0) < 0.001)
                {
                    rod.favBait = true;
                }

                // Show hit animation
                rod.hit = true;
                this.monitor.LogWithLocation($"Catching fish: {fishId}");
                Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, rod.bobber.Value + new Vector2(-140f, -160f)), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true)
                {
                    scaleChangeChange = -0.005f,
                    motion = new Vector2(0.0f, -0.1f),
                    endFunction = _ => this.StartMinigameEndFunction(rod, user, fishId),
                });
                location.localSound("FishHit");

                return true;
            }

            bool TryCatchSpecial()
            {
                // Void mayonnaise
                if (location.Name.Equals("WitchSwamp") && !Game1.MasterPlayer.mailReceived.Contains("henchmanGone") && Game1.random.NextDouble() < 0.25 && !Game1.player.hasItemInInventory(308, 1))
                {
                    rod.pullFishFromWater(ObjectsReference.VoidMayonnaise, -1, 0, 0, false, false, false);
                    return true;
                }

                // Secret note
                if (user.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08 && location.tryToCreateUnseenSecretNote(user) is { ParentSheetIndex: var noteIndex })
                {
                    rod.pullFishFromWater(noteIndex, -1, 0, 0, false, false, false);
                    return true;
                }

                return false;
            }

            bool TryCatchTrash()
            {
                // Trash
                var selected = this.api.Trash.WhereAvailable(user).ChooseOrDefault(Game1.random);
                if (!(selected is { Value: { ItemId: var trashId } }))
                {
                    return false;
                }

                // Notify listeners that trash is being caught
                var catchingArgs = new TrashCatchingEventArgs(user, rod, trashId);
                this.fishingEvents.OnTrashCatching(this, catchingArgs);

                if (!this.globalItemProvider.TryCreate(catchingArgs.TrashIndex, out var trash))
                {
                    this.monitor.LogWithLocation($"Failed to create an instance of '{trash}'. Trash will revert to a default value.");
                    catchingArgs.TrashIndex = NamespacedId.FromObjectIndex(ObjectsReference.Trash);
                    trash = new SObject(ObjectsReference.Trash, 1);
                }

                // Catch the trash
                // TODO: fire dummy event, then override it with a custom event channel
                // rod.pullFishFromWater(catchingArgs.TrashIndex, -1, 0, 0, false, false, false);

                // Notify listeners that trash was caught
                // var caughtArgs = new TrashCaughtEventArgs(user, rod, catchingArgs.TrashIndex);
                // this.fishingEvents.OnTrashCaught(this, caughtArgs);

                return true;
            }
        }

        private void StartMinigameEndFunction(FishingRod rod, Farmer user, NamespacedId fish)
        {
            var beginReeling = this.helper.Reflection.GetField<NetEvent0>(rod, "beginReelingEvent").GetValue();
            beginReeling.Fire();
            rod.isReeling = true;
            rod.hit = false;

            // Animation
            switch (user.FacingDirection)
            {
                case 1:
                    user.FarmerSprite.setCurrentSingleFrame(48);
                    break;
                case 3:
                    user.FarmerSprite.setCurrentSingleFrame(48, 32000, false, true);
                    break;
            }

            user.FarmerSprite.PauseForSingleAnimation = true;

            var clearWaterDistance = this.helper.Reflection.GetField<int>(rod, "clearWaterDistance").GetValue();
            var clearWaterDistanceFactor = clearWaterDistance / 5f;
            var fishingLevelFactor = 1 + user.FishingLevel / 2;
            var sizeFactor = clearWaterDistanceFactor * (Game1.random.Next(fishingLevelFactor, Math.Max(6, fishingLevelFactor)) / 5f);
            if (rod.favBait)
            {
                sizeFactor *= 1.2f;
            }

            // Check if there should be treasure
            var fishSize = Math.Max(0.0f, Math.Min(1f, sizeFactor * (float)(1.0 + Game1.random.Next(-10, 11) / 100.0)));
            var treasure = !Game1.isFestival();
            treasure &= user.fishCaught is { } caughtFish && caughtFish.Any();
            treasure &= Game1.random.NextDouble() < this.api.TreasureChances.GetChance(user, this.api.FishingStreak);
            // Game1.activeClickableMenu = new BobberBar(extra, fishSize, treasure, this.attachments[1] != null ? this.attachments[1].ParentSheetIndex : -1);
            // TODO: Game1.activeClickableMenu = new CustomBobberBar(this._mod, user, caughtFish, fishSize, treasure, rod.attachments[1]?.ParentSheetIndex ?? -1);
        }

        private void OpenTreasureEndFunction(FishingRod rod, Farmer user, int includeFish)
        {
            this.monitor.LogWithLocation("Successfully replaced treasure");

            // Gain experience and call vanilla code for this
            var clearWaterDistance = this.helper.Reflection.GetField<int>(rod, "clearWaterDistance")?.GetValue() ?? 5;
            user.gainExperience(5, 10 * (clearWaterDistance + 1));
            rod.doneFishing(user, true);
            user.completelyStopAnimatingOrDoingAction();

            // REWARDS
            var rewards = new List<Item>();
            if (includeFish == 1)
            {
                // TODO: whichFish should support namespaced IDs
                var whichFish = this.helper.Reflection.GetField<int>(rod, "whichFish").GetValue();
                var fishQuality = this.helper.Reflection.GetField<int>(rod, "fishQuality").GetValue();
                rewards.Add(new SObject(whichFish, 1, false, -1, fishQuality));
            }

            var possibleLoot = this.api.Treasure.WhereAvailable(user).ToHashSet();

            // Select rewards
            var config = this.treasureConfig.Value;
            var chance = 1d;
            while (possibleLoot.Count > 0 && rewards.Count < config.MaxTreasureQuantity && Game1.random.NextDouble() <= chance)
            {
                var weightedTreasure = possibleLoot.Choose(Game1.random);

                // Choose an ID for the treasure
                var ids = weightedTreasure.Value.ItemIds.ToArray();
                var id = ids[Game1.random.Next(ids.Length)];

                var count = Game1.random.Next(weightedTreasure.Value.MinQuantity, weightedTreasure.Value.MaxQuantity);
                if (count <= 0)
                {
                    continue;
                }

                // Lost books have custom handling
                if (id == NamespacedId.FromObjectIndex(ObjectsReference.LostBook))
                {
                    if (user.archaeologyFound == null || !user.archaeologyFound.TryGetValue(102, out var found) || found[0] >= 21)
                    {
                        possibleLoot.Remove(weightedTreasure);
                        continue;
                    }

                    Game1.showGlobalMessage("You found a lost book. The library has been expanded.");
                }

                // Create reward item
                if (!this.globalItemProvider.TryCreate(id, out var reward))
                {
                    this.monitor.LogWithLocation($"Unable to create instance of {id} for treasure reward, reverting to stone.", LogLevel.Warn);
                    reward = new SObject(ObjectsReference.Stone, 1);
                }

                /*
                if (weightedTreasure.Value.MeleeWeapon)
                {
                    reward = new MeleeWeapon(id);
                }
                else if (id >= Ring.ringLowerIndexRange && id <= Ring.ringUpperIndexRange)
                {
                    reward = new Ring(id);
                }
                else if (id >= 504 && id <= 513)
                {
                    reward = new Boots(id);
                }
                else
                {
                    // Random quantity
                    var count = Game1.random.Next(weightedTreasure.Value.MinAmount, weightedTreasure.Value.MaxAmount);
                    reward = new SObject(Vector2.Zero, id, count);
                }
                */

                // Add the reward
                switch (reward)
                {
                    case SObject sobj:
                    {
                        sobj.Stack = count;
                        rewards.Add(reward);
                        break;
                    }
                    default:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            rewards.Add(reward);
                        }

                        break;
                    }
                }


                // Check if this reward shouldn't be duplicated
                if (!config.AllowDuplicateLoot || !weightedTreasure.Value.AllowDuplicates)
                    possibleLoot.Remove(weightedTreasure);

                // Update chance
                chance *= config.TreasureChances.GetChance(user, this.api.FishingStreak);
                // chance *= config.AdditionalLootChance + streak * config.StreakAdditionalLootChance;
            }

            // Add bait if no rewards were selected. NOTE: This should never happen
            if (rewards.Count == 0)
            {
                this.monitor.Log("Could not find any valid loot for the treasure chest. Check your treasure settings?", LogLevel.Warn);
                rewards.Add(new SObject(685, Game1.random.Next(2, 5) * 5));
            }

            // Show rewards GUI
            this.monitor.Log($"Treasure rewards: {string.Join(", ", rewards)}");
            Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            ((ItemGrabMenu)Game1.activeClickableMenu).source = 3;
            user.completelyStopAnimatingOrDoingAction();
        }
    }
}