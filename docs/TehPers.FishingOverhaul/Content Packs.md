# Teh's Fishing Overhaul - Content packs

Each content pack can have several different files. They are all optional.

| File                              | Purpose                                                                                       |
| --------------------------------- | --------------------------------------------------------------------------------------------- |
| [`fishTraits.json`](#fish-traits) | Add new fish traits. This configures how the fish behave, but not where/when they are caught. |
| [`fish.json`](#fish)              | Add new fish availabilities. This configures where/when fish can be caught.                   |
| [`trash.json`](#trash)            | Add new trash availabilities.                                                                 |
| [`treasure.json`](#treasure)      | Add new treasure avaailabilities.                                                             |

There are also JSON schemas available for each of these files. If your editor supports JSON schemas, then it is recommended you reference the appropriate schema:

| Editor             | How to reference the schema                                                                         |
| ------------------ | --------------------------------------------------------------------------------------------------- |
| Visual Studio      | At the top of the file editor right under the tabs, paste the schema URL into the "Schema" textbox. |
| Visual Studio Code | Add a `$schema` property to the root object in the file.                                            |

## Fish traits

[**JSON Schema**][fish-traits schema]

The `fishTraits.json` file configures the traits for each fish.

| Property     | Type     | Required | Default | Description          |
| ------------ | -------- | -------- | ------- | -------------------- |
| `$schema`    | `string` | No       | N/A     | Optional schema URL. |
| `FishTraits` | `object` | Yes      | N/A     | Fish traits to add.  |

Fish traits is a dictionary where the key is the namespaced key of the fish and the value is the actual traits:

| Property        | Type      | Required | Default | Description                                       |
| --------------- | --------- | -------- | ------- | ------------------------------------------------- |
| `DartFrequency` | `integer` | Yes      | N/A     | How often the fish darts in the fishing minigame. |
| `DartBehavior`  | `string`  | Yes      | N/A     | How the fish darts during the fishing minigame.   |
| `MinSize`       | `integer` | Yes      | N/A     | The minimum size the fish can be.                 |
| `MaxSize`       | `integer` | Yes      | N/A     | The maximum size the fish can be.                 |
| `IsLegendary`   | `boolean` | No       | `false` | Whether the fish is legendary.                    |

## Fish

[**JSON Schema**][fish schema]

The `fish.json` file configures where and when fish can be caught.

| Property      | Type     | Required | Default | Description          |
| ------------- | -------- | -------- | ------- | -------------------- |
| `$schema`     | `string` | No       | N/A     | Optional schema URL. |
| `FishEntries` | `array`  | Yes      | N/A     | Fish entries to add. |

Fish entries each configure when a specific fish can be made available. Multiple entries may refer to the same fish, allowing complex customization over when a fish is available and what the chances of catching that fish are.

| Property           | Type     | Required | Default | Description                               |
| ------------------ | -------- | -------- | ------- | ----------------------------------------- |
| `FishKey`          | `string` | Yes      | N/A     | The namespaced key for the fish.          |
| `AvailabilityInfo` | `object` | Yes      | N/A     | The fish availability data for the entry. |

Fish availability determines when a fish is available and includes all the normal availability properties as well.

| Property          | Type      | Required | Default | Description                                                                                                                          |
| ----------------- | --------- | -------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| `DepthMultiplier` | `number`  | No       | 0.1     | Effect that sending the bobber by less than the max distance has on the chance. This value should be no more than 1. Default is 0.1. |
| `MaxDepth`        | `integer` | No       | 4       | The required fishing depth to maximize the chances of catching the fish. Default is 4.                                               |
| ...               | ...       | ...      | ...     | Other common availability properties.                                                                                                |

## Trash

[**JSON Schema**][trash schema]

The `trash.json` file configures where and when trash can be caught.

| Property       | Type     | Required | Default | Description           |
| -------------- | -------- | -------- | ------- | --------------------- |
| `$schema`      | `string` | No       | N/A     | Optional schema URL.  |
| `TrashEntries` | `array`  | Yes      | N/A     | Trash entries to add. |

Trash entries each configure when a specific trash can be made available. Multiple entries may refer to the same trash item, allowing complex customization over when a trash is available and what the chances of catching that trash are.

| Property           | Type     | Required | Default | Description                            |
| ------------------ | -------- | -------- | ------- | -------------------------------------- |
| `TrashKey`         | `string` | Yes      | N/A     | The namespaced key for the trash item. |
| `AvailabilityInfo` | `object` | Yes      | N/A     | The availability data for the entry.   |

The availability uses the common availability properties.

## Treasure

[**JSON Schema**][treasure schema]

The `treasure.json` file configures where and when treasure can be caught.

| Property          | Type     | Required | Default | Description              |
| ----------------- | -------- | -------- | ------- | ------------------------ |
| `$schema`         | `string` | No       | N/A     | Optional schema URL.     |
| `TreasureEntries` | `array`  | Yes      | N/A     | Treasure entries to add. |

Treasure entries each configure when a specific treasure can be made available. Multiple entries may refer to the same treasure item, allowing complex customization over when a treasure is available and what the chances of catching that treasure are.

| Property           | Type      | Required | Default | Description                                                                 |
| ------------------ | --------- | -------- | ------- | --------------------------------------------------------------------------- |
| `AvailabilityInfo` | `object`  | Yes      | N/A     | The availability data for the entry.                                        |
| `ItemKeys`         | `array`   | Yes      | N/A     | The possible namespaced keys for the loot. The item key is chosen randomly. |
| `MinQuantity`      | `integer` | No       | 1       | The minimum quantity of the item. This is only valid for stackable items.   |
| `MaxQuantity`      | `integer` | No       | 1       | The maximum quantity of the item. This is only valid for stackable items.   |
| `AllowDuplicates`  | `boolean` | No       | `true`  | Whether this can be found multiple times in one chest.                      |

The availability uses the common availability properties.

## Common

There are a few common properties and types that are used throughout multiple config files.

### Namespaced key

A namespaced key is a unique identifier that is associated with an item. For example, a namespaced key may refer to wood, pineapples, iridium bands, or any other item in the game. Each namespaced key consists of both a namespace and a key. The format of a namespaced key in a JSON file is `namespace:key`. For vanilla items and items added through vanilla content files, namespaced keys all have the namespace "StardewValley" and follow this format:

| Item type | Key format                                                                  |
| --------- | --------------------------------------------------------------------------- |
| Craftable | `StardewValley:BigCraftable/<id>`                                           |
| Boots     | `StardewValley:Boots/<id>`                                                  |
| Clothing  | `StardewValley:Clothing/<id>`                                               |
| Flooring  | `StardewValley:Flooring/<id>`                                               |
| Furniture | `StardewValley:Furniture/<id>`                                              |
| Hat       | `StardewValley:Hat/<id>`                                                    |
| Object    | `StardewValley:Object/<id>`                                                 |
| Ring      | `StardewValley:Ring/<id>`                                                   |
| Tool      | `StardewValley:Tool/<type>` or `StardewValley:Tool/<type>/<quality number>` |
| Wallpaper | `StardewValley:Wallpaper/<id>`                                              |
| Weapon    | `StardewValley:Weapon/<id>`                                                 |

Not all key formats are listed. Also, other mods may add their own namespaces and key formats.

### Availability

Fish, trash, and treasure have availability information to determine when they can be found. These are the properties that are common to them all:

| Property           | Type       | Required | Default   | Description                                                                                                                    |
| ------------------ | ---------- | -------- | --------- | ------------------------------------------------------------------------------------------------------------------------------ |
| `BaseChance`       | `number`   | Yes      | N/A       | The base chance this will be caught. This is not a percentage chance, but rather a weight relative to all available entries.   |
| `WaterTypes`       | `array`    | No       | `["All"]` | The type of water this can be caught in. Each location handles this differently. ("River", "PondOrOcean", "Freshwater", "All") |
| `MinFishingLevel`  | `integer`  | No       | 0         | Required fishing level to see this.                                                                                            |
| `MaxFishingLevel`  | `integer?` | No       | `null`    | Maximum fishing level required to see this, or null for no max.                                                                |
| `IncludeLocations` | `array`    | No       | `[]`      | List of locations this should be available in. (see below)                                                                     |
| `ExcludeLocations` | `array`    | No       | `[]`      | List of locations this should not be available in. This takes priority over `IncludeLocations`.                                |
| `When`             | `object`   | No       | `{}`      | Content Patcher [conditions] for when this should be available.                                                                |

`IncludeLocations` and `ExcludeLocations` are arrays of location names. If `IncludeLocations` is empty, then it is assumed that all locations (except locations in `ExcludeLocations`) are valid. Additionally, `ExcludeLocations` takes priority over `IncludeLocations`. If a location appears in both arrays, then the item will not be available at that location.

Some locations have multiple names. For example, the mines have the location name "UndergroundMine", but each floor has the same location name. To specify which floor an item should be available on, use the name "UndergroundMine/N", where "N" is the floor number. Each floor can be referenced by either the name "UndergroundMine" or "UndergroundMine/N". This means that an item can be added to all floors in the mines by including it in the location "UndergroundMine", and can optionally be excluded from specific floors with "UndergroundMine/N".

[fish-traits schema]: https://raw.githubusercontent.com/TehPers/StardewValleyMods/full-rewrite/docs/TehPers.FishingOverhaul/schemas/contentPacks/fishTraits.schema.json
[fish schema]: https://raw.githubusercontent.com/TehPers/StardewValleyMods/full-rewrite/docs/TehPers.FishingOverhaul/schemas/contentPacks/fish.schema.json
[trash schema]: https://raw.githubusercontent.com/TehPers/StardewValleyMods/full-rewrite/docs/TehPers.FishingOverhaul/schemas/contentPacks/trash.schema.json
[treasure schema]: https://raw.githubusercontent.com/TehPers/StardewValleyMods/full-rewrite/docs/TehPers.FishingOverhaul/schemas/contentPacks/treasure.schema.json
[conditions]: https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/author-tokens-guide.md#conditions
