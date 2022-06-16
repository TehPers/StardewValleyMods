using StardewValley;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A horizontal separator in a menu.
    /// </summary>
    public class MenuHorizontalSeparator : WrapperComponent
    {
        /// <summary>
        /// Creates a new horizontal menu separator.
        /// </summary>
        public MenuHorizontalSeparator()
            : base(MenuHorizontalSeparator.CreateInner())
        {
        }

        private static IGuiComponent CreateInner()
        {
            return HorizontalLayout.Of(
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = PartialGuiSize.One,
                        SourceRectangle = new(0, 64, 64, 64),
                    },
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = new(null, 1),
                        SourceRectangle = new(128, 64, 64, 64),
                    },
                    new StretchedTexture(Game1.menuTexture)
                    {
                        MinScale = GuiSize.One,
                        MaxScale = PartialGuiSize.One,
                        SourceRectangle = new(192, 64, 64, 64),
                    }
                )
                .IgnoreResponse();
        }
    }
}
