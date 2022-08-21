using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Gui.Api;

namespace TehPers.Core.Gui;

/// <inheritdoc />
internal class GuiBuilder : IGuiBuilder
{
    private readonly Dictionary<string, object> extensions = new();

    /// <inheritdoc />
    public bool TryAddExtension(string key, object extension)
    {
        return this.extensions.TryAdd(key, extension);
    }

    /// <inheritdoc />
    public object? TryGetExtension(string key)
    {
        return this.extensions.TryGetValue(key, out var value) ? value : default;
    }
}
