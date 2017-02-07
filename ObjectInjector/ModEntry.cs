using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;

namespace TehPers.Stardew.ObjectInjector {
    public class ModEntry : Mod {
        public bool loaded = false;

        public override void Entry(IModHelper helper) {
            GameEvents.UpdateTick += UpdateTick;
        }

        private void UpdateTick(object sender, EventArgs e) {
            if (!loaded && Game1.objectInformation != null) {
                loaded = true;
                string path = Path.Combine(this.Helper.DirectoryPath, "Configs");

                if (!Directory.Exists(path)) {
                    try {
                        this.Monitor.Log("Creating directory " + path);
                        Directory.CreateDirectory(path);
                    } catch (Exception ex) {
                        this.Monitor.Log("Could not create directory " + path + "! Please create it yourself.", LogLevel.Error);
                        this.Monitor.Log(ex.Message, LogLevel.Error);
                        return;
                    }
                }

                string[] configList = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
                foreach (string configPath in configList) {
                    try {
                        ObjectConfig config = this.Helper.ReadJsonFile<ObjectConfig>(configPath);
                        if (config != null && config.Enabled) {
                            this.Monitor.Log("Loading " + Path.GetFileName(configPath), LogLevel.Info);
                            foreach (KeyValuePair<int, ObjectInfo> data in config.InjectedInfo)
                                Game1.objectInformation[data.Key] = data.Value.ToString();
                        }
                        this.Helper.WriteJsonFile<ObjectConfig>(configPath, config);
                    } catch (Exception ex) {
                        this.Monitor.Log("Failed to load " + Path.GetFileName(configPath) + ".", LogLevel.Error);
                        this.Monitor.Log(ex.Message, LogLevel.Error);
                        this.Monitor.Log("Maybe your format is invalid?", LogLevel.Warn);
                    }
                }
            }
        }
    }
}
