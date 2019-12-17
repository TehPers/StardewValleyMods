using System;
using System.Collections.Generic;
using StardewValley;
using TehPers.Core.Api;
using TehPers.FishingFramework.Api.Config;
using TehPers.FishingFramework.Api.Events;

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
        /// Gets the global configuration. In singleplayer, this is loaded from the player's config files. In multiplayer, this is loaded from the server host's config files.
        /// </summary>
        IGlobalConfiguration GlobalConfig { get; }

        /// <summary>
        /// Gets the personal configuration. This is always loaded from the player's config files.
        /// </summary>
        IPersonalConfiguration PersonalConfig { get; }

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

        /// <summary>
        /// Raised before a fish is caught, right before the minigame appears.
        /// </summary>
        event EventHandler<FishCatchingEventArgs> FishCatching;

        /// <summary>
        /// Raised after a fish is caught, once the minigame has completed.
        /// </summary>
        event EventHandler<FishCaughtEventArgs> FishCaught;

        /// <summary>
        /// Raised after a fish is lost, once the minigame has completed.
        /// </summary>
        event EventHandler<FishLostEventArgs> FishLost;

        /// <summary>
        /// Raised before a treasure chest is opened.
        /// </summary>
        event EventHandler<TreasureOpeningEventArgs> TreasureOpening;

        /// <summary>
        /// Raised after a treasure chest is opened.
        /// </summary>
        event EventHandler<TreasureOpenedEventArgs> TreasureOpened;

        /// <summary>
        /// Raised before trash is caught.
        /// </summary>
        event EventHandler<TrashCatchingEventArgs> TrashCatching;

        /// <summary>
        /// Raised after trash is caught.
        /// </summary>
        event EventHandler<TrashCaughtEventArgs> TrashCaught;
    }
}
