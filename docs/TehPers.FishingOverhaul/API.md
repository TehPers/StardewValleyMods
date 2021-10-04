# Teh's Fishing Overhaul - API

## Simplified API

TODO: simplified API

## Full API

To access the full API, add a reference to the following NuGet package:

```text
> TehPers.FishingOverhaul.Api
```

Additionally, in order to include the referenced library with your mod, you will need to add the following line to your project's `.csproj` file inside of a `<PropertyGroup>` tag:

```xml
<PropertyGroup>
    <!-- Add this to the property group -->
    <BundleExtraAssemblies>ThirdParty</BundleExtraAssemblies>
</PropertyGroup>
```

If you compile your mod, you should now see a couple new files in your mod's output directory:

- `TehPers.FishingOverhaul.Api.dll` (and `.pdb`): These are required to use the API for Teh's Fishing Overhaul
- `TehPers.Core.Api.dll` (and `.pdb`): These are required to use the API for Teh's Core Mod, which is needed to access the fishing APIs
- `Ninject.dll`: A dependency injection framework used by Teh's Core API to expose APIs

**Make sure to add `TehPers.FishingOverhaul` and `TehPers.Core` as dependencies to your mod's manifest!** If your mod does not need them to function, then add them as optional dependencies so that they are loaded first.

### Requesting the API instance

The different API types can be pulled through a mod kernel. For more information, check out the relevant documentation on Teh's Core API. In short, create your mod kernel and request the types that you need:

```cs
// YourMod.cs (class that extends StardewValleyAPI.Mod)

// Request the mod kernel
var kernel = ModServices.Factory.GetKernel(this);

// Inject any content sources you want. Make sure to inject any dependencies they have as well.
// Several types are automatically injected for you, including IModHelper and IManifest
kernel.GlobalProxyRoot.Bind<IFishingContentSource>()
    .To<YourContentSource>()
    .InSingletonScope(); // any scope works fine

// Request the fishing API
var fishingApi = kernel.Get<IFishingApi>();
```

### `IFishingApi`

This interface exposes the primary fishing API. This interface primarily acts as a source of truth for how fishing should behave. New content cannot be added through this interface, however it can be used to see how fishing behaves with the content that is already loaded. For example, a separate fishing HUD mod could use this interface to see what fish can be caught by the current user, what the chances of catching those fish are, and which of those fish are legendary. Similarly, treasure and trash can be retrieved from this interface.

Make sure to read the documentation for how to use the interface. To actually create instances of the items from their `NamespacedKey`s, look at `INamespaceRegistry` from Teh's Core API.

### `IFishingContentSource`

This interface allows custom fishing content to be added to the game. Fishing content includes:

- Adding new fish traits to allow items to be treated as fish
- Adding new fish/trash/treasure entries to make items catchable as fish/trash/treasure

The interface is fairly straightforward, so make sure to read the documentation for it. Whenever fishing content should be reloaded, make sure to invoke `IFishingApi.RequestReload()`.
