using System;

namespace TehPers.Core.Api.Configuration
{
    /// <inheritdoc />
    public class SimpleConfiguration<TData> : IConfiguration<TData>
    {
        private TData value;

        /// <inheritdoc />
        public TData Value
        {
            get => this.value;
            set
            {
                var prev = this.value;
                this.value = value;
                this.OnChanged(new ConfigurationChangedEventArgs<TData>(prev, value));
            }
        }

        /// <inheritdoc />
        public event EventHandler<ConfigurationChangedEventArgs<TData>> Changed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConfiguration{TData}"/> class.
        /// </summary>
        /// <param name="value">The initial value stored in this configuration.</param>
        public SimpleConfiguration(TData value)
        {
            this.value = value;
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="args">The arguments for the event.</param>
        protected void OnChanged(ConfigurationChangedEventArgs<TData> args)
        {
            this.Changed?.Invoke(this, args);
        }
    }
}