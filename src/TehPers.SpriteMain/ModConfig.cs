using System.ComponentModel;
using TehPers.Core.Api.Json;

namespace TehPers.SpriteMain
{
    [JsonDescribe]
    internal class ModConfig
    {
        [Description("The scaler (\"Scale2X\", \"Scale3X\"), or null for no custom scaling.")]
        public ScalerName? Scaler { get; set; }
    }
}