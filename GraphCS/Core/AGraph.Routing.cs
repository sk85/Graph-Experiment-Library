using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    /// <summary>
    /// Methods for routing
    /// </summary>
    abstract partial class AGraph
    {
        /// <summary>
        ///   Execute routing according to GetNext.
        ///   (Routing fails when loop is detexted.)
        /// </summary>
        /// <param name="node1">Start node</param>
        /// <param name="node2">Destination node</param>
        /// <param name="GetNext">
        ///   Function that 
        ///     arg1 = current node,
        ///     arg2 = destination node,
        ///     return = node to send msg.
        ///   If there is no nodes tosend msg, return current.
        /// </param>
        /// <returns>
        ///   Step count. 
        ///   It returns -1 when routing failed.
        /// </returns>
        public int Routing(uint node1, uint node2, Func<uint, uint, uint> GetNext)
        {
            var visited = new bool[NodeNum];
            var current = node1;
            int step = 0;

            while (current != node2)
            {
                visited[current] = true;
                var next = GetNext(current, node2);

                // Loop detected or no nodes to send msg
                if (next == current || visited[next])
                {
                    return -1;
                }
                else
                {
                    step++;
                    current = next;
                }
            }

            return step;
        }

        // Simple routing
        public uint GetNext_Simple(uint current, uint destination)
        {
            var r = CalcRelativeDistance(current, destination);
            for (int i = 0; i < GetDegree(current); i++)
            {
                var neighbor = GetNeighbor(current, i);
                if (r[i] < 0 && !FaultFlags[neighbor])
                {
                    return neighbor;
                }
            }
            return current;
        }

        // Normal routing
        public uint GetNext_Normal(uint current, uint destination)
        {
            var r = CalcRelativeDistance(current, destination);
            var side = current;
            for (int i = 0; i < GetDegree(current); i++)
            {
                var neighbor = GetNeighbor(current, i);
                if (!FaultFlags[neighbor])
                {
                    if (r[i] < 0)
                    {
                        return neighbor;
                    }
                    else if (r[i] == 0)
                    {
                        side = neighbor;
                    }
                }
            }
            return side;
        }
    }
}
