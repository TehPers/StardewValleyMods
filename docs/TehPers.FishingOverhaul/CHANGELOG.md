# Changelog

Changelog for [Teh's Fishing Overhaul].

## 3.2.0 - 2021-12-23

### Added

- Add fishing bait and bobber to `FishingInfo` in the API.
- Add event for after the default fishing info has been created to modify it.
  - Map overrides, EMP overrides, and magic bait have been changed to event handlers for this.
- Add event for after fish/trash/treasure chances have been created to modify them.
  - Curiosity lure's effect has been changed to an event handler for this.
- Add methods for modifying the chances of finding fish and treasure in `ISimplifiedFishingApi`.
  This should make it easier for other mods to modify those chances without needing to directly
  reference the full API.

### Changed

- **(Breaking change)** Renamed `IFishingApi.OpenedChest` to `OpeningChest` and it now uses
  `OpeningChestEventArgs` instead of passing the list of items directly. This gives mods more
  flexibility if they want to modify the contents of the chest.
- **(Breaking change)** `IFishingApi.CaughtItem` now uses `CaughtItemEventArgs` instead of
  passing the caught item directly.
- **(Breaking change)** Renamed `CustomEvent` to `CustomEventArgs` and converted it to a class.
  This change was made to keep it consistent with other event handlers.

### Fixed

- Fixed magic bait only checking for fish between 6:00 (6 am) and 20:00 (8 pm).
- Fixed `IFishingApi.OpenedChest` not being invoked when a fishing chest is opened.

## 3.1.2 - 2021-12-22

### Fixed

- Fix "AnyWithItem" treasure entry filter matching every entry _except_ the entries with that item.
- Fix "ItemKeys" treasure entry filter matching every entry that contains _any_ of those items
  rather than _all_ of those items.
- Fix JSON schemas having unnecessary newlines.

### Changed

- Warnings when loading content packs should now be more helpful.

### Removed

- Remove duplicate strange doll treasure entry.

## 3.1.1 - 2021-12-21

### Added

- Add global dart frequency config option for adjusting fish dart frequency across all fish.

### Fixed

- Fix West Ginger Island having tiger slime eggs instead of snake skulls.
- Fix caught trash not showing up in collections.
- Fix game thinking you keep catching record sized items after catching a single record-sized fish.
- Fix Secret Notes and Journal Scraps being far more common than they should be.

## 3.1.0 - 2021-12-20

### Added

- Add config option to enable or disable treasure chances being inverted on perfect catch.

### Changed

- Fishing depth (how far the bobber is from you) is now represented by the chances in the HUD.
- Improved content pack documentation and fixed some parts that were wrong.

### Fixed

- Fix treasure chances being always inverted. This should now only happen on perfect catches
  when the setting is enabled.

## 3.0.2 - 2021-12-17

### Fixed

- Fix Beach farm not having the correct fish and trash.
- Fix secret items not being catchable in farms.
- Fix map property overrides in farms not being applied.
- Fix max depth in availability info being ignored.

## 3.0.1 - 2021-12-13

### Fixed

- Fix incorrect fish being available in the mines.

## 3.0.0 - 2021-12-13

### Added

- Add two dependencies: Content Patcher and TehCore. Most users will already have CP installed
  though.
- Generic Mod Config Menu is now supported. You can edit the configs in-game now.
- All kinds of items can be used as fish, trash, and treasure now.
- Add better control over when items are available to be caught (as fish, trash, or treasure).
- Add content pack support. Combined with CP support, some powerful content packs can be made now.
- Add a `tfo_entries` console command to see what fish/trash/treasure entries are registered.
- Add an example content pack showing how different types of items can be added as entries within
  the mod.

### Changed

- Mod is now dependent on SDV 1.5.5 and will not be backported to any earlier versions of the game.
- Legendary fish cannot be caught multiple times by default anymore. Instead, that functionality
  has been moved to a content pack.

### Removed

- The unaware fish event is gone now. I may add it again as a separate add-on mod later, but there
  are currently no plans to reintroduce it to TFO directly.
- The config settings for treasure quality are now gone. They don't do anything anyway.
- You can no longer configure fish in a single JSON file. Instead, you must use a content pack.

[teh's fishing overhaul]: https://www.nexusmods.com/stardewvalley/mods/866
