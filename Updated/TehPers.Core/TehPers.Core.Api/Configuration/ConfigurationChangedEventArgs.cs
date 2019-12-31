using System;

namespace TehPers.Core.Api.Configuration
{
    /// <summary>
    /// Event arguments for the event raised when a configuration's value changes.
    /// </summary>
    /// <typeparam name="TData">The type of data stored within the configuration.</typeparam>
    public class ConfigurationChangedEventArgs<TData> : EventArgs
    {
        /// <summary>
        /// Gets the previous value the configuration had.
        /// </summary>
        public TData PreviousValue { get; }

        /// <summary>
        /// Gets the new value the configuration has.
        /// </summary>
        public TData NewValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChangedEventArgs{TData}"/> class.
        /// </summary>
        /// <param name="previousValue">The previous value the configuration had.</param>
        /// <param name="newValue">The new value the configuration has.</param>
        public ConfigurationChangedEventArgs(TData previousValue, TData newValue)
        {
            this.PreviousValue = previousValue;
            this.NewValue = newValue;
        }
    }
}