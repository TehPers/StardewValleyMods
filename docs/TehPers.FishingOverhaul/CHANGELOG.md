# Changelog

Changelog for [Teh's Fishing Overhaul].

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

- Add two dependencies: Content Patcher and TehCore. Most users will already have CP installed though.
- Generic Mod Config Menu is now supported. You can edit the configs in-game now.
- All kinds of items can be used as fish, trash, and treasure now.
- Add better control over when items are available to be caught (as fish, trash, or treasure).
- Add content pack support. Combined with CP support, some powerful content packs can be made now.
- Add a `tfo_entries` console command to see what fish/trash/treasure entries are registered.
- Add an example content pack showing how different types of items can be added as entries within the mod.

### Changed

- Mod is now dependent on SDV 1.5.5 and will not be backported to any earlier versions of the game.
- Legendary fish cannot be caught multiple times by default anymore. Instead, that functionality has been moved to a content pack.

### Removed

- The unaware fish event is gone now. I may add it again as a separate add-on mod later, but there are currently no plans to reintroduce it to TFO directly.
- The config settings for treasure quality are now gone. They don't do anything anyway.
- You can no longer configure fish in a single JSON file. Instead, you must use a content pack.

[Teh's Fishing Overhaul]: https://www.nexusmods.com/stardewvalley/mods/866
