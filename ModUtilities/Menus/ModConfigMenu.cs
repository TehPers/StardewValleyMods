using ModUtilities.Helpers;
using ModUtilities.Menus.Components;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ModUtilities.Menus {
    public class ModConfigMenu : ModMenu {
        public ModConfigMenu(int x, int y, int width, int height) : base(x, y, width, height) {
            // Title
            LabelComponent title = new LabelComponent()
                .SetText("Mod Config")
                .SetScale(1.5f)
                .Chain(l => l.Location = new Location((this.Component.ChildBounds.Width - l.Size.Width) / 2, 0));
            this.Component.AddChild(title);

            // Keybind option
            LabelComponent keybindLabel = new LabelComponent()
                .SetText("Open Mod Config:")
                .Chain(l => l.Location = new Location(0, title.Location.Y + title.Size.Height));
            this.Component.AddChild(keybindLabel);

            // Keybind selector
            KeybindComponent keybindSelector = new KeybindComponent()
                .Chain(c => c.SelectedKey = ModUtilities.Instance.Config.ModConfigKey)
                .Chain(c => c.Location = new Location(keybindLabel.Location.X + keybindLabel.Size.Width, keybindLabel.Location.Y))
                .Chain(c => c.Size = new Size(this.Component.ChildBounds.Width - keybindLabel.Size.Width, keybindLabel.Size.Height));
            this.Component.AddChild(keybindSelector);

            // Checkbox
            CheckboxComponent checkbox = new CheckboxComponent(false)
                .Chain(c => c.Location = new Location(0, keybindSelector.Location.Y + keybindSelector.Size.Height));
            this.Component.AddChild(checkbox);

            // Checkbox label
            LabelComponent checkboxLabel = new LabelComponent()
                .SetText("Test Checkbox")
                .Chain(c => c.Location = new Location(checkbox.Size.Width, checkbox.Location.Y + (checkbox.Size.Height - c.Size.Height) / 2));
            this.Component.AddChild(checkboxLabel);

            // Textbox
            TextboxComponent textbox = new TextboxComponent()
                .Chain(c => c.Location = new Location(0, checkbox.Location.Y + checkbox.Size.Height))
                .Chain(c => c.Size = new Size(this.Component.ChildBounds.Width, c.Size.Height));
            this.Component.AddChild(textbox);
        }
    }
}
