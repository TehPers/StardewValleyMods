using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using TehCore.Configs;

namespace FishingOverhaul.Configs {

    [JsonDescribe]
    public class ConfigMain {

        [Description("Whether or not this mod should make changes to the game")]
        public bool ModEnabled { get; set; } = true;

        [Description("Whether to make the config files as small as possible. This makes them really hard to edit!")]
        public bool MinifyConfigs { get; set; } = false;

        [Description("Settings for trash")]
        public ConfigTrash TrashSettings { get; set; } = new ConfigTrash();

        [Description("Settings for streaks")]
        public ConfigStreak StreakSettings { get; set; } = new ConfigStreak();

        [Description("A test key setting")]
        public Keys TestKey { get; set; } = Keys.None;

        [JsonDescribe]
        public class ConfigTrash {
            [Description("Base chance that the player will get trash")]
            public float BaseChance { get; set; } = 0.4F;

            /// <summary>Value added to <see cref="BaseChance"/> for each perfect catch in this streak</summary>
            [Description("Value added to " + nameof(ConfigTrash.BaseChance) + " for each perfect catch in this streak")]
            public float StreakEffect { get; set; } = -0.05F;
        }

        [JsonDescribe]
        public class ConfigStreak {

        }
    }
}
