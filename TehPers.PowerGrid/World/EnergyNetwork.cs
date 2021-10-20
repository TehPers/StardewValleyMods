using System.Collections.Generic;

namespace TehPers.PowerGrid.World
{
    internal class EnergyNetwork : HashSet<IEnergyConductor>
    {
        // TODO: actual adjacency matrix
        /* Example:
         *
         * b-a-c
         *
         * Matrix:
         * |   | a | b | c |
         * | a |   | x | x |
         * | b | x |   |   |
         * | c | x |   |   |
         *
         * On remove a:
         * matrix says a is adjacent to b and c
         * remove the adjacency and rebuild network (or split it)
         * also only need to check for split if more than 1 adjacency
         * for 0 adjacencies, unless something went wrong, remove network
         */
        private Dictionary<IEnergyConductor, List<IEnergyConductor>> adjacencies;
    }
}