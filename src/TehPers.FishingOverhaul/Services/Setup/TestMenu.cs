using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Linq;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Extensions;
using TehPers.Core.Gui.Api.Guis;

namespace TehPers.FishingOverhaul.Services.Setup
{
    public class TestMenu : IGui<TestMenu.Message>
    {
        private readonly IModHelper helper;
        private readonly ITextInput.IState textState;
        private readonly IDropdown<int>.IState dropdownState;
        private ClickType? lastClick;
        private int clicks;

        /// <inheritdoc />
        public bool CaptureInput => true;

        public TestMenu(IModHelper helper)
        {
            this.helper = helper;
            this.textState = new ITextInput.State();
            this.dropdownState = new IDropdown<int>.State(
                Enumerable.Range(0, 10).Select(n => (n, $"Item{n}")).ToList()
            );
        }

        /// <inheritdoc />
        public void Update(Message message)
        {
            switch (message)
            {
                case Message.Clicked(var clickType):
                    this.lastClick = clickType;
                    this.clicks += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }
        }

        /// <inheritdoc />
        public IGuiComponent View(IGuiContext<Message> ctx)
        {
            return ctx.VerticalLayout(
                    layout =>
                    {
                        // Center layout
                        layout = layout.Aligned(horizontal: HorizontalAlignment.Center);

                        // Show last click
                        ctx.Label(
                                this.lastClick is { } lastClick
                                    ? $"Last click type: {lastClick}"
                                    : "Click me!"
                            )
                            .Aligned(HorizontalAlignment.Center)
                            .OnClick(clickType => ctx.Update(new Message.Clicked(clickType)))
                            .AddTo(layout);

                        // Show counter
                        if (this.clicks > 0)
                        {
                            ctx.HorizontalLayout(
                                    ctx.Label("You have clicked "),
                                    ctx.Label(this.clicks.ToString("G")).WithColor(Color.DarkGreen),
                                    this.clicks > 1 ? ctx.Label(" times!") : ctx.Label(" time!")
                                )
                                .AddTo(layout);
                        }

                        ctx.TextBox(this.textState, this.helper.Input).AddTo(layout);
                        ctx.Dropdown(this.dropdownState).AddTo(layout);
                    }
                )
                .Aligned(HorizontalAlignment.Center, VerticalAlignment.Center)
                .Constrained()
                .WithMinSize(new PartialGuiSize(null, 100f))
                .WithPadding(64)
                .WithBackground(ctx.MenuBackground());
        }

        public abstract record Message
        {
            private Message() { }

            public sealed record Clicked(ClickType ClickType) : Message;
        }
    }
}
