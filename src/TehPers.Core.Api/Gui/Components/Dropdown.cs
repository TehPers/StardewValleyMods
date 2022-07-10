using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Linq;
using TehPers.Core.Api.Gui.Layouts;
using TehPers.Core.Api.Gui.States;

namespace TehPers.Core.Api.Gui.Components
{
    internal record Dropdown<T>(DropdownState<T> State) : ComponentWrapper
    {
        public override IGuiComponent Inner => this.CreateInner();

        public float LayerDepth { get; init; } = 1f;

        private IGuiComponent CreateInner()
        {
            return GuiComponent.Horizontal(
                builder =>
                {
                    var bg = GuiComponent.TextureBox(
                        Game1.mouseCursors,
                        new(433, 451, 1, 1),
                        new(434, 451, 1, 1),
                        new(435, 451, 1, 1),
                        new(433, 452, 1, 1),
                        new(434, 452, 1, 1),
                        new(435, 452, 1, 1),
                        new(433, 453, 1, 1),
                        new(434, 453, 1, 1),
                        new(435, 453, 1, 1),
                        minScale: new(4, 4),
                        layerDepth: this.LayerDepth
                    );
                    if (this.State.Selected is var (_, label))
                    {
                        GuiComponent.Label(label, font: Game1.smallFont, color: Game1.textColor, layerDepth: this.LayerDepth)
                            .WithPadding(4, 0)
                            .Clipped()
                            .Aligned(HorizontalAlignment.Left, VerticalAlignment.Center)
                            .WithBackground(bg)
                            .AddTo(builder);
                    }
                    else
                    {
                        GuiComponent.Empty().WithBackground(bg).AddTo(builder);
                    }

                    GuiComponent.Texture(
                        Game1.mouseCursors,
                        sourceRectangle: OptionsDropDown.dropDownButtonSource,
                        minScale: new(4, 4),
                        maxScale: new(4, 4),
                        layerDepth: this.LayerDepth
                    ).AddTo(builder);
                })
                .OnClick(clickType =>
                {
                    if (clickType == ClickType.Left)
                    {
                        this.State.Dropped = !this.State.Dropped;
                    }
                });
        }

        public override void Handle(GuiEvent e, Rectangle bounds)
        {
            base.Handle(e, bounds);

            if (e is GuiEvent.ReceiveClick)
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
                    GuiEvent.KeyboardInput(var key) => (key is Keys.Enter, key is Keys.Escape, key is Keys.Up, key is Keys.Down, 0),
                    GuiEvent.GamePadInput(var button) => (button is Buttons.A, button is Buttons.B, button is Buttons.DPadUp, button is Buttons.DPadDown, 0),
                    GuiEvent.Scroll(var direction) => (false, false, false, false, -direction / 120),
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
                overlay.Handle(e, new(bounds.Left, bounds.Bottom, bounds.Width, (int)Math.Ceiling(overlaySize.Height)));
            }
        }

        private IGuiComponent CreateDropdownOverlay()
        {
            return GuiComponent.Vertical(builder =>
            {
                var hoveredIndex = this.State.HoveredIndex;
                var visibleItems = this.State.Items
                    .Select((item, index) => (index, item.Label))
                    .Skip(this.State.TopVisibleIndex)
                    .Take(this.State.MaxVisibleItems);
                foreach (var (index, label) in visibleItems)
                {
                    // Create label
                    var labelComponent = GuiComponent.Label(label, font: Game1.smallFont, layerDepth: this.LayerDepth)
                        .Clipped()
                        .Aligned(HorizontalAlignment.Left, VerticalAlignment.Center)
                        .Constrained(minSize: new(null, OptionsDropDown.dropDownButtonSource.Height * 4));

                    // Add background if needed
                    if (index == hoveredIndex)
                    {
                        labelComponent = labelComponent
                            .WithBackground(
                                GuiComponent.Texture(Game1.staminaRect, color: Color.Wheat, minScale: GuiSize.Zero, maxScale: PartialGuiSize.Empty)
                            );
                    }

                    // Add to layout
                    labelComponent
                        .OnHover(() => this.State.HoveredIndex = index)
                        .OnClick(ClickType.Left, () =>
                        {
                            this.State.SelectedIndex = index;
                            this.State.Dropped = false;
                        })
                        .AddTo(builder);
                }
            })
            .WithPadding(4)
            .WithBackground(
                GuiComponent.TextureBox(
                    Game1.mouseCursors,
                    new(433, 451, 1, 1),
                    new(434, 451, 1, 1),
                    new(435, 451, 1, 1),
                    new(433, 452, 1, 1),
                    new(434, 452, 1, 1),
                    new(435, 452, 1, 1),
                    new(433, 453, 1, 1),
                    new(434, 453, 1, 1),
                    new(435, 453, 1, 1),
                    minScale: new(4, 4),
                    layerDepth: this.LayerDepth
                )
            )
            .WithPadding(0, OptionsDropDown.dropDownButtonSource.Width * 4, 0, 0);
        }
    }
}
