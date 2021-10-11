using System.ComponentModel;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// A set of constraints for coordinates.
    /// </summary>
    [JsonDescribe]
    public record CoordinateConstraint
    {
        [Description("Coordinate value must be greater than this.")]
        public float? GreaterThan { get; init; }

        [Description("Coordinate value must be greater than or equal to this.")]
        public float? GreaterThanEq { get; init; }

        [Description("Coordinate value must be less than this.")]
        public float? LessThan { get; init; }

        [Description("Coordinate value must be less than or equal to this.")]
        public float? LessThanEq { get; init; }

        public bool Matches(float value)
        {
            return (this.GreaterThan is not { } gt || value > gt)
                && (this.GreaterThanEq is not { } gte || value >= gte)
                && (this.LessThan is not { } lt || value < lt)
                && (this.LessThanEq is not { } lte || value <= lte);
        }
    }
}