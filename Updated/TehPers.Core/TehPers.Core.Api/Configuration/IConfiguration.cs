using System;

namespace TehPers.Core.Api.Configuration
{
    /// <summary>
    /// A configuration for a service.
    /// </summary>
    /// <typeparam name="TData">The type of data in this configuration.</typeparam>
    public interface IConfiguration<TData>
    {
        /// <summary>
        /// Gets or sets the data stored in this configuration.
        /// </summary>
        TData Value { get; set; }

        /// <summary>
        /// Raised after the value of this configuration changes.
        /// </summary>
        event EventHandler<ConfigurationChangedEventArgs<TData>> Changed;
    }
}