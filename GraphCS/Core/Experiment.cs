using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    /// <summary>
    /// State of experiment.
    /// This class contains all parameters for routing experiment.
    /// </summary>
    /// <typeparam name="GraphType">GraphType must be derived class of AGraph</typeparam>
    /// <typeparam name="NodeType">NodeType must be derived class of ANode</typeparam>
    partial class Experiment<NodeType> where NodeType : ANode, new()
    {
        /// <summary>
        /// Random object
        /// </summary>
        public Random Rand { get; }

        /// <summary>
        /// Graph object
        /// </summary>
        public AGraph<NodeType> G { get; }

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
        public Experiment(AGraph<NodeType> g, int seed)
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
            while (FaultFlags[x = Rand.Next(G.NodeNum)]) ;
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

        public int CalcDistance()
        {
            var que = new Queue<NodeType>();
            var distance = new int[G.NodeNum];

            que.Enqueue(SourceNode);
            distance[SourceNode.Addr] = 1;
            while (true)
            {
                NodeType current = que.Dequeue();
                foreach (var neighbor in G.GetNeighbor(current))
                {
                    if (!FaultFlags[neighbor.Addr] && distance[neighbor.Addr] == 0)
                    {
                        if (neighbor == DestinationNode) return distance[current.Addr];
                        distance[neighbor.Addr] = distance[current.Addr] + 1;
                        que.Enqueue(neighbor);
                    }
                }
            }
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

        #region Routing

        /// <summary>
        /// 迂回しない。
        /// </summary>
        /// <returns>ステップ数(失敗:-1)</returns>
        public int Routing_NoDetour()
        {
            var current = new NodeType();
            current.Addr = SourceNode.Addr;
            var step = 0;

            while (current != DestinationNode)
            {
                step++;

                // 相対距離を計算
                var rel = G.CalcRelativeDistance(current, DestinationNode);
                
                // 候補頂点を探す
                NodeType forward = null;
                for (int i = 0; i < G.Dimension; i++)
                {
                    var neighbor = G.GetNeighbor(current, i);
                    if (rel[i] == -1 && !FaultFlags[neighbor.Addr])
                    {
                        forward = neighbor;
                        break;
                    }
                }

                // 非故障な候補頂点があればルーティング
                if (forward != null)
                {
                    current = forward;
                }
                // なければ失敗
                else
                {
                    return -1;
                }
            }
            return step;
        }

        /// <summary>
        /// 後方への迂回をしない。
        /// 横迂回時にランダムに選ばない。
        /// </summary>
        /// <returns>ステップ数(ループ:-2、見つからない:-1)</returns>
        public int Routing_NoBackTrack_NoRandom()
        {
            var visited = new bool[G.NodeNum];
            var current = new NodeType();
            var prevIndex = -1;
            current.Addr = SourceNode.Addr;
            var step = 0;

            while (current != DestinationNode)
            {
                step++;
                visited[current.Addr] = true;

                // 相対距離を計算
                var rel = G.CalcRelativeDistance(current, DestinationNode);

                // 候補頂点を探す
                NodeType forward = null, side = null;
                for (int i = 0; i < G.Dimension; i++)
                {
                    var neighbor = G.GetNeighbor(current, i);
                    if (!FaultFlags[neighbor.Addr])
                    {
                        if (rel[i] == -1)
                        {
                            prevIndex = i;
                            forward = neighbor;
                            break;
                        }
                        else if (rel[i] == 0 && i != prevIndex)
                        {
                            prevIndex = i;
                            side = neighbor;
                        }
                    }
                }

                // 前方が見つかっていれば前方へ
                if (forward != null)
                {
                    current = forward;
                }
                // 横方が見つかっていれば横方へ
                else if (side != null)
                {
                    if (visited[side.Addr]) return -2;
                    current = side;
                }
                // なければ失敗
                else
                {
                    return -1;
                }
            }
            return step;
        }

        /// <summary>
        /// 後方への迂回をしない。
        /// </summary>
        /// <param name="timeoutLimit">タイムアウトまでのステップ数</param>
        /// <returns>ステップ数(タイムアウト:-2、失敗:-1)</returns>
        public int Routing_NoBackTrack(int timeoutLimit)
        {
            var current = new NodeType();
            var prevIndex = -1;
            current.Addr = SourceNode.Addr;
            var step = 0;

            while (current != DestinationNode)
            {
                // タイムアウト判定
                if (step++ > timeoutLimit) return -2;

                // 相対距離を計算
                var rel = G.CalcRelativeDistance(current, DestinationNode);

                // 候補頂点を探す
                NodeType forward = null;
                int sideCount = 0;
                for (int i = 0; i < G.Dimension; i++)
                {
                    var neighbor = G.GetNeighbor(current, i);
                    if (!FaultFlags[neighbor.Addr])
                    {
                        if (rel[i] == -1)
                        {
                            prevIndex = i;
                            forward = neighbor;
                            break;
                        }
                        else if (rel[i] == 0 && i != prevIndex)
                        {
                            sideCount++;
                        }
                    }
                }

                // 前方が見つかっていれば前方へ
                if (forward != null)
                {
                    current = forward;
                }
                // 横方が見つかっていれば横方をランダムに選ぶ
                else if (sideCount > 0)
                {
                    int ii = 0;
                    for (int i = Rand.Next(sideCount); i > 0; ii++)
                    {
                        if (rel[ii] == 0) i--;
                    }
                    current = G.GetNeighbor(current, ii);
                }
                // なければ失敗
                else
                {
                    return -1;
                }
            }
            return step;
        }

        /// <summary>
        /// 迂回時にランダムに選ばない。
        /// </summary>
        /// <returns>ステップ数(タイムアウト:-2、見つからない:-1)</returns>
        public int Routing_NoRandom(int timeoutLimit)
        {
            var current = new NodeType();
            var prevIndex = -1;
            current.Addr = SourceNode.Addr;
            var step = 0;

            while (current != DestinationNode)
            {
                if (++step > timeoutLimit) return -1;

                // 相対距離を計算
                var rel = G.CalcRelativeDistance(current, DestinationNode);

                // 候補頂点を探す
                int forward = -1, side = -1, back = -1;
                for (int i = 0; i < G.Dimension; i++)
                {
                    var neighbor = G.GetNeighbor(current, i);
                    if (!FaultFlags[neighbor.Addr])
                    {
                        if (rel[i] == -1)
                        {
                            forward = i;
                            break;
                        }
                        else if (i != prevIndex)
                        {
                            if (rel[i] == 0)
                            {
                                side = i;
                            }
                            else
                            {
                                back = i;
                            }
                        }
                    }
                }

                // 前方が見つかっていれば前方へ
                if (forward != -1)
                {
                    current = G.GetNeighbor(current, forward);
                }
                // 横方が見つかっていれば横方へ
                else if (side != -1)
                {
                    current = G.GetNeighbor(current, side);
                }
                // 後方が見つかっていれば横方へ
                else if (back != -1)
                {
                    current = G.GetNeighbor(current, back);
                }
                // なければ失敗
                else
                {
                    return -1;
                }
            }
            return step;
        }

        /// <summary>
        /// ルーティングメソッド。
        /// </summary>
        /// <param name="timeoutLimit">タイムアウトまでのステップ数</param>
        /// <returns>ステップ数(タイムアウト:-2、見つからない:-1)</returns>
        public int Routing(int timeoutLimit)
        {
            var current = new NodeType();
            int previewIndex = -1;
            current.Addr = SourceNode.Addr;
            var step = 0;

            while (current != DestinationNode)
            {
                // タイムアウト判定
                if (++step >= timeoutLimit) return -2;

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

        #endregion
    }
}
