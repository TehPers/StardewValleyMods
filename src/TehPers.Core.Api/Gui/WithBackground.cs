using Microsoft.Xna.Framework;
using System;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Draws a component with a background.
    /// </summary>
    /// <typeparam name="TFgResponse">The type of the foreground component's response.</typeparam>
    /// <typeparam name="TBgResponse">The type of the background component's response.</typeparam>
    /// <param name="Foreground">The foreground component.</param>
    /// <param name="Background">The background component.</param>
    public record WithBackground<TFgResponse, TBgResponse>(
        IGuiComponent<TFgResponse> Foreground,
        IGuiComponent<TBgResponse> Background
    ) : IGuiComponent<WithBackground<TFgResponse, TBgResponse>.Response>
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
                AllowBuffer = bgConstraints.AllowBuffer && fgConstraints.AllowBuffer,
            };
        }

        /// <inheritdoc />
        public Response Handle(GuiEvent e, Rectangle bounds)
        {
            var bg = this.Background.Handle(e, bounds);
            var fg = this.Foreground.Handle(e, bounds);
            return new(fg, bg);
        }

        /// <summary>
        /// The response from a <see cref="WithBackground{TFgState,TBgState}"/> component.
        /// </summary>
        /// <param name="Foreground">The foreground response.</param>
        /// <param name="Background">The background response.</param>
        public record Response(TFgResponse Foreground, TBgResponse Background);
    }
}
