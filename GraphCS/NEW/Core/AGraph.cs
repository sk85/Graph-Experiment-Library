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
    abstract class AGraph<NodeType> where NodeType : ANode, new()
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

        #region Graph definition and distance calculation
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

            throw new Exception("It is suggested that node1 and node2 is UNCONNECTED.");
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

        /// <summary>
        /// Calculate relative distance. 
        /// </summary>
        /// <param name="current">Current node</param>
        /// <param name="destination">Destination node</param>
        /// <returns>rel[i] is relative distance of i-th neighbor of current node</returns>
        public virtual int[] CalcRelativeDistance(NodeType current, NodeType destination)
        {
            var rel = new int[Dimension];
            var dis = CalcDistance(current, destination);
            for (int i = 0; i < Dimension; i++)
            {
                rel[i] = CalcDistance(GetNeighbor(current, i), destination);
            }
            return rel;
        }
        #endregion

        #region For experiment
        /// <summary>
        /// Generating faults.
        /// </summary>
        /// <param name="faultRatio">Fault ratio in [0, 1]</param>
        /// <param name="rand">Random object</param>
        /// <returns></returns>
        public bool[] GenerateFaults(double faultRatio, Random rand)
        {
            int num = (int)(NodeNum * faultRatio);
            var fault = new bool[num];

            for (int i = 0; i < num; i++)
            {
                fault[CalcArbitaryNodeID(fault, rand)] = true;
            }

            return fault;
        }

        /// <summary>
        /// Judging that node1 and node2 are connected or not by depth first search.
        /// </summary>
        /// <param name="node1">Node</param>
        /// <param name="node2">Node</param>
        /// <param name="fault">Fault flags</param>
        /// <returns>True only if they are connected</returns>
        public bool IsConnected(NodeType node1, NodeType node2, bool[] fault)
        {
            var visited = new bool[NodeNum];
            var stack = new Stack<NodeType>();
            stack.Push(node1);
            visited[node1.Addr] = true;
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var neighbor in GetNeighbor(current))
                {
                    if (!visited[neighbor.Addr] && !fault[neighbor.Addr])
                    {
                        if (neighbor == node2) return true;
                        visited[neighbor.Addr] = true;
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
        private int CalcArbitaryNodeID(bool[] fault, Random rand)
        {
            int x;
            while (!fault[x = (int)(rand.NextDouble() * NodeNum)]) ;
            return x;
        }

        /// <summary>
        /// Returns ExperimentParametor
        /// </summary>
        /// <returns>ExperimentParametors</returns>
        public void GetExperimentParam(double faultRatio, Random rand, bool[] fault, out NodeType node1, out NodeType node2)
        {
            fault = GenerateFaults(faultRatio, rand);
            node1 = new NodeType();
            node2 = new NodeType();
            do
            {
                node1.Addr = CalcArbitaryNodeID(fault, rand);
                if (GetNeighbor(node1).Any(n => !fault[n.Addr]))
                {
                    do
                    {
                        node2.Addr = CalcArbitaryNodeID(fault, rand);
                    } while (!IsConnected(node1, node2, fault));
                    return;
                }
            } while (true);
        }
        #endregion

        #region Routing

        /// <summary>
        /// 前方隣接頂点が1つしか見つけられないルーティング
        /// </summary>
        /// <param name="node1">出発頂点</param>
        /// <param name="node2">目的頂点</param>
        /// <param name="timeoutLimit">タイムアウト上限</param>
        /// <param name="detour">迂回をするか否か</param>
        /// <returns>ステップ数(袋小路:-1, タイムアウト:-2)</returns>
        public int Routing_ForwardSingle(NodeType node1, NodeType node2, int timeoutLimit, bool detour)
        {
            var current = node1;
            var preview = node1;
            var step = 0;

            while (current != node2)
            {
                // タイムアウト判定
                if (step >= timeoutLimit) return -2;

                // ランダムに一個前方を見つける
                uint next;
                {
                    var rel = CalcRelativeDistance(current, node2);
                    var count = rel.Count(x => x == -1);
                    var ii = Rand.Next(count);
                    int i;
                    for (i = 0; ii > 0; i++)
                    {
                        if (rel[i] == -1) ii--;
                    }
                    next = GetNeighbor(current, i);
                }

                // 故障していた場合
                if (FaultFlags[next])
                {
                    // 迂回ありのとき
                    if (detour)
                    {
                        foreach (var x in GetNeighbor(current))
                        {
                            if (!FaultFlags[x] && x != preview)
                            {
                                preview = current;
                                current = x;
                                continue;
                            }
                        }
                        // 非故障かつ後退しない頂点が見つからない = 袋小路
                        return -1;
                    }
                    // 迂回なしのとき
                    else
                    {
                        return -1;
                    }
                }
                // 故障していない場合
                else
                {
                    preview = current;
                    current = next;
                }

                step++;
            }

            return step;
        }
        #endregion
    }

    /// <summary>
    /// State of experiment.
    /// This class contains all parameters for routing experiment.
    /// </summary>
    /// <typeparam name="GraphType">GraphType must be derived class of AGraph</typeparam>
    /// <typeparam name="NodeType">NodeType must be derived class of ANode</typeparam>
    class Experiment<GraphType, NodeType>
        where GraphType : AGraph<NodeType>
        where NodeType : ANode, new()
    {
        /// <summary>
        /// Graph object
        /// </summary>
        public GraphType G { get; }

        /// <summary>
        /// Random object
        /// </summary>
        private Random Rand { get; }

        /// <summary>
        /// FaultFlags[i] = true means i-th node is fault.
        /// </summary>
        public bool[] FaultFlags { get; private set; }

        /// <summary>
        /// Ininialize new instance with graph object and random object
        /// </summary>
        /// <param name="g">Graph object</param>
        /// <param name="seed">Seed of random</param>
        public Experiment(GraphType g, int seed)
        {
            G = g;
            Rand = new Random(seed);
        }

        /// <summary>
        /// Initialize FaultFlags
        /// </summary>
        /// <param name="faultRatio">Fault ratio in [0, 1]</param>
        /// <param name="rand">Random object</param>
        /// <returns></returns>
        public bool[] GenerateFaults(double faultRatio)
        {
            int num = (int)(G.NodeNum * faultRatio);
            var fault = new bool[num];

            for (int i = 0; i < num; i++)
            {
                fault[CalcArbitaryNodeID()] = true;
            }

            return fault;
        }

        /// <summary>
        /// Retuns unfault node randomly.
        /// </summary>
        /// <returns>Node</returns>
        private int CalcArbitaryNodeID()
        {
            int x;
            while (FaultFlags[x = (int)(Rand.NextDouble() * G.NodeNum)]) ;
            return x;
        }
    }
}
