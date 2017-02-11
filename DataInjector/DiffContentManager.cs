using Entoarox.Framework;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace TehPers.Stardew.DataInjector {
    public class DiffContentManager /*: IContentInjector*/ {
        private string path;
        public ContentManager modContent;
        public ContentManager entoContent = null;

        public DiffContentManager(string path) {
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

        public void InjectContent<T>(string assetName, ref T asset) {
            if (asset is Dictionary<int, string>) {
                asset = (T) (object) this.MergeMods(asset as Dictionary<int, string>, assetName);
            } else {
                asset = this.LoadModded(asset, assetName);
            }
        }

        public Dictionary<K, V> MergeMods<K, V>(Dictionary<K, V> orig, string assetName) {
            Dictionary<K, V> diffs = new Dictionary<K, V>();
            Dictionary<K, string> diffMods = new Dictionary<K, string>();
            string searchPath = Path.Combine("*", assetName + ".xnb");
            string[] modDirs = Directory.GetDirectories(path);

            // Load order
            ModConfig config = ModEntry.INSTANCE.config;
            config.LoadOrder.AddRange(modDirs.Where(f => !config.LoadOrder.Contains(Path.GetFileName(f))).Select(f => Path.GetFileName(f)));
            modDirs = modDirs.OrderBy(f => config.LoadOrder.IndexOf(Path.GetFileName(f))).ToArray();
            ModEntry.INSTANCE.Helper.WriteConfig(config);

            string[] fileList = modDirs.Where(dir => File.Exists(Path.Combine(dir, assetName + ".xnb"))).ToArray();
            foreach (string filePath in fileList) {
                Uri modUri = new Uri(Path.Combine(this.path, filePath));
                Uri fileUri = new Uri(Path.Combine(filePath, assetName));
                Uri localUri = modUri.MakeRelativeUri(fileUri);
                string localConfigPath = localUri.ToString().Replace('/', '\\');
                localConfigPath = WebUtility.UrlDecode(localConfigPath);

                try {
                    // TODO: Load the xnb file, then calculate diffs with the original and merge the changes
                    Dictionary<K, V> injections = this.modContent.Load<Dictionary<K, V>>(localConfigPath);

                    bool collision = false;
                    foreach (KeyValuePair<K, V> injection in injections) {
                        if (!(orig.ContainsKey(injection.Key) && orig[injection.Key].Equals(injection.Value))) {
                            if (!collision && diffs.ContainsKey(injection.Key)) {
                                ModEntry.INSTANCE.Monitor.Log("Collision detected with " + diffMods[injection.Key] + "! Overwriting...");
                                collision = true;
                            }
                            diffs[injection.Key] = injection.Value;
                            diffMods[injection.Key] = Path.GetFileName(filePath);
                        }
                    }
                } catch (Exception ex) {
                    ModEntry.INSTANCE.Monitor.Log("Failed to load " + localConfigPath + ".", LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.Message, LogLevel.Error);
                }
            }

            foreach (KeyValuePair<K, V> diff in diffs)
                orig[diff.Key] = diff.Value;

            if (diffs.Count > 0)
                ModEntry.INSTANCE.Monitor.Log(string.Format("{0} changes injected into {1}.xnb", diffs.Count, assetName), LogLevel.Info);

            return orig;
        }

        public T LoadModded<T>(T orig, string assetName) {
            T diff = orig;
            string diffMod = null;
            string searchPath = Path.Combine("*", assetName + ".xnb");
            string[] modDirs = Directory.GetDirectories(path);

            // Load order
            ModConfig config = ModEntry.INSTANCE.config;
            config.LoadOrder.AddRange(modDirs.Where(f => !config.LoadOrder.Contains(Path.GetFileName(f))).Select(f => Path.GetFileName(f)));
            modDirs = modDirs.OrderBy(f => config.LoadOrder.IndexOf(Path.GetFileName(f))).ToArray();
            ModEntry.INSTANCE.Helper.WriteConfig(config);

            string[] fileList = modDirs.Where(dir => File.Exists(Path.Combine(dir, assetName + ".xnb"))).ToArray();
            foreach (string filePath in fileList) {
                Uri modUri = new Uri(Path.Combine(this.path, filePath));
                Uri fileUri = new Uri(Path.Combine(filePath, assetName));
                Uri localUri = modUri.MakeRelativeUri(fileUri);
                string localConfigPath = localUri.ToString().Replace('/', '\\');
                localConfigPath = WebUtility.UrlDecode(localConfigPath);

                try {
                    // TODO: Load the xnb file, then calculate diffs with the original and merge the changes
                    T injection = this.modContent.Load<T>(localConfigPath);

                    if (diffMod != null)
                        ModEntry.INSTANCE.Monitor.Log("Collision detected with " + diffMod + "! Overwriting...");

                    diff = injection;
                    diffMod = Path.GetFileName(filePath);
                } catch (Exception ex) {
                    ModEntry.INSTANCE.Monitor.Log("Failed to load " + localConfigPath + ".", LogLevel.Error);
                    ModEntry.INSTANCE.Monitor.Log(ex.Message, LogLevel.Error);
                }
            }

            if (diffMod != null)
                ModEntry.INSTANCE.Monitor.Log(string.Format("{0} injected asset {1}.xnb", diffMod, assetName), LogLevel.Info);

            return diff;
        }
    }
}
