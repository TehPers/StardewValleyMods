using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TehPers.PowerGrid.Units;
using SObject = StardewValley.Object;

namespace TehPers.PowerGrid.World
{
    /// <summary>
    /// An object which can conduct power.
    /// </summary>
    public interface IEnergyConductor
    {
        IEnumerable<Vector2> Connections { get; }

        /// <summary>
        /// Updates this conductor's state.
        /// </summary>
        /// <param name="info">Information about the energy state of this conductor.</param>
        void Update(EnergyTickInfo info);
    }

    public interface IEnergyProducer : IEnergyConductor
    {
        Power Production { get; }
    }

    public interface IEnergyConsumer : IEnergyConductor
    {
        Power Consumption { get; }
    }
}