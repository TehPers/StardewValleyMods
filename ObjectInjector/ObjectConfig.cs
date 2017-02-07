using System.Collections.Generic;

namespace TehPers.Stardew.ObjectInjector {
    public class ObjectConfig {
        public bool Enabled { get; set; } = true;
        public Dictionary<int, ObjectInfo> InjectedInfo { get; set; } = new Dictionary<int, ObjectInfo>() {
            { 999, new ObjectInfo("Test", "Replace me") }
        };
    }

    public class ObjectInfo {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Edibility { get; set; }
        public string Type { get; set; }
        public int? Category { get; set; }
        public string Description { get; set; }

        public ObjectInfo(string name, string description, int price = -1, int edibility = -300, string type = "Basic", int? category = null) {
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Edibility = edibility;
            this.Type = type;
            this.Category = category;
        }

        public override string ToString() {
            return string.Format("{0}/{1}/{2}/{3}{4}/{5}", Name, Price, Edibility, Type, (Category != null ? " " + Category : ""), Description);
        }
    }
}
