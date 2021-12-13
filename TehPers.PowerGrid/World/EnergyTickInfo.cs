using TehPers.PowerGrid.Units;

namespace TehPers.PowerGrid.World
{
    /// <summary>
    /// Information about the energy state of a conductor.
    /// </summary>
    /// <param name="Satisfaction">The power satisfaction available to the conductor.</param>
    public record EnergyTickInfo(Power Satisfaction);
}