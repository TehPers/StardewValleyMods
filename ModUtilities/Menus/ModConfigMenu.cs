using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Helpers;
using ModUtilities.Menus.Components;
using ModUtilities.Menus.Components.Interfaces;
using StardewConfigFramework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ModUtilities.Menus {
    public class ModConfigMenu : ModMenu {
        public ScrollableComponent ConfigParent { get; }
        private Mod _parentMod;

        public ModConfigMenu() : this((int) (Game1.viewport.Width * 0.25), (int) (Game1.viewport.Height * 0.125), (int) (Game1.viewport.Width * 0.5), (int) (Game1.viewport.Height * 0.75)) { }

        public ModConfigMenu(int x, int y, int width, int height) : base(x, y, width, height) {
            // Title
            LabelComponent title = new LabelComponent()
                .SetText("Mod Config")
                .SetScale(1.5f)
                .Chain(l => l.Location = new Location((this.Component.ChildBounds.Width - l.Size.Width) / 2, 0));
            this.Component.AddChild(title);

            // Main scrollable area
            this.ConfigParent = new ScrollableComponent()
                .Chain(c => c.Location = new Location(0, title.Location.Y + title.Size.Height))
                .Chain(c => c.Size = new Size(this.Component.ChildBounds.Width, this.Component.ChildBounds.Height - title.Size.Height));
            this.Component.AddChild(this.ConfigParent);
        }

        public void SetParentMod(Mod parent) => this._parentMod = parent;

        #region AddItem Built-Ins
        public TextboxComponent AddItem(Expression<Func<string>> propertySelector, string name) => this.AddItem<string, TextboxComponent>(propertySelector, name);
        public CheckboxComponent AddItem(Expression<Func<bool>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => {
                CheckboxComponent checkbox = new CheckboxComponent().Chain(c => c.Location = location);
                parent.AddChild(ModConfigMenu.DefaultLabelFactory(location, labelText)
                    ?.Chain(c => c.Location = new Location(checkbox.Location.X + checkbox.Size.Width, checkbox.Location.Y + (checkbox.Size.Height - c.Size.Height) / 2)));
                return checkbox;
            });
        }

        public KeybindComponent AddItem(Expression<Func<Keys>> propertySelector, string name) => this.AddItem<Keys, KeybindComponent>(propertySelector, name);

        #region NumericTextboxComponent
        public NumericTextboxComponent AddItem(Expression<Func<byte>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, false, byte.MinValue, byte.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<sbyte>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, false, sbyte.MinValue, sbyte.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<ushort>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, false, ushort.MinValue, ushort.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<short>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, false, short.MinValue, short.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<uint>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, false, uint.MinValue, uint.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<int>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, false, int.MinValue, int.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<ulong>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, false, ulong.MinValue, ulong.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<long>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, false, long.MinValue, long.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<float>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, true, float.MinValue, float.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<double>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, location, labelText) => ModConfigMenu.NumericTextboxFactory(parent, location, labelText, true, double.MinValue, double.MaxValue));
        }

        private static NumericTextboxComponent NumericTextboxFactory(Component parent, Location location, string labelText, bool allowDecimal, double minimum, double maximum) {
            return ModConfigMenu.DefaultComponentFactory<NumericTextboxComponent>(parent, location, labelText)
                .Chain(c => c.AllowDecimal = allowDecimal)
                .Chain(c => c.Minimum = minimum)
                .Chain(c => c.Maximum = maximum);
        }
        #endregion
        #endregion

        public TComponent AddItem<TProperty, TComponent>(Expression<Func<TProperty>> propertySelector, string name) where TComponent : Component, IValueComponent<TProperty>, new() {
            return this.AddItem(propertySelector, name, ModConfigMenu.DefaultComponentFactory<TComponent>);
        }

        public TComponent AddItem<TProperty, TComponent>(Expression<Func<TProperty>> propertySelector, string name, ComponentFactory<TComponent, TProperty> componentFactory) where TComponent : Component, IValueComponent<TProperty> {
            if (!(propertySelector.Body is MemberExpression ex))
                throw new ArgumentException("Expression must select a field or property", nameof(propertySelector));

            // Get parent of the property
            Type parentType = ex.Expression?.Type;
            if (parentType == null)
                throw new ArgumentException($"Selected field or property must be from an instance of type {nameof(IAutoConfig)}", nameof(propertySelector));
            object parent = Expression.Lambda<Func<object>>(ex.Expression).Compile()();

            // Create the component (label is created by the function)
            TComponent component = componentFactory(this.ConfigParent, new Location(0, this.ConfigParent.TotalChildrenHeight), name);
            this.ConfigParent.AddChild(component);

            // Get the property's value
            TProperty value;
            switch (ex.Member) {
                case FieldInfo fieldInfo:
                    value = (TProperty) fieldInfo.GetValue(parent);
                    component.ValueChanged += (sender, e) => {
                        fieldInfo.SetValue(parent, component.GetValue());
                        this.OnSettingChanged(ex.Member);
                    };
                    break;
                case PropertyInfo propertyInfo:
                    value = (TProperty) propertyInfo.GetValue(parent);
                    component.ValueChanged += (sender, e) => {
                        propertyInfo.SetValue(parent, component.GetValue());
                        this.OnSettingChanged(ex.Member);
                    };
                    break;
                default:
                    throw new ArgumentException("Expression must select a field or property", nameof(propertySelector));
            }
            component.SetValue(value);

            // Add support for Stardew Config Menu
            if (this._parentMod != null) {
                object options = SCMHelper.GetModOptions(this._parentMod);
                if (options != null) {
                    switch (component) {
                        case CheckboxComponent checkbox:
                            SCMHelper.AddCheckbox(options as ModOptions, checkbox.IsChecked, name, (id, isChecked) => checkbox.IsChecked = isChecked);
                            break;
                        default:
                            break;
                    }
                }
            }

            return component;
        }

        private static TComponent DefaultComponentFactory<TComponent>(Component parent, Location location, string labelText) where TComponent : Component, new() {
            LabelComponent label = ModConfigMenu.DefaultLabelFactory(location, labelText);
            parent.AddChild(label);
            return new TComponent()
                .Chain(c => c.Location = new Location(location.X + label.Size.Width, location.Y))
                .Chain(c => c.Size = new Size(parent.ChildBounds.Width - label.Size.Width, c.Size.Height));
        }

        /// <summary>
        /// Creates a component and its label
        /// </summary>
        /// <typeparam name="TComponent">The type of component being created</typeparam>
        /// <typeparam name="TValue">The type of value the component represents</typeparam>
        /// <param name="parent">The parent component</param>
        /// <param name="location">Where the component should be created</param>
        /// <param name="labelText">The text associated with the label, or null for no label</param>
        /// <returns>The component that was created (not the label)</returns>
        public delegate TComponent ComponentFactory<out TComponent, TValue>(Component parent, Location location, string labelText) where TComponent : IValueComponent<TValue>;

        private static LabelComponent DefaultLabelFactory(Location location, string label) {
            return label == null ? null : new LabelComponent().SetText(label).Chain(c => c.Location = location);
        }

        public event EventHandler<SettingChangedEventArgs> SettingChanged;
        protected virtual void OnSettingChanged(MemberInfo setting) => this.OnSettingChanged(new SettingChangedEventArgs(setting));
        protected virtual void OnSettingChanged(SettingChangedEventArgs e) => this.SettingChanged?.Invoke(this, e);

        public class SettingChangedEventArgs : EventArgs {
            private MemberInfo Setting { get; }

            public SettingChangedEventArgs(MemberInfo setting) {
                this.Setting = setting;
            }
        }
    }
}

