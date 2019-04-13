using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace TehPers.ShroomSpotter {
    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public ModConfig Config;
        public List<UpdateEvent> UpdateEvents = new List<UpdateEvent>();
        public delegate bool UpdateEvent();

        public ModEntry() {
            ModEntry.INSTANCE = this;
        }

        public override void Entry(IModHelper helper) {
            this.Config = helper.ReadConfig<ModConfig>();
            if (!this.Config.ModEnabled) {
                return;
            }



            this.Helper.Events.Input.ButtonPressed += this.ButtonPressed;
            this.Helper.Events.Display.RenderingActiveMenu += this.RenderingActiveMenu;
            this.Helper.Events.Display.RenderedActiveMenu += this.RenderedActiveMenu;
        }

        #region Events
        private void ButtonPressed(object sender, ButtonPressedEventArgs e) {
            // Check the pressed button
            if (e.Button != this.Config.GetShroomLevels) {
                return;
            }

            // Find all shroom levels
            List<int> shroomLevels = new List<int>();
            int daysTilShroom = -1;
            while (shroomLevels.Count == 0 && ++daysTilShroom < 50) shroomLevels = this.GetShroomLayers(daysTilShroom);

            if (shroomLevels.Count > 0) {
                if (daysTilShroom == 0)
                    Game1.showGlobalMessage("Shroom layers will spawn on these mine levels: " + string.Join<int>(", ", shroomLevels));
                else
                    Game1.showGlobalMessage("Shrooms will spawn in " + daysTilShroom + " day(s) on these mine levels: " + string.Join<int>(", ", shroomLevels));
            } else Game1.showGlobalMessage("No shroom layers will spawn in the next 50 days!");
        }

        private void RenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e) {
            // Only render shrooms on the billboard
            if (!(Game1.activeClickableMenu is Billboard menu)) {
                return;
            }

            // Try to get the calendar field
            if (!(this.Helper.Reflection.GetField<List<ClickableTextureComponent>>(menu, nameof(Billboard.calendarDays))?.GetValue() is List<ClickableTextureComponent> calendarDays)) {
                return;
            }

            // Get the current hover text
            IReflectedField<string> hoverField = this.Helper.Reflection.GetField<string>(menu, "hoverText");
            string hoverText = hoverField.GetValue();

            // Make sure the current hover text doesn't already have mushroom information
            if (hoverText.Contains("Shrooms") || hoverText.Contains("shrooms")) {
                return;
            }

            // Update the hover text
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();
            for (int day = 1; day <= 28; day++) {
                // Check if the mouse is over the current calendar day
                ClickableTextureComponent component = calendarDays[day - 1];
                if (!component.bounds.Contains(mouseX, mouseY)) {
                    continue;
                }

                // Add any mushroom text
                List<int> shrooms = this.GetShroomLayers(day - Game1.dayOfMonth);
                if (hoverText.Length > 0)
                    hoverText += "\n";
                if (shrooms.Count > 0)
                    hoverText += "Shrooms: " + string.Join(", ", shrooms);
                else
                    hoverText += "No shrooms";

                break;
            }

            hoverField.SetValue(hoverText);
        }

        private void RenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e) {
            // Only render shrooms on the billboard
            if (!(Game1.activeClickableMenu is Billboard menu)) {
                return;
            }

            // Try to get the calendar field
            if (!(this.Helper.Reflection.GetField<List<ClickableTextureComponent>>(menu, nameof(Billboard.calendarDays))?.GetValue() is List<ClickableTextureComponent> calendarDays)) {
                return;
            }

            // Get the current hover text
            string hoverText = this.Helper.Reflection.GetField<string>(menu, "hoverText").GetValue();

            // Draw the shrooms on the calendar
            for (int day = 1; day <= 28; day++) {
                ClickableTextureComponent component = calendarDays[day - 1];
                List<int> shrooms = this.GetShroomLayers(day - Game1.dayOfMonth);

                // Check if a shroom layer exists on this day
                if (shrooms.Count <= 0) {
                    continue;
                }

                // Draw the shroom
                Rectangle source = GameLocation.getSourceRectForObject((int) this.Config.ShroomIcon);
                Vector2 dest = new Vector2(component.bounds.Right - 8f * Game1.pixelZoom, component.bounds.Y);
                e.SpriteBatch.Draw(Game1.objectSpriteSheet, dest, source, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom / 2f, SpriteEffects.None, 1f);
            }

            // Redraw the hover text so it appears over the mushrooms
            IClickableMenu.drawHoverText(e.SpriteBatch, hoverText, Game1.dialogueFont);
        }
        #endregion

        public List<int> GetShroomLayers(int relativeDay) {
            List<int> shroomLevels = new List<int>();
            for (int mineLevel = 1; mineLevel < 120; mineLevel++) {
                Random random = new Random((int) Game1.stats.DaysPlayed + relativeDay + mineLevel + (int) Game1.uniqueIDForThisGame / 2);

                // Simulate all the random values grabbed before the shrooms
                if (random.NextDouble() < 0.3 && mineLevel > 2) {
                    random.NextDouble();
                }
                random.NextDouble();
                if (random.NextDouble() < 0.035 && mineLevel >= 80 && mineLevel <= 120 && mineLevel % 5 != 0) {
                    shroomLevels.Add(mineLevel);
                }
            }

            return shroomLevels;
        }
    }
}