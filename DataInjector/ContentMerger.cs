using Entoarox.Framework.ContentManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using xTile.Dimensions;

namespace TehPers.Stardew.DataInjector {
    public class ContentMerger : ContentHandler {
        private string path;
        public ContentManager modContent;
        public ContentManager entoContent = null;
        internal Dictionary<string, object> cache = new Dictionary<string, object>();

        private static List<Type> unmergables = new List<Type>();

        public override bool Injector { get; } = true;

        public ContentMerger(string path) {
            if (!Directory.Exists(path)) {
                try {
                    ModEntry.INSTANCE.Monitor.Log("Creating directory " + path);
                    Directory.CreateDirectory(path);
                } catch (Exception ex) {
                    ModEntry.INSTANCE.Monitor.Log("Could not create directory " + path + "! Please create it yourself.", LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.Message, LogLevel.Error);
                    return;
                }
            }

            this.modContent = new ContentManager(Game1.content.ServiceProvider, path);
            this.path = path;
        }

        public override bool CanInject<T>(string assetName) {
            return cache.ContainsKey(assetName) || this.getModDirs(assetName).Length > 0;
        }

        public override T Load<T>(string assetName, Func<string, T> loadBase) {
            throw new NotImplementedException();
        }

        public override void Inject<T>(string assetName, ref T asset) {
            if (cache.ContainsKey(assetName))
                asset = (T) cache[assetName];
            else {
                if (this.getModDirs(assetName).Length > 0) {
                    Type t = typeof(T);
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                        asset = (T) this.GetType().GetMethod("MergeDictionary", BindingFlags.Public | BindingFlags.Instance)
                            .MakeGenericMethod(t.GetGenericArguments())
                            .Invoke(this, new object[] { asset, assetName });
                        //asset = (T) (object) this.MergeMods(asset as Dictionary<int, string>, assetName);
                    } else if (t == typeof(Texture2D)) {
                        Texture2D texture = asset as Texture2D;
                        if (texture == null || texture.Format == SurfaceFormat.Color)
                            asset = (T) (object) this.MergeTextures<Color>(asset as Texture2D, assetName);
                        else {
                            ModEntry.INSTANCE.Monitor.Log("Cannot merge this texture format, overriding instead: " + Enum.GetName(typeof(SurfaceFormat), texture.Format), LogLevel.Info);
                            asset = this.ReplaceIfExists(asset, assetName);
                        }
                    } else {
                        if (!unmergables.Contains(typeof(T))) {
                            ModEntry.INSTANCE.Monitor.Log("Cannot merge this type, overriding instead: " + typeof(T).ToString(), LogLevel.Trace);
                            unmergables.Add(typeof(T));
                        }
                        asset = this.ReplaceIfExists(asset, assetName);
                    }
                }
                cache[assetName] = asset;
            }
        }

        public string[] getModDirs(string assetName) {
            string searchPath = Path.Combine("*", assetName + ".xnb");
            string[] modDirs = Directory.GetDirectories(path);

            // Load order
            ModConfig config = ModEntry.INSTANCE.config;
            config.LoadOrder.AddRange(modDirs.Where(f => !config.LoadOrder.Contains(Path.GetFileName(f))).Select(f => Path.GetFileName(f)));
            modDirs = modDirs.OrderBy(f => config.LoadOrder.IndexOf(Path.GetFileName(f))).ToArray();
            ModEntry.INSTANCE.Helper.WriteConfig(config);

            return modDirs.Where(dir => File.Exists(Path.Combine(dir, assetName + ".xnb"))).ToArray();
        }

        public string getModLocalPath(string modPath, string assetName) {
            Uri modUri = new Uri(Path.Combine(this.path, modPath));
            Uri fileUri = new Uri(Path.Combine(modPath, assetName));
            Uri localUri = modUri.MakeRelativeUri(fileUri);
            return WebUtility.UrlDecode(localUri.ToString().Replace('/', '\\'));
        }

        public Dictionary<TKey, TVal> MergeDictionary<TKey, TVal>(Dictionary<TKey, TVal> orig, string assetName) {
            Dictionary<TKey, TVal> diffs = new Dictionary<TKey, TVal>();
            Dictionary<TKey, string> diffMods = new Dictionary<TKey, string>();

            foreach (string modPath in this.getModDirs(assetName)) {
                string localPath = this.getModLocalPath(modPath, assetName);

                try {
                    // TODO: Load the xnb file, then calculate diffs with the original and merge the changes
                    Dictionary<TKey, TVal> injections = this.modContent.Load<Dictionary<TKey, TVal>>(localPath);

                    bool collision = false;
                    foreach (KeyValuePair<TKey, TVal> injection in injections) {
                        if (!(orig.ContainsKey(injection.Key) && orig[injection.Key].Equals(injection.Value))) {
                            if (!collision && diffs.ContainsKey(injection.Key)) {
                                ModEntry.INSTANCE.Monitor.Log(string.Format("Collision detected between {0} and {1}! Overwriting...", diffMods[injection.Key], Path.GetFileName(modPath)), LogLevel.Warn);
                                collision = true;
                            }
                            diffs[injection.Key] = injection.Value;
                            diffMods[injection.Key] = Path.GetFileName(modPath);
                        }
                    }
                } catch (Exception ex) {
                    ModEntry.INSTANCE.Monitor.Log("Failed to load " + localPath + ".", LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.Message, LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.StackTrace, LogLevel.Trace);
                }
            }

            foreach (KeyValuePair<TKey, TVal> diff in diffs)
                orig[diff.Key] = diff.Value;

            if (diffs.Count > 0)
                ModEntry.INSTANCE.Monitor.Log(string.Format("{0} changes injected into {1}.xnb", diffs.Count, assetName), LogLevel.Info);

            return orig;
        }

        public Texture2D MergeTextures<TFormat>(Texture2D orig, string assetName) where TFormat : struct {
            string[] modDirs = this.getModDirs(assetName);
            Dictionary<int, int> diffMods = new Dictionary<int, int>();
            TFormat[] origData = new TFormat[orig.Width * orig.Height];
            Size origSize = new Size(orig.Width, orig.Height);
            orig.GetData(origData);
            List<TFormat> diffData = new List<TFormat>(origData);

            for (int modI = 0; modI < modDirs.Length; modI++) {
                string modPath = modDirs[modI];
                string localPath = this.getModLocalPath(modPath, assetName);
                Texture2D modTexture = null;

                try {
                    // TODO: Load the xnb file, then calculate diffs with the original and merge the changes
                    modTexture = this.modContent.Load<Texture2D>(localPath);

                    bool collision = false;
                    TFormat[] modData = new TFormat[modTexture.Width * modTexture.Height];
                    Size modSize = new Size(modTexture.Width, modTexture.Height);
                    modTexture.GetData(modData);

                    if (modSize.Width != origSize.Width)
                        ModEntry.INSTANCE.Monitor.Log("Mod's texture is too wide for the game, so it's being trimmed: " + Path.GetFileName(modPath), LogLevel.Warn);

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
                                    string thisMod = Path.GetFileName(modPath);
                                    string otherMod = Path.GetFileName(modDirs[diffMods[i]]);
                                    ModEntry.INSTANCE.Monitor.Log(string.Format("Collision detected between {0} and {1}! Overwriting...", thisMod, otherMod), LogLevel.Warn);
                                    collision = true;
                                }

                                while (i >= diffData.Count) diffData.Add(default(TFormat));

                                diffData[i] = pixel;
                                diffMods[i] = modI;
                            }
                        }
                    }
                } catch (Exception ex) {
                    ModEntry.INSTANCE.Monitor.Log("Failed to load " + localPath + ".", LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.Message, LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.StackTrace, LogLevel.Trace);
                }

                if (modTexture != null) modTexture.Dispose();
            }

            HashSet<int> relevantMods = new HashSet<int>(diffMods.Values);

            if (relevantMods.Count > 1)
                ModEntry.INSTANCE.Monitor.Log(string.Format("{0} mods injected changes into {1}.xnb", relevantMods.Count, assetName), LogLevel.Info);
            else if (relevantMods.Count == 1)
                ModEntry.INSTANCE.Monitor.Log(string.Format("{0} injected changes into {1}.xnb", Path.GetFileName(modDirs[relevantMods.First()]), assetName), LogLevel.Info);

            Texture2D result = new Texture2D(orig.GraphicsDevice, origSize.Width, (int) Math.Ceiling((double) diffData.Count / origSize.Width));
            result.SetData(diffData.ToArray());
            orig.Dispose();
            return result;
        }

        public T ReplaceIfExists<T>(T orig, string assetName) {
            T diff = orig;
            string diffMod = null;

            foreach (string modPath in this.getModDirs(assetName)) {
                string localPath = this.getModLocalPath(modPath, assetName);

                try {
                    // TODO: Load the xnb file, then calculate diffs with the original and merge the changes
                    T injection = this.modContent.Load<T>(localPath);

                    if (diffMod != null)
                        ModEntry.INSTANCE.Monitor.Log("Collision detected with " + diffMod + "! Overwriting...", LogLevel.Warn);

                    diff = injection;
                    diffMod = Path.GetFileName(modPath);
                } catch (Exception ex) {
                    ModEntry.INSTANCE.Monitor.Log("Failed to load " + localPath + ".", LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.Message, LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.StackTrace, LogLevel.Trace);
                }
            }

            if (diffMod != null)
                ModEntry.INSTANCE.Monitor.Log(string.Format("{0} replaced {1}.xnb", diffMod, assetName), LogLevel.Info);

            return diff;
        }
    }
}
