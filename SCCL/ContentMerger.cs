using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TehPers.Stardew.Framework;
using TehPers.Stardew.SCCL.API;
using TehPers.Stardew.SCCL.Configs;
using xTile.Dimensions;

namespace TehPers.Stardew.SCCL {
    public class ContentMerger {
        public HashSet<string> Dirty { get; } = new HashSet<string>();
        private HashSet<Type> Unmergables { get; } = new HashSet<Type>();
        private Dictionary<string, object> Cache { get; } = new Dictionary<string, object>();
        private Dictionary<string, object> Unmerged { get; } = new Dictionary<string, object>();

        internal ContentMerger() { }

        public object Merge(string assetName, object orig) {
            ModConfig config = ModEntry.INSTANCE.config;

            try {
                object asset = orig == null ? this.getModAssets<object>(assetName)[0].Value : orig;

                if (this.getModAssets<object>(assetName).Count > 0) {
                    Type t = asset.GetType();
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                        asset = this.GetType().GetMethod("MergeDictionary", BindingFlags.NonPublic | BindingFlags.Instance)
                            .MakeGenericMethod(t.GetGenericArguments())
                            .Invoke(this, new object[] { asset, assetName });
                        //asset = (T) (object) this.MergeMods(asset as Dictionary<int, string>, assetName);
                    } else if (!config.OverwriteAllTextures && t == typeof(Texture2D)) {
                        Texture2D texture = asset as Texture2D;
                        if (texture == null || texture.Format == SurfaceFormat.Color)
                            asset = this.MergeTextures<Color>(texture, assetName);
                        else {
                            ModEntry.INSTANCE.Monitor.Log("Cannot merge this texture format, overriding instead: " + Enum.GetName(typeof(SurfaceFormat), texture.Format), LogLevel.Info);
                            asset = this.ReplaceIfExists(asset, assetName);
                        }
                    } else {
                        if (!Unmergables.Contains(t)) {
                            ModEntry.INSTANCE.Monitor.Log("Cannot merge this type, overriding instead: " + t.ToString(), LogLevel.Trace);
                            Unmergables.Add(t);
                        }
                        asset = this.ReplaceIfExists(asset, assetName);
                    }
                }

                return asset;
            } catch (Exception ex) {
                ModEntry.INSTANCE.Monitor.Log("An error occured while merging " + assetName, LogLevel.Error);
            }

            return null;
        }

        public void AssetLoading(object sender, IContentEventHelper e) {
            Unmerged[e.AssetName] = e.Data;
            Cache[e.AssetName] = this.Merge(e.AssetName, e.Data);
            this.Dirty.Remove(e.AssetName);
            e.ReplaceWith(Cache[e.AssetName]);
        }

        public void RefreshAssets() {
            foreach (string assetName in Dirty) {
                if (!Cache.ContainsKey(assetName)) continue;

                object orig = Cache[assetName];
                object n = null;

                if (Unmerged.ContainsKey(assetName)) n = this.Merge(assetName, Unmerged[assetName]);
                else n = this.Merge(assetName, null);

                if (n != null) {
                    if (orig is Texture2D && n is Texture2D) {
                        //(orig as Texture2D).SetData<Color>()
                    }
                }
            }
            Dirty.Clear();
        }

        public List<KeyValuePair<string, T>> getModAssets<T>(string assetName) {
            // Update load order with any missing mods
            ModConfig config = ModEntry.INSTANCE.config;
            config.LoadOrder.AddRange(
                from modKV in ContentAPI.mods
                let mod = modKV.Key
                where !config.LoadOrder.Contains(mod)
                select mod
                );
            ModEntry.INSTANCE.Helper.WriteConfig(config);

            return (
                from injector in ContentAPI.mods.Values
                where injector.Enabled
                orderby config.LoadOrder.IndexOf(injector.Name)
                where injector.ModContent.ContainsKey(assetName)
                from asset in injector.ModContent[assetName]
                where asset is T
                select new KeyValuePair<string, T>(injector.Name, (T) asset)
                ).ToList();
        }

        #region Mergers
        private Dictionary<TKey, TVal> MergeDictionary<TKey, TVal>(Dictionary<TKey, TVal> orig, string assetName) {
            Dictionary<TKey, TVal> diffs = new Dictionary<TKey, TVal>();
            Dictionary<TKey, string> diffMods = new Dictionary<TKey, string>();
            List<KeyValuePair<string, Dictionary<TKey, TVal>>> mods = getModAssets<Dictionary<TKey, TVal>>(assetName);

            bool collision = false;

            foreach (KeyValuePair<string, Dictionary<TKey, TVal>> modKV in mods) {
                foreach (KeyValuePair<TKey, TVal> injection in modKV.Value) {
                    if (!(orig.ContainsKey(injection.Key) && orig[injection.Key].Equals(injection.Value))) {
                        if (!collision && diffs.ContainsKey(injection.Key)) {
                            ModEntry.INSTANCE.Monitor.Log(string.Format("Collision detected between {0} and {1}! Overwriting...", diffMods[injection.Key], modKV.Key), LogLevel.Warn);
                            collision = true;
                        }
                        diffs[injection.Key] = injection.Value;
                        diffMods[injection.Key] = modKV.Key;
                    }
                }
            }

            Dictionary<TKey, TVal> merged = new Dictionary<TKey, TVal>(orig);
            foreach (KeyValuePair<TKey, TVal> diff in diffs)
                merged[diff.Key] = diff.Value;

            if (diffs.Count > 0)
                ModEntry.INSTANCE.Monitor.Log(string.Format("{0} injected {1} changes into {2}.xnb", string.Join(", ", diffMods.Values.ToHashSet()), diffs.Count, assetName), LogLevel.Info);

            return merged;
        }

        // TODO: Needs to have texture injection offsets, so mods can inject a texture at (u, v) in the original
        /// <remarks>Will not dispose of <paramref name="orig"/></remarks>
        private Texture2D MergeTextures<TFormat>(Texture2D orig, string assetName) where TFormat : struct {
            Dictionary<int, string> diffMods = new Dictionary<int, string>();
            TFormat[] origData = new TFormat[orig.Width * orig.Height];
            Size origSize = new Size(orig.Width, orig.Height);
            orig.GetData(origData);
            List<TFormat> diffData = new List<TFormat>(origData);

            List<KeyValuePair<string, Texture2D>> mods = getModAssets<Texture2D>(assetName);

            if (mods.Count > 1) {
                foreach (KeyValuePair<string, Texture2D> modKV in mods) {
                    string mod = modKV.Key;
                    Texture2D modTexture = modKV.Value;
                    bool collision = false;
                    TFormat[] modData = new TFormat[modTexture.Width * modTexture.Height];
                    Size modSize = new Size(modTexture.Width, modTexture.Height);
                    modTexture.GetData(modData);

                    if (modSize.Width != origSize.Width)
                        ModEntry.INSTANCE.Monitor.Log("Mod's texture is too wide for the game, so it's being trimmed: " + mod, LogLevel.Warn);

                    for (int y = 0; y < modSize.Height; y++) { // Use mod's height
                        for (int x = 0; x < origSize.Width; x++) { // Use original's width
                            int i = y * origSize.Width + x; // Use the original's index because we're keeping that width for the final
                            TFormat pixel;
                            if (modSize.Width < origSize.Width) // If the mod's texture does not contain this pixel coordinate
                                pixel = origData[i];
                            else // Otherwise if it contains this pixel
                                pixel = modData[y * modSize.Width + x];

                            if (i >= origData.Length || !origData[i].Equals(pixel)) {
                                if (!collision && diffMods.ContainsKey(i)) {
                                    ModEntry.INSTANCE.Monitor.Log(string.Format("Collision detected between {0} and {1}! Overwriting...", mod, diffMods[i]), LogLevel.Warn);
                                    collision = true;
                                }

                                while (i >= diffData.Count) diffData.Add(default(TFormat));

                                diffData[i] = pixel;
                                diffMods[i] = mod;
                            }
                        }
                    }
                }
            } else {
                diffMods[0] = mods.First().Key;
            }

            ModEntry.INSTANCE.Monitor.Log(string.Format("{0} injected changes into {1}.xnb", string.Join(", ", diffMods.Values.ToHashSet()), assetName), LogLevel.Info);
            if (mods.Count == 1) return mods.First().Value;

            Texture2D merged = new Texture2D(orig.GraphicsDevice, origSize.Width, (int) Math.Ceiling((double) diffData.Count / origSize.Width));
            merged.SetData(diffData.ToArray());
            return merged;
        }

        private T ReplaceIfExists<T>(T orig, string assetName) {
            T replaced = orig;
            string diffMod = null;

            List<KeyValuePair<string, T>> mods = getModAssets<T>(assetName);

            foreach (KeyValuePair<string, T> modKV in mods) {
                if (diffMod != null)
                    ModEntry.INSTANCE.Monitor.Log("Collision detected with " + diffMod + "! Overwriting...", LogLevel.Warn);

                replaced = modKV.Value;
                diffMod = modKV.Key;
            }

            if (diffMod != null)
                ModEntry.INSTANCE.Monitor.Log(string.Format("{0} replaced {1}.xnb", diffMod, assetName), LogLevel.Info);

            return replaced;
        }
        #endregion
    }
}
