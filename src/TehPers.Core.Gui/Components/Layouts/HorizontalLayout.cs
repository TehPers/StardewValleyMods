using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Components.Layouts;

namespace TehPers.Core.Gui.Components.Layouts;

/// <inheritdoc cref="IHorizontalLayout"/>
internal record HorizontalLayout
    (IGuiBuilder Builder, ImmutableArray<IGuiComponent> Components) : BaseGuiComponent(Builder),
        IHorizontalLayout
{
    /// <inheritdoc />
    public override IGuiConstraints GetConstraints()
    {
        return this.Components.Aggregate(
            new GuiConstraints {MaxSize = new PartialGuiSize(0, null)},
            (prev, component) =>
            {
                var innerConstraints = component.GetConstraints();
                return new()
                {
                    MinSize = new GuiSize(
                        prev.MinSize.Width + innerConstraints.MinSize.Width,
                        Math.Max(prev.MinSize.Height, innerConstraints.MinSize.Height)
                    ),
                    MaxSize = new PartialGuiSize(
                        (prev.MaxSize.Width, innerConstraints.MaxSize.Width) switch
                        {
                            (null, _) => null,
                            (_, null) => null,
                            ({ } w1, { } w2) => w1 + w2,
                        },
                        (prev.MaxSize.Height, innerConstraints.MaxSize.Height) switch
                        {
                            (null, var h) => h,
                            (var h, null) => h,
                            ({ } h1, { } h2) => Math.Min(h1, h2),
                        }
                    )
                };
            }
        );
    }

    /// <inheritdoc />
    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        // Get excess horizontal space
        var sizedComponents = this.Components.Select(
                component => new SizedComponent(component, component.GetConstraints())
            )
            .ToList();
        var excessWidth = sizedComponents.Aggregate(
            (float)bounds.Width,
            (excess, cur) => excess - cur.MinWidth
        );
        excessWidth = Math.Max(0, excessWidth);

        // Scale excess across all components
        while (excessWidth > 0)
        {
            var remainingComponents =
                sizedComponents.Where(c => c.RemainingWidth is not <= 0).ToList();
            if (!remainingComponents.Any())
            {
                break;
            }

            var minAddedWidth = remainingComponents.Min(c => c.RemainingWidth ?? excessWidth);
            var addedWidth = Math.Min(excessWidth / remainingComponents.Count, minAddedWidth);
            foreach (var c in remainingComponents)
            {
                c.AdditionalWidth += addedWidth;
            }

            excessWidth -= addedWidth * remainingComponents.Count;
        }

        // Layout components, using up excess space if able
        foreach (var sizedComponent in sizedComponents)
        {
            // Calculate height and y-position
            var height = sizedComponent.Constraints.MaxSize.Height switch
            {
                null => bounds.Height,
                { } maxHeight => (int)Math.Ceiling(Math.Min(maxHeight, bounds.Height)),
            };

            // Calculate width
            var width = (int)Math.Ceiling(sizedComponent.MinWidth + sizedComponent.AdditionalWidth);
            sizedComponent.Component.Handle(e, new(bounds.X, bounds.Y, width, height));

            // Update remaining area
            bounds = new(bounds.X + width, bounds.Y, Math.Max(0, bounds.Width - width), height);
        }
    }

    public IHorizontalLayout WithComponents(IEnumerable<IGuiComponent> components)
    {
        return this with {Components = components.ToImmutableArray()};
    }

    private class SizedComponent
    {
        public IGuiComponent Component { get; }
        public IGuiConstraints Constraints { get; }
        public float AdditionalWidth { get; set; }

        public float MinWidth => this.Constraints.MinSize.Width;

        public float? RemainingWidth => this.Constraints.MaxSize.Width switch
        {
            null => null,
            { } maxWidth => maxWidth - this.Constraints.MinSize.Width - this.AdditionalWidth
        };

        public SizedComponent(IGuiComponent component, IGuiConstraints constraints)
        {
            this.Component = component;
            this.Constraints = constraints;
            this.AdditionalWidth = 0;
        }
    }
}
