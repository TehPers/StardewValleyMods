using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using TehPers.Core.Enums;

namespace TehPers.Core.Helpers.Json {
    public class InventorySource {
        public InventoryType Type { get; set; }
        public string LocationName { get; set; }
        public Vector2 Position { get; set; }
        public bool IsStructure { get; set; }
        public long? FarmerId { get; set; }

        public InventorySource() { }

        public InventorySource(Farmer farmer) {
            this.Type = InventoryType.PLAYER;
            this.FarmerId = farmer.UniqueMultiplayerID;
        }

        public InventorySource(GameLocation location, Chest chest) {
            this.Type = InventoryType.CHEST;
            this.LocationName = location.Name;
            this.Position = chest.TileLocation;
            this.IsStructure = location.isStructure.Value;
        }
    }
}