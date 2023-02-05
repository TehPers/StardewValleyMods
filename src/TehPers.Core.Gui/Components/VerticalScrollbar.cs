using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IVerticalScrollbar" />
internal record VerticalScrollbar
    (IGuiBuilder Builder, IVerticalScrollbar.IState State) : BaseGuiComponent(Builder),
        IVerticalScrollbar
{
    public float LayerDepth { get; init; }

    public override IGuiConstraints GetConstraints()
    {
        var partialBar = this.GuiBuilder.TextureBox(
                Game1.mouseCursors,
                topLeft: new(435, 463, 2, 3),
                topCenter: new(437, 463, 2, 3),
                topRight: new(439, 463, 2, 3),
                centerLeft: new(435, 466, 2, 4),
                center: new(437, 466, 2, 4),
                centerRight: new(439, 466, 2, 4),
                bottomLeft: new(435, 470, 2, 3),
                bottomCenter: new(437, 470, 2, 3),
                bottomRight: new(439, 470, 2, 3)
            )
            .WithMinScale(new GuiSize(Game1.pixelZoom, Game1.pixelZoom));
        var partialTrack = this.GuiBuilder.TextureBox(Game1.mouseCursors, new(403, 383, 6, 6))
            .WithMinScale(new GuiSize(Game1.pixelZoom, Game1.pixelZoom));

        return partialBar.WithBackground(partialTrack).GetConstraints();
    }

    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        if (e.IsScroll(out var direction))
        {
            this.State.Value -= direction / 120;
        }

        this.CreateInner(bounds).Handle(e, bounds);
    }

    private IGuiComponent CreateInner(Rectangle bounds)
    {
        // Create bar
        var range = this.State.MaxValue - this.State.MinValue + 1;
        var barHeight = bounds.Height / (float)range;
        var valueFromMin = this.State.Value - this.State.MinValue;
        var bar = this.GuiBuilder.VerticalLayout(
            // Space above bar
            this.GuiBuilder.Empty()
                .Constrained()
                .WithMaxSize(new PartialGuiSize(null, barHeight * valueFromMin)),
            // Bar
            this.GuiBuilder.TextureBox(
                    Game1.mouseCursors,
                    topLeft: new(435, 463, 2, 3),
                    topCenter: new(437, 463, 2, 3),
                    topRight: new(439, 463, 2, 3),
                    centerLeft: new(435, 466, 2, 4),
                    center: new(437, 466, 2, 4),
                    centerRight: new(439, 466, 2, 4),
                    bottomLeft: new(435, 470, 2, 3),
                    bottomCenter: new(437, 470, 2, 3),
                    bottomRight: new(439, 470, 2, 3)
                )
                .WithMinScale(new GuiSize(Game1.pixelZoom, Game1.pixelZoom))
                .WithLayerDepth(layerDepth: this.LayerDepth)
                .Constrained()
                .WithMaxSize(new PartialGuiSize(null, barHeight)),
            // Space below bar
            this.GuiBuilder.Empty()
        );

        // Create track
        var track = this.GuiBuilder.TextureBox(Game1.mouseCursors, new(403, 383, 6, 6))
            .WithMinScale(new GuiSize(Game1.pixelZoom, Game1.pixelZoom))
            .WithLayerDepth(this.LayerDepth);

        return bar.WithBackground(track);
    }

    /// <inheritdoc />
    public IVerticalScrollbar WithLayerDepth(float layerDepth)
    {
        return this with {LayerDepth = layerDepth};
    }

    /// <inheritdoc />
    public IVerticalScrollbar WithState(IVerticalScrollbar.IState state)
    {
        return this with {State = state};
    }
}
