using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    /// <summary>
    /// Methods for experiment
    /// </summary>
    abstract partial class AGraph
    {
        /// <summary>
        /// Initialize FaultFlags randomly according to faultRatio.
        /// </summary>
        /// <param name="faultRatio">Fault ratio in [0, 1]</param>
        public void GenerateFaults(double faultRatio)
        {
            FaultRatio = faultRatio;

            // Set all flags false
            for (uint i = 0; i < NodeNum; i++) FaultFlags[i] = false;
            
            for (uint i = 0; i < FaultNodeNum; i++)
            {
                uint rand = (uint)(Rand.NextDouble() * (NodeNum - i));
                uint index = 0, count = 0;

                while (count <= rand)
                {
                    if (!FaultFlags[index++]) count++;
                }
                FaultFlags[index - 1] = true;
            }
        }

        /// <summary>
        /// Returns node1 and node2 are connected or not.
        /// </summary>
        /// <param name="node1">Node</param>
        /// <param name="node2">Node</param>
        /// <returns>Node1 and node2 are connected or not</returns>
        public bool IsConnected(uint node1, uint node2)
        {
            // Depth first search
            var visited = new bool[NodeNum];
            var stack = new Stack<uint>();
            stack.Push(node1);
            visited[node1] = true;
            while (stack.Count > 0)
            {
                uint current = stack.Pop();
                foreach (var neighbor in GetNeighbor(current))
                {
                    if (!visited[neighbor] && !FaultFlags[neighbor])
                    {
                        if (neighbor == node2) return true;
                        visited[neighbor] = true;
                        stack.Push(neighbor);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Retuns unfault node randomly.
        /// </summary>
        /// <returns>Node</returns>
        public uint GetArbitaryNode()
        {
            uint unfaultNum = NodeNum - FaultNodeNum;
            uint rand = (uint)(Rand.NextDouble() * unfaultNum);
            uint index = 0, count = 0;
            while (count <= rand)
            {
                if (!FaultFlags[index++])
                {
                    count++;
                }
            }
            return index - 1;
        }

        /// <summary>
        /// Returns unfault node that is connected with argument node randomly.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Node (If no connected nodes, returns argument node)</returns>
        public uint GetArbitaryConnectedNodes(uint node)
        {
            // If all neighbor nodes are fault, it is failure.
            if (GetNeighbor(node).Any(n => !FaultFlags[n]))
            {
                uint node2;
                do
                {
                    node2 = GetArbitaryNode();
                }
                while (node == node2 || !IsConnected(node, node2));
                return node2;
            }
            else
            {
                return node;
            }
        }

        /// <summary>
        /// Returns ExperimentParam
        /// </summary>
        /// <returns>ExperimentParam</returns>
        public ExprimentParam GetExperimentParam()
        {
            uint node1, node2;
            do
            {
                node1 = GetArbitaryNode();
                node2 = GetArbitaryConnectedNodes(node1);
            } while (node1 == node2);
            return new ExprimentParam(node1, node2);
        }

        /// <summary>
        /// Parameters for experiment
        /// </summary>
        public struct ExprimentParam
        {
            public uint Node1 { get; }
            public uint Node2 { get; }
            public ExprimentParam(uint node1, uint node2)
            {
                Node1 = node1;
                Node2 = node2;
            }
        }

        /// <summary>
        /// ToString method for debug.
        /// </summary>
        /// <returns>Information text</returns>
        public override string ToString()
        {
            string str = string.Format(
                    "{0,5}|{1,16}|{2,5}|\n",
                    "ID",
                    "Addr",
                    "IsFault"
                );
            for (uint i = 0; i < NodeNum; i++)
            {
                str += string.Format(
                    "{0,5}|{1,16}|{2,7}|\n",
                    i,
                    Debug.UintToBinaryString(i, 16, 16),
                    FaultFlags[i]
                );
            }
            return str;
        }
    }
}
