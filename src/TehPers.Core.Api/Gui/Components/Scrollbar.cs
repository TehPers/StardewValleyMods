using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.Core.Api.Gui.Layouts;
using TehPers.Core.Api.Gui.States;

namespace TehPers.Core.Api.Gui.Components
{
    internal record Scrollbar(ScrollbarState State) : ComponentWrapper
    {
        /// <summary>
        /// The layer depth to draw this component at.
        /// </summary>
        public float LayerDepth { get; init; }

        public override IGuiComponent Inner => this.CreateInner();

        private IGuiComponent CreateInner()
        {
            var bar = new Bar(this.State, this.LayerDepth);
            var track = GuiComponent.TextureBox(
                Game1.mouseCursors,
                new(403, 383, 6, 6),
                new(Game1.pixelZoom, Game1.pixelZoom),
                this.LayerDepth
            );

            return bar.WithBackground(track);
        }

        private record Bar(ScrollbarState State, float LayerDepth) : IGuiComponent
        {
            public GuiConstraints GetConstraints()
            {
                return new()
                {
                    MinSize = new(Game1.pixelZoom * 6, Game1.pixelZoom * 10),
                    MaxSize = new(Game1.pixelZoom * 6, null),
                };
            }

            public void Handle(GuiEvent e, Rectangle bounds)
            {
                var range = this.State.MaximumValue - this.State.MinimumValue + 1;
                var barHeight = bounds.Height / (float)range;
                var valueFromMin = this.State.Value - this.State.MinimumValue;

                GuiComponent.Vertical(
                        builder =>
                        {
                            // Space above bar
                            GuiComponent.Empty()
                                .Constrained(maxSize: new(null, barHeight * valueFromMin))
                                .AddTo(builder);
                            // Bar
                            GuiComponent.TextureBox(
                                    Game1.mouseCursors,
                                    topLeft: new(435, 463, 2, 3),
                                    topCenter: new(437, 463, 2, 3),
                                    topRight: new(439, 463, 2, 3),
                                    centerLeft: new(435, 466, 2, 4),
                                    center: new(437, 466, 2, 4),
                                    centerRight: new(439, 466, 2, 4),
                                    bottomLeft: new(435, 470, 2, 3),
                                    bottomCenter: new(437, 470, 2, 3),
                                    bottomRight: new(439, 470, 2, 3),
                                    minScale: new(Game1.pixelZoom, Game1.pixelZoom),
                                    layerDepth: this.LayerDepth
                                )
                                .Constrained(maxSize: new(null, barHeight))
                                .AddTo(builder);
                            // Space below bar
                            GuiComponent.Empty().AddTo(builder);
                        }
                    )
                    .Handle(e, bounds);
            }
        }
    }
}
