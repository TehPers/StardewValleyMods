using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Components;

/// <summary>
/// Shrinks a GUI component to its minimum size.
/// </summary>
internal record Shrinker(IGuiBuilder Builder, IGuiComponent Inner) : ComponentWrapper(
    Builder,
    Inner
), IShrinker
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var innerConstraints = base.GetConstraints();
        return new GuiConstraints(
            innerConstraints.MinSize,
            new PartialGuiSize(innerConstraints.MinSize)
        );
    }

    public IShrinker WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }
}
