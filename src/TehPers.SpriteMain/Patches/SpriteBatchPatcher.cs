using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Activation;
using TehPers.Core.Api.Setup;
using TehPers.SpriteMain.Scalers;

namespace TehPers.SpriteMain.Patches
{
    internal partial class SpriteBatchPatcher : Patcher, ISetup
    {
        private static SpriteBatchPatcher? instance;

        private readonly HashSet<SpriteBatch> ignoredBatches = new();
        private readonly Dictionary<Texture2D, Texture2D> scaledTextures = new();
        private readonly Dictionary<TextureCacheKey, TextureCacheResult> textureCache = new();

        private IScaler? scaler;

        private SpriteBatchPatcher(Harmony harmony)
            : base(harmony)
        {
        }

        public static SpriteBatchPatcher Create(IContext context)
        {
            SpriteBatchPatcher.instance ??= new(context.Kernel.Get<Harmony>());
            return SpriteBatchPatcher.instance;
        }

        public void Setup()
        {
            // Patch SpriteBatch.Draw(...)
            var drawMethods = typeof(SpriteBatch)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.Name == nameof(SpriteBatch.Draw));
            foreach (var drawMethod in drawMethods)
            {
                var prefix = AccessTools.Method(
                    typeof(SpriteBatchPatcher),
                    nameof(SpriteBatchPatcher.SpriteBatch_Draw_Prefix),
                    drawMethod.GetParameters()
                        .Select(param => param.ParameterType)
                        .Prepend(typeof(SpriteBatch))
                        .ToArray()
                );
                if (prefix is null)
                {
                    continue;
                }

                this.Patch(drawMethod, prefix: new(prefix));
            }
        }

        public void SetScaler(IScaler? scaler)
        {
            this.scaler = scaler;
            this.ResetCache();
        }

        public void ResetCache()
        {
            this.scaledTextures.Clear();
            this.textureCache.Clear();
        }

        private static bool Draw(
            SpriteBatch sb,
            Texture2D texture,
            Rectangle source,
            Rectangle destination,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effects,
            float layerDepth
        )
        {
            // Ignore buffers
            if (texture is RenderTarget2D)
            {
                return true;
            }

            // Get patcher instance
            if (SpriteBatchPatcher.instance is not { } patcher)
            {
                return true;
            }

            // Defer to regular drawing code if no scaling is selected
            if (patcher.scaler is not { } scaler)
            {
                return true;
            }

            // Check if this batch is already being overridden
            if (!patcher.ignoredBatches.Add(sb))
            {
                return true;
            }

            try
            {
                var cacheKey = new TextureCacheKey(texture, source);
                if (!patcher.textureCache.TryGetValue(cacheKey, out var cacheResult))
                {
                    // Get or create scaled texture
                    if (!patcher.scaledTextures.TryGetValue(texture, out var scaledTexture))
                    {
                        scaledTexture = new(
                            texture.GraphicsDevice,
                            (int)(texture.Width * scaler.Scale),
                            (int)(texture.Height * scaler.Scale),
                            false,
                            texture.Format
                        );
                        patcher.scaledTextures[texture] = scaledTexture;
                    }
                    
                    var destRect = scaler.DrawScaled(texture, source, scaledTexture);
                    cacheResult = new(scaledTexture, destRect, scaler.Scale);
                    patcher.textureCache[cacheKey] = cacheResult;
                }

                sb.Draw(
                    cacheResult.ScaledTexture,
                    destination,
                    cacheResult.SourceRectangle,
                    color,
                    rotation,
                    origin * cacheResult.OriginScale,
                    effects,
                    layerDepth
                );
            }
            finally
            {
                patcher.ignoredBatches.Remove(sb);
            }

            return false;
        }
    }
}