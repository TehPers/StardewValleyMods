using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Components.Layouts;

namespace TehPers.Core.Gui.Components.Layouts;

/// <inheritdoc cref="IVerticalLayout"/>
internal record VerticalLayout
    (IGuiBuilder Builder, ImmutableArray<IGuiComponent> Components) : BaseGuiComponent(Builder),
        IVerticalLayout
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return this.Components.Aggregate(
            new GuiConstraints {MaxSize = new PartialGuiSize(null, 0)},
            (prev, component) =>
            {
                var constraints = component.GetConstraints();
                return new()
                {
                    MinSize = new GuiSize(
                        Math.Max(prev.MinSize.Width, constraints.MinSize.Width),
                        prev.MinSize.Height + constraints.MinSize.Height
                    ),
                    MaxSize = new PartialGuiSize(
                        (prev.MaxSize.Width, constraints.MaxSize.Width) switch
                        {
                            (null, var w) => w,
                            (var w, null) => w,
                            ({ } w1, { } w2) => Math.Min(w1, w2),
                        },
                        (prev.MaxSize.Height, constraints.MaxSize.Height) switch
                        {
                            (null, _) => null,
                            (_, null) => null,
                            ({ } h1, { } h2) => h1 + h2,
                        }
                    )
                };
            }
        );
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        // Get excess vertical space
        var sizedComponents = this.Components.Select(
                component => new SizedComponent(component, component.GetConstraints())
            )
            .ToList();
        var excessHeight = sizedComponents.Aggregate(
            (float)bounds.Height,
            (excess, cur) => excess - cur.MinHeight
        );
        excessHeight = Math.Max(0, excessHeight);

        // Scale excess across all components
        while (excessHeight > 0)
        {
            var remainingComponents =
                sizedComponents.Where(c => c.RemainingHeight is not <= 0).ToList();
            if (!remainingComponents.Any())
            {
                break;
            }

            var minAddedHeight = remainingComponents.Min(c => c.RemainingHeight ?? excessHeight);
            var addedHeight = Math.Min(excessHeight / remainingComponents.Count, minAddedHeight);
            foreach (var c in remainingComponents)
            {
                c.AdditionalHeight += addedHeight;
            }

            excessHeight -= addedHeight * remainingComponents.Count;
        }

        // Layout components, using up excess space if able
        foreach (var sizedComponent in sizedComponents)
        {
            // Calculate width and x-position
            var width = sizedComponent.Constraints.MaxSize.Width switch
            {
                null => bounds.Width,
                { } maxWidth => (int)Math.Ceiling(Math.Min(maxWidth, bounds.Width)),
            };

            // Calculate height
            var height = (int)Math.Ceiling(
                sizedComponent.MinHeight + sizedComponent.AdditionalHeight
            );
            sizedComponent.Component.Handle(e, new(bounds.X, bounds.Y, width, height));

            // Update remaining area
            bounds = new(bounds.X, bounds.Y + height, width, Math.Max(0, bounds.Height - height));
        }
    }

    /// <inheritdoc />
    public IVerticalLayout WithComponents(IEnumerable<IGuiComponent> components)
    {
        return this with {Components = components.ToImmutableArray()};
    }

    private class SizedComponent
    {
        public IGuiComponent Component { get; }
        public IGuiConstraints Constraints { get; }
        public float AdditionalHeight { get; set; }

        public float MinHeight => this.Constraints.MinSize.Height;

        public float? RemainingHeight => this.Constraints.MaxSize.Height switch
        {
            null => null,
            { } maxHeight => maxHeight - this.Constraints.MinSize.Height - this.AdditionalHeight
        };

        public SizedComponent(IGuiComponent component, IGuiConstraints constraints)
        {
            this.Component = component;
            this.Constraints = constraints;
            this.AdditionalHeight = 0;
        }
    }
}
