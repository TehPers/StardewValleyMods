using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Enums;
using ModUtilities.Menus.Components;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ModUtilities.Menus {
    public class ModMenu : IClickableMenu {
        public MenuComponent Component { get; } = new MenuComponent();

        public ModMenu(int x, int y, int width, int height) : base(x, y, width, height) {
            this.Component.Bounds = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            this.Component.Click(new Location(x, y), playSound, MouseButtons.LEFT);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            this.Component.Click(new Location(x, y), playSound, MouseButtons.RIGHT);
        }

        public override void receiveScrollWheelAction(int direction) {
            this.Component.Scroll(new Location(Game1.getMouseX(), Game1.getMouseY()), direction);
        }

        public override void receiveKeyPress(Keys key) {
            Component focused = this.Component.GetFocusedComponent() ?? this.Component;
            if (!focused.PressKey(key))
                base.receiveKeyPress(key);
        }

        public void EnterText(string text) {
            Component focused = this.Component.GetFocusedComponent() ?? this.Component;
            focused.EnterText(text);
        }

        public override void performHoverAction(int x, int y) {
            //this._hoverText = "";
            //this._hoverTitle = "";
        }

        protected virtual void DrawCursor(SpriteBatch b) {
            this.drawMouse(b);
        }

        protected virtual void DrawHoverText(SpriteBatch b) {
            //IClickableMenu.drawHoverText(b, this._hoverText, Game1.smallFont, 0, 0, -1, this._hoverTitle.Length > 0 ? this._hoverTitle : null);
        }

        public override void draw(SpriteBatch b) {
            if (this.Component == null)
                return;

            this.Component.Draw(b);
            this.DrawCursor(b);

            // Draw


            // Old
            //int num1 = this.xPositionOnScreen + Game1.tileSize - Game1.pixelZoom * 3;
            //int num2 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
            //b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, new Vector2(num1, num2), Color.White);
            //Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(Game1.player.bathingClothes ? 108 : this._playerPanelFrames[this._playerPanelIndex], 0, false, false), Game1.player.bathingClothes ? 108 : this._playerPanelFrames[this._playerPanelIndex], new Rectangle(this._playerPanelFrames[this._playerPanelIndex] * 16, Game1.player.bathingClothes ? 576 : 0, 16, 32), new Vector2(num1 + Game1.tileSize / 2, num2 + Game1.tileSize / 2), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, Game1.player);
            //if (Game1.timeOfDay >= 1900)
            //    Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(this._playerPanelFrames[this._playerPanelIndex], 0, false, false), this._playerPanelFrames[this._playerPanelIndex], new Rectangle(this._playerPanelFrames[this._playerPanelIndex] * 16, 0, 16, 32), new Vector2(num1 + Game1.tileSize / 2, num2 + Game1.tileSize / 2), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0.0f, 1f, Game1.player);
            //b.DrawString(Game1.smallFont, Game1.player.name, new Vector2(num1 + Game1.tileSize - Game1.smallFont.MeasureString(Game1.player.name).X / 2f, num2 + 3 * Game1.tileSize + 4), Game1.textColor);
            //b.DrawString(Game1.smallFont, Game1.player.getTitle(), new Vector2(num1 + Game1.tileSize - Game1.smallFont.MeasureString(Game1.player.getTitle()).X / 2f, num2 + 4 * Game1.tileSize - Game1.tileSize / 2), Game1.textColor);
            //int num3 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru ? this.xPositionOnScreen + this.width - Game1.tileSize * 7 - Game1.tileSize * 3 / 4 : this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 * Game1.tileSize - 8;
            //int num4 = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - Game1.pixelZoom * 2;
            //int num5 = 0;
            //for (int index1 = 0; index1 < 10; ++index1) {
            //    for (int index2 = 0; index2 < 5; ++index2) {
            //        bool flag1 = false;
            //        bool flag2 = false;
            //        string text = "";
            //        int number = 0;
            //        Rectangle rectangle = Rectangle.Empty;
            //        switch (index2) {
            //            case 0:
            //                flag1 = Game1.player.FarmingLevel > index1;
            //                if (index1 == 0)
            //                    text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604");
            //                number = Game1.player.FarmingLevel;
            //                flag2 = Game1.player.addedFarmingLevel > 0;
            //                rectangle = new Rectangle(10, 428, 10, 10);
            //                break;
            //            case 1:
            //                flag1 = Game1.player.MiningLevel > index1;
            //                if (index1 == 0)
            //                    text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605");
            //                number = Game1.player.MiningLevel;
            //                flag2 = Game1.player.addedMiningLevel > 0;
            //                rectangle = new Rectangle(30, 428, 10, 10);
            //                break;
            //            case 2:
            //                flag1 = Game1.player.ForagingLevel > index1;
            //                if (index1 == 0)
            //                    text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606");
            //                number = Game1.player.ForagingLevel;
            //                flag2 = Game1.player.addedForagingLevel > 0;
            //                rectangle = new Rectangle(60, 428, 10, 10);
            //                break;
            //            case 3:
            //                flag1 = Game1.player.FishingLevel > index1;
            //                if (index1 == 0)
            //                    text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607");
            //                number = Game1.player.FishingLevel;
            //                flag2 = Game1.player.addedFishingLevel > 0;
            //                rectangle = new Rectangle(20, 428, 10, 10);
            //                break;
            //            case 4:
            //                flag1 = Game1.player.CombatLevel > index1;
            //                if (index1 == 0)
            //                    text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608");
            //                number = Game1.player.CombatLevel;
            //                flag2 = Game1.player.addedCombatLevel > 0;
            //                rectangle = new Rectangle(120, 428, 10, 10);
            //                break;
            //            case 5:
            //                flag1 = Game1.player.LuckLevel > index1;
            //                if (index1 == 0)
            //                    text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11609");
            //                number = Game1.player.LuckLevel;
            //                flag2 = Game1.player.addedLuckLevel > 0;
            //                rectangle = new Rectangle(50, 428, 10, 10);
            //                break;
            //        }
            //        if (!text.Equals("")) {
            //            b.DrawString(Game1.smallFont, text, new Vector2(num3 - Game1.smallFont.MeasureString(text).X + Game1.pixelZoom - Game1.tileSize, num4 + Game1.pixelZoom + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), Game1.textColor);
            //            b.Draw(Game1.mouseCursors, new Vector2(num3 - Game1.pixelZoom * 14, num4 + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), rectangle, Color.Black * 0.3f, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.85f);
            //            b.Draw(Game1.mouseCursors, new Vector2(num3 - Game1.pixelZoom * 13, num4 - Game1.pixelZoom + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), rectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.87f);
            //        }
            //        if (!flag1 && (index1 + 1) % 5 == 0) {
            //            b.Draw(Game1.mouseCursors, new Vector2(num5 + num3 - Game1.pixelZoom + index1 * (Game1.tileSize / 2 + Game1.pixelZoom), num4 + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), new Rectangle(145, 338, 14, 9), Color.Black * 0.35f, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.87f);
            //            b.Draw(Game1.mouseCursors, new Vector2(num5 + num3 + index1 * (Game1.tileSize / 2 + Game1.pixelZoom), num4 - Game1.pixelZoom + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), new Rectangle(145 + (flag1 ? 14 : 0), 338, 14, 9), Color.White * (flag1 ? 1f : 0.65f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.87f);
            //        } else if ((index1 + 1) % 5 != 0) {
            //            b.Draw(Game1.mouseCursors, new Vector2(num5 + num3 - Game1.pixelZoom + index1 * (Game1.tileSize / 2 + Game1.pixelZoom), num4 + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), new Rectangle(129, 338, 8, 9), Color.Black * 0.35f, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.85f);
            //            b.Draw(Game1.mouseCursors, new Vector2(num5 + num3 + index1 * (Game1.tileSize / 2 + Game1.pixelZoom), num4 - Game1.pixelZoom + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), new Rectangle(129 + (flag1 ? 8 : 0), 338, 8, 9), Color.White * (flag1 ? 1f : 0.65f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.87f);
            //        }
            //        if (index1 == 9) {
            //            NumberSprite.draw(number, b, new Vector2(num5 + num3 + (index1 + 2) * (Game1.tileSize / 2 + Game1.pixelZoom) + Game1.pixelZoom * 3 + (number >= 10 ? Game1.pixelZoom * 3 : 0), num4 + Game1.pixelZoom * 4 + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), Color.Black * 0.35f, 1f, 0.85f, 1f, 0);
            //            NumberSprite.draw(number, b, new Vector2(num5 + num3 + (index1 + 2) * (Game1.tileSize / 2 + Game1.pixelZoom) + Game1.pixelZoom * 4 + (number >= 10 ? Game1.pixelZoom * 3 : 0), num4 + Game1.pixelZoom * 3 + index2 * (Game1.tileSize / 2 + Game1.pixelZoom * 6)), (flag2 ? Color.LightGreen : Color.SandyBrown) * (number == 0 ? 0.75f : 1f), 1f, 0.87f, 1f, 0);
            //        }
            //    }
            //    if ((index1 + 1) % 5 == 0)
            //        num5 += Game1.pixelZoom * 6;
            //}

            //IClickableMenu.drawHoverText(b, this._hoverText, Game1.smallFont, 0, 0, -1, this._hoverTitle.Length > 0 ? this._hoverTitle : null);
        }
    }
}
