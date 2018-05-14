using System.ComponentModel;
using TehPers.Core.Helpers.Json;

namespace TehPers.Core.Configs {
    [JsonDescribe]
    internal class ConfigMain {
        [Description("Settings for input handing.")]
        public ConfigInputs Input { get; set; } = new ConfigInputs();
    }

    internal class ConfigInputs {
        [Description("For text inputs, number of seconds between when an input key is pressed and when it starts repeating.")]
        public float KeyRepeatDelay { get; set; } = 1F;

        [Description("For text inputs, The minimum number of times a key is repeated each second it's held.")]
        public float KeyRepeatMinFrequency { get; set; } = 15F;

        [Description("For text inputs, the maximum number of times a key is repeated each second it's held.")]
        public float KeyRepeatMaxFrequency { get; set; } = 30F;

        [Description("For text inputs, the number of seconds it takes for the frequency to go from min to max.")]
        public float KeyRepeatRampTime { get; set; } = 0.5F;
    }
}
