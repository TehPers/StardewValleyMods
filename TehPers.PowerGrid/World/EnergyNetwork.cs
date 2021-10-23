using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

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

    internal class NetworkFinder
    {
        public NetworkFinder()
        {
        }
        
        public IEnumerable<EnergyNetwork> FindNetworks(
            GameLocation location,
            IEnumerable<IEnergyConductor> conductors
        )
        {
            var nodes = location.Objects.Pairs.SelectMany(
                    kv => kv.Value is IEnergyConductor conductor
                        ? new[]
                        {
                            new NetworkNode
                            {
                                Conductor = conductor,
                                Position = kv.Value.TileLocation
                            }
                        }
                        : Enumerable.Empty<NetworkNode>()
                )
                .ToDictionary(node => node.Position, node => node);

            foreach (var node in nodes.Values)
            {
                // Create network from producers
                if (node is not { Conductor: IEnergyProducer producer })
                {
                    continue;
                }

                // Expand outward (BFS) from the producer to find the entire network
                var queue = new Queue<Vector2>(producer.OutgoingConnections);
                var network = new EnergyNetwork { producer };
                while (queue.TryDequeue(out var nextPos))
                {
                    // Ensure there is a node at this position and get it
                    if (!nodes.TryGetValue(nextPos, out var nextNode))
                    {
                        continue;
                    }

                    // Add the node to a network
                    if (node.Visited)
                    {
                        // Merge the network the node is in and the current network
                    }
                    else
                    {
                        // Add the node to this network
                        network.Add(node);
                        node.Visited = true;
                    }
                }
            }
        }

        private void BuildNetwork(
            Dictionary<Vector2, NetworkNode> nodes,
            EnergyNetwork network,
            NetworkNode currentNode
        )
        {
            // Add this node to the network
            network.Add(currentNode.Conductor);
            currentNode.Visited = true;

            // Add the nodes connected to this to the network
        }

        private class NetworkNode
        {
            public IEnergyConductor Conductor { get; init; }
            public Vector2 Position { get; init; }
            public bool Visited { get; set; }
        }
    }
}