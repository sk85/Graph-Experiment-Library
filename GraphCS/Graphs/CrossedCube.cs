using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using GraphCS.Core;

namespace GraphCS.Graphs
{
    partial class CrossedCube : AGraph<BinaryNode>
    {
        public CrossedCube(int dim) : base(dim) { }

        public override string Name => "CrossedCube";

        public override int GetDegree(BinaryNode node) => Dimension;

        public override BinaryNode GetNeighbor(BinaryNode node, int i)
        {
            int mask = ((node.Addr & 0x55555555 & ((1 << i) - 1)) << 1) | (1 << i);
            return node ^ mask;
        }

        protected override int CalcNodeNum() => 1 << Dimension;

        public override int CalcDistance(BinaryNode node1, BinaryNode node2)
        {
            int score = 0;
            for (int i = Dimension - (Dimension & 1); i >= 0; i -= 2)
            {
                if (score == 0)
                {
                    score = (node1[i + 1] ^ node2[i + 1]) + (node1[i] ^ node2[i]);
                }
                else
                {
                    bool A = node1[i] == 1, B = node2[i] == 1, C = node1[i + 1] == node2[i + 1];
                    if (!(A && B && (C ^ (score & 1) == 1) || !A && !B && C))
                    {
                        score += 1;
                    }
                }
            }
            return score;
        }

        public override int[] CalcRelativeDistance(BinaryNode current, BinaryNode destination)
        {
            var r = new int[Dimension + 1]; // 相対距離

            // distance-preserving pair related
            Func<int, int, int, int, int, bool> DPPR = (u1, u2, v1, v2, sum) =>
                (u1 == 1 && v1 == 1 && (u2 == v2 ^ (sum & 1) == 1)) || u1 == 0 && v1 == 0 && u2 == v2;

            // 変数名が長いと見づらいので
            BinaryNode u = current, v = destination;

            // 同一頂点ならすべて後方
            if (u == v)
            {
                for (int i = 0; i < Dimension; i++) r[i] = 1;
                return r;
            }

            // Score、Scoreの累積和、MSBを計算
            var score = new int[Dimension];     // 論文ではρ
            var cumsum = new int[Dimension];    // scoreの左方向からの累積和
            int mspi = 0;    // 異なるペアのうち一番大きな添字
            for (int i = Dimension - 2 + (Dimension & 1); i >= 0; i -= 2)   // iはビットペアの右側(偶ビット)
            {
                int pi = i >> 1;    // ペアのインデックス
                cumsum[pi] = cumsum[pi + 1] + score[pi + 1];
                if (cumsum[pi] == 0)
                {
                    score[pi] = (u[i + 1] ^ v[i + 1]) + (u[i] ^ v[i]);
                    if (score[pi] != 0) mspi = pi;
                }
                else if (!DPPR(u[i], u[i + 1], v[i], v[i + 1], cumsum[pi]))
                {
                    score[pi] = 1;
                }
            }

            int f1 = 0, f2 = -1;
            for (int i = 0; i < Dimension; i += 2)  // iはビットペアの右側(偶ビット)
            {
                int pi = i >> 1;

                if (pi < mspi)
                {
                    // 0のはずが1になる場合
                    if (DPPR(u[i], u[i + 1], v[i], v[i + 1], cumsum[pi]))
                    {
                        r[i] = r[i + 1] = 1;
                    }
                    // 1のはずが0になる場合1
                    else if (DPPR(u[i] ^ 1, u[i + 1], v[i], v[i + 1], cumsum[pi]))
                    {
                        r[i] = -1;
                        r[i + 1] = f1;
                    }
                    // 1のはずが0になる場合2
                    else if (DPPR(u[i], u[i + 1] ^ 1, v[i], v[i + 1], cumsum[pi]))
                    {
                        r[i] = f1;
                        r[i + 1] = -1;
                    }
                    else
                    {
                        r[i] = r[i + 1] = f1;
                    }

                    // フラグの更新
                    if (u[i] == 1 && v[i] == 1) // 直近で損得が起きるとこ
                    {
                        f1 = u[i + 1] == v[i + 1] ^ (cumsum[pi] & 1) == 1 ? 1 : -1;
                    }
                    if (score[pi] > 0)  // mspが右にずれたときの次
                    {
                        f2 = (v[i + 1] ^ v[i]) == u[i + 1] ? f1 : -1;
                    }
                }
                else if (pi == mspi)
                {
                    if (score[pi] == 1) // 両方とも違わない場合は、mspiがずれるので
                    {
                        var kk = i + (u[i] ^ v[i]);
                        r[kk ^ 1] = f2;
                        r[kk] = 1;
                    }
                    else
                    {
                        r[i] = r[i + 1] = -1;
                    }
                }
                else
                {
                    r[i] = r[i + 1] = score[mspi] == 1 ? 1 : f1;
                }
            }
            return r.Take(Dimension).ToArray();
        }

        /// <summary>
        /// EfeのRoute。
        /// </summary>
        /// <param name="u">Source node</param>
        /// <param name="v">Destination node</param>
        /// <param name="n1">Forward neighbor</param>
        /// <param name="n2">Forward neighbor(or null)</param>
        public void Efe_GetNext(BinaryNode u, BinaryNode v, out BinaryNode n1, out BinaryNode n2)
        {
            // Leftmost Different Bit Index
            int l = Dimension - 1;
            while (u[l] == v[l]) l--;

            if ((l & 1) == 1 && u[l - 1] != v[l - 1])
            {
                n1 = GetNeighbor(u, l);
                n2 = GetNeighbor(u, l - 1);
            }
            else
            {
                for (int k = l - 1 - (l & 1); k > 0; k -= 2)
                {
                    // 上がずれる
                    if ((u[k] ^ u[k - 1]) != v[k])
                    {
                        // 下もずれる
                        if (u[k - 1] != v[k - 1])
                        {
                            // {0}のパターン
                            n1 = GetNeighbor(u, k - 1);
                            n2 = null;
                        }
                        else
                        {
                            // {1, 2}パターン
                            n1 = GetNeighbor(u, k);
                            n2 = GetNeighbor(u, l);
                        }
                        return;
                    }
                    else
                    {
                        // 下だけずれる
                        if (u[k - 1] != v[k - 1])
                        {
                            // {2}のパターン
                            n1 = GetNeighbor(u, l);
                            n2 = null;
                            return;
                        }
                    }
                }
                n1 = GetNeighbor(u, l);
                n2 = null;
            }
        }

        /// <summary>
        /// Efeのルーティング。
        /// 迂回しない。
        /// </summary>
        /// <returns>ステップ数(失敗:-1)</returns>
        public int Efe_Routing_NoDetour(BinaryNode node1, BinaryNode node2, bool[] FaultFlags)
        {
            var current = new BinaryNode(node1);
            int step = 0;

            while (current != node2)
            {
                step++;
                Efe_GetNext(current, node2, out var n1, out var n2);
                if (!FaultFlags[n1.Addr])
                {
                    current = n1;
                }
                else if (n2 != null && !FaultFlags[n2.Addr])
                {
                    current = n2;
                }
                else
                {
                    return -1;
                }
            }
            return step;
        }

        /// <summary>
        /// Efeのルーティング。
        /// </summary>
        /// <returns>ステップ数(タイムアウト:-2、失敗:-1)</returns>
        public int Efe_Routing(BinaryNode node1, BinaryNode node2, bool[] FaultFlags, int timeoutLimit)
        {
            BinaryNode prev = null;
            var rand = new Random(0);
            var current = new BinaryNode(node1);
            int step = 0;

            while (current != node2)
            {
                if (++step > timeoutLimit) return -2;

                Efe_GetNext(current, node2, out var n1, out var n2);

                if (!FaultFlags[n1.Addr])
                {
                    prev = current;
                    current = n1;
                }
                else if (n2 != null && !FaultFlags[n2.Addr])
                {
                    prev = current;
                    current = n2;
                }
                else
                {
                    var q = GetNeighbor(current).Where(x => x != n1 && x != n2 && x != prev && !FaultFlags[x.Addr]);
                    var count = q.Count();
                    if (count > 0)
                    {
                        prev = current;
                        current = q.ElementAt(rand.Next(count));
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            return step;
        }
    }
}
