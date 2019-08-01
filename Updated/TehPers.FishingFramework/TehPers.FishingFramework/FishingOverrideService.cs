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
using TehPers.Core.DependencyInjection.Api.Lifecycle.GameLoop;
using TehPers.FishingFramework.Api;
using Object = StardewValley.Object;

namespace TehPers.FishingFramework
{
    internal class FishingOverrideService : IUpdateTickedHandler
    {
        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;
        private readonly IFishingApi _api;
        private readonly FieldInfo _pullFishField;
        private readonly FieldInfo _netEventOnEvent;
        private readonly MethodInfo _doPullFishFromWater;
        private readonly HashSet<FishingRod> _overridden;

        public HashSet<FishingRod> OverridingCatch { get; }

        public FishingOverrideService(IMonitor monitor, IModHelper helper, IFishingApi api)
        {
            this._monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this._helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this._api = api ?? throw new ArgumentNullException(nameof(api));

            this._pullFishField = typeof(FishingRod).GetField("pullFishFromWaterEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            this._netEventOnEvent = typeof(AbstractNetEvent1<byte[]>).GetField("onEvent", BindingFlags.Instance | BindingFlags.NonPublic);
            this._doPullFishFromWater = typeof(FishingRod).GetMethod("doPullFishFromWater", BindingFlags.NonPublic | BindingFlags.Instance);
            this._overridden = new HashSet<FishingRod>();

            this.OverridingCatch = new HashSet<FishingRod>();
        }

        public void OnUpdateTicked(object sender, UpdateTickedEventArgs args)
        {
            // Only the server should override fishing
            if (Game1.IsMultiplayer && !Game1.IsServer)
            {
                return;
            }

            // Handle all farmers at once
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                // Make sure the player has a fishing rod equipped
                if (!(farmer.CurrentTool is FishingRod rod))
                {
                    continue;
                }

                // Override the pullFishFromWater event (for trash)
                if (this._overridden.Add(rod))
                {
                    this.OverridePullFishEvent(rod);
                }

                if (!rod.isFishing)
                {
                    this.OverridingCatch.Remove(rod);
                }

                // Check if the rod hit a fish
                bool catching = false;
                for (int i = 0; i < Game1.screenOverlayTempSprites.Count; i++)
                {
                    TemporaryAnimatedSprite animation = Game1.screenOverlayTempSprites[i];
                    if (animation.endFunction == rod.startMinigameEndFunction)
                    {
                        Game1.screenOverlayTempSprites.RemoveAt(i);
                        catching = true;
                        i--;
                    }
                }

                // Replace catch if it did
                if (catching)
                {
                    this.OnPullFromNibble(rod, Game1.player);
                }

                // Replace the end function for the custom treasure
                foreach (TemporaryAnimatedSprite anim in rod.animations)
                {
                    if (anim.endFunction == rod.openTreasureMenuEndFunction)
                    {
                        anim.endFunction = extra => this.OpenTreasureEndFunction(rod, Game1.player, extra);
                    }
                }
            }
        }

        private void OverridePullFishEvent(FishingRod rod)
        {
            // Make sure the pullFishFromWaterEvent has a value (it should)
            if (!(this._pullFishField?.GetValue(rod) is NetEventBinary pullFishEvent))
                return;

            // Try to get the event field
            if (!(this._netEventOnEvent?.GetValue(pullFishEvent) is AbstractNetEvent1<byte[]>.Event eventField))
                return;

            // Remove all the handlers from the event
            foreach (Delegate method in eventField.GetInvocationList())
            {
                if (method is AbstractNetEvent1<byte[]>.Event handler)
                {
                    pullFishEvent.onEvent -= handler;
                }
                else
                {
                    this._monitor.Log($"Failed to remove {method} ({method.Method.Name}) from {nameof(AbstractNetEvent1<byte[]>.onEvent)}.", LogLevel.Warn);
                }
            }

            // Add a new event handler
            pullFishEvent.AddReaderHandler(reader => this.DoPullFishFromWater(rod, this._helper.Reflection.GetField<Farmer>(rod, "lastUser").GetValue(), reader));
            this._monitor.Log("Events overridden for fishing rod", LogLevel.Trace);
        }

        private void DoPullFishFromWater(FishingRod rod, Farmer user, BinaryReader reader)
        {
            // Check if this should be overridden
            if (this.OverridingCatch.Remove(rod))
            {
                // Call normal handler
                this._monitor.Log("Calling vanilla fishing logic", LogLevel.Trace);
                this._doPullFishFromWater.Invoke(rod, new object[] { reader });
            }
            else
            {
                // Override this catch (should be trash or something)
                this.OnPullFromNibble(rod, user);
            }
        }

        private void OnPullFromNibble(FishingRod rod, Farmer user)
        {
            // Make sure this rod doesn't get the pullFishFromWater event overridden when it shouldn't be
            this.OverridingCatch.Add(rod);
            this._monitor.Log("Overriding vanilla catch", LogLevel.Trace);

            if (rod == null)
            {
                throw new Exception("Fishing rod is null, please report this");
            }

            if (user == null)
            {
                throw new Exception("User is null, please report this");
            }

            // Get some info about the rod
            int clearWaterDistance = this._helper.Reflection.GetField<int>(rod, "clearWaterDistance").GetValue();
            GameLocation location = user.currentLocation;

            int? fish = null;

            // Check if legendary fish are being overridden as well
            if (!ModFishing.Instance.MainConfig.CustomLegendaries)
            {
                // Check if a legendary would be caught
                double baitValue = rod.attachments[0]?.Price / 10.0 ?? 0.0;
                bool bubblyZone = false;
                if (!(location.fishSplashPoint is null) && location.fishSplashPoint.Value != Point.Zero)
                    bubblyZone = new Rectangle(location.fishSplashPoint.X * 64, location.fishSplashPoint.Y * 64, 64, 64).Intersects(new Rectangle((int)rod.bobber.X - 80, (int)rod.bobber.Y - 80, 64, 64));

                // NotNull
                Object normalFish = location.getFish(rod.fishingNibbleAccumulator, rod.attachments[0]?.ParentSheetIndex ?? -1, clearWaterDistance + (bubblyZone ? 1 : 0), user, baitValue + (bubblyZone ? 0.4 : 0.0));

                // If so, select that fish
                if (ModFishing.Instance.Api.IsLegendary(normalFish.ParentSheetIndex))
                {
                    fish = normalFish.ParentSheetIndex;
                }
            }

            // Void mayonnaise
            if (location.Name.Equals("WitchSwamp") && !Game1.MasterPlayer.mailReceived.Contains("henchmanGone") && Game1.random.NextDouble() < 0.25 && !Game1.player.hasItemInInventory(308, 1))
            {
                rod.pullFishFromWater(308, -1, 0, 0, false);
                return;
            }

            // Choose a random fish if one hasn't been chosen yet
            if (fish == null)
            {
                fish = FishHelper.GetRandomFish(user);
            }

            // Check if a fish was chosen
            if (fish == null)
            {
                // Secret note
                if (user.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08)
                {
                    Object unseenSecretNote = location.tryToCreateUnseenSecretNote(user);
                    if (unseenSecretNote != null)
                    {
                        rod.pullFishFromWater(unseenSecretNote.ParentSheetIndex, -1, 0, 0, false);
                        return;
                    }
                }

                // Trash
                int? trash = FishHelper.GetRandomTrash(user);
                if (trash.HasValue)
                {
                    // Invoke event
                    FishingEventArgs eventArgs = new FishingEventArgs(trash.Value, user, rod);
                    this._api.OnTrashCaught(eventArgs);
                    trash = eventArgs.ParentSheetIndex;

                    this._monitor.Log($"Catching trash: {trash}", LogLevel.Trace);
                    rod.pullFishFromWater(trash.Value, -1, 0, 0, false);
                }
                else
                {
                    this._monitor.Log($"No possible trash found for {location.Name}, using stone instead. This is probably caused by another mod removing trash data.", LogLevel.Warn);
                    rod.pullFishFromWater(Objects.Stone, -1, 0, 0, false);
                }
            }
            else
            {
                // Invoke event
                FishingEventArgs eventArgs = new FishingEventArgs(fish.Value, user, rod);
                this._api.OnBeforeFishCatching(eventArgs);
                fish = eventArgs.ParentSheetIndex;

                // Check if favBait should be set to true (still not sure what this does...)
                Object fishObject = new Object(fish.Value, 1);
                if (Math.Abs(fishObject.Scale.X - 1.0) < 0.001)
                    rod.favBait = true;

                // Show hit animation
                rod.hit = true;
                this._monitor.Log($"Catching fish: {fish}", LogLevel.Trace);
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
            IReflectedField<int> clearWaterDistanceField = this._helper.Reflection.GetField<int>(rod, "clearWaterDistance");
            clearWaterDistanceField.SetValue(FishingRod.distanceToLand((int)(rod.bobber.X / 64.0 - 1.0), (int)(rod.bobber.Y / 64.0 - 1.0), user.currentLocation));

            // Calculate size of fish
            float num = 1f * (clearWaterDistanceField.GetValue() / 5f) * (Game1.random.Next(1 + Math.Min(10, user.FishingLevel) / 2, 6) / 5f);
            if (rod.favBait)
                num *= 1.2f;
            float fishSize = Math.Max(0.0f, Math.Min(1f, num * (float)(1.0 + Game1.random.Next(-10, 10) / 100.0)));

            // Check if there should be treasure
            bool treasure = !Game1.isFestival();
            treasure &= user.fishCaught != null && user.fishCaught.Count > 1;
            treasure &= Game1.random.NextDouble() < this._api.GetTreasureChance(user, rod);
            Game1.activeClickableMenu = new CustomBobberBar(this._mod, user, fish, fishSize, treasure, rod.attachments[1]?.ParentSheetIndex ?? -1);
        }

        private void OpenTreasureEndFunction(FishingRod rod, Farmer user, int includeFish)
        {
            this._monitor.Log("Successfully replaced treasure", LogLevel.Trace);

            ConfigMain.ConfigGlobalTreasure config = ModFishing.Instance.MainConfig.GlobalTreasureSettings;
            int clearWaterDistance = this._helper.Reflection.GetField<int>(rod, "clearWaterDistance")?.GetValue() ?? 5;

            int whichFish = this._helper.Reflection.GetField<int>(rod, "whichFish").GetValue();
            int fishQuality = this._helper.Reflection.GetField<int>(rod, "fishQuality").GetValue();

            // Gain experience and call vanilla code for this
            user.gainExperience(5, 10 * (clearWaterDistance + 1));
            rod.doneFishing(user, true);
            user.completelyStopAnimatingOrDoingAction();

            // REWARDS
            List<Item> rewards = new List<Item>();
            if (includeFish == 1)
                rewards.Add(new Object(whichFish, 1, false, -1, fishQuality));

            List<IWeightedElement<ITreasureData>> possibleLoot = ModFishing.Instance.TreasureConfig.PossibleLoot
                .Cast<ITreasureData>()
                .Where(treasure => treasure.IsValid(user))
                .ToWeighted(treasure => treasure.GetWeight(), treasure => treasure)
                .ToList();

            // Select rewards
            float chance = 1f;
            int streak = this._api.GetStreak(user);
            while (possibleLoot.Count > 0 && rewards.Count < config.MaxTreasureQuantity && Game1.random.NextDouble() <= chance)
            {
                IWeightedElement<ITreasureData> weightedTreasure = possibleLoot.Choose(Game1.random);

                // Choose an ID for the treasure
                IList<int> ids = weightedTreasure.Value.PossibleIds();
                int id = ids[Game1.random.Next(ids.Count)];

                // Lost books have custom handling
                if (id == Objects.LostBook)
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
                    int count = Game1.random.Next(weightedTreasure.Value.MinAmount, weightedTreasure.Value.MaxAmount);

                    reward = new Object(Vector2.Zero, id, count);
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
                this._monitor.Log("Could not find any valid loot for the treasure chest. Check your treasure.json?", LogLevel.Warn);
                rewards.Add(new Object(685, Game1.random.Next(2, 5) * 5));
            }

            // Show rewards GUI
            this._monitor.Log($"Treasure rewards: {string.Join(", ", rewards)}", LogLevel.Trace);
            Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            ((ItemGrabMenu)Game1.activeClickableMenu).source = 3;
            user.completelyStopAnimatingOrDoingAction();
        }
    }
}