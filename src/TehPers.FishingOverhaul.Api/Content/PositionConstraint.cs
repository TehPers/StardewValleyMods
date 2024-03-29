﻿using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// A set of constraints for positions.
    /// </summary>
    public record PositionConstraint
    {
        /// <summary>
        /// Constraints for the x-coordinate.
        /// </summary>
        [DefaultValue(null)]
        public CoordinateConstraint? X { get; init; }

        /// <summary>
        /// Constraints for the Y-coordinate.
        /// </summary>
        [DefaultValue(null)]
        public CoordinateConstraint? Y { get; init; }

        /// <summary>
        /// Checks whether a position matches these constraints.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns><see langword="true"/> if the position matches, <see langword="false"/> otherwise.</returns>
        public bool Matches(Vector2 position)
        {
            var (x, y) = position;
            return this.X?.Matches(x) is not false
                && this.Y?.Matches(y) is not false;
        }
    }
}
