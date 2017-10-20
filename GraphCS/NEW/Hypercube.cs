using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphCS.NEW.Core;

namespace GraphCS.NEW
{
    class Hypercube : AGraph<BinaryNode>
    {
        /// <summary>
        /// Name of the graph
        /// </summary>
        public override string Name
        {
            get { return $"{Dimension}-Hypercube"; }
        }

        /// <summary>
        /// Initialize the new graph instance with specified dimension
        /// </summary>
        /// <param name="dim">Dimension</param>
        public Hypercube(int dim) : base(dim)
        {
        }

        /// <summary>
        /// Calculate current node number.
        /// (n-Hypercube has 2^n nodes.)
        /// </summary>
        /// <returns>Number of nodes</returns>
        protected override int CalcNodeNum()
        {
            return 1 << Dimension;
        }

        /// <summary>
        /// Returns a degree of the node.
        /// (n-Hypercube is n-regular.)
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Degree</returns>
        public override int GetDegree(BinaryNode node)
        {
            return Dimension;
        }

        /// <summary>
        /// Returns i-th neighbor of the node.
        /// Each neighbors are different with i.
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="i">Identifier of neighbor</param>
        /// <returns>i-th neighbor of the node</returns>
        public override BinaryNode GetNeighbor(BinaryNode node, int i)
        {
            return node ^ (1 << i);
        }

        /// <summary>
        /// Calculate the distance between node1 and node2.
        /// It is optimized for hypercube.
        /// </summary>
        /// <param name="node1">Source node</param>
        /// <param name="node2">Destination node</param>
        /// <returns>Distance</returns>
        public override int CalcDistance(BinaryNode node1, BinaryNode node2)
        {
            int c = node1.Addr ^ node2.Addr;
            c = (c & 0x55555555) + (c >> 1 & 0x55555555);
            c = (c & 0x33333333) + (c >> 2 & 0x33333333);
            c = (c & 0x0f0f0f0f) + (c >> 4 & 0x0f0f0f0f);
            c = (c & 0x00ff00ff) + (c >> 8 & 0x00ff00ff);
            return (c & 0x0000ffff) + (c >> 16 & 0x0000ffff);
        }
    }
}
