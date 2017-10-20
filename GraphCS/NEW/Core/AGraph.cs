using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.NEW.Core
{
    /// <summary>
    /// Abstract class of graph.
    /// </summary>
    abstract class AGraph<NodeType> where NodeType : ANode
    {
        #region Member variables / Properties

        /// <summary>
        /// Name of the graph
        /// </summary>
        public abstract string Name { get; }

        private int __Dimension;
        /// <summary>
        /// Dimension of the graph.
        /// When it is changed, NodeNum is also updated.
        /// </summary>
        public int Dimension
        {
            get { return __Dimension; }
            set
            {
                __Dimension = value;
                NodeNum = CalcNodeNum();
            }
        }

        /// <summary>
        /// Number of vertices.
        /// Updated only when Dimension is changed.
        /// </summary>
        public int NodeNum { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize the new graph instance with specified dimension
        /// </summary>
        /// <param name="dim">Dimension</param>
        protected AGraph(int dim)
        {
            Dimension = dim;
        }

        #endregion

        /// <summary>
        /// Calculate current node number.
        /// </summary>
        /// <returns>ノード数</returns>
        protected abstract int CalcNodeNum();

        /// <summary>
        /// Returns a degree of the node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Degree</returns>
        public abstract int GetDegree(NodeType node);

        /// <summary>
        /// Returns i-th neighbor of the node.
        /// Each neighbors are different with i.
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="i">Identifier of neighbor</param>
        /// <returns>i-th neighbor of the node</returns>
        public abstract NodeType GetNeighbor(NodeType node, int i);

        /// <summary>
        /// Returns all neighbors of the node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Enumerated neighbor of the node</returns>
        public IEnumerable<NodeType> GetNeighbor(NodeType node)
        {
            for (int i = 0; i < GetDegree(node); i++)
            {
                yield return GetNeighbor(node, i);
            }
        }


        /// <summary>
        /// Calculate the distance between node1 and node2 by BFS
        /// (breadth first search)
        /// </summary>
        /// <param name="node1">Source node</param>
        /// <param name="node2">Destination node</param>
        /// <returns>Distance</returns>
        public int CalcDistanceBFS(NodeType node1, NodeType node2)
        {
            if (node1 == node2) return 0;

            var que = new Queue<NodeType>();
            var distance = new int[NodeNum];
            for (int i = 0; i < NodeNum; i++)
            {
                distance[i] = 100000;
            }
            
            que.Enqueue(node1);
            distance[node1.Addr] = 0;
            while (que.Count > 0)
            {
                NodeType current = que.Dequeue();
                foreach (var neighbor in GetNeighbor(current))
                {
                    if (distance[neighbor.Addr] == 100000)
                    {
                        if (neighbor == node2) return distance[current.Addr] + 1;
                        distance[neighbor.Addr] = distance[current.Addr] + 1;
                        que.Enqueue(neighbor);
                    }
                }
            }

            throw new Exception("2頂点が非連結?");
        }

        /// <summary>
        /// Calculate the distance between node1 and node2.
        /// It calls CalcDistanceBFS at default.
        /// </summary>
        /// <param name="node1">Source node</param>
        /// <param name="node2">Destination node</param>
        /// <returns>Distance</returns>
        public virtual int CalcDistance(NodeType node1, NodeType node2)
        {
            return CalcDistanceBFS(node1, node2);
        }
    }
}
