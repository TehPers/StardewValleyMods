using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Linq;
using TehPers.Core.Api.Gui;
using TehPers.Core.Api.Gui.Layouts;
using TehPers.Core.Api.Gui.States;

namespace TehPers.FishingOverhaul.Services.Setup
{
    // TODO: remove this
    internal class TestMenu : ManagedMenu
    {
        private readonly IModHelper helper;
        private string text = "Click me!";
        private int count = 0;
        private readonly TextInputState textState = new();
        private readonly ScrollState scrollState = new();

        private readonly DropdownState<int> dropdownState = new(
            Enumerable.Range(1, 10).Select(n => (n, $"Item{n}")).ToList()
        );

        public TestMenu(IModHelper helper)
            : base(helper, true)
        {
            this.helper = helper;
        }

        protected override IGuiComponent CreateRoot()
        {
            this.KeyboardSubscriber!.Selected = this.textState.Focused;
            return GuiComponent.Vertical(
                    HorizontalAlignment.Center,
                    builder =>
                    {
                        GuiComponent.Label(this.text)
                            .Aligned(HorizontalAlignment.Center)
                            .OnClick(
                                clickType =>
                                {
                                    this.text += $" <{clickType}>";
                                    this.count += 1;
                                }
                            )
                            .AddTo(builder);
                        GuiComponent.Horizontal(
                                row =>
                                {
                                    GuiComponent.Label("You have clicked ").AddTo(row);
                                    GuiComponent.Label(
                                            this.count.ToString("G"),
                                            color: Color.DarkGreen
                                        )
                                        .AddTo(row);
                                    GuiComponent.Label(" times!").AddTo(row);
                                }
                            )
                            .AddTo(builder);
                        GuiComponent.TextBox(this.textState, this.helper.Input).AddTo(builder);
                        GuiComponent.Dropdown(this.dropdownState).AddTo(builder);
                    }
                )
                .Aligned(HorizontalAlignment.Center, VerticalAlignment.Center)
                .VerticallyScrollable(this.scrollState)
                .Constrained(minSize: new(null, 100f))
                .WithPadding(64)
                .WithBackground(GuiComponent.MenuBackground());
        }
    }
}
