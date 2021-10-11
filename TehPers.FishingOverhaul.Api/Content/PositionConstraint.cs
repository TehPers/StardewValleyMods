using System.ComponentModel;
using Microsoft.Xna.Framework;
using TehPers.Core.Api.Json;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// A set of constraints for positions.
    /// </summary>
    [JsonDescribe]
    public record PositionConstraint
    {
        [Description("Constraints for the x-coordinate.")]
        public CoordinateConstraint? X { get; init; }

        [Description("Constraints for the Y-coordinate.")]
        public CoordinateConstraint? Y { get; init; }

        public bool Matches(Vector2 position)
        {
            var (x, y) = position;
            return this.X?.Matches(x) is not false
                && this.Y?.Matches(y) is not false;
        }
    }
}