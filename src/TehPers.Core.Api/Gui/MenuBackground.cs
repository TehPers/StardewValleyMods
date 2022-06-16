using StardewValley;
using TehPers.Core.Api.Gui.Layouts;

namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// Fills a space with an empty menu background and borders.
    /// </summary>
    public class MenuBackground : WrapperComponent
    {
        /// <summary>
        /// Creates a new menu background.
        /// </summary>
        public MenuBackground()
            : base(MenuBackground.CreateInner())
        {
        }

        private static IGuiComponent CreateInner()
        {
            return VerticalLayout.Build(
                    builder =>
                    {
                        // Top row
                        builder.Add(
                            HorizontalLayout.Build(
                                builder =>
                                {
                                    builder.Add(
                                        new StretchedTexture(Game1.menuTexture)
                                        {
                                            MinScale = GuiSize.One,
                                            MaxScale = PartialGuiSize.One,
                                            SourceRectangle = new(0, 0, 64, 64),
                                        }
                                    );
                                    builder.Add(
                                        new StretchedTexture(Game1.menuTexture)
                                        {
                                            MinScale = GuiSize.One,
                                            MaxScale = new(null, 1),
                                            SourceRectangle = new(128, 0, 64, 64),
                                        }
                                    );
                                    builder.Add(
                                        new StretchedTexture(Game1.menuTexture)
                                        {
                                            MinScale = GuiSize.One,
                                            MaxScale = PartialGuiSize.One,
                                            SourceRectangle = new(192, 0, 64, 64),
                                        }
                                    );
                                }
                            )
                        );

                        // Middle row
                        builder.Add(
                            HorizontalLayout.Build(
                                builder =>
                                {
                                    builder.Add(
                                        new StretchedTexture(Game1.menuTexture)
                                        {
                                            MinScale = GuiSize.One,
                                            MaxScale = new(1, null),
                                            SourceRectangle = new(0, 128, 64, 64),
                                        }
                                    );
                                    builder.Add(new EmptySpace());
                                    builder.Add(
                                        new StretchedTexture(Game1.menuTexture)
                                        {
                                            MinScale = GuiSize.One,
                                            MaxScale = new(1, null),
                                            SourceRectangle = new(192, 128, 64, 64),
                                        }
                                    );
                                }
                            )
                        );

                        // Bottom row
                        builder.Add(
                            HorizontalLayout.Build(
                                builder =>
                                {
                                    builder.Add(
                                        new StretchedTexture(Game1.menuTexture)
                                        {
                                            MinScale = GuiSize.One,
                                            MaxScale = PartialGuiSize.One,
                                            SourceRectangle = new(0, 192, 64, 64),
                                        }
                                    );
                                    builder.Add(
                                        new StretchedTexture(Game1.menuTexture)
                                        {
                                            MinScale = GuiSize.One,
                                            MaxScale = new(null, 1),
                                            SourceRectangle = new(128, 192, 64, 64),
                                        }
                                    );
                                    builder.Add(
                                        new StretchedTexture(Game1.menuTexture)
                                        {
                                            MinScale = GuiSize.One,
                                            MaxScale = PartialGuiSize.One,
                                            SourceRectangle = new(192, 192, 64, 64),
                                        }
                                    );
                                }
                            )
                        );
                    }
                )
                .WithBackground(
                    new StretchedTexture(Game1.menuTexture) {SourceRectangle = new(64, 128, 64, 64)}
                        .WithPadding(32)
                );
        }
    }
}
