using System.Collections.Generic;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.ContentPacks.Tokens.Parsing;

namespace TehPers.CoreMod.ContentPacks.Data {
    internal class ContentPackData {
        /// <summary>Paths to additional content files which should be loaded as part of this content pack.</summary>
        public string[] Include { get; set; } = new string[0];

        // TODO: Support both string values (comma separated values like CP) and array values
        /// <summary>Details about the config file. Format is { propertyName, possibleValues }.</summary>
        public Dictionary<string, PossibleConfigValuesData> Config { get; set; } = new Dictionary<string, PossibleConfigValuesData>();

        /// <summary>Custom base objects added by the content pack, which override the texture Maps/springobjects.</summary>
        public Dictionary<string, SObjectData> Objects { get; set; } = new Dictionary<string, SObjectData>();
    }
}
