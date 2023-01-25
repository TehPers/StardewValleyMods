using StardewModdingAPI;
using System;

namespace TehPers.Core.Gui.Api.Extensions;

/// <summary>
/// Initialization methods for TehCore - Gui.
/// </summary>
public static class ModInitializer
{
    internal const string modUniqueId = "TehPers.Core.Gui";

    internal static readonly string extensionUniqueId =
        $"{ModInitializer.modUniqueId}:defaultComponentProvider/{typeof(ModInitializer).Assembly.FullName}";

    /// <summary>
    /// Gets and initializes the TehCore GUI API.
    /// </summary>
    /// <param name="registry">The mod registry.</param>
    /// <returns>The TehCore GUI API.</returns>
    public static ICoreGuiApi GetGuiApi(this IModRegistry registry)
    {
        // Get the core mod API
        var api = registry.GetApi<ICoreGuiApi>(ModInitializer.modUniqueId);
        if (api is null)
        {
            throw new InvalidOperationException(
                $"{nameof(ModInitializer.GetGuiApi)} must be called after TehCore - Gui loads. Make sure to add '{ModInitializer.modUniqueId}' as a dependency to your mod's manifest.json."
            );
        }

        // Initialize the default component provider
        // This must be done by each mod using TehPers.Core.Gui because the interfaces are different types (with the same names) for each consumer
        ModInitializer.InitializeGuiApi(api);

        return api;
    }

    /// <summary>
    /// Initializes the TehCore - GUI API. This is automatically called by <see cref="GetGuiApi"/>.
    /// </summary>
    /// <param name="api">The TehCore - GUI API.</param>
    public static void InitializeGuiApi(ICoreGuiApi api)
    {
        api.GuiBuilder.TryAddExtension(
            ModInitializer.extensionUniqueId,
            api.InternalApi.CreateDefaultComponentProvider()
        );
    }
}
