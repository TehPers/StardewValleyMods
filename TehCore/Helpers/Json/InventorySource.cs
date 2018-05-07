using Microsoft.Xna.Framework;
using TehCore.Enums;

namespace TehCore.Helpers.Json {
    public class InventorySource {
        public InventoryType Type { get; set; }
        public string LocationName { get; set; }
        public Vector2 Position { get; set; }
        public bool IsStructure { get; set; }

        public InventorySource(InventoryType type, string locationName, Vector2 position, bool isStructure) {
            this.Type = type;
            this.LocationName = locationName;
            this.Position = position;
        }
    }
}