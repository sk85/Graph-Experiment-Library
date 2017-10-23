using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.NEW.Core
{
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
        /// Random object
        /// </summary>
        private Random Rand { get; }

        /// <summary>
        /// Graph object
        /// </summary>
        public GraphType G { get; }

        private double __FaultRatio;
        /// <summary>
        /// Node-fault ratio
        /// </summary>
        public double FaultRatio
        {
            get
            {
                return __FaultRatio;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(
                        "FaultRatio", 
                        "FaultRatio must be in [0, 1]"
                    );
                }
                else
                {
                    __FaultRatio = value;
                }
            }
        }

        /// <summary>
        /// FaultFlags[i] = true means i-th node is fault.
        /// </summary>
        public bool[] FaultFlags { get; private set; }

        /// <summary>
        /// Source node
        /// </summary>
        public NodeType SourceNode { get; private set; }

        /// <summary>
        /// Destination Node
        /// </summary>
        public NodeType DestinationNode { get; private set; }

        /// <summary>
        /// Ininialize new instance with graph object and random object
        /// </summary>
        /// <param name="g">Graph object</param>
        /// <param name="seed">Seed of random</param>
        public Experiment(GraphType g, int seed)
        {
            G = g;
            Rand = new Random(seed);
            SourceNode = new NodeType();
            DestinationNode = new NodeType();
        }

        /// <summary>
        /// Initialize FaultFlags
        /// </summary>
        /// <param name="faultRatio">Fault ratio in [0, 1]</param>
        /// <returns></returns>
        private void GenerateFaults(double faultRatio)
        {
            if (FaultFlags == null || FaultFlags.Length != G.NodeNum)
            {
                FaultFlags = new bool[G.NodeNum];
            }
            else
            {
                for (int i = 0; i < G.NodeNum; i++)
                {
                    FaultFlags[i] = false;
                }
            }

            int num = (int)(G.NodeNum * faultRatio);

            for (int i = 0; i < num; i++)
            {
                FaultFlags[CalcArbitaryNodeID()] = true;
            }
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

        /// <summary>
        /// Judging that node1 and node2 are connected or not by depth first search.
        /// </summary>
        /// <param name="node1">Node</param>
        /// <param name="node2">Node</param>
        /// <returns>True only if they are connected</returns>
        private bool IsConnected(NodeType node1, NodeType node2)
        {
            var visited = new bool[G.NodeNum];
            var stack = new Stack<NodeType>();
            stack.Push(node1);
            visited[node1.Addr] = true;
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var neighbor in G.GetNeighbor(current))
                {
                    if (!visited[neighbor.Addr] && !FaultFlags[neighbor.Addr])
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
        /// Initialize next experiment (fault state, source, destination)
        /// </summary>
        /// <returns>ExperimentParametors</returns>
        public void Next()
        {
            GenerateFaults(FaultRatio);
            do
            {
                SourceNode.Addr = CalcArbitaryNodeID();
                if (G.GetNeighbor(SourceNode).Any(n => !FaultFlags[n.Addr]))
                {
                    do
                    {
                        DestinationNode.Addr = CalcArbitaryNodeID();
                    } while (!IsConnected(SourceNode, DestinationNode));
                    return;
                }
            } while (true);
        }

        public int Routing_proposed(int timeoutLimit)
        {
            var current = new NodeType();
            int previewIndex = -1;
            current.Addr = SourceNode.Addr;
            var step = 0;

            while (current != DestinationNode)
            {
                // タイムアウト判定
                if (step++ >= timeoutLimit) return -2;

                // 相対距離を計算
                var rel = G.CalcRelativeDistance(current, DestinationNode);

                // 非故障かつ直前のノードでない前方、横、後方の数をそれぞれ数える
                int[] count = { 0, 0, 0 };
                for (int i = 0; i < G.Dimension; i++)
                {
                    if (i == previewIndex || FaultFlags[G.GetNeighbor(current, i).Addr])
                    {
                        rel[i] = -5;
                    }
                    else
                    {
                        count[1 + rel[i]]++;
                    }
                }

                // 前方、横、後方の順であればそこへ遷移
                int j = -1;
                if (count[0] > 0)
                {
                    int k = Rand.Next(count[0]);
                    while (k >= 0)
                    {
                        if (rel[++j] == -1) k--;
                    }
                }
                else if (count[1] > 0)
                {
                    int k = Rand.Next(count[1]);
                    while (k >= 0)
                    {
                        if (rel[++j] == 0) k--;
                    }
                }
                else if (count[2] > 0)
                {
                    int k = Rand.Next(count[2]);
                    while (k >= 0)
                    {
                        if (rel[++j] == 1) k--;
                    }
                }
                // なければ失敗
                else
                {
                    return -1;
                }

                previewIndex = j;
                current = G.GetNeighbor(current, j);
            }
            return step;
        }

#if DEBUG
        public void DEBUG_GenerateFaults()
        {
            Console.WriteLine("Debug \"GenerateFaults\"");

            for (double faultRatio = 0.0; faultRatio < 1.0; faultRatio += 0.1)
            {
                for (int i = 0; i < 100; i++)
                {
                    GenerateFaults(faultRatio);
                    var num = (int)(G.NodeNum * faultRatio);
                    for (int j = 0; j < G.NodeNum; j++)
                    {
                        if (FaultFlags[j]) num--;
                    }
                    if (num != 0)
                    {
                        Console.WriteLine("> NG");
                        return;
                    }
                }
            }
            Console.WriteLine("> OK");
        }
#endif
    }
}
