using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    /// <summary>
    /// クロスドキューブのクラスです。
    /// </summary>
    partial class CrossedCube : AGraph
    {
        public override string Name
        {
            get { return "CrossedCube"; }
        }

        /// <summary>
        /// AGraphのコンストラクタを呼びます。
        /// </summary>
        /// <param name="dim">次元数</param>
        /// <param name="randSeed">乱数のシード</param>
        public CrossedCube(int dim, int randSeed) : base(dim, randSeed) { }

        /// <summary>
        /// 現在の次元数からノード数を計算して返します。
        /// <para>AGraphから継承されたメンバーです。</para>
        /// </summary>
        /// <param name="dim">次元数</param>
        /// <returns>ノード数</returns>
        protected override UInt32 CalcNodeNum(int dim)
        {
            return (UInt32)1 << dim;
        }

        /// <summary>
        /// nodeの次数を返します。
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <returns></returns>
        public override int GetDegree(Node node)
        {
            return Dimension;
        }

        /// <summary>
        /// nodeの第indexエッジと接続する隣接ノードを返します。
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="index">エッジの番号</param>
        /// <returns>隣接ノードのアドレス</returns>
        public override Node GetNeighbor(Node node, int index)
        {
            const UInt32 mask1 = 0x55555555;            // 01010101....01
            UInt32 mask2 = ((UInt32)1 << index) - 1;    // 00...0111...11
            UInt32 mask = ((node.ID & mask1 & mask2) << 1) | ((UInt32)1 << index);
            return new BinaryNode(node.ID ^ mask);
        }

        public IEnumerable<int> GetFowardNeighbor(BinaryNode node1, BinaryNode node2)
        {
            UInt32 u = node1.Addr, v = node2.Addr, diff = u ^ v;

            // 出発頂点と目的頂点が同じなら前方は存在しない
            if (u == v) yield break;

            int count = 0;
            int tmp = -1;   // 前方かわからない隣接頂点のindex

            // iは現在見ているビットペアの左側のindex
            for (int i = (Dimension - 1) | 1; i > 0; i -= 2)
            {
                // OK
                // 初回
                if (count == 0)
                {
                    if (node1[i] != node2[i])
                    {
                        // 両方異なる
                        if (node1[i - 1] != node2[i - 1])
                        {
                            yield return i;
                            yield return i - 1;
                            count += 2;
                        }
                        // 左のみ異なる
                        else
                        {
                            tmp = i;
                            count++;
                        }
                    }
                    // 右のみ異なる
                    else if (node1[i - 1] != node2[i - 1])
                    {
                        tmp = i - 1;
                        count++;
                    }
                }

                // OK
                // count=1
                else if (count == 1)
                {
                    // 左が異なる
                    if (node1[i] != node2[i])
                    {
                        // 両方異なる
                        if (node1[i - 1] != node2[i - 1])
                        {
                            // 右が０  (00, 11), (10, 01)
                            if (node1[i - 1] == 0)
                            {
                                yield return i - 1;
                                count++;
                            }
                            // 右が１  (01, 10), (11, 00)
                            else
                            {
                                yield return tmp;
                                count++;
                            }
                        }
                        // 左だけ異なる
                        else
                        {
                            // 右が０  (00, 10), (10, 00)
                            if (node1[i - 1] == 0)
                            {
                                yield return i;
                                yield return tmp;
                                tmp = -1;
                                count++;
                            }
                            // 右が１  (01, 11), (11, 01)
                            else
                            {
                                // 第iは勝手に揃うので何もしない。
                                // countも増えず、tmpもそのまま
                            }
                        }
                    }
                    // 左が同じ
                    else
                    {
                        // 右だけ異なる
                        if (node1[i - 1] != node2[i - 1])
                        {
                            // 右が０  (00, 01), (10, 11)
                            if (node1[i - 1] == 0)
                            {
                                yield return tmp;
                                count++;
                            }
                            // 右が１  (01, 00), (11, 10)
                            else
                            {
                                yield return i - 1;
                                count++;
                            }
                        }
                        // 両方同じ
                        else
                        {
                            // 右が０  (00, 00), (10, 10)
                            if (node1[i - 1] == 0)
                            {
                                // 何もしない。
                                // countも増えず、tmpもそのまま
                            }
                            // 右が１  (01, 01), (11, 11)
                            else
                            {
                                // iがずれちゃうので先に揃えとく
                                yield return tmp;
                                yield return i;
                                count++;
                            }
                        }
                    }
                }

                // TODO
                // count >= 2
                else
                {
                    // 左が異なる
                    if (node1[i] != node2[i])
                    {
                        // 両方異なる
                        if (node1[i - 1] != node2[i - 1])
                        {
                            // 右が０  (00, 11), (10, 01)
                            if (node1[i - 1] == 0)
                            {
                                // countが奇数
                                if ((count & 1) == 1)
                                {
                                    yield return i - 1;
                                    count++;
                                }
                                // countが偶数
                                else
                                {
                                    // 何もしないがcountは増える
                                    count++;
                                }
                            }
                            // 右が１  (01, 10), (11, 00)
                            else
                            {
                                // 何もしないがcountは増える
                                count++;
                            }
                        }
                        // 左だけ異なる
                        else
                        {
                            // 右が０  (00, 10), (10, 00)
                            if (node1[i - 1] == 0)
                            {
                                yield return i;
                                count++;
                            }
                            // 右が１  (01, 11), (11, 01)
                            else
                            {
                                // countが奇数
                                if ((count & 1) == 1)
                                {
                                    // 何もしないしcountは増えない
                                }
                                // countが偶数
                                else
                                {
                                    yield return i;
                                    count++;
                                }
                            }
                        }
                    }
                    // 左が同じ
                    else
                    {
                        // TODO
                        // 右だけ異なる
                        if (node1[i - 1] != node2[i - 1])
                        {
                            // 右が０  (00, 01), (10, 11)
                            if (node1[i - 1] == 0)
                            {
                                // countが奇数
                                if ((count & 1) == 1)
                                {
                                    // 何もしないがcountは増える
                                    count++;
                                }
                                // countが偶数
                                else
                                {
                                    yield return i - 1;
                                    count++;
                                }
                            }
                            // 右が１  (01, 00), (11, 10)
                            else
                            {
                                yield return i - 1;
                                count++;
                            }
                        }
                        // 両方同じ
                        else
                        {
                            // 右が０  (00, 00), (10, 10)
                            if (node1[i - 1] == 0)
                            {
                                // 何もしないしcountは増えない
                            }
                            // 右が１  (01, 01), (11, 11)
                            else
                            {
                                // countが奇数
                                if ((count & 1) == 1)
                                {
                                    yield return i;
                                    count++;
                                }
                                // countが偶数
                                else
                                {
                                    // 何もしないしcountは増えない
                                }
                            }
                        }
                    }
                }
            }

            if (count == 1)
            {
                yield return tmp;
            }
        }

        public IEnumerable<int> GetFowardNeighbor2(BinaryNode node1, BinaryNode node2)
        {
            UInt32 u = node1.Addr, v = node2.Addr, diff = u ^ v;

            // 出発頂点と目的頂点が同じなら前方は存在しない
            if (u == v) yield break;

            int count = 0;      // 現在見つかっている枝の数
            //List<int> tmp = new List<int>();
            int tmp = -1;

            for (int i = Dimension | 1; i > 0; i -= 2)
            {
                // どちらも同じなら何もしない
                
                if (node1[i] != node2[i] && node1[i - 1] == node2[i - 1])
                {
                    // 左だけ異なる、右が1	(01, 11), (11, 01)
                    if (node1[i - 1] == 1)
                    {
                        if (count == 0)
                        {
                            tmp = i;
                            count++;
                        }
                        else
                        {
                            count++;
                        }
                    }
                    // 左だけ異なる，右が0	(00, 10), (10, 00)
                    else
                    {
                        if (count == 0)
                        {
                            tmp = i;
                            count++;
                        }
                        else
                        {
                            if (tmp != -1)
                            {
                                yield return tmp;
                                tmp = -1;
                            }
                            
                            yield return i;
                            count++;
                        }
                    }
                    
                }
                else if (node1[i] == node2[i] && node1[i - 1] != node2[i - 1])
                {
                    // 右だけ異なる，右が１	(01, 00), (11, 10)
                    if (node1[i - 1] == 1)
                    {
                        if (count == 0)
                        {
                            tmp = i - 1;
                            count++;
                        }
                        else if (count == 1)
                        {
                            tmp = i - 1;
                            count++;
                        }
                        else
                        {
                            count++;
                        }
                    }
                    // 右だけ異なる，右が０	(00, 10), (10, 00)
                    else
                    {
                        if (count == 0)
                        {
                            tmp = i - 1;
                            count++;
                        }
                        else
                        {
                            count++;
                        }
                    }
                }
                else if (node1[i] != node2[i] && node1[i - 1] != node2[i - 1])
                {
                    // 両方異なる，右が１	(01, 10), (11, 00)
                    if (node1[i - 1] == 1)
                    {
                        if (count == 0)
                        {
                            yield return i;
                            yield return i - 1;
                            count += 2;
                        }
                        else
                        {
                            count++;
                        }
                    }
                    // 両方異なる，右が０	(00, 11), (10, 01)
                    else
                    {
                        if (count == 0)
                        {
                            yield return i;
                            yield return i - 1;
                            count += 2;
                        }
                        else if (count == 1)
                        {
                            yield return i - 1;
                            tmp = -1;
                            count++;
                        }
                        else
                        {
                            count++;
                        }
                    }
                }
            }

        }





        private bool[,,] CalcCapability()
        {
            bool[,,] capability = new bool[NodeNum, Dimension, Dimension];

            for (UInt32 nodeID = 0; nodeID < NodeNum; nodeID++)
            {
                for (int i = 0; i < Dimension; i++)
                {
                    capability[nodeID, i, 0] = !FaultFlags[nodeID];
                }
            }

            for (int d = 1; d < Dimension; d++)
            {
                for (int k = 0; k < Dimension; k++)
                {
                    for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
                    {
                        int min = 1000;
                        for (int i = 0; i < Dimension; i++)
                        {
                            int count = GetNeighbor(node).Count(n => capability[n.ID, i, d - 1]);
                            if (count < min) min = count;
                        }
                        capability[node.ID, k, d] = min > Dimension - (k + 1);
                    }
                }
            }
            return capability;
        }

        private double[,,] CalcProbability()
        {
            double[,,] probability = new double[NodeNum, Dimension, Dimension];

            // k = 1のときは故障頂点の割合
            for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
            {
                double prob = (double)(GetNeighbor(node).Count(n => FaultFlags[n.ID])) / Dimension;
                for (int k = 0; k < Dimension; k++)
                    probability[node.ID, k, 0] = prob;
            }

            // k >= 2のとき
            for (int d = 1; d < Dimension; d++)
            {
                for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
                {
                    for (int k = 0; k < Dimension; k++)
                    {
                        probability[node.ID, k, d] = 1.0;
                        foreach (var neighbor in GetNeighbor(node).Where(n => !FaultFlags[n.ID]))
                        {
                            double ave = 0;
                            for (int i = 0; i < Dimension; i++)
                            {
                                ave += (1 - probability[neighbor.ID, i, d - 1]) * k / Dimension;
                            }
                            probability[node.ID, k, d] = 1 - ave / Dimension;
                        }
                    }
                }
            }

            return probability;
        }

        private Node GetNext(Node node1, Node node2, bool[,,] score)
        {
            int distance = CalcDistance(node1, node2);
            if (distance == 1) return node2;

            Node next = null;
            var fn = CalcForwardNeighbor(node1, node2);
            int fnCount = fn.Count();
            foreach (var n in fn)
            {
                if (score[n.ID, fnCount - 1, distance - 2])
                    return n;
                else if (next == null && !FaultFlags[n.ID])
                    next = n;
            }
            return next;
        }

        private Node GetNext(Node node1, Node node2, double[,,] score)
        {
            int distance = CalcDistance(node1, node2);
            if (distance == 1) return node2;

            Node minNode = null;
            double minScore = 2.0;
            var fn = CalcForwardNeighbor(node1, node2);
            int fnCount = fn.Count();
            foreach (var node in fn.Where(n => !FaultFlags[n.ID]))
            {
                if (score[node.ID, fnCount - 1, distance - 2] < minScore)
                {
                    minNode = node;
                    minScore = score[node.ID, fnCount - 1, distance - 2];
                }
            }
            return minNode;
        }

        public int Routing_Capability(Node node1, Node node2)
        {
            bool[,,] capability = CalcCapability();
            return RoutingBase(node1, node2, GetNext, capability);
        }

        public int Routing_Probability(Node node1, Node node2)
        {
            var probability = CalcProbability();
            return RoutingBase(node1, node2, GetNext, probability);
        }
    }
}
