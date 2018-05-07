using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FishingOverhaul.Configs;
using Microsoft.Xna.Framework;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using TehCore;
using TehCore.Enums;
using TehCore.Helpers;
using TehCore.Saves;
using SObject = StardewValley.Object;

namespace FishingOverhaul {
    public class CustomFishingRod : FishingRod, ICustomItem<CustomFishingRod.RodData> {
        private readonly ReflectedField<FishingRod, int> _clearWaterDistance;
        private readonly ReflectedField<FishingRod, int> _whichFish;
        private readonly ReflectedField<FishingRod, int> _fishQuality;

        public CustomFishingRod() {
            // this.isFishing {bool} - whether the rod is tossed
            // this.castedButBobberStillInAir {bool} - whether the bobber is in midair (tossing animation)
            // this.pullingOutOfWater {bool} - whether the player is pulling a fish out of the water (pulling animation)
            // this.isNibbling {bool} - whether the bobber is shaking and the player can catch something ('!' appears over player's head)
            // this.hit {bool} - whether a fish has been hit ("HIT" appears)

            this._clearWaterDistance = new ReflectedField<FishingRod, int>(this, "clearWaterDistance");
            this._whichFish = new ReflectedField<FishingRod, int>(this, "whichFish");
            this._fishQuality = new ReflectedField<FishingRod, int>(this, "fishQuality");
        }

        public CustomFishingRod(FishingRod other) : this() {
            // Copy over properties
            this.UpgradeLevel = other.UpgradeLevel;
            this.IndexOfMenuItemView = other.IndexOfMenuItemView;
            this.numAttachmentSlots.Value = other.numAttachmentSlots.Value;

            // Copy over attachments
            for (int i = 0; i < other.attachments.Count; i++) {
                this.attachments.Add(other.attachments[i]);
            }
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who) {
            if (!who.IsLocalPlayer) {
                base.DoFunction(location, x, y, power, who);
                return;
            }

            // DEBUG: Always ready to catch a fish
            if (this.isFishing)
                this.isNibbling = true;

            if (this.isFishing && this.isNibbling) {
                // Animation
                who.FarmerSprite.PauseForSingleAnimation = false;
                switch (who.FacingDirection) {
                    case 0:
                        who.FarmerSprite.animateBackwardsOnce(299, 35f);
                        break;
                    case 1:
                        who.FarmerSprite.animateBackwardsOnce(300, 35f);
                        break;
                    case 2:
                        who.FarmerSprite.animateBackwardsOnce(301, 35f);
                        break;
                    case 3:
                        who.FarmerSprite.animateBackwardsOnce(302, 35f);
                        break;
                }

                // Fish caught, override it
                this.isNibbling = false;
                int? fish = null;

                // Check if legendary fish are being overridden as well
                if (!ModFishing.Instance.MainConfig.CustomLegendaries) {
                    // Check if a legendary would be caught
                    double baitValue = this.attachments[0]?.Price / 10.0 ?? 0.0;
                    bool bubblyZone = false;
                    if (location.fishSplashPoint.Value != Point.Zero)
                        bubblyZone = new Rectangle(location.fishSplashPoint.X * 64, location.fishSplashPoint.Y * 64, 64, 64).Intersects(new Rectangle((int) this.bobber.X - 80, (int) this.bobber.Y - 80, 64, 64));
                    SObject normalFish = location.getFish(this.fishingNibbleAccumulator, this.attachments[0]?.ParentSheetIndex ?? -1, this._clearWaterDistance.Value + (bubblyZone ? 1 : 0), this.lastUser, baitValue + (bubblyZone ? 0.4 : 0.0));

                    // If so, select that fish
                    if (FishHelper.IsLegendary(normalFish.ParentSheetIndex)) {
                        fish = normalFish.ParentSheetIndex;
                    }
                }

                // Void mayonnaise
                if (location.Name.Equals("WitchSwamp") && !Game1.MasterPlayer.mailReceived.Contains("henchmanGone") && Game1.random.NextDouble() < 0.25 && !Game1.player.hasItemInInventory(308, 1)) {
                    this.pullFishFromWater(308, -1, 0, 0, false);
                    return;
                }

                // Choose a random fish if one hasn't been chosen yet
                if (fish == null) {
                    fish = FishHelper.GetRandomFish(this.lastUser);
                }

                // Check if a fish was chosen
                if (fish == null) {
                    // Secret note
                    if (who.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08) {
                        SObject unseenSecretNote = location.tryToCreateUnseenSecretNote(who);
                        this.pullFishFromWater(unseenSecretNote.ParentSheetIndex, -1, 0, 0, false);
                        return;
                    }

                    // Trash
                    this.pullFishFromWater(FishHelper.GetRandomTrash(), -1, 0, 0, false);
                } else {
                    // Make sure this hasn't been handled yet
                    if (this.hit)
                        return;

                    // Check if favBait should be set to true (still not sure what this does...)
                    SObject fishObject = new SObject(fish.Value, 1);
                    if (Math.Abs(fishObject.Scale.X - 1.0) < 0.001)
                        this.favBait = true;

                    // Show hit animation
                    this.hit = true;
                    Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, this.bobber.Value + new Vector2(-140f, -160f)), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true) {
                        scaleChangeChange = -0.005f,
                        motion = new Vector2(0.0f, -0.1f),
                        endFunction = this.StartMinigameEndFunction,
                        extraInfoForEndBehavior = fish.Value
                    });
                    location.localSound("FishHit");
                }

                // Base call, but without affecting anything
                this.isCasting = true;
                base.DoFunction(location, x, y, power, who);
                this.isCasting = false;
            } else {
                base.DoFunction(location, x, y, power, who);
            }
        }

        public override void tickUpdate(GameTime time, Farmer who) {
            base.tickUpdate(time, who);

            // Replace the end function for the treasure opening animation
            foreach (TemporaryAnimatedSprite anim in this.animations) {
                if (anim.endFunction == this.openChestEndFunction) {
                    anim.endFunction = this.OpenChestEndFunction;
                }
            }
        }

        public void OpenChestEndFunction(int extra) {
            base.openChestEndFunction(extra);
            if (!this.lastUser.IsLocalPlayer)
                return;

            // Replace the end function for the custom treasure
            foreach (TemporaryAnimatedSprite anim in this.animations) {
                if (anim.endFunction == this.openTreasureMenuEndFunction) {
                    anim.endFunction = this.OpenTreasureEndFunction;
                }
            }
        }

        public void StartMinigameEndFunction(int fish) {
            this.isReeling = true;
            this.hit = false;

            // Animation
            switch (this.lastUser.FacingDirection) {
                case 1:
                    this.lastUser.FarmerSprite.setCurrentSingleFrame(48);
                    break;
                case 3:
                    this.lastUser.FarmerSprite.setCurrentSingleFrame(48, 32000, false, true);
                    break;
            }
            this.lastUser.FarmerSprite.PauseForSingleAnimation = true;

            // Distance from bobber to land
            this._clearWaterDistance.Value = FishingRod.distanceToLand((int) (this.bobber.X / 64.0 - 1.0), (int) (this.bobber.Y / 64.0 - 1.0), this.lastUser.currentLocation);

            // Calculate size of fish
            float num = 1f * (this._clearWaterDistance.Value / 5f) * (Game1.random.Next(1 + Math.Min(10, this.lastUser.FishingLevel) / 2, 6) / 5f);
            if (this.favBait)
                num *= 1.2f;
            float fishSize = Math.Max(0.0f, Math.Min(1f, num * (float) (1.0 + Game1.random.Next(-10, 10) / 100.0)));

            // Check if there should be treasure
            bool treasure = !Game1.isFestival();
            treasure &= this.lastUser.fishCaught != null && this.lastUser.fishCaught.Count > 1;
            treasure &= Game1.random.NextDouble() < FishHelper.GetTreasureChance(this.lastUser, this);
            Game1.activeClickableMenu = new CustomBobberBar(this.lastUser, fish, fishSize, treasure, this.attachments[1]?.ParentSheetIndex ?? -1);
        }

        public void OpenTreasureEndFunction(int extra) {
            ModFishing.Instance.Monitor.Log("Successfully replaced treasure", LogLevel.Trace);

            ConfigMain.ConfigGlobalTreasure config = ModFishing.Instance.MainConfig.GlobalTreasureSettings;
            int clearWaterDistance = 5;
            //if (lastUser))
            //    clearWaterDistance = FishingRodOverrides.ClearWaterDistances[this.lastUser];
            //else
            //    ModFishing.Instance.Monitor.Log("The bobber bar was not replaced. Fishing might not be overridden by this mod", LogLevel.Warn);

            if (this.lastUser.IsLocalPlayer) {
                if (this.attachments[0] != null) {
                    --this.attachments[0].Stack;
                    if (this.attachments[0].Stack <= 0) {
                        this.attachments[0] = null;
                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14085"));
                    }
                }
                if (this.attachments[1] != null) {
                    this.attachments[1].Scale = new Vector2(this.attachments[1].Scale.X, this.attachments[1].Scale.Y - 0.05f);
                    if (this.attachments[1].Scale.Y <= 0.0) {
                        this.attachments[1] = null;
                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14086"));
                    }
                }
            }

            int whichFish = this._whichFish.Value;
            int fishQuality = this._fishQuality.Value;
            this.lastUser.gainExperience(5, 10 * (clearWaterDistance + 1));
            this.doneFishing(this.lastUser, true);
            this.lastUser.completelyStopAnimatingOrDoingAction();

            // REWARDS
            List<Item> rewards = new List<Item>();
            if (extra == 1) rewards.Add(new SObject(whichFish, 1, false, -1, fishQuality));

            List<TreasureData> possibleLoot = new List<TreasureData>(ModFishing.Instance.TreasureConfig.PossibleLoot)
                .Where(treasure => treasure.IsValid(this.lastUser.FishingLevel)).ToList();

            // Select rewards
            float chance = 1f;
            int streak = FishHelper.GetStreak(this.lastUser);
            while (possibleLoot.Count > 0 && rewards.Count < config.MaxTreasureQuantity && Game1.random.NextDouble() <= chance) {
                TreasureData treasure = possibleLoot.Choose(Game1.random);

                // Choose an ID for the treasure
                int id = treasure.Id + Game1.random.Next(treasure.IdRange - 1);

                // Lost books have custom handling
                if (id == Objects.LostBook) {
                    if (this.lastUser.archaeologyFound == null || !this.lastUser.archaeologyFound.ContainsKey(102) || this.lastUser.archaeologyFound[102][0] >= 21) {
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
                    int count = Game1.random.Next(treasure.MinAmount, treasure.MaxAmount);

                    reward = new SObject(Vector2.Zero, id, count);
                }

                // Add the reward
                rewards.Add(reward);

                // Check if this reward shouldn't be duplicated
                if (!config.AllowDuplicateLoot || !treasure.AllowDuplicates)
                    possibleLoot.Remove(treasure);

                // Update chance
                chance *= config.AdditionalLootChance + streak * config.StreakAdditionalLootChance;

                //rewards.Add(new StardewValley.Object(Vector2.Zero, Objects.BAIT, Game1.random.Next(10, 25)));
            }

            // Add bait if no rewards were selected. NOTE: This should never happen
            if (rewards.Count == 0) {
                ModFishing.Instance.Monitor.Log("Could not find any valid loot for the treasure chest. Check your treasure.json?", LogLevel.Warn);
                rewards.Add(new SObject(685, Game1.random.Next(2, 5) * 5));
            }

            // Show rewards GUI
            Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            ((ItemGrabMenu) Game1.activeClickableMenu).source = 3;
            this.lastUser.completelyStopAnimatingOrDoingAction();
        }

        public override string getDescription() {
            return "[DEBUG] A handy custom fishing rod.";
        }

        public override string DisplayName { get; set; } = "Custom Fishing Rod";

        public FishingRod AsNormalFishingRod() {
            // Copy over properties
            FishingRod rod = new FishingRod {
                UpgradeLevel = this.UpgradeLevel,
                IndexOfMenuItemView = this.IndexOfMenuItemView
            };
            rod.numAttachmentSlots.Value = this.numAttachmentSlots.Value;

            // Copy over attachments
            for (int i = 0; i < this.attachments.Count; i++) {
                rod.attachments.Add(this.attachments[i]);
            }

            return rod;
        }

        public RodData Save() {
            return new RodData {
                SerializedData = SaveHelper.XmlSerializableToDictionary<FishingRod>(this)
            };
        }

        public void Load(RodData model) {
            SaveHelper.DictionaryToXmlSerializable<FishingRod>(this, model.SerializedData);
        }

        public class RodData {
            public Dictionary<string, object> SerializedData { get; set; }
        }
    }
}