using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using TehPers.Core.Api.Enums;
using TehPers.Core.Helpers.Static;
using TehPers.FishingOverhaul.Api;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul {
    internal class FishingRodOverrider {
        private readonly FieldInfo _pullFishField = typeof(FishingRod).GetField("pullFishFromWaterEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly FieldInfo _netEventOnEvent = typeof(AbstractNetEvent1<byte[]>).GetField("onEvent", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly MethodInfo _doPullFishFromWater = typeof(FishingRod).GetMethod("doPullFishFromWater", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly HashSet<FishingRod> _overridden = new HashSet<FishingRod>();
        public readonly HashSet<FishingRod> OverridingCatch = new HashSet<FishingRod>();

        public FishingRodOverrider(IMod mod) {
            mod.Helper.Events.GameLoop.UpdateTicked += Update;
        }

        private void Update(object sender, EventArgs e) {
            // Make sure the player has a fishing rod equipped
            if (!(Game1.player.CurrentTool is FishingRod rod))
                return;

            // Override the pullFishFromWater event (for trash)
            if (_overridden.Add(rod))
                OverridePullFishEvent(rod);

            if (!rod.isFishing) {
                OverridingCatch.Remove(rod);
            }

            // Check if the rod hit a fish
            var catching = false;
            for (var i = 0; i < Game1.screenOverlayTempSprites.Count; i++) {
                var animation = Game1.screenOverlayTempSprites[i];
                if (animation.endFunction != rod.startMinigameEndFunction) continue;
                Game1.screenOverlayTempSprites.RemoveAt(i);
                catching = true;
                i--;
            }

            // Replace catch if it did
            if (catching) {
                OnPullFromNibble(rod, Game1.player);
            }

            if (Game1.IsMultiplayer && !Game1.IsServer) return;
            // Replace the end function for the custom treasure
            foreach (var anim in rod.animations.Where(anim => anim.endFunction == rod.openTreasureMenuEndFunction))
            {
                anim.endFunction = extra => OpenTreasureEndFunction(rod, Game1.player, extra);
            }
        }

        private void OverridePullFishEvent(FishingRod rod) {
            // Make sure the pullFishFromWaterEvent has a value (it should)
            if (!(_pullFishField?.GetValue(rod) is NetEventBinary pullFishEvent))
                return;

            // Try to get the event field
            if (!(_netEventOnEvent?.GetValue(pullFishEvent) is AbstractNetEvent1<byte[]>.Event eventField))
                return;

            // Remove all the handlers from the event
            foreach (var method in eventField.GetInvocationList()) {
                if (method is AbstractNetEvent1<byte[]>.Event handler) {
                    pullFishEvent.onEvent -= handler;
                } else {
                    ModFishing.Instance.Monitor.Log($"Failed to remove {method} ({method.Method.Name}) from {nameof(AbstractNetEvent1<byte[]>.onEvent)}.", LogLevel.Warn);
                }
            }

            // Add a new event handler
            pullFishEvent.AddReaderHandler(reader => DoPullFishFromWater(rod, ModFishing.Instance.Helper.Reflection.GetField<Farmer>(rod, "lastUser").GetValue(), reader));
            ModFishing.Instance.Monitor.Log("Events overridden for fishing rod");
        }

        private void DoPullFishFromWater(FishingRod rod, Farmer user, BinaryReader reader) {
            // Check if this should be overridden
            if (OverridingCatch.Remove(rod)) {
                // Call normal handler
                ModFishing.Instance.Monitor.Log("Calling vanilla fishing logic");
                _doPullFishFromWater.Invoke(rod, new object[] { reader });
            } else {
                // Override this catch (should be trash or something)
                OnPullFromNibble(rod, user);
            }
        }

        private void OnPullFromNibble(FishingRod rod, Farmer user) {
            // Make sure this rod doesn't get the pullFishFromWater event overridden when it shouldn't be
            OverridingCatch.Add(rod);
            ModFishing.Instance.Monitor.Log("Overriding vanilla catch");

            if (rod == null) {
                throw new Exception("Fishing rod is null, please report this");
            }

            if (user == null) {
                throw new Exception("User is null, please report this");
            }

            if (!rod.isFishing) {
                // MBD: Not sure if its always been this way, but in SMAPI 3.1/SDV 1.4 the rod can still generate hits when treasure is being displayed.
                //      Acting on such hits puts the mod into a catching loop that denies the user their treasure, perpetually displays the catch bar
                //      until the user cancels (and loses their streak) and doesn't award the fish.  To prevent this, make sure the rod is actually
                //      fishing before acting on a nibble.
                return;
            }

            // Get some info about the rod
            var clearWaterDistance = ModFishing.Instance.Helper.Reflection.GetField<int>(rod, "clearWaterDistance").GetValue();
            var location = user.currentLocation;

            int? fish = null;

            // Check if legendary fish are being overridden as well
            if (!ModFishing.Instance.MainConfig.CustomLegendaries) {
                // Check if a legendary would be caught
                var baitValue = rod.attachments[0]?.Price / 10.0 ?? 0.0;
                var bubblyZone = false;
                if (!(location.fishSplashPoint is null) && location.fishSplashPoint.Value != Point.Zero)
                    bubblyZone = new Rectangle(location.fishSplashPoint.X * 64, location.fishSplashPoint.Y * 64, 64, 64).Intersects(new Rectangle((int) rod.bobber.X - 80, (int) rod.bobber.Y - 80, 64, 64));

                // NotNull
                var normalFish = location.getFish(rod.fishingNibbleAccumulator, rod.attachments[0]?.ParentSheetIndex ?? -1, clearWaterDistance + (bubblyZone ? 1 : 0), user, baitValue + (bubblyZone ? 0.4 : 0.0), rod.bobber);

                // If so, select that fish
                if (ModFishing.Instance.Api.IsLegendary(normalFish.ParentSheetIndex)) {
                    fish = normalFish.ParentSheetIndex;
                }
            }
            var rodTile = new Vector2(rod.bobber.X / 64, rod.bobber.Y / 64);
            if (location is BuildableGameLocation buildableLocation && buildableLocation.getBuildingAt(rodTile) is FishPond activePond) {
                rod.pullFishFromWater(activePond.fishType.Value, -1, 0, 0, false, false, false);
                return;
            }

            if (location.Name.Equals("WitchSwamp") && !Game1.MasterPlayer.mailReceived.Contains("henchmanGone") && Game1.random.NextDouble() < 0.25 && !Game1.player.hasItemInInventory(308, 1)) {
                // Void mayonnaise
                rod.pullFishFromWater(308, -1, 0, 0, false, false, false);
                return;
            }

            // Choose a random fish if one hasn't been chosen yet
            fish ??= FishHelper.GetRandomFish(user);

            // Check if a fish was chosen
            if (fish == null) {
                // Secret note
                if (user.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08) {
                    var unseenSecretNote = location.tryToCreateUnseenSecretNote(user);
                    if (unseenSecretNote != null) {
                        rod.pullFishFromWater(unseenSecretNote.ParentSheetIndex, -1, 0, 0, false, false, false);
                        return;
                    }
                }

                // Trash
                var trash = FishHelper.GetRandomTrash(user);
                if (trash.HasValue) {
                    // Invoke event
                    var eventArgs = new FishingEventArgs(trash.Value, user, rod);
                    ModFishing.Instance.Api.OnTrashCaught(eventArgs);
                    trash = eventArgs.ParentSheetIndex;

                    ModFishing.Instance.Monitor.Log($"Catching trash: {trash}");
                    rod.pullFishFromWater(trash.Value, -1, 0, 0, false, false, false);
                } else {
                    ModFishing.Instance.Monitor.Log($"No possible trash found for {location.Name}, using stone instead. This is probably caused by another mod removing trash data.", LogLevel.Warn);
                    rod.pullFishFromWater(Objects.Stone, -1, 0, 0, false, false, false);
                }
            } else {
                // Invoke event
                var eventArgs = new FishingEventArgs(fish.Value, user, rod);
                ModFishing.Instance.Api.OnBeforeFishCatching(eventArgs);
                fish = eventArgs.ParentSheetIndex;

                // Check if favBait should be set to true (still not sure what this does...)
                var fishObject = new SObject(fish.Value, 1);
                if (Math.Abs(fishObject.Scale.X - 1.0) < 0.001)
                    rod.favBait = true;

                // Show hit animation
                rod.hit = true;
                ModFishing.Instance.Monitor.Log($"Catching fish: {fish}");
                Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, rod.bobber.Value + new Vector2(-140f, -160f)), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true) {
                    scaleChangeChange = -0.005f,
                    motion = new Vector2(0.0f, -0.1f),
                    endFunction = extra => StartMinigameEndFunction(rod, user, extra),
                    extraInfoForEndBehavior = fish.Value
                });
                location.localSound("FishHit");
            }
        }

        private void StartMinigameEndFunction(FishingRod rod, Farmer user, int fish) {
            rod.isReeling = true;
            rod.hit = false;

            // Animation
            switch (user.FacingDirection) {
                case 1:
                    user.FarmerSprite.setCurrentSingleFrame(48);
                    break;
                case 3:
                    user.FarmerSprite.setCurrentSingleFrame(48, 32000, false, true);
                    break;
            }
            user.FarmerSprite.PauseForSingleAnimation = true;

            // Distance from bobber to land
            var clearWaterDistanceField = ModFishing.Instance.Helper.Reflection.GetField<int>(rod, "clearWaterDistance");
            clearWaterDistanceField.SetValue(FishingRod.distanceToLand((int) (rod.bobber.X / 64.0 - 1.0), (int) (rod.bobber.Y / 64.0 - 1.0), user.currentLocation));

            // Calculate size of fish
            var num = 1f * (clearWaterDistanceField.GetValue() / 5f) * (Game1.random.Next(1 + Math.Min(10, user.FishingLevel) / 2, 6) / 5f);
            if (rod.favBait)
                num *= 1.2f;
            var fishSize = Math.Max(0.0f, Math.Min(1f, num * (float) (1.0 + Game1.random.Next(-10, 10) / 100.0)));

            // Check if there should be treasure
            var treasure = !Game1.isFestival();
            treasure &= user.fishCaught != null && user.fishCaught.FieldDict.Count > 1;
            treasure &= Game1.random.NextDouble() < ModFishing.Instance.Api.GetTreasureChance(user, rod);
            Game1.activeClickableMenu = new CustomBobberBar(user, fish, fishSize, treasure, rod.attachments[1]?.ParentSheetIndex ?? -1);
        }

        private void OpenTreasureEndFunction(FishingRod rod, Farmer user, int includeFish) {
            ModFishing.Instance.Monitor.Log("Successfully replaced treasure");

            var config = ModFishing.Instance.MainConfig.GlobalTreasureSettings;
            var clearWaterDistance = ModFishing.Instance.Helper.Reflection.GetField<int>(rod, "clearWaterDistance")?.GetValue() ?? 5;

            var whichFish = ModFishing.Instance.Helper.Reflection.GetField<int>(rod, "whichFish").GetValue();
            var fishQuality = ModFishing.Instance.Helper.Reflection.GetField<int>(rod, "fishQuality").GetValue();

            // Gain experience and call vanilla code for this
            user.gainExperience(5, 10 * (clearWaterDistance + 1));
            rod.doneFishing(user, true);
            user.completelyStopAnimatingOrDoingAction();

            // REWARDS
            var rewards = new List<Item>();
            if (includeFish == 1)
                rewards.Add(new SObject(whichFish, 1, false, -1, fishQuality));

            var possibleLoot = new List<ITreasureData>(ModFishing.Instance.TreasureConfig.PossibleLoot)
                .Where(treasure => treasure.IsValid(user)).ToList();

            // Select rewards
            var chance = 1f;
            var streak = ModFishing.Instance.Api.GetStreak(user);
            while (possibleLoot.Count > 0 && rewards.Count < config.MaxTreasureQuantity && Game1.random.NextDouble() <= chance) {
                var treasure = possibleLoot.Choose(Game1.random);

                // Choose an ID for the treasure
                var ids = treasure.PossibleIds();
                var id = ids[Game1.random.Next(ids.Count)];

                // Lost books have custom handling
                if (id == Objects.LostBook) {
                    if (user.archaeologyFound == null || !user.archaeologyFound.ContainsKey(102) || user.archaeologyFound[102][0] >= 21) {
                        possibleLoot.Remove(treasure);
                        continue;
                    }
                    Game1.showGlobalMessage("You found a lost book. The library has been expanded.");
                }

                // Create reward item
                Item reward;
                if (treasure.MeleeWeapon) {
                    reward = new MeleeWeapon(id);
                } else if (id >= Ring.ringLowerIndexRange && id <= Ring.ringUpperIndexRange) {
                    reward = new Ring(id);
                } else if (id >= 504 && id <= 513) {
                    reward = new Boots(id);
                } else {
                    // Random quantity
                    var count = Game1.random.Next(treasure.MinAmount, treasure.MaxAmount);

                    reward = new SObject(Vector2.Zero, id, count);
                }

                // Add the reward
                rewards.Add(reward);

                // Check if this reward shouldn't be duplicated
                if (!config.AllowDuplicateLoot || !treasure.AllowDuplicates)
                    possibleLoot.Remove(treasure);

                // Update chance
                chance *= config.AdditionalLootChance + streak * config.StreakAdditionalLootChance;
            }

            // Add bait if no rewards were selected. NOTE: This should never happen
            if (rewards.Count == 0) {
                ModFishing.Instance.Monitor.Log("Could not find any valid loot for the treasure chest. Check your treasure.json?", LogLevel.Warn);
                rewards.Add(new SObject(685, Game1.random.Next(2, 5) * 5));
            }

            // Show rewards GUI
            ModFishing.Instance.Monitor.Log($"Treasure rewards: {string.Join(", ", rewards)}");
            Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            ((ItemGrabMenu) Game1.activeClickableMenu).source = 3;
            user.completelyStopAnimatingOrDoingAction();
        }
    }
}
