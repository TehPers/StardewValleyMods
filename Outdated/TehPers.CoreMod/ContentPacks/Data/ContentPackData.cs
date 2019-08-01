using System.Collections.Generic;

namespace TehPers.CoreMod.ContentPacks.Data {
    // TODO: Support both string values (comma separated values like CP) and array values
    internal class ContentPackData {
        /// <summary>Paths to additional content files which should be loaded as part of this content pack.</summary>
        public string[] Include { get; set; } = new string[0];

        /// <summary>Details about the config file. Format is { propertyName, possibleValues }.</summary>
        public Dictionary<string, PossibleConfigValuesData> Config { get; set; } = new Dictionary<string, PossibleConfigValuesData>();

        /// <summary>Custom base objects.</summary>
        public Dictionary<string, SObjectData> Objects { get; set; } = new Dictionary<string, SObjectData>();

        /// <summary>Custom food.</summary>
        public Dictionary<string, FoodData> Food { get; set; } = new Dictionary<string, FoodData>();

        /// <summary>Custom weapons.</summary>
        public Dictionary<string, WeaponData> Weapons { get; set; } = new Dictionary<string, WeaponData>();

        /// <summary>Custom recipes.</summary>
        public List<RecipeData> Recipes { get; set; } = new List<RecipeData>();
    }
}
