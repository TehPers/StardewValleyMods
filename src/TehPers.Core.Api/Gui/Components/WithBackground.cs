using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui.Components
{
    /// <summary>
    /// Draws a component with a background.
    /// </summary>
    /// <param name="Foreground">The foreground component.</param>
    /// <param name="Background">The background component.</param>
    internal record WithBackground(IGuiComponent Foreground, IGuiComponent Background) : IGuiComponent
    {
        /// <inheritdoc />
        public GuiConstraints GetConstraints()
        {
            var fgConstraints = this.Foreground.GetConstraints();
            var bgConstraints = this.Background.GetConstraints();
            return new()
            {
                MinSize = new(
                    Math.Max(bgConstraints.MinSize.Width, fgConstraints.MinSize.Width),
                    Math.Max(bgConstraints.MinSize.Height, fgConstraints.MinSize.Height)
                ),
                MaxSize = new(
                    (bgConstraints.MaxSize.Width, fgConstraints.MaxSize.Width) switch
                    {
                        (null, var w) => w,
                        (var w, null) => w,
                        ({ } w1, { } w2) => Math.Min(w1, w2),
                    },
                    (bgConstraints.MaxSize.Height, fgConstraints.MaxSize.Height) switch
                    {
                        (null, var h) => h,
                        (var h, null) => h,
                        ({ } h1, { } h2) => Math.Min(h1, h2),
                    }
                ),
            };
        }

        /// <inheritdoc />
        public void Handle(GuiEvent e, Rectangle bounds)
        {
            this.Background.Handle(e, bounds);
            this.Foreground.Handle(e, bounds);
        }
    }
}
