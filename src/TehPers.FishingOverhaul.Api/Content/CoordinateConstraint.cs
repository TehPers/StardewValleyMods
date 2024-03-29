﻿using System.ComponentModel;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// A set of constraints for coordinates.
    /// </summary>
    public record CoordinateConstraint
    {
        /// <summary>
        /// Coordinate value must be greater than this.
        /// </summary>
        [DefaultValue(null)]
        public float? GreaterThan { get; init; }

        /// <summary>
        /// Coordinate value must be greater than or equal to this.
        /// </summary>
        [DefaultValue(null)]
        public float? GreaterThanEq { get; init; }

        /// <summary>
        /// Coordinate value must be less than this.
        /// </summary>
        [DefaultValue(null)]
        public float? LessThan { get; init; }

        /// <summary>
        /// Coordinate value must be less than or equal to this.
        /// </summary>
        [DefaultValue(null)]
        public float? LessThanEq { get; init; }

        /// <summary>
        /// Checks whether a coordinate value matches these constraints.
        /// </summary>
        /// <param name="value">The value of the coordinate.</param>
        /// <returns><see langword="true"/> if the value matches, <see langword="false"/> otherwise.</returns>
        public bool Matches(float value)
        {
            return (this.GreaterThan is not { } gt || value > gt)
                && (this.GreaterThanEq is not { } gte || value >= gte)
                && (this.LessThan is not { } lt || value < lt)
                && (this.LessThanEq is not { } lte || value <= lte);
        }
    }
}
