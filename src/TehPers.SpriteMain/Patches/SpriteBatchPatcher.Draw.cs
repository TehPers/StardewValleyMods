using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TehPers.SpriteMain.Patches
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony naming scheme")]
    partial class SpriteBatchPatcher
    {
        private static Rectangle GetDefaultSource(Texture2D texture)
        {
            return new(0, 0, texture.Width, texture.Height);
        }

        private static bool SpriteBatch_Draw_Prefix(
            SpriteBatch __instance,
            Texture2D texture,
            Vector2 position,
            Color color
        )
        {
            return SpriteBatchPatcher.SpriteBatch_Draw_Prefix(
                __instance,
                texture,
                position,
                null,
                color,
                0f,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }

        private static bool SpriteBatch_Draw_Prefix(
            SpriteBatch __instance,
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color
        )
        {
            return SpriteBatchPatcher.SpriteBatch_Draw_Prefix(
                __instance,
                texture,
                position,
                sourceRectangle,
                color,
                0f,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }

        private static bool SpriteBatch_Draw_Prefix(
            SpriteBatch __instance,
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth
        )
        {
            return SpriteBatchPatcher.SpriteBatch_Draw_Prefix(
                __instance,
                texture,
                position,
                sourceRectangle,
                color,
                rotation,
                origin,
                new Vector2(scale, scale),
                effects,
                layerDepth
            );
        }

        private static bool SpriteBatch_Draw_Prefix(
            SpriteBatch __instance,
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth
        )
        {
            // return true;

            var (x, y) = position;
            var (scaleX, scaleY) = scale;
            var dest = new Rectangle(
                (int)x,
                (int)y,
                (int)((sourceRectangle?.Width ?? texture.Width) * scaleX),
                (int)((sourceRectangle?.Height ?? texture.Height) * scaleY)
            );
            return SpriteBatchPatcher.SpriteBatch_Draw_Prefix(
                __instance,
                texture,
                dest,
                sourceRectangle,
                color,
                rotation,
                origin,
                effects,
                layerDepth
            );
        }

        private static bool SpriteBatch_Draw_Prefix(
            SpriteBatch __instance,
            Texture2D texture,
            Rectangle destinationRectangle,
            Color color
        )
        {
            return SpriteBatchPatcher.SpriteBatch_Draw_Prefix(
                __instance,
                texture,
                destinationRectangle,
                null,
                color,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );
        }

        private static bool SpriteBatch_Draw_Prefix(
            SpriteBatch __instance,
            Texture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color
        )
        {
            return SpriteBatchPatcher.SpriteBatch_Draw_Prefix(
                __instance,
                texture,
                destinationRectangle,
                sourceRectangle,
                color,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );
        }

        private static bool SpriteBatch_Draw_Prefix(
            SpriteBatch __instance,
            Texture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effects,
            float layerDepth
        )
        {
            return SpriteBatchPatcher.Draw(
                __instance,
                texture,
                sourceRectangle ?? SpriteBatchPatcher.GetDefaultSource(texture),
                destinationRectangle,
                color,
                rotation,
                origin,
                effects,
                layerDepth
            );
        }
    }
}