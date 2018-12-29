using System;
using System.Collections.Generic;

namespace TehPers.CoreMod.Api.Conflux.Responsive {
    public abstract class ResponsiveValue<T> : IResponsiveValue<T> {
        private T _value;

        /// <inheritdoc />
        public T Value {
            get => this._value;
            protected set => this.SetValue(value);
        }

        protected ResponsiveValue() : this(default) { }
        protected ResponsiveValue(T initialValue) {
            this._value = initialValue;
        }

        private void SetValue(T value) {
            this._value = value;

            // Call all the handlers
            HashSet<WeakReference<Action<T>>> deadReferences = new HashSet<WeakReference<Action<T>>>();
            foreach (WeakReference<Action<T>> reference in this._valueChangedHandlers) {
                if (!reference.TryGetTarget(out Action<T> handler)) {
                    // Track the dead reference
                    deadReferences.Add(reference);
                } else {
                    handler(value);
                }
            }

            // Remove dead references
            this._valueChangedHandlers.ExceptWith(deadReferences);
        }

        private readonly HashSet<WeakReference<Action<T>>> _valueChangedHandlers = new HashSet<WeakReference<Action<T>>>();

        /// <inheritdoc />
        public event Action<T> ValueChanged {
            add => this._valueChangedHandlers.Add(new WeakReference<Action<T>>(value));
            remove => this._valueChangedHandlers.RemoveWhere(reference => !reference.TryGetTarget(out Action<T> handler) || handler == value);
        }
    }
}
