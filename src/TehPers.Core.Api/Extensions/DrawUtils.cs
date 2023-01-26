using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using TehPers.Core.Api.Extensions.Drawing;
using SObject = StardewValley.Object;

namespace TehPers.Core.Api.Extensions
{
    /// <summary>
    /// Extensions for easier drawing.
    /// </summary>
    public static class DrawUtils
    {
        /// <summary>
        /// A texture containing a 1x1 white pixel.
        /// </summary>
        public static Texture2D WhitePixel { get; }

        static DrawUtils()
        {
            DrawUtils.WhitePixel = new(Game1.spriteBatch.GraphicsDevice, 1, 1);
            DrawUtils.WhitePixel.SetData(new[] {Color.White});
        }

        /// <summary>
        /// Draws a string with a shadow behind it.
        /// </summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="font">The font the text should use.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position of the string.</param>
        /// <param name="color">The color of the string. The shadow is black.</param>
        /// <param name="depth">The depth of the string.</param>
        /// <param name="shadowDepth">The depth of the shadow.</param>
        public static void DrawStringWithShadow(
            this SpriteBatch batch,
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            float depth = 0F,
            float shadowDepth = 0.005F
        )
        {
            batch.DrawStringWithShadow(
                font,
                text,
                position,
                color,
                Color.Black,
                Vector2.One,
                depth,
                shadowDepth
            );
        }

        /// <summary>
        /// Draws a string with a shadow behind it.
        /// </summary>
        /// <param name="batch">The batch to draw with.</param>
        /// <param name="font">The font the text should use.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The position of the string.</param>
        /// <param name="color">The color of the string. The shadow is black.</param>
        /// <param name="shadowColor">The color of the shadow.</param>
        /// <param name="scale">The amount to scale the size of the string by.</param>
        /// <param name="depth">The depth of the string.</param>
        /// <param name="shadowDepth">The depth of the shadow.</param>
        public static void DrawStringWithShadow(
            this SpriteBatch batch,
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            Color shadowColor,
            Vector2 scale,
            float depth,
            float shadowDepth
        )
        {
            batch.DrawString(
                font,
                text,
                position + Vector2.One * Game1.pixelZoom * scale / 2f,
                shadowColor,
                0F,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                shadowDepth
            );
            batch.DrawString(
                font,
                text,
                position,
                color,
                0F,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                depth
            );
        }


        /// <summary>
        /// Correctly draws this item in a menu. Many of the vanilla implementations are incorrect
        /// with parameters other than what vanilla code normally uses. This method adjusts those
        /// parameters before calling the vanilla
        /// <see cref="Item.drawInMenu(SpriteBatch, Vector2, float, float, float, StackDrawType, Color, bool)"/>
        /// method.
        /// </summary>
        /// <param name="item">The item to draw.</param>
        /// <param name="spriteBatch">The batch to draw to.</param>
        /// <param name="location">The location to draw the item.</param>
        /// <param name="scaleSize">The scale of the item.</param>
        /// <param name="transparency">The item's transparency/opacity.</param>
        /// <param name="layerDepth">The layer depth to draw the item at.</param>
        /// <param name="drawStackNumber">How to draw the stack number.</param>
        /// <param name="color">The color to tint the item.</param>
        /// <param name="drawShadow">Whether to draw the item's shadow.</param>
        /// <param name="origin">The origin to scale the item from.</param>
        public static void DrawInMenuCorrected(
            this Item item,
            SpriteBatch spriteBatch,
            Vector2 location,
            float scaleSize,
            float transparency,
            float layerDepth,
            StackDrawType drawStackNumber,
            Color color,
            bool drawShadow,
            IDrawOrigin origin
        )
        {
            var drawingProperties = DrawUtils.GetDefaultDrawingProperties(item)
                ?? throw new ArgumentException(
                    $"Unknown drawing properties for {item} ({item.GetType().FullName})",
                    nameof(item)
                );

            item.DrawInMenuCorrected(
                spriteBatch,
                location,
                scaleSize,
                transparency,
                layerDepth,
                drawStackNumber,
                color,
                drawShadow,
                origin,
                drawingProperties
            );
        }

        /// <summary>
        /// Correctly draws this item in a menu. Many of the vanilla implementations are incorrect
        /// with parameters other than what vanilla code normally uses. This method adjusts those
        /// parameters before calling the vanilla
        /// <see cref="Item.drawInMenu(SpriteBatch, Vector2, float, float, float, StackDrawType, Color, bool)"/>
        /// method.
        /// </summary>
        /// <param name="item">The item to draw.</param>
        /// <param name="spriteBatch">The batch to draw to.</param>
        /// <param name="location">The location to draw the item.</param>
        /// <param name="scaleSize">The scale of the item.</param>
        /// <param name="transparency">The item's transparency/opacity.</param>
        /// <param name="layerDepth">The layer depth to draw the item at.</param>
        /// <param name="drawStackNumber">How to draw the stack number.</param>
        /// <param name="color">The color to tint the item.</param>
        /// <param name="drawShadow">Whether to draw the item's shadow.</param>
        /// <param name="origin">The origin to scale the item from.</param>
        /// <param name="drawingProperties">Properties about how the item is drawn in the vanilla code.</param>
        public static void DrawInMenuCorrected(
            this Item item,
            SpriteBatch spriteBatch,
            Vector2 location,
            float scaleSize,
            float transparency,
            float layerDepth,
            StackDrawType drawStackNumber,
            Color color,
            bool drawShadow,
            IDrawOrigin origin,
            IDrawingProperties drawingProperties
        )
        {
            var realOrigin = drawingProperties.Origin(scaleSize);
            var realScale = drawingProperties.RealScale(scaleSize);
            var realOffset = drawingProperties.Offset(scaleSize);
            var realSize = drawingProperties.SourceSize * realScale;
            var positionCorrection =
                -realOffset + realOrigin * realScale + origin.GetTranslation(realSize);
            item.drawInMenu(
                spriteBatch,
                location + positionCorrection,
                scaleSize,
                transparency,
                layerDepth,
                drawStackNumber,
                color,
                drawShadow
            );
        }

        /// <summary>
        /// Gets the default <see cref="IDrawingProperties"/> for an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The default drawing properties.</returns>
        public static IDrawingProperties? GetDefaultDrawingProperties(Item item)
        {
            return item switch
            {
                // BigCraftable
                SObject {bigCraftable: {Value: true}} => new BigCraftableDrawingProperties(),
                // Boots
                Boots => new BootsDrawingProperties(),
                // Clothing
                Clothing => new ClothingDrawingProperties(),
                // Flooring
                Wallpaper {isFloor: {Value: true}} => new FlooringDrawingProperties(),
                // Wallpaper
                Wallpaper => new WallpaperDrawingProperties(),
                // Furniture
                Furniture {sourceRect: {Value: var sourceRect}} => new FurnitureDrawingProperties(
                    new(sourceRect.Width, sourceRect.Height),
                    (sourceRect.Width / 16, sourceRect.Height / 16) switch
                    {
                        (>= 7, _) => 0.5f,
                        (>= 6, _) => 0.66f,
                        (>= 5, _) => 0.75f,
                        (_, >= 5) => 0.8f,
                        (_, >= 3) => 1f,
                        (<= 2, _) => 2f,
                        (<= 4, _) => 1f,
                    }
                ),
                // Hat
                Hat => new HatDrawingProperties(),
                // Object
                SObject => new ObjectDrawingProperties(),
                // Ring
                Ring => new RingDrawingProperties(),
                // Weapon
                MeleeWeapon {type: {Value: var type}} weapon => new MeleeWeaponDrawingProperties(
                    type,
                    weapon.isScythe()
                ),
                // Tool
                Tool => new ToolDrawingProperties(),
                _ => null,
            };
        }

        /// <summary>
        /// Calculates the intersection of two rectangles.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>A rectangle representing the intersection, or null if they do not intersect.</returns>
        public static Rectangle? Intersection(this Rectangle a, Rectangle b)
        {
            var x1 = Math.Max(a.X, b.X);
            var y1 = Math.Max(a.Y, b.Y);
            var x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            var y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            // Make sure it's a valid rectangle (the rectangles intersect)
            if (x1 >= x2 || y1 >= y2)
            {
                return null;
            }

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        /// <summary>
        /// Executes some code with a given scissor rectangle (drawing will be limited to the
        /// rectangle).
        /// </summary>
        /// <param name="batch">The <see cref="SpriteBatch"/> being used for drawing calls</param>
        /// <param name="scissorRect">The rectangle to limit drawing to</param>
        /// <param name="action">The function to execute with the given scissor rectangle</param>
        /// <param name="respectExistingScissor">Whether to limit the new scissor rectangle to a subrectangle of the current scissor rectangle</param>
        public static void WithScissorRect(
            this SpriteBatch batch,
            Rectangle scissorRect,
            Action<SpriteBatch> action,
            bool respectExistingScissor = true
        )
        {
            // Stop the old drawing code
            batch.End();

            // Keep track of the old parameters
            // This needs to come after End() so they're applied to the GraphicsDevice
            var oldScissor = batch.GraphicsDevice.ScissorRectangle;
            var oldBlendState = batch.GraphicsDevice.BlendState;
            var oldStencilState = batch.GraphicsDevice.DepthStencilState;
            var oldRasterizerState = batch.GraphicsDevice.RasterizerState;
            var oldSamplerState = batch.GraphicsDevice.SamplerStates[0];

            // Trim current scissor to the existing one if necessary
            if (respectExistingScissor)
            {
                scissorRect = scissorRect.Intersection(oldScissor) ?? new Rectangle(0, 0, 0, 0);
            }

            // Draw with the new scissor rectangle
            using (var rasterizerState = new RasterizerState {ScissorTestEnable = true})
            {
                batch.Begin(
                    SpriteSortMode.BackToFront,
                    oldBlendState,
                    oldSamplerState,
                    DepthStencilState.Default,
                    rasterizerState
                );

                // Set scissor rectangle
                batch.GraphicsDevice.ScissorRectangle = scissorRect;

                // Perform the action
                action(batch);

                // Draw the batch
                batch.End();
            }

            // Reset scissor rectangle
            batch.GraphicsDevice.ScissorRectangle = oldScissor;

            // Return to last state
            batch.Begin(
                SpriteSortMode.BackToFront,
                oldBlendState,
                oldSamplerState,
                oldStencilState,
                oldRasterizerState
            );
        }
    }
}
