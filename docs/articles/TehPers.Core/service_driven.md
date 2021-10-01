# Creating a Service Driven Mod

Any mod that depends on services provided by itself, other mods, or even SMAPI can be turned into a service-driven mod. The easiest way to do so is to implement `IServiceDrivenMod` in your mod's class and modify your mod's `Entry` method:

```cs
public class ModEntry : Mod, IServiceDrivenMod {
    public void Entry(IModHelper helper) {
        this.Register();
    }

    public void RegisterServices(IModKernel modKernel) {
        // Bind your services here
    }

    public void GameLoaded(IModKernel modKernel) {
        // All services should be bound now, so
        // perform any initialization logic here
    }
}
```
