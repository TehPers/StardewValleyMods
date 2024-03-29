﻿using System;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Api.Effects
{
    /// <summary>
    /// A registered fishing effect.
    /// </summary>
    /// <param name="Name">The name of the effect.</param>
    /// <param name="EntryType">The entry type (which must extend <see cref="FishingEffectEntry"/>).</param>
    public sealed record FishingEffectRegistration(string Name, Type EntryType)
    {
        /// <summary>
        /// Creates a fishing effect registration.
        /// </summary>
        /// <typeparam name="T">The entry type.</typeparam>
        /// <param name="name">The name of the effect.</param>
        /// <returns>The registration.</returns>
        public static FishingEffectRegistration Of<T>(string name)
            where T : FishingEffectEntry
        {
            return new(name, typeof(T));
        }
    }
}
