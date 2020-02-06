using System.Collections.Generic;
using System.ComponentModel;
using TehPers.Core.Api;
using TehPers.Core.Api.Json;

namespace TehPers.Core.Items
{
    [JsonDescribe]
    public class IndexRegistryData
    {
        [Description("The format version of this file.")]
        public string Version { get; set; } = "1.0";

        [Description("The assigned indexes.")]
        public Dictionary<NamespacedId, int> Indexes { get; set; } = new Dictionary<NamespacedId, int>();
    }
}