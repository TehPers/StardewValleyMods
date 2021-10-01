# Getting Started

Before you can use the core mod's API, you must add the core mod as a dependency. If your mod does not require the core mod to be installed to function, you should still add it as an optional dependency to ensure that the core mod is initialized first.

## Getting a mod service

Simple mods rarely need more than to get a service or API exposed by another mod call some functions on it. For simple cases like these, it is possible to just request that service.

```cs
var factory = this.Helper.ModRegistry.GetApi<IModKernelFactory>("TehPers.Core");
var modService = factory.GlobalServices.Get<IPineappleService>();
```

This is recommended for simple use cases. If you would like to expose your own services to other mods, then it is instead recommended that you create your own mod kernel.

## Creating mod services

Sometimes mods need to expose services or APIs to other mods. While SMAPI's built-in API system works for most cases, it cannot handle mods which expose multiple APIs or which have APIs with custom nested types. For example, this API cannot be exposed with SMAPI:

```cs
public interface IPineappleApi {
    IPineappleProperties GetProperties(PineappleId id);
    void PlacePineapple(PineappleId id, GameLocation location);
}
```

Additionally, if your mod exposes multiple different services, it may be preferable to keep those services separated rather than merge them all into a single type. For example:

```cs
public interface IPineapplePropertyService {
    IPineappleProperties GetProperties(PineappleId id);
}

public interface IPineapplePlacementService {
    void PlacePineapple(PineappleId id, GameLocation location);
}
```

This can be accomplished by exposing your mod's services through the core mod. Services are exposed via dependency injection. This means that services can be automatically constructed and injected into each other.

```cs
internal class PineapplePropertyService : IPineapplePropertyService {
    public PineapplePropertyService(IMonitor monitor) {
        monitor.Log("Your mod's monitor is automatically injected.");
    }

    /* ... */
}

internal class PineapplePlacementService : IPineapplePlacementService {
    public PineapplePlacementService(IPineapplePropertyService propertyService) {
        // propertyService is automatically injected
    }

    /* ... */
}

// In RegisterServices
modKernel.GlobalProxyRoot
    .Bind<IPineapplePropertyService>()
    .To<PineapplePropertyService>();

modKernel.GlobalProxyRoot
    .Bind<IPineapplePlacementService>()
    .To<PineapplePlacementService>();
```
