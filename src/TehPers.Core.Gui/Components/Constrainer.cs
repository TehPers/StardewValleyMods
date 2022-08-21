using System;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IConstrainer"/>
internal record Constrainer(IGuiBuilder Builder, IGuiComponent Inner) : ComponentWrapper(
    Builder,
    Inner
), IConstrainer
{
    /// <summary>
    /// The additional minimum size constraints for the component. This cannot make the
    /// component smaller than its previous minimum size.
    /// </summary>
    public IPartialGuiSize MinSize { get; init; } = PartialGuiSize.Empty;

    /// <summary>
    /// The additional maximum size constraints for the component. This cannot make the
    /// component larger than its previous maximum size.
    /// </summary>
    public IPartialGuiSize MaxSize { get; init; } = PartialGuiSize.Empty;

    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        var innerConstraints = base.GetConstraints();
        var minWidth = this.MinSize.Width switch
        {
            { } w => Math.Max(w, innerConstraints.MinSize.Width),
            _ => innerConstraints.MinSize.Width,
        };
        var minHeight = this.MinSize.Height switch
        {
            { } h => Math.Max(h, innerConstraints.MinSize.Height),
            _ => innerConstraints.MinSize.Height,
        };
        var maxWidth = (innerConstraints.MaxSize.Width, this.MaxSize.Width) switch
        {
            ({ } w1, { } w2) => Math.Min(w1, w2),
            (null, var w) => w,
            var (w, _) => w,
        };
        var maxHeight = (innerConstraints.MaxSize.Height, this.MaxSize.Height) switch
        {
            ({ } h1, { } h2) => Math.Min(h1, h2),
            (null, var h) => h,
            var (h, _) => h,
        };
        
        return new GuiConstraints(
            new GuiSize(minWidth, minHeight),
            new PartialGuiSize(maxWidth, maxHeight)
        );
    }

    /// <inheritdoc />
    public IConstrainer WithInner(IGuiComponent inner)
    {
        return this with {Inner = inner};
    }

    /// <inheritdoc />
    public IConstrainer WithMinSize(IPartialGuiSize minSize)
    {
        return this with {MinSize = minSize};
    }

    public IConstrainer WithMaxSize(IPartialGuiSize maxSize)
    {
        return this with {MaxSize = maxSize};
    }
}
