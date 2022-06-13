using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Fills a space with an empty menu background and borders.
    /// </summary>
    public record MenuBackground : BaseGuiComponent
    {
        private IGuiComponent Inner { get; init; } = VerticalLayout.Of(
                HorizontalLayout.Of(
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = PartialGuiSize.One,
                        SourceRectangle = new(0, 0, 64, 64),
                    },
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = new(null, 1),
                        SourceRectangle = new(128, 0, 64, 64),
                    },
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = PartialGuiSize.One,
                        SourceRectangle = new(192, 0, 64, 64),
                    }
                ),
                HorizontalLayout.Of(
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = new(1, null),
                        SourceRectangle = new(0, 128, 64, 64),
                    },
                    new EmptySpace(),
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = new(1, null),
                        SourceRectangle = new(192, 128, 64, 64),
                    }
                ),
                HorizontalLayout.Of(
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = PartialGuiSize.One,
                        SourceRectangle = new(0, 192, 64, 64),
                    },
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = new(null, 1),
                        SourceRectangle = new(128, 192, 64, 64),
                    },
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = PartialGuiSize.One,
                        SourceRectangle = new(192, 192, 64, 64),
                    }
                )
            )
            .WithBackground(
                new StretchedTexture(Game1.menuTexture) {SourceRectangle = new(64, 128, 64, 64)}
                    .WithPadding(32, 32)
            );

        /// <inheritdoc />
        public override GuiConstraints Constraints => this.Inner.Constraints;

        /// <inheritdoc />
        public override void CalculateLayouts(Rectangle bounds, List<ComponentLayout> layouts)
        {
            base.CalculateLayouts(bounds, layouts);
            this.Inner.CalculateLayouts(bounds, layouts);
        }

        /// <inheritdoc />
        public override bool Update(
            GuiEvent e,
            IImmutableDictionary<IGuiComponent, Rectangle> componentBounds,
            [NotNullWhen(true)] out IGuiComponent? newComponent
        )
        {
            if (this.Inner.Update(e, componentBounds, out var newInner))
            {
                newComponent = this with {Inner = newInner};
                return true;
            }

            newComponent = default;
            return false;
        }
    }
}
