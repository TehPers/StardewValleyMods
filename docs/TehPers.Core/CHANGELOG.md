# Changelog

Changelog for [TehCore].

## 1.0.2 - Unreleased

### Added

- Add ModDrop update keys.
- Add special handling for creating Caroline's necklace.

### Changed

- Update to new SMAPI content API.
- Change `BindingExtensions.BindForeignModApi<TApi>` to give a nullable binding because the mod could not have an API.

### Removed

- Remove asset tracking because SMAPI supports asset loading events now. This also cleans up the console output quite a bit.

## 1.0.1 - 2021-12-23

### Added

- Improved documentation for public API members.

### Removed

- Removed useless `DataStore` class from the API. Services are not expected to be stateless anyway.

[tehcore]: https://www.nexusmods.com/stardewvalley/mods/3256
