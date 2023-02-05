using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Linq;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.Core.Gui.Components;

/// <inheritdoc cref="IDropdown{T}"/>
internal record Dropdown<T>(IGuiBuilder Builder, IDropdown<T>.IState State) : BaseGuiComponent(
    Builder
), IDropdown<T>
{
    public float LayerDepth { get; init; } = 1f;

    public override IGuiConstraints GetConstraints()
    {
        return this.CreateInner().GetConstraints();
    }

    public override void Handle(IGuiEvent e, Rectangle bounds)
    {
        this.CreateInner().Handle(e, bounds);

        if (e.IsReceiveClick(out _, out _))
        {
            // TODO: make sure this works and closes it when clicking off the dropdown
            this.State.Dropped = false;
        }

        // Add dropdown overlay if needed
        if (this.State.Dropped)
        {
            // Handle keyboard/gamepad input
            var (select, cancel, up, down, scrollAmt) = e switch
            {
                _ when e.IsKeyboardInput(out var key) => (key is Keys.Enter, key is Keys.Escape,
                    key is Keys.Up, key is Keys.Down, 0),
                _ when e.IsGamePadInput(out var button) => (button is Buttons.A,
                    button is Buttons.B, button is Buttons.DPadUp, button is Buttons.DPadDown, 0),
                _ when e.IsScroll(out var direction) => (false, false, false, false,
                    -direction / 120),
                _ => (false, false, false, false, 0),
            };

            if (select)
            {
                this.State.SelectedIndex = this.State.HoveredIndex ?? this.State.SelectedIndex;
                this.State.Dropped = false;
            }

            if (cancel)
            {
                this.State.Dropped = false;
            }

            if (up)
            {
                var hoveredIndex = this.State.HoveredIndex switch
                {
                    { } i => Math.Max(0, i - 1),
                    _ => this.State.Items.Count - 1,
                };
                this.State.HoveredIndex = hoveredIndex;
                this.State.TopVisibleIndex = Math.Clamp(
                    this.State.TopVisibleIndex,
                    hoveredIndex - this.State.MaxVisibleItems + 1,
                    hoveredIndex
                );
            }

            if (down)
            {
                var hoveredIndex = this.State.HoveredIndex switch
                {
                    { } i => Math.Min(this.State.Items.Count - 1, i + 1),
                    _ => 0,
                };
                this.State.HoveredIndex = hoveredIndex;
                this.State.TopVisibleIndex = Math.Clamp(
                    this.State.TopVisibleIndex,
                    hoveredIndex - this.State.MaxVisibleItems + 1,
                    hoveredIndex
                );
            }

            if (scrollAmt != 0)
            {
                this.State.TopVisibleIndex += scrollAmt;
            }

            // This draws outside of its own bounds intentionally
            var overlay = this.CreateDropdownOverlay();
            var overlaySize = overlay.GetConstraints().MinSize;
            overlay.Handle(
                e,
                new(bounds.Left, bounds.Bottom, bounds.Width, (int)Math.Ceiling(overlaySize.Height))
            );
        }
    }

    private IGuiComponent CreateInner()
    {
        return this.GuiBuilder.HorizontalLayout(
                builder =>
                {
                    var bg = this.GuiBuilder.TextureBox(
                            Game1.mouseCursors,
                            new(433, 451, 1, 1),
                            new(434, 451, 1, 1),
                            new(435, 451, 1, 1),
                            new(433, 452, 1, 1),
                            new(434, 452, 1, 1),
                            new(435, 452, 1, 1),
                            new(433, 453, 1, 1),
                            new(434, 453, 1, 1),
                            new(435, 453, 1, 1)
                        )
                        .WithMinScale(new GuiSize(4, 4))
                        .WithLayerDepth(this.LayerDepth);
                    if (this.State.Selected is var (_, label))
                    {
                        this.GuiBuilder.Label(label)
                            .WithFont(Game1.smallFont)
                            .WithColor(Game1.textColor)
                            .WithLayerDepth(this.LayerDepth)
                            .WithPadding(4, 0)
                            .Clipped()
                            .Aligned(HorizontalAlignment.Left, VerticalAlignment.Center)
                            .WithBackground(bg)
                            .AddTo(builder);
                    }
                    else
                    {
                        this.GuiBuilder.Empty().WithBackground(bg).AddTo(builder);
                    }

                    this.GuiBuilder.Texture(Game1.mouseCursors)
                        .WithSourceRectangle(OptionsDropDown.dropDownButtonSource)
                        .WithMinScale(new GuiSize(4, 4))
                        .WithMaxScale(new PartialGuiSize(4, 4))
                        .WithLayerDepth(this.LayerDepth)
                        .AddTo(builder);
                }
            )
            .OnClick(
                clickType =>
                {
                    if (clickType == ClickType.Left)
                    {
                        this.State.Dropped = !this.State.Dropped;
                    }
                }
            );
    }

    private IGuiComponent CreateDropdownOverlay()
    {
        return this.GuiBuilder.VerticalLayout(
                builder =>
                {
                    var hoveredIndex = this.State.HoveredIndex;
                    var visibleItems = this.State.Items.Select((item, index) => (index, item.Label))
                        .Skip(this.State.TopVisibleIndex)
                        .Take(this.State.MaxVisibleItems);
                    foreach (var (index, label) in visibleItems)
                    {
                        // Create label
                        var labelComponent = (IGuiComponent)this.GuiBuilder.Label(label)
                            .WithFont(Game1.smallFont)
                            .WithLayerDepth(this.LayerDepth)
                            .Clipped()
                            .Aligned(HorizontalAlignment.Left, VerticalAlignment.Center)
                            .Constrained()
                            .WithMinSize(
                                new PartialGuiSize(
                                    null,
                                    OptionsDropDown.dropDownButtonSource.Height * 4
                                )
                            );

                        // Add background if needed
                        if (index == hoveredIndex)
                        {
                            labelComponent = labelComponent.WithBackground(
                                this.GuiBuilder.Texture(Game1.staminaRect)
                                    .WithColor(Color.Wheat)
                                    .WithMinScale(GuiSize.Zero)
                                    .WithMaxScale(PartialGuiSize.Empty)
                            );
                        }

                        // Add to layout
                        labelComponent.OnHover(() => this.State.HoveredIndex = index)
                            .OnClick(
                                ClickType.Left,
                                () =>
                                {
                                    this.State.SelectedIndex = index;
                                    this.State.Dropped = false;
                                }
                            )
                            .AddTo(builder);
                    }
                }
            )
            .WithPadding(4)
            .WithBackground(
                this.GuiBuilder.TextureBox(
                        Game1.mouseCursors,
                        new(433, 451, 1, 1),
                        new(434, 451, 1, 1),
                        new(435, 451, 1, 1),
                        new(433, 452, 1, 1),
                        new(434, 452, 1, 1),
                        new(435, 452, 1, 1),
                        new(433, 453, 1, 1),
                        new(434, 453, 1, 1),
                        new(435, 453, 1, 1)
                    )
                    .WithMinScale(new GuiSize(4, 4))
                    .WithLayerDepth(this.LayerDepth)
            )
            .WithPadding(0, OptionsDropDown.dropDownButtonSource.Width * 4, 0, 0);
    }

    public IDropdown<T> WithState(IDropdown<T>.IState state)
    {
        return this with {State = state};
    }

    public IDropdown<T> WithLayerDepth(float layerDepth)
    {
        return this with {LayerDepth = layerDepth};
    }
}
