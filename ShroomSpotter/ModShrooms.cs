using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ShroomSpotter {
    public class ModShrooms : Mod {
        public static ModShrooms Instance;

        public ModConfig Config { get; set; }
        public List<UpdateEvent> UpdateEvents = new List<UpdateEvent>();
        public delegate bool UpdateEvent();

        public override void Entry(IModHelper helper) {
            ModShrooms.Instance = this;
            this.Config = helper.ReadConfig<ModConfig>();
            if (!this.Config.ModEnabled) return;

            //this.Monitor.Log("It is *HIGHLY* recommended you install a Health Bars mod for enemies!", LogLevel.Info);

            ControlEvents.KeyPressed += this.KeyPressed;
            GraphicsEvents.OnPreRenderGuiEvent += this.OnPreRenderGuiEvent;
            GraphicsEvents.OnPostRenderGuiEvent += this.OnPostRenderGuiEvent;
            //MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }

        #region Events
        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            if (e.KeyPressed != this.Config.GetShroomLevels)
                return;

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

        private void OnPreRenderGuiEvent(object sender, EventArgs e) {
            if (!(Game1.activeClickableMenu is Billboard menu))
                return;

            // Get the calendar days components in the billboard
            FieldInfo calendarField = menu.GetType().GetField("calendarDays", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (calendarField == null) {
                this.Monitor.Log("Could not find field 'calendarDays' in Billboard!", LogLevel.Error);
                return;
            }

            // Get the hover text field
            List<ClickableTextureComponent> calendarDays = (List<ClickableTextureComponent>) calendarField.GetValue(menu);
            IReflectedField<string> hoverField = this.Helper.Reflection.GetField<string>(menu, "hoverText");
            string hoverText = hoverField.GetValue();
            if (calendarDays == null || hoverText.Contains("Shrooms") || hoverText.Contains("shrooms"))
                return;

            // Check if the mouse is over any of the days
            for (int day = 1; day <= 28; day++) {
                ClickableTextureComponent component = calendarDays[day - 1];
                if (component.bounds.Contains(Game1.getMouseX(), Game1.getMouseY())) {
                    List<int> shrooms = this.GetShroomLayers(day - Game1.dayOfMonth);

                    // Add to the hover text for this day
                    if (hoverText.Length > 0)
                        hoverText += "\n";
                    if (shrooms.Count > 0)
                        hoverText += "Shrooms: " + string.Join(", ", shrooms);
                    else
                        hoverText += "No shrooms";
                    hoverField.SetValue(hoverText);
                    break;
                }
            }
        }

        private void OnPostRenderGuiEvent(object sender, EventArgs e) {
            if (!(Game1.activeClickableMenu is Billboard menu))
                return;

            // Get calendar days components in the billboard
            FieldInfo calendarField = menu.GetType().GetField("calendarDays", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (calendarField == null) {
                this.Monitor.Log("Could not find field 'calendarDays' in Billboard!", LogLevel.Error);
                return;
            }

            // Get the current hover text (needs to be redrawn later)
            List<ClickableTextureComponent> calendarDays = (List<ClickableTextureComponent>) calendarField.GetValue(menu);
            if (calendarDays == null) return;
            string hoverText = this.Helper.Reflection.GetField<string>(menu, "hoverText").GetValue();
            SpriteBatch b = Game1.spriteBatch;

            // Draw a shroom on each day shrooms can be found
            for (int day = 1; day <= 28; day++) {
                ClickableTextureComponent component = calendarDays[day - 1];
                List<int> shrooms = this.GetShroomLayers(day - Game1.dayOfMonth);

                // Draw a shroom if there should be one for this day
                if (shrooms.Count > 0) {
                    const int id = 422;
                    Rectangle source = GameLocation.getSourceRectForObject(id);
                    Vector2 dest = new Vector2(component.bounds.X, component.bounds.Y + 10f * Game1.pixelZoom);
                    b.Draw(Game1.objectSpriteSheet, dest, source, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom / 2f, SpriteEffects.None, 1f);
                }
            }

            // Redraw the hover text
            IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
        }
        #endregion

        public List<int> GetShroomLayers(int relativeDay) {
            List<int> shroomLevels = new List<int>();
            for (int mineLevel = 1; mineLevel < 120; mineLevel++) {
                Random random = new Random((int) Game1.stats.DaysPlayed + relativeDay + mineLevel + (int) Game1.uniqueIDForThisGame / 2);

                // Simulate all the random values grabbed before the shrooms
                if (random.NextDouble() < 0.3 && mineLevel > 2)
                    random.NextDouble();
                random.NextDouble();
                if (random.NextDouble() < 0.035 && mineLevel >= 80 && mineLevel <= 120 && mineLevel % 5 != 0)
                    shroomLevels.Add(mineLevel);
            }

            return shroomLevels;
        }
    }
}
