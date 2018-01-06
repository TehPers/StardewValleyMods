using ModUtilities.Helpers;
using ModUtilities.Menus.Components;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ModUtilities.Menus {
    public class ModConfigMenu : ModMenu {

        public ModConfigMenu() : this((int) (Game1.viewport.Width * 0.25), (int) (Game1.viewport.Height * 0.125), (int) (Game1.viewport.Width * 0.5), (int) (Game1.viewport.Height * 0.75)) { }

        public ModConfigMenu(int x, int y, int width, int height) : base(x, y, width, height) {
            // Title
            LabelComponent title = new LabelComponent()
                .SetText("Mod Config")
                .SetScale(1.5f)
                .Chain(l => l.Location = new Location((this.Component.ChildBounds.Width - l.Size.Width) / 2, 0));
            this.Component.AddChild(title);

            // Main scrollable area
            ScrollableComponent scrollableComponent = new ScrollableComponent()
                .Chain(c => c.Location = new Location(0, title.Location.Y + title.Size.Height))
                .Chain(c => c.Size = new Size(this.Component.ChildBounds.Width, this.Component.ChildBounds.Height - title.Size.Height));
            this.Component.AddChild(scrollableComponent);

            // Keybind option
            LabelComponent keybindLabel = new LabelComponent()
                .SetText("Open Mod Config:")
                .Chain(l => l.Location = new Location(0, 0));
            scrollableComponent.AddChild(keybindLabel);

            // Keybind selector
            KeybindComponent keybindSelector = new KeybindComponent()
                .Chain(c => c.SelectedKey = ModUtilities.Instance.Config.ModConfigKey)
                .Chain(c => c.Location = new Location(keybindLabel.Location.X + keybindLabel.Size.Width, keybindLabel.Location.Y))
                .Chain(c => c.Size = new Size(scrollableComponent.ChildBounds.Width - keybindLabel.Size.Width, keybindLabel.Size.Height));
            scrollableComponent.AddChild(keybindSelector);

            // Checkbox
            CheckboxComponent checkbox = new CheckboxComponent(false)
                .Chain(c => c.Location = new Location(0, keybindSelector.Location.Y + keybindSelector.Size.Height));
            scrollableComponent.AddChild(checkbox);

            // Checkbox label
            LabelComponent checkboxLabel = new LabelComponent()
                .SetText("Test Checkbox")
                .Chain(c => c.Location = new Location(checkbox.Size.Width, checkbox.Location.Y + (checkbox.Size.Height - c.Size.Height) / 2));
            scrollableComponent.AddChild(checkboxLabel);

            // Textbox
            TextboxComponent textbox = new TextboxComponent()
                .Chain(c => c.Location = new Location(0, checkbox.Location.Y + checkbox.Size.Height))
                .Chain(c => c.Size = new Size(scrollableComponent.ChildBounds.Width, c.Size.Height));
            scrollableComponent.AddChild(textbox);

            // Dropdown
            DropdownComponent dropdown = new DropdownComponent(new[] { "Option 1", "Option 2", "Option 3", "Option 4", "Option 5", "Option 6" })
                .Chain(c => c.Location = new Location(0, textbox.Location.Y + textbox.Size.Height))
                .Chain(c => c.Size = new Size(scrollableComponent.ChildBounds.Width, textbox.Size.Height));
            scrollableComponent.AddChild(dropdown);

            // Scrollbar - Horizontal
            ScrollbarComponent scrollbarH = new ScrollbarComponent(true)
                .Chain(c => c.Location = new Location(0, dropdown.Location.Y + dropdown.Size.Height))
                .Chain(c => c.Size = new Size(scrollableComponent.ChildBounds.Width, c.Size.Height));
            scrollableComponent.AddChild(scrollbarH);

            // Scrollbar - Vertical
            ScrollbarComponent scrollbarV = new ScrollbarComponent(false)
                .Chain(c => c.Location = new Location(0, scrollbarH.Location.Y + scrollbarH.Size.Height))
                .Chain(c => c.Size = new Size(c.Size.Width, scrollableComponent.ChildBounds.Height));
            scrollableComponent.AddChild(scrollbarV);
        }
    }
}
