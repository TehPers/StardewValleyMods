using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishingOverhaul.Configs;

namespace FishingOverhaul {
    public class FishingSettings {
        public int? FishChanceOverride { get; set; }
        public Dictionary<string, Dictionary<int, FishData>> FishDataOverrides { get; set; }

    }
}
