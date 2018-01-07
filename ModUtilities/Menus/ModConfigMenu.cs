using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using ModUtilities.Helpers;
using ModUtilities.Menus.Components;
using ModUtilities.Menus.Components.Interfaces;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ModUtilities.Menus {
    public class ModConfigMenu : ModMenu {
        public ScrollableComponent ConfigParent { get; }

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

        #region AddProperty Built-Ins
        public TextboxComponent AddItem(Expression<Func<string>> propertySelector, string name) => this.AddItem<string, TextboxComponent>(propertySelector, name);
        public CheckboxComponent AddItem(Expression<Func<bool>> propertySelector, string name) => this.AddItem<bool, CheckboxComponent>(propertySelector, name);
        public KeybindComponent AddItem(Expression<Func<Keys>> propertySelector, string name) => this.AddItem<Keys, KeybindComponent>(propertySelector, name);

        #region NumericTextboxComponent
        public NumericTextboxComponent AddItem(Expression<Func<byte>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, false, byte.MinValue, byte.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<sbyte>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, false, sbyte.MinValue, sbyte.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<ushort>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, false, ushort.MinValue, ushort.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<short>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, false, short.MinValue, short.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<uint>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, false, uint.MinValue, uint.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<int>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, false, int.MinValue, int.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<ulong>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, false, ulong.MinValue, ulong.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<long>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, false, long.MinValue, long.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<float>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, true, float.MinValue, float.MaxValue));
        }

        public NumericTextboxComponent AddItem(Expression<Func<double>> propertySelector, string name) {
            return this.AddItem(propertySelector, name, (parent, widthRemaining) => ModConfigMenu.NumericTextboxFactory(parent, widthRemaining, true, double.MinValue, double.MaxValue));
        }

        private static NumericTextboxComponent NumericTextboxFactory(Component parent, int widthRemaining, bool allowDecimal, double minimum, double maximum) {
            return ModConfigMenu.DefaultComponentFactory<NumericTextboxComponent>(parent, widthRemaining)
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

            // Create the label
            LabelComponent label = name == null ? null : new LabelComponent().SetText(name);
            
            // Create the component
            int labelWidth = label?.Size.Width ?? 0;
            TComponent component = componentFactory(this.ConfigParent, this.ConfigParent.ChildBounds.Width - labelWidth);
            this.ConfigParent.AddChild(component);
            component.Location = new Location(labelWidth, this.ConfigParent.TotalChildrenHeight);

            // Position the label
            if (label != null) {
                label.Location = new Location(0, component.Location.Y + (component.Size.Height - label.Size.Height) / 2);
                this.ConfigParent.AddChild(label);
            }

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

            return component;
        }

        private static TComponent DefaultComponentFactory<TComponent>(Component parent, int widthRemaining) where TComponent : Component, new() {
            return new TComponent().Chain(c => c.Size = new Size(widthRemaining, c.Size.Height));
        }

        public delegate TComponent ComponentFactory<out TComponent, TValue>(Component parent, int widthRemaining) where TComponent : IValueComponent<TValue>;

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

