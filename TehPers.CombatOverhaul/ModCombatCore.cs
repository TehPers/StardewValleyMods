using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using TehPers.Core;
using TehPers.Core.Api.Weighted;
using TehPers.Core.Helpers.Static;

namespace TehPers.CombatOverhaul {
    public class ModCombatCore : Mod {
        private static ModCombatCore Instance { get; set; }

        private TehCoreApi _coreApi;
        private HarmonyInstance _harmony;
        private readonly MethodInfo _loggingMethod = typeof(ModCombatCore).GetMethod(nameof(ModCombatCore.LogMethod_Prefix), BindingFlags.NonPublic | BindingFlags.Static);

        public override void Entry(IModHelper helper) {
            ModCombatCore.Instance = this;
            this._coreApi = TehCoreApi.Create(this);

            // Apply any patches
            GameEvents.FirstUpdateTick += (sender, e) => this.ApplyPatches();

            GameEvents.UpdateTick += (sender, e) => this.OnUpdate();
            GraphicsEvents.OnPostRenderHudEvent += this.PostRenderHud;
        }

        private void ApplyPatches() {
            this._harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            MethodInfo target = typeof(Tool).GetMethod(nameof(Tool.DoFunction), BindingFlags.Public | BindingFlags.Instance);
            MethodInfo prefix = typeof(ModCombatCore).GetMethod(nameof(ModCombatCore.MeleeWeapon_DoFunction_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
            this.PatchWithLogging(target);
            this._harmony.Patch(target, new HarmonyMethod(prefix), null);

            // target = typeof(MeleeWeapon).GetMethod(nameof(MeleeWeapon.doSwipe), BindingFlags.Public | BindingFlags.Instance);
            // this.PatchWithLogging(target);
        }

        private void PatchWithLogging(MethodInfo target) {
            this._harmony.Patch(target, new HarmonyMethod(this._loggingMethod), null);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static bool MeleeWeapon_DoFunction_Prefix(GameLocation location, int x, int y, int power, Farmer who, Tool __instance, ref Farmer ___lastUser) {
            if (!(__instance is MeleeWeapon weapon))
                return true;

            ___lastUser = who;
            Game1.recentMultiplayerRandom = new Random((short) Game1.random.Next(short.MinValue, 32768));
            ToolFactory.getIndexFromTool(__instance);
            if (who.UsingTool && Game1.mouseClickPolling < 50 && weapon.type.Value != 1 && weapon.InitialParentTileIndex != 47 && MeleeWeapon.timedHitTimer <= 0 && who.FarmerSprite.currentAnimationIndex == 5 && who.FarmerSprite.timer < who.FarmerSprite.interval / 4.0)
                return false;
            if (weapon.type.Value == 2 && weapon.isOnSpecial) {
                weapon.triggerClubFunction(who);
            } else {
                if (who.FarmerSprite.currentAnimationIndex <= 0)
                    return false;
                MeleeWeapon.timedHitTimer = 500;
            }

            Game1.showGlobalMessage("Overrode attack");
            return false;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static void LogMethod_Prefix(MethodInfo __originalMethod) {
            ModCombatCore.Instance.Monitor.Log($"{__originalMethod.DeclaringType?.FullName ?? "?"}.{__originalMethod.Name}", LogLevel.Info);
        }

        private void OnUpdate() {
            _ = 12;

            // MeleeWeapon.timedHitTimer = 0;
            // Game1.player.forceCanMove();
            // Game1.player.forceCanMove();
            // Game1.player.UsingTool = false;
            // Game1.player.FarmerSprite.PauseForSingleAnimation = false;
            // Game1.player.noMovementPause = 0;
            // Game1.player.freezePause = 0;
        }

        private void PostRenderHud(object sender, EventArgs eventArgs) {
            if (Game1.eventUp || Game1.player.CurrentTool is FishingRod)
                return;

            // Setup the sprite batch
            SpriteBatch batch = Game1.spriteBatch;
            batch.End();
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Setup debug text
            StringBuilder text = new StringBuilder();
            text.AppendLine("Debug Info:");
            text.AppendLine($"Tool Type: {Game1.player.CurrentTool?.GetType().FullName}");
            text.AppendLine($"forceTimePass: {Game1.player.forceTimePass}");
            text.AppendLine($"movementDirections: {string.Join(", ", Game1.player.movementDirections)}");
            text.AppendLine($"isEating: {Game1.player.isEating}");
            text.AppendLine($"CanMove: {Game1.player.CanMove}");
            text.AppendLine($"freezeControls: {Game1.freezeControls}");
            text.AppendLine($"freezePause: {Game1.player.freezePause}");
            text.AppendLine($"UsingTool: {Game1.player.UsingTool}");
            text.AppendLine($"usingSlingshot: {Game1.player.usingSlingshot}");
            text.AppendLine($"PauseForSingleAnimation: {Game1.player.FarmerSprite.PauseForSingleAnimation}");
            // if (!Game1.eventUp || this.movementDirections.Count <= 0 || (this.currentLocation.currentEvent == null || this.currentLocation.currentEvent.playerControlSequence)) {

            // Draw the text and background
            if (text.Length > 0) {
                string contents = text.ToString();
                Vector2 size = Game1.smallFont.MeasureString(contents);

                // Background
                batch.Draw(DrawHelpers.WhitePixel, new Rectangle(0, 0, (int) size.X, (int) size.Y), null, new Color(0, 0, 0, 0.25F), 0f, Vector2.Zero, SpriteEffects.None, 0.85F);

                // Text
                batch.DrawStringWithShadow(Game1.smallFont, text.ToString(), Vector2.Zero, Color.White, 0.8F);
            }

            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }
    }
}
