using StardewValley;
using SObject = StardewValley.Object;

namespace TehCore.Saves {
    public interface ICustomItem {
        /// <summary>When this item is saved, what it should be replaced with in the inventory.</summary>
        /// <returns>The object this should be replaced with when saving.</returns>
        Item GetReplacement();

        /// <summary>Whether the custom serializer should serialize this item.</summary>
        bool ShouldSerialize { get; }

        /// <summary>Whether the custom serializer should deserialize this item.</summary>
        bool ShouldDeserialize { get; }
    }

    public interface ICustomItem<TModel> : ICustomItem {
        /// <summary>Stores all the info about this item onto a single object.</summary>
        /// <returns>An object containing the info about this item that needs to be saved.</returns>
        TModel Save();

        /// <summary>Loads save data into this object.</summary>
        /// <param name="model">The object containing the data to load.</param>
        void Load(TModel model);
    }
}
