using System;

namespace ModUtilities.Menus.Components.Interfaces {
    public interface IValueComponent<T> : IValueComponent {
        void SetValue(T value);
        T GetValue();
    }

    public interface IValueComponent {
        event EventHandler ValueChanged;
    }
}
