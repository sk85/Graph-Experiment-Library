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

        public override int CalcDistance(Node node1, Node node2)
        {
            BinaryNode u = new BinaryNode(node1.ID), v = new BinaryNode(node2.ID);

            if (node1.ID == node2.ID) return 0;
            
            int score;
            int i = Dimension - 1;

            while (i >= 0 && u[i] == v[i]) { i--; }  // MSBを探す

            i -= i & 1; // double bitの右側に合わせる

            // j = i^* のとき
            score = (u[i + 1] ^ v[i + 1]) + (u[i] ^ v[i]);

            i -= 2;

            // j < i^* のとき
            while (i >= 0)
            {
                if (!((u[i + 1] == v[i + 1] && u[i] == 1 && v[i] == 1 && (score & 1) == 0) ||
                      (u[i + 1] != v[i + 1] && u[i] == 1 && v[i] == 1 && (score & 1) == 1) ||
                      (u[i + 1] == v[i + 1] && u[i] == 0 && v[i] == 0)))
                {
                    score += 1;
                }
                i -= 2;
            }
            return score;
        }


        int[] CalcRhoSum(BinaryNode node1, BinaryNode node2)
        {
            int[] _rhoSum = new int[Dimension >> 1];
            int i = Dimension - 1;

            while (i >= 0 && node1[i] == node2[i]) { i--; }  // MSBを探す
            i -= i & 1; // double bitの右側に合わせる

            // j = k のとき
            _rhoSum[i >> 1] = (node1[i + 1] ^ node2[i + 1]) + (node1[i] ^ node2[i]);

            i -= 2;

            // j < k のとき
            while (i >= 0)
            {
                bool f = node1[i] == 1 && node2[i] == 1
                        && !(node1[i + 1] == node2[i + 1] ^ (_rhoSum[(i >> 1) + 1] & 1) == 0)
                        || node1[i + 1] == node2[i + 1]
                        && node1[i] == 0 && node2[i] == 0;
                if (!f)
                {
                    _rhoSum[i >> 1] = _rhoSum[(i >> 1) + 1] + 1;
                }
                else
                {
                    _rhoSum[i >> 1] = _rhoSum[(i >> 1) + 1];
                }
                i -= 2;
            }
            return _rhoSum;
        }

        public void a(BinaryNode node1, BinaryNode node2)
        {
            int[] d = new int[Dimension];   // d[i] = d(node1, node2) - d(第i隣接頂点, node2)
            int MSP;    // Most Significant Pair
            int[] type = new int[Dimension - 1];

            // MSPの計算
            MSP = Dimension - 1;
            while (MSP >= 0 && node1[MSP] == node2[MSP])
            {
                MSP--;
            }
            MSP = (MSP >> 1) << 1;

            // 前処理
            for (int i = 0; i < Dimension; ++i)
            {
                // (01,11),(11,01)かつ奇数・・・タイプ0
                // (01,01),(11,11)かつ偶数・・・タイプ1
                if ((i >> 1) > MSP)
                    type[i] = -1;
            }

            bool flag = node1[MSP << 1] != node2[MSP << 1] && node1[(MSP << 1) + 1] != node2[(MSP << 1) + 1];
            for (int i = Dimension - 1; i >= 0; --i)
            {
                // MSBより上位のビットのとき
                if ((i >> 1) > MSP)
                {
                    if (flag)
                    {
                        d[i]--;
                    }
                }
                // MSP以下のビットのとき
                else
                {
                    if (node1[i] != node2[i])
                    {
                        d[i]--;
                    }
                }



            }
        }

        public int CalcFowardNeighbor(BinaryNode node1, BinaryNode node2)
        {

            if (node1.ID == node2.ID) return 0;

            int k;    // Most left different pair index 
            int[] d = new int[Dimension];   // d[i] = d(u^i, v) - d(u, v)
            int[] r = new int[Dimension >> 1];  // r[i] = 第iペアの状態
                                                // １：(01,11)(11,01)かつ奇数 OR (01,01)(11,11)かつ偶数
                                                // ２：(01,11)(11,01)かつ偶数 OR (01,01)(11,11)かつ奇数
                                                // ０：その他

            // Calculate k
            k = Dimension - 1;
            while (k >= 0 && node1[k] == node2[k]) k--;
            k = k >> 1;

            // Calculate r
            int sum = (node1[(k << 1) + 1] ^ node2[(k << 1) + 1]) + (node1[k << 1] ^ node2[k << 1]);
            for (int i = k - 1; i >= 0; i--)
            {
                if (node1[(i << 1)] == 1 && node2[(i << 1)] == 1)
                {
                    if (node1[(i << 1) + 1] == node2[(i << 1) + 1])
                    {
                        if ((sum & 1) == 0)
                        {
                            r[i] = 1;
                        }
                        else
                        {
                            r[i] = 2;
                            sum++;
                        }
                    }
                    else
                    {
                        if ((sum & 1) == 1)
                        {
                            r[i] = 1;
                        }
                        else
                        {
                            r[i] = 2;
                            sum++;
                        }
                    }
                }
                else if (!(node1[(i << 1)] == 0 && node2[(i << 1)] == 0 && node1[(i << 1) + 1] == node2[(i << 1) + 1]))
                {
                    sum++;
                }
            }

            if (node1.ID == 1 && node2.ID == 51)
            {
                Console.WriteLine(node1.ToString(2));
                Console.WriteLine(node2.ToString(2));
                Console.Write("{0,33}", ":");
                for (int i = (Dimension >> 1) - 1; i >= 0; i--)
                {
                    Console.Write(" {0} ", r[i]);
                }
                Console.Write("\n");
            }

            for (int i = 1; i < k; i++)
            {
                if (r[i] == 0) r[i] = r[i - 1];
            }

            if (node1.ID == 1 && node2.ID == 51)
            {
                Console.WriteLine(node1.ToString(2));
                Console.WriteLine(node2.ToString(2));
                Console.Write("{0,33}", ":");
                for (int i = (Dimension >> 1) - 1; i >= 0; i--)
                {
                    Console.Write(" {0} ", r[i]);
                }
                Console.Write("\n");
            }

            // Main part
            bool f = node1[(k << 1) + 1] != node2[(k << 1) + 1] && node1[k << 1] != node2[k << 1];
            for (int i = Dimension - 1; i >= 0; i--)
            {
                if ((i >> 1) > k)
                {
                    // kペアより左の変化量
                    d[i] = 1;
                    // kペアのrhoの変化量
                    if (f) d[i]--;
                    // kより右のrhoの変化量
                    if (k > 0 && r[k - 1] == 2) d[i]--;
                }
                else if ((i >> 1) == k)
                {
                    // kペアが両方異なる場合
                    if (f)
                    {
                        // kより右は変わらず，kが1つ減る
                        d[i] = -1;
                    }
                    // 片方だけ異なる場合
                    else
                    {
                        // 第iの方が異なる場合
                        if (node1[i] != node2[i])
                        {
                            // kペアが00になるので，kが右にずれる
                            int j = (k << 1) - 1;
                            while (j >= 0 && node1[j] == node2[j]) j--; // 次に異なるビットを探す
                            if (j >= 0)
                            {
                                if (node1.ID == 4 && node2.ID == 23)
                                {
                                    sum = 0;
                                }
                                j ^= j & 1;
                                // 変化が起こる場合
                                // ２．1手で済むところが2手に : (00, 11), (10, 01), (01, 00), (11, 10)
                                if (node1[j] == 0 && node2[j] == 1 && node1[j + 1] != node2[j + 1] ||
                                    node1[j] == 1 && node2[j] == 0 && node1[j + 1] == node2[j + 1])
                                {
                                    if (j > 1 && r[(j >> 1) - 1] == 2)
                                    {
                                        d[i] = -1;
                                    }
                                    else
                                    {
                                        d[i] = 0;
                                    }
                                }
                                else
                                {
                                    d[i] = -1;
                                }
                            }
                            else
                            {
                                d[i] = -1;
                            }
                        }
                        // 逆が異なる場合
                        else
                        {
                            // kペアが11になる
                            // kより右は変わらなず，kが一つ増える
                            d[i] = 1;
                        }
                    }
                }
            }

            int bin = 0;
            for (int i = 0; i < Dimension; i++)
            {
                if (d[i] == -1)
                    bin |= 1 << i;
            }

            return bin;
        }


        public int test()
        {
            for (BinaryNode node1 = new BinaryNode(0); node1.ID < NodeNum; node1.ID++)
            {
                for (BinaryNode node2 = new BinaryNode(0); node2.ID < NodeNum; node2.ID++)
                {
                    // 距離
                    int distance = CalcDistance(node1, node2);
                    
                    Console.WriteLine("d({0}, {1}) = {2}", node1.ID, node2.ID, distance);

                    // MSP
                    int k = Dimension - 1;
                    while (k >= 0 && node1[k] == node2[k]) k--;
                    k = k >> 1;

                    // 最左ビットからMSBまでループ
                    int bin = 0;
                    for (int i = Dimension - 1; (i >> 1) >= k; i--)
                    {
                        BinaryNode neighbor = new BinaryNode(GetNeighbor(node1, i).ID);
                        if (CalcDistance(neighbor, node2) < distance)
                        {
                            bin |= 1 << i;
                        }
                    }

                    int tmp = CalcFowardNeighbor(node1, node2);
                    if (bin != tmp)
                    {
                        Console.WriteLine("------------------------------");
                        Console.WriteLine("  u = {0,5} = {1}", node1.ID, node1.ToString(2));
                        Console.WriteLine("  v = {0,5} = {1}", node2.ID, node2.ToString(2));
                        Console.WriteLine(" u^v=       = {1}\n", node1.ID ^ node2.ID, (node1 ^ node2).ToString(2));
                        Console.WriteLine(" 実 =       = {0}", Experiment.Tools.UIntToBinStr((uint)bin, 32, 2));
                        Console.WriteLine(" 誤 =       = {0}", Experiment.Tools.UIntToBinStr((uint)tmp, 32, 2));
                        Console.ReadKey();
                    }
                    
                }
            }
            return 0;
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
