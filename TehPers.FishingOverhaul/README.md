# Teh's Fishing Overhaul

**Version** 3.0.0 for **SDV** 3.5.5+ and **SMAPI** 3.13.0+

Completely reworks fishing. Changes include:

- Control every aspect of fishing! Configure the chances of hitting fish instead of trash, the difficulty of catching fish, the chances of finding treasure, what fish/treasure/trash you'll find, where you'll find it all, and how the fish behave.
  - Add new fish, adjust fish behaviors, and control where they can be caught. If you want to be able to catch diamonds, femurs, or even fishing rods while fishing, go ahead!
- Perfect catches are more rewarding. As you get more perfect catches in a row, your streak increases, and fishing becomes more rewarding.
- Legendary fish are no longer caught by standing in a specific spot and casting your line. Now, they are caught like any other fish, but there is a very low chance you'll find one! (configurable)
  - Optionally allow legendary fish to be caught multiple times
- Support for [Generic Mod Config Menu][gmcm], including in-game config editing.
  - Some complex settings may not be modifiable through GMCM though.
- Create your own content packs, or install a content pack created by someone else!

## Configs

Most config settings can be modified in-game with [Generic Mod Config Menu][gmcm]. For more fine-tuned control over the settings, the configs can also be modified through their respective `.json` files in the mod's `config` folder.

| File            | Purpose                                                     |
| --------------- | ----------------------------------------------------------- |
| `fish.json`     | Config settings regarding fishing and the fishing minigame. |
| `treasure.json` | Config settings regarding treasure.                         |
| `hud.json`      | Config settings regarding the fishing HUD.                  |

If you want to configure which fish, trash, or treasure are available to catch and when you can catch them, [create][create a content pack] or install a content pack.

## Content packs

This mod supports content packs. Each content pack can have several different files. They are all optional.

| File              | Purpose                                                                                       |
| ----------------- | --------------------------------------------------------------------------------------------- |
| `fishTraits.json` | Add new fish traits. This configures how the fish behave, but not where/when they are caught. |
| `fish.json`       | Add new fish availabilities. This configures where/when fish can be caught.                   |
| `trash.json`      | Add new trash availabilities.                                                                 |
| `treasure.json`   | Add new treasure avaailabilities.                                                             |

There are also JSON schemas available for each of these files. If your editor supports JSON schemas, then it is recommended you reference the appropriate schema:

| Editor             | How to reference the schema                                                                         |
| ------------------ | --------------------------------------------------------------------------------------------------- |
| Visual Studio      | At the top of the file editor right under the tabs, paste the schema URL into the "Schema" textbox. |
| Visual Studio Code | Add a `$schema` property to the root object in the file.                                            |

### Common

#### Namespaced key

A namespaced key is a unique identifier that is associated with an item. For example, a namespaced key may refer to wood, pineapples, iridium bands, or any other item in the game. Each namespaced key consists of both a namespace and a key. The format of a namespaced key in a JSON file is `namespace:key`. For vanilla items and items added through vanilla content files, namespaced keys all have the namespace "StardewValley" and follow this format:

| Item type | Key format                        |
| --------- | --------------------------------- |
| Craftable | `StardewValley:BigCraftable/<id>` |
| Boots     | `StardewValley:Boots/<id>`        |
| Clothing  | `StardewValley:Clothing/<id>`     |
| Flooring  | `StardewValley:Flooring/<id>`     |
| Furniture | `StardewValley:Furniture/<id>`    |
| Hat       | `StardewValley:Hat/<id>`          |
| Object    | `StardewValley:Object/<id>`       |
| Ring      | `StardewValley:Ring/<id>`         |
| Tool      | `StardewValley:Tool/<id>`         |
| Wallpaper | `StardewValley:Wallpaper/<id>`    |
| Weapon    | `StardewValley:Weapon/<id>`       |

Other mods may add their own namespaces and key formats.

#### Availability

Fish, trash, and treasure have availability information to determine when they can be found. These are the properties that are common to them all:

| Property           | Type       | Required | Value                                                                                                                                          |
| ------------------ | ---------- | -------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| `BaseChance`       | `number`   | Yes      | The base chance this will be caught. This is not a percentage chance, but rather a weight relative to all available entries.                   |
| `StartTime`        | `integer`  | No       | Time this becomes available (inclusive).                                                                                                       |
| `EndTime`          | `integer`  | No       | Time this is no longer available (exclusive).                                                                                                  |
| `Seasons`          | `array`    | No       | Seasons this can be caught in. Default is all. ("Spring", "Summer", "Fall", "Winter", "All")                                                   |
| `Weathers`         | `array`    | No       | Weathers this can be caught in. Default is all. ("Sunny", "Rainy", "All")                                                                      |
| `WaterTypes`       | `array`    | No       | The type of water this can be caught in. Each location handles this differently. Default is all. ("River", "PondOrOcean", "Freshwater", "All") |
| `MinFishingLevel`  | `integer`  | No       | Required fishing level to see this.                                                                                                            |
| `MaxFishingLevel`  | `integer?` | No       | Maximum fishing level required to see this, or null for no max.                                                                                |
| `IncludeLocations` | `array`    | No       | List of locations this should be available in. (see below)                                                                                     |
| `ExcludeLocations` | `array`    | No       | List of locations this should not be available in. This takes priority over `IncludeLocations`.                                                |

`IncludeLocations` and `ExcludeLocations` are arrays of location names. If `IncludeLocations` is empty, then it is assumed that all locations (except locations in `ExcludeLocations`) are valid. Additionally, `ExcludeLocations` takes priority over `IncludeLocations`. If a location appears in both arrays, then the item will not be available at that location.

Some locations have multiple names. For example, the mines have the location name "UndergroundMine", but each floor has the same location name. To specify which floor an item should be available on, use the name "UndergroundMine/N", where "N" is the floor number. Each floor can be referenced by either the name "UndergroundMine" or "UndergroundMine/N". This means that an item can be added to all floors in the mines by including it in the location "UndergroundMine", and can optionally be excluded from specific floors with "UndergroundMine/N".

### Fish traits

[**JSON Schema**][fish-traits schema]

The `fishTraits.json` file configures the traits for each fish.

| Property     | Type     | Required | Value                |
| ------------ | -------- | -------- | -------------------- |
| `$schema`    | `string` | No       | Optional schema URL. |
| `FishTraits` | `object` | Yes      | Fish traits to add.  |

Fish traits is a dictionary where the key is the namespaced key of the fish and the value is the actual traits:

| Property        | Type      | Required | Value                                             |
| --------------- | --------- | -------- | ------------------------------------------------- |
| `DartFrequency` | `integer` | Yes      | How often the fish darts in the fishing minigame. |
| `DartBehavior`  | `string`  | Yes      | How the fish darts during the fishing minigame.   |
| `MinSize`       | `integer` | Yes      | The minimum size the fish can be.                 |
| `MaxSize`       | `integer` | Yes      | The maximum size the fish can be.                 |
| `IsLegendary`   | `boolean` | No       | Whether the fish is legendary.                    |

### Fish

[**JSON Schema**][fish schema]

The `fish.json` file configures where and when fish can be caught.

| Property      | Type     | Required | Value                |
| ------------- | -------- | -------- | -------------------- |
| `$schema`     | `string` | No       | Optional schema URL. |
| `FishEntries` | `array`  | Yes      | Fish entries to add. |

Fish entries each configure when a specific fish can be made available. Multiple entries may refer to the same fish, allowing complex customization over when a fish is available and what the chances of catching that fish are.

| Property       | Type     | Required | Value                                     |
| -------------- | -------- | -------- | ----------------------------------------- |
| `FishKey`      | `string` | Yes      | The namespaced key for the fish.          |
| `Availability` | `object` | Yes      | The fish availability data for the entry. |

Fish availability determines when a fish is available and includes all the normal availability properties as well.

| Property          | Type      | Required | Value                                                                                                                |
| ----------------- | --------- | -------- | -------------------------------------------------------------------------------------------------------------------- |
| `DepthMultiplier` | `number`  | No       | Effect that sending the bobber by less than the max distance has on the chance. This value should be no more than 1. |
| `MaxDepth`        | `integer` | No       | The required fishing depth to maximize the chances of catching the fish.                                             |
| ...               | ...       | ...      | Other common availability properties.                                                                                |

### Trash

[**JSON Schema**][trash schema]

The `trash.json` file configures where and when trash can be caught.

| Property       | Type     | Required | Value                 |
| -------------- | -------- | -------- | --------------------- |
| `$schema`      | `string` | No       | Optional schema URL.  |
| `TrashEntries` | `array`  | Yes      | Trash entries to add. |

Trash entries each configure when a specific trash can be made available. Multiple entries may refer to the same trash item, allowing complex customization over when a trash is available and what the chances of catching that trash are.

| Property       | Type     | Required | Value                                  |
| -------------- | -------- | -------- | -------------------------------------- |
| `TrashKey`     | `string` | Yes      | The namespaced key for the trash item. |
| `Availability` | `object` | Yes      | The availability data for the entry.   |

The availability uses the common availability properties.

### Treasure

[**JSON Schema**][treasure schema]

The `trash.json` file configures where and when trash can be caught.

| Property          | Type     | Required | Value                    |
| ----------------- | -------- | -------- | ------------------------ |
| `$schema`         | `string` | No       | Optional schema URL.     |
| `TreasureEntries` | `array`  | Yes      | Treasure entries to add. |

Treasure entries each configure when a specific treasure can be made available. Multiple entries may refer to the same treasure item, allowing complex customization over when a treasure is available and what the chances of catching that treasure are.

| Property       | Type     | Required | Value                                     |
| -------------- | -------- | -------- | ----------------------------------------- |
| `TreasureKey`  | `string` | Yes      | The namespaced key for the treasure item. |
| `Availability` | `object` | Yes      | The availability data for the entry.      |

The availability uses the common availability properties.

## API

[gmcm]: https://www.nexusmods.com/stardewvalley/mods/5098
[create a content pack]: https://stardewvalleywiki.com/Modding:Content_packs#Create_a_content_pack
[fish-traits schema]: https://github.com/TehPers/StardewValleyMods/raw/full-rewrite/docs/schemas/TehPers.FishingOverhaul/contentPacks/fishTraits.schema.json
[fish schema]: https://github.com/TehPers/StardewValleyMods/raw/full-rewrite/docs/schemas/TehPers.FishingOverhaul/contentPacks/fish.schema.json
[trash schema]: https://github.com/TehPers/StardewValleyMods/raw/full-rewrite/docs/schemas/TehPers.FishingOverhaul/contentPacks/trash.schema.json
[treasure schema]: https://github.com/TehPers/StardewValleyMods/raw/full-rewrite/docs/schemas/TehPers.FishingOverhaul/contentPacks/treasure.schema.json
