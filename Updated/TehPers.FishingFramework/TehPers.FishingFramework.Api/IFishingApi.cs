using System.Collections.Generic;
using StardewValley;
using TehPers.Core.Api;
using TehPers.FishingFramework.Api.Config;

namespace TehPers.FishingFramework.Api
{
    /// <summary>
    /// The fishing API which allows mods to view and modify fishing properties. This API only affects the local player. If you would like your mod to affect other players, please use the SMAPI multiplayer API and rely on other players having your mod installed.
    /// </summary>
    public interface IFishingApi
    {
        /// <summary>
        /// Gets all of the fish that can be caught while fishing.
        /// </summary>
        ISet<IFishAvailability> Fish { get; }

        /// <summary>
        /// Gets a mapping of fish keys to fish traits.
        /// </summary>
        IDictionary<NamespacedId, IFishTraits> FishTraits { get; }

        /// <summary>
        /// Gets all of the trash that can be found while fishing.
        /// </summary>
        ISet<ITrashAvailability> Trash { get; }

        /// <summary>
        /// Gets all of the treasure that can be found while fishing.
        /// </summary>
        ISet<ITreasureAvailability> Treasure { get; }

        /// <summary>
        /// Gets or sets the perfect fishing streak of the local <see cref="Farmer"/>.
        /// </summary>
        int FishingStreak { get; set; }

        /// <summary>
        /// Gets a mapping between <see cref="Farmer"/>s and their chances of finding fish (instead of trash) while fishing.
        /// </summary>
        IFishingChances FishChances { get; }

        /// <summary>
        /// Gets a mapping between <see cref="Farmer"/>s and their chances of finding treasure while fishing.
        /// </summary>
        IFishingChances TreasureChances { get; }
    }
}
