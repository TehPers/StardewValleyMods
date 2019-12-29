using System;
using Ninject;
using Ninject.Planning.Bindings;
using StardewModdingAPI;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Api.DependencyInjection
{
    /// <summary>
    /// Constrains a <see cref="IAssetProvider"/> so that a particular source is injected.
    /// </summary>
    public class ContentSourceAttribute : ConstraintAttribute
    {
        /// <summary>
        /// Gets the requested content source.
        /// </summary>
        public ContentSource Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSourceAttribute"/> class.
        /// </summary>
        /// <param name="source">The content source that should be used.</param>
        public ContentSourceAttribute(ContentSource source)
        {
            this.Source = source;
        }

        /// <inheritdoc />
        public override bool Matches(IBindingMetadata metadata)
        {
            _ = metadata ?? throw new ArgumentNullException(nameof(metadata));

            return metadata.Has(nameof(ContentSource)) && metadata.Get<ContentSource>(nameof(ContentSource)) == this.Source;
        }
    }
}
