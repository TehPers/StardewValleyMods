using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using TehPers.Core.Api.DependencyInjection.Lifecycle.GameLoop;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Weighted;
using TehPers.FishingFramework.Api;
using TehPers.FishingFramework.Api.Extensions;
using SObject = StardewValley.Object;

namespace TehPers.FishingFramework
{
    internal class FishingOverrideService : IUpdateTickedHandler
    {
        private readonly IMonitor monitor;
        private readonly IModHelper helper;
        private readonly IFishingApi api;
        private readonly FieldInfo pullFishField;
        private readonly FieldInfo netEventOnEvent;
        private readonly MethodInfo doPullFishFromWater;
        private readonly HashSet<FishingRod> overridden;

        public HashSet<FishingRod> OverridingCatch { get; }

        public FishingOverrideService(IMonitor monitor, IModHelper helper, FishingApi api)
        {
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.api = api ?? throw new ArgumentNullException(nameof(api));

            this.pullFishField = typeof(FishingRod).GetField("pullFishFromWaterEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            this.netEventOnEvent = typeof(AbstractNetEvent1<byte[]>).GetField("onEvent", BindingFlags.Instance | BindingFlags.NonPublic);
            this.doPullFishFromWater = typeof(FishingRod).GetMethod("doPullFishFromWater", BindingFlags.NonPublic | BindingFlags.Instance);
            this.overridden = new HashSet<FishingRod>();

            this.OverridingCatch = new HashSet<FishingRod>();
        }

        public void OnUpdateTicked(object sender, UpdateTickedEventArgs args)
        {
            // Get the client's farmer
            var currentFarmer = Game1.player;

            // Make sure the farmer has a fishing rod equipped
            if (!(currentFarmer.CurrentTool is FishingRod rod))
            {
                return;
            }

            // Override the pullFishFromWater event (for trash)
            if (this.overridden.Add(rod))
            {
                this.OverridePullFishEvent(rod);
            }

            if (!rod.isFishing)
            {
                this.OverridingCatch.Remove(rod);
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
                    // anim.endFunction = extra => this.OpenTreasureEndFunction(rod, Game1.player, extra);
                }
            }
        }

        private void OverridePullFishEvent(FishingRod rod)
        {
            // Make sure the pullFishFromWaterEvent has a value (it should)
            if (!(this.pullFishField?.GetValue(rod) is NetEventBinary pullFishEvent))
                return;

            // Try to get the event field
            if (!(this.netEventOnEvent?.GetValue(pullFishEvent) is AbstractNetEvent1<byte[]>.Event eventField))
                return;

            // Remove all the handlers from the event
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
            this.monitor.Log("Events overridden for fishing rod", LogLevel.Trace);
        }

        private void DoPullFishFromWater(FishingRod rod, Farmer user, BinaryReader reader)
        {
            // Check if this should be overridden
            if (this.OverridingCatch.Remove(rod))
            {
                // Call normal handler
                this.monitor.Log("Calling vanilla fishing logic", LogLevel.Trace);
                this.doPullFishFromWater.Invoke(rod, new object[] { reader });
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
            this.OverridingCatch.Add(rod);

            // Get some info about the rod
            var clearWaterDistance = this.helper.Reflection.GetField<int>(rod, "clearWaterDistance").GetValue();
            var location = user.currentLocation;
            var bobberTile = new Vector2(rod.bobber.X / 64F, rod.bobber.Y / 64F);

            int? fish = null;

            // Check if legendary fish are being overridden as well
            if (!this.api.GlobalConfig.ShouldOverrideVanillaLegendaries)
            {
                // Check if a legendary would be caught
                var baitValue = rod.attachments[0]?.Price / 10.0 ?? 0.0;
                var bubblyZone = false;
                if (!(location.fishSplashPoint is null) && location.fishSplashPoint.Value != Point.Zero)
                    bubblyZone = new Rectangle(location.fishSplashPoint.X * 64, location.fishSplashPoint.Y * 64, 64, 64).Intersects(new Rectangle((int)rod.bobber.X - 80, (int)rod.bobber.Y - 80, 64, 64));
                var normalFish = location.getFish(rod.fishingNibbleAccumulator, rod.attachments[0]?.ParentSheetIndex ?? -1, clearWaterDistance + (bubblyZone ? 1 : 0), user, baitValue + (bubblyZone ? 0.4 : 0.0), bobberTile, user.currentLocation.Name);

                // If so, select that fish
                if (this.api.IsLegendaryFish(normalFish.ParentSheetIndex))
                {
                    fish = normalFish.ParentSheetIndex;
                }
            }

            // Void mayonnaise
            if (location.Name.Equals("WitchSwamp") && !Game1.MasterPlayer.mailReceived.Contains("henchmanGone") && Game1.random.NextDouble() < 0.25 && !Game1.player.hasItemInInventory(308, 1))
            {
                rod.pullFishFromWater(308, -1, 0, 0, false, false, false);
                return;
            }

            // Choose a random fish if one hasn't been chosen yet
            if (fish == null && Game1.random.NextDouble() < this.api.GetChance(this.api.FishChances, user, this.api.FishingStreak))
            {
                var fishKey = this.api.Fish.WhereAvailable(user).Choose(Game1.random).Value;
                fish = default;
            }

            // Check if a fish was chosen
            if (fish == null)
            {
                // Secret note
                if (user.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08)
                {
                    var unseenSecretNote = location.tryToCreateUnseenSecretNote(user);
                    if (unseenSecretNote != null)
                    {
                        rod.pullFishFromWater(unseenSecretNote.ParentSheetIndex, -1, 0, 0, false, false, false);
                        return;
                    }
                }

                // Trash
                int? trash = this.api.TrashByLocationName.GetRandomTrash(user);
                if (trash.HasValue)
                {
                    // Invoke event
                    FishingEventArgs eventArgs = new FishingEventArgs(trash.Value, user, rod);
                    this.api.OnTrashCaught(eventArgs);
                    trash = eventArgs.ParentSheetIndex;

                    this.monitor.Log($"Catching trash: {trash}", LogLevel.Trace);
                    rod.pullFishFromWater(trash.Value, -1, 0, 0, false, false, false);
                }
                else
                {
                    this.monitor.Log($"No possible trash found for {location.Name}, using stone instead. This is probably caused by another mod removing trash data.", LogLevel.Warn);
                    rod.pullFishFromWater(ObjectsReference.Stone, -1, 0, 0, false, false, false);
                }
            }
            else
            {
                // Invoke event
                var eventArgs = new FishingEventArgs(fish.Value, user, rod);
                this.api.OnFishCatching(eventArgs);
                fish = eventArgs.ParentSheetIndex;

                // Check if favBait should be set to true (still not sure what this does...)
                var fishObject = new SObject(fish.Value, 1);
                if (Math.Abs(fishObject.Scale.X - 1.0) < 0.001)
                    rod.favBait = true;

                // Show hit animation
                rod.hit = true;
                this.monitor.Log($"Catching fish: {fish}", LogLevel.Trace);
                Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, rod.bobber.Value + new Vector2(-140f, -160f)), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true)
                {
                    scaleChangeChange = -0.005f,
                    motion = new Vector2(0.0f, -0.1f),
                    endFunction = extra => this.StartMinigameEndFunction(rod, user, extra),
                    extraInfoForEndBehavior = fish.Value
                });
                location.localSound("FishHit");
            }
        }

        private void StartMinigameEndFunction(FishingRod rod, Farmer user, int fish)
        {
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
                default:
                    break;
            }
            user.FarmerSprite.PauseForSingleAnimation = true;

            // Distance from bobber to land
            var clearWaterDistanceField = this.helper.Reflection.GetField<int>(rod, "clearWaterDistance");
            clearWaterDistanceField.SetValue(FishingRod.distanceToLand((int)(rod.bobber.X / 64.0 - 1.0), (int)(rod.bobber.Y / 64.0 - 1.0), user.currentLocation));

            // Calculate size of fish
            var num = 1f * (clearWaterDistanceField.GetValue() / 5f) * (Game1.random.Next(1 + Math.Min(10, user.FishingLevel) / 2, 6) / 5f);
            if (rod.favBait)
                num *= 1.2f;
            var fishSize = Math.Max(0.0f, Math.Min(1f, num * (float)(1.0 + Game1.random.Next(-10, 10) / 100.0)));

            // Check if there should be treasure
            var treasure = !Game1.isFestival();
            treasure &= user.fishCaught != null && user.fishCaught.Count > 1;
            treasure &= Game1.random.NextDouble() < this.api.GetTreasureChance(user, rod);
            Game1.activeClickableMenu = new CustomBobberBar(this._mod, user, fish, fishSize, treasure, rod.attachments[1]?.ParentSheetIndex ?? -1);
        }

        private void OpenTreasureEndFunction(FishingRod rod, Farmer user, int includeFish)
        {
            this.monitor.Log("Successfully replaced treasure", LogLevel.Trace);

            ConfigMain.ConfigGlobalTreasure config = ModFishing.Instance.MainConfig.GlobalTreasureSettings;
            var clearWaterDistance = this.helper.Reflection.GetField<int>(rod, "clearWaterDistance")?.GetValue() ?? 5;

            var whichFish = this.helper.Reflection.GetField<int>(rod, "whichFish").GetValue();
            var fishQuality = this.helper.Reflection.GetField<int>(rod, "fishQuality").GetValue();

            // Gain experience and call vanilla code for this
            user.gainExperience(5, 10 * (clearWaterDistance + 1));
            rod.doneFishing(user, true);
            user.completelyStopAnimatingOrDoingAction();

            // REWARDS
            var rewards = new List<Item>();
            if (includeFish == 1)
                rewards.Add(new SObject(whichFish, 1, false, -1, fishQuality));

            List<IWeightedValue<ITreasureAvailability>> possibleLoot = ModFishing.Instance.TreasureConfig.PossibleLoot
                .Cast<ITreasureAvailability>()
                .Where(treasure => treasure.IsValid(user))
                .ToWeighted(treasure => treasure.GetWeight(), treasure => treasure)
                .ToList();

            // Select rewards
            var chance = 1f;
            int streak = this.api.GetStreak(user);
            while (possibleLoot.Count > 0 && rewards.Count < config.MaxTreasureQuantity && Game1.random.NextDouble() <= chance)
            {
                var weightedTreasure = possibleLoot.Choose(Game1.random);

                // Choose an ID for the treasure
                IList<int> ids = weightedTreasure.Value.PossibleIds();
                var id = ids[Game1.random.Next(ids.Count)];

                // Lost books have custom handling
                if (id == ObjectsReference.LostBook)
                {
                    if (user.archaeologyFound == null || !user.archaeologyFound.ContainsKey(102) || user.archaeologyFound[102][0] >= 21)
                    {
                        possibleLoot.Remove(weightedTreasure);
                        continue;
                    }
                    Game1.showGlobalMessage("You found a lost book. The library has been expanded.");
                }

                // Create reward item
                Item reward;
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

                // Add the reward
                rewards.Add(reward);

                // Check if this reward shouldn't be duplicated
                if (!config.AllowDuplicateLoot || !weightedTreasure.Value.AllowDuplicates)
                    possibleLoot.Remove(weightedTreasure);

                // Update chance
                chance *= config.AdditionalLootChance + streak * config.StreakAdditionalLootChance;
            }

            // Add bait if no rewards were selected. NOTE: This should never happen
            if (rewards.Count == 0)
            {
                this.monitor.Log("Could not find any valid loot for the treasure chest. Check your treasure.json?", LogLevel.Warn);
                rewards.Add(new SObject(685, Game1.random.Next(2, 5) * 5));
            }

            // Show rewards GUI
            this.monitor.Log($"Treasure rewards: {string.Join(", ", rewards)}", LogLevel.Trace);
            Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            ((ItemGrabMenu)Game1.activeClickableMenu).source = 3;
            user.completelyStopAnimatingOrDoingAction();
        }
    }
}