using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace FishingOverhaul {
    public class CustomFishingRod : FishingRod {

        protected ReflectedField<FishingRod, int> ClearWaterDistance { get; set; }

        public CustomFishingRod() {
            // this.isFishing {bool} - whether the rod is tossed
            // this.castedButBobberStillInAir {bool} - whether the bobber is in midair (tossing animation)
            // this.pullingOutOfWater {bool} - whether the player is pulling a fish out of the water (pulling animation)
            // this.isNibbling {bool} - whether the bobber is shaking and the player can catch something ('!' appears over player's head)
            // this.hit {bool} - whether a fish has been hit ("HIT" appears)

            this.ClearWaterDistance = new ReflectedField<FishingRod, int>(this, "clearWaterDistance");
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who) {
            if (this.isFishing && this.isNibbling) {
                // Fish caught, override it
                this.isNibbling = false;

                double baitPotency = this.attachments[0]?.Price / 10.0 ?? 0.0;
                bool b = false; // Not really sure what this does
                if (location.fishSplashPoint.Value != Point.Zero) {
                    b = new Rectangle(location.fishSplashPoint.X * 64, location.fishSplashPoint.Y * 64, 64, 64).Intersects(new Rectangle((int) this.bobber.X - 80, (int) this.bobber.Y - 80, 64, 64));
                }

                if (location.Name.Equals("WitchSwamp") && !Game1.MasterPlayer.mailReceived.Contains("henchmanGone") && Game1.random.NextDouble() < 0.25 && !Game1.player.hasItemInInventory(308, 1)) {
                    this.pullFishFromWater(308, -1, 0, 0, false);
                    return;
                }

                bool trash = false;
                int? fish = trash ? null : FishHelper.GetRandomFish(this.ClearWaterDistance.Value);
                if (fish == null) {
                    // Secret note
                    if (who.hasMagnifyingGlass && Game1.random.NextDouble() < 0.08) {
                        Object unseenSecretNote = location.tryToCreateUnseenSecretNote(who);
                        this.pullFishFromWater(unseenSecretNote.ParentSheetIndex, -1, 0, 0, false);
                        return;
                    }

                    // Trash
                    this.pullFishFromWater(FishHelper.GetRandomTrash(), -1, 0, 0, false);
                } else {
                    // Make sure this isn't another player and hasn't been handled yet
                    if (this.hit || !who.IsLocalPlayer)
                        return;

                    // Show hit animation
                    this.hit = true;
                    Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, this.bobber.Value + new Vector2(-140f, -160f)), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true) {
                        scaleChangeChange = -0.005f,
                        motion = new Vector2(0.0f, -0.1f),
                        endFunction = this.startMinigameEndFunction,
                        extraInfoForEndBehavior = fish.Value
                    });
                    location.localSound("FishHit");

                    // Not sure what this line does or how to implement it:
                    // if (@object.scale.X == 1F) this.favBait = true;
                }
            }


            base.DoFunction(location, x, y, power, who);

            if ((this.isFishing || this.castedButBobberStillInAir || this.pullingOutOfWater || this.isNibbling || this.hit) && this.isNibbling) {

            }
        }

        public void StartMinigameEndFunction(int extra) {
            /*this.isReeling = true;
            this.hit = false;
            switch (this.lastUser.FacingDirection) {
                case 1:
                    this.lastUser.FarmerSprite.setCurrentSingleFrame(48, (short) 32000, false, false);
                    break;
                case 3:
                    this.lastUser.FarmerSprite.setCurrentSingleFrame(48, (short) 32000, false, true);
                    break;
            }
            this.lastUser.FarmerSprite.PauseForSingleAnimation = true;
            this.clearWaterDistance = FishingRod.distanceToLand((int) ((double) this.bobber.X / 64.0 - 1.0), (int) ((double) this.bobber.Y / 64.0 - 1.0), this.lastUser.currentLocation);
            float num = 1f * ((float) this.clearWaterDistance / 5f) * ((float) Game1.random.Next(1 + Math.Min(10, this.lastUser.FishingLevel) / 2, 6) / 5f);
            if (this.favBait)
                num *= 1.2f;
            float fishSize = Math.Max(0.0f, Math.Min(1f, num * (float) (1.0 + (double) Game1.random.Next(-10, 10) / 100.0)));
            bool treasure = !Game1.isFestival() && this.lastUser.fishCaught != null && this.lastUser.fishCaught.Count > 1 && Game1.random.NextDouble() < FishingRod.baseChanceForTreasure + (double) this.lastUser.LuckLevel * 0.005 + (this.getBaitAttachmentIndex() == 703 ? FishingRod.baseChanceForTreasure : 0.0) + (this.getBobberAttachmentIndex() == 693 ? FishingRod.baseChanceForTreasure / 3.0 : 0.0) + Game1.dailyLuck / 2.0 + (this.lastUser.professions.Contains(9) ? FishingRod.baseChanceForTreasure : 0.0);
            Game1.activeClickableMenu = (IClickableMenu) new BobberBar(extra, fishSize, treasure, this.attachments[1] != null ? this.attachments[1].ParentSheetIndex : -1);*/
        }

        public override string getDescription() {
            return "[DEBUG] A handy custom fishing rod.";
        }
    }
}