using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    class CrossedCube : AGraph
    {
        // Constructor
        public CrossedCube(int dim, int randSeed) : base(dim, randSeed) { }


        // Abstract members
        public override string Name
        {
            get { return "CrossedCube"; }
        }

        public override int GetDegree(uint Node)
        {
            return Dimension;
        }

        public override uint GetNeighbor(uint node, int index)
        {
            const UInt32 mask1 = 0x55555555;            // 01010101....01
            UInt32 mask2 = ((UInt32)1 << index) - 1;    // 00...0111...11
            UInt32 mask = ((node & mask1 & mask2) << 1) | ((UInt32)1 << index);
            return node ^ mask;
        }

        protected override uint CalcNodeNum()
        {
            return (uint)1 << Dimension;
        }

        public override int CalcDistance(uint node1, uint node2)
        {
            Binary u = new Binary(node1), v = new Binary(node2);
            int score = 0;
            for (int i = Dimension - (Dimension & 1); i >= 0; i -= 2) // iをdouble bit の右側に合わせる
            {
                if (score == 0)
                {
                    score = (int)((u[i + 1] ^ v[i + 1]) + (u[i] ^ v[i]));
                }
                else
                {
                    if (!(u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (score & 1) == 1)
                            || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1]))
                    {
                        score += 1;
                    }
                }
            }
            return score;
        }

        public int[] CalcRho(Binary u, Binary v)
        {
            int[] p = new int[(Dimension + 1) >> 1];
            int sum = 0;

            for (int i = Dimension - 2 + (Dimension & 1); i >= 0; i -= 2) // Set i to right bit index in leftmost bitpair
            {
                int pi = i >> 1;    // bitpair index
                if (sum == 0)
                {
                    p[pi] = (int)((u[i + 1] ^ v[i + 1]) + (u[i] ^ v[i]));
                    sum = p[pi];
                }
                else
                {
                    if (!(u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (sum & 1) == 1)
                            || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1]))
                    {
                        p[pi] = 1;
                        sum += 1;
                    }
                }
            }
            return p;
        }

        public override int[] CalcRelativeDistance(uint node1, uint node2)
        {
            Binary u = new Binary(node1), v = new Binary(node2);
            var r = new int[Dimension + 1];

            if (node1 == node2)
            {
                for (int i = 0; i < Dimension; i++) r[i] = 1;
                return r;
            }

            // 関数ローの計算 : O(n)
            var p = CalcRho(u, v);

            // 関数ローの累積和の計算 : O(n)
            var pSum = new int[Dimension];
            for (int i = (Dimension - 1) / 2 - 1; i >= 0; i--) pSum[i] = pSum[i + 1] + p[i + 1];

            // MSBの計算 : O(n)
            var k = Dimension;
            while (k-- > 0 && u[k] == v[k]);

            // 目印の計算 : O(n)
            var mark = new int[Dimension];
            for (int i = 0; i < Dimension; i += 2)
            {
                var pi = i >> 1;    // pair index
                if (u[i] == 1 && v[i] == 1)
                {
                    mark[pi + 1] = u[i + 1] == v[i + 1] ^ (pSum[pi] & 1) == 1 ? 1 : -1;
                }
                else
                {
                    mark[pi + 1] = mark[pi];
                }
            }

            // 目印2
            var mark2 = 0;
            for (int i = (k - 1) / 2; i >= 0; i -= 2)
            {
                if (u[i] == 1 && v[i] == 1)
                {
                    mark2 = u[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1 ? 1 : -1;
                    break;
                }
            }

            var flag_second = false;
            int mark_index = -1;

            // 右から順に計算していく
            for (int i = 0; i < Dimension; i += 2)
            {
                Binary n0 = new Binary(GetNeighbor(node1, i));
                Binary n1 = new Binary(GetNeighbor(node1, i + 1));
                
                if (k / 2 > i / 2)
                {
                    // 0のはずが1になる場合
                    if (u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                               || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1])
                    {
                        r[i] = 1;
                        r[i + 1] = 1;
                    }
                    // 1のはずが0になる場合
                    else if (n0[i] == 1 && v[i] == 1 && (n0[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                               || n0[i] == 0 && v[i] == 0 && n0[i + 1] == v[i + 1])
                    {
                        r[i] = -1;
                        r[i + 1] = mark[i >> 1];
                    }
                    else if (n1[i] == 1 && v[i] == 1 && (n1[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                              || n1[i] == 0 && v[i] == 0 && n1[i + 1] == v[i + 1])
                    {
                        r[i] = mark[i >> 1];
                        r[i + 1] = -1;
                    }
                    else
                    {
                        r[i] = mark[i >> 1];
                        r[i + 1] = r[i];
                    }
                }
                else if (k >> 1 == i >> 1)
                {
                    if (p[i / 2] == 1)
                    {
                        r[k] = flag_second ? mark[mark_index >> 1] : -1;
                        r[k ^ 1] = 1;
                    }
                    else
                    {
                        r[i] = -1;
                        r[i + 1] = -1;
                    }
                }
                else
                {
                    if (p[k >> 1] == 1)
                    {
                        r[i] = 1;
                        r[i + 1] = 1;
                    }
                    else
                    {
                        r[i] = mark[k >> 1];
                        r[i + 1] = mark[k >> 1];
                    }
                }

                // フラグの更新
                if (p[i / 2] > 0)
                {
                    flag_second = (v[i + 1] ^ v[i]) == u[i + 1];
                    mark_index = i;
                }
            }

            return r.Take(Dimension).ToArray();
        }

        public int[] R(uint node1, uint node2)
        {
            var u = new Binary(node1);  // 現在の頂点
            var v = new Binary(node2);  // 目的頂点
            var p = new int[(Dimension >> 1) + 1];    // 関数ロー
            var pSum = new int[Dimension];      // 関数ローの累積和
            int k = Dimension;  // u, vのMSB
            var r = new int[Dimension + 1]; // u, vの相対距離ベクトル
            var mark = new int[Dimension];  // 目印ベクトル

            if (node1 == node2)
            {
                for (int i = 0; i < Dimension; i++) r[i] = 1;
                return r;
            }

            // ローの計算
            {
                int sum = 0;

                for (int i = Dimension - 2 + (Dimension & 1); i >= 0; i -= 2) // iをdouble bit の右側に合わせる
                {
                    if (sum == 0)
                    {
                        p[i / 2] = (int)((u[i + 1] ^ v[i + 1]) + (u[i] ^ v[i]));
                        sum = p[i / 2];
                    }
                    else
                    {
                        if (!(u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (sum & 1) == 1)
                                || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1]))
                        {
                            p[i / 2] = 1;
                            sum += 1;
                        }
                    }
                }
            }
            // ローの累積和の計算
            {

                for (int i = (Dimension - 1) / 2 - 1; i >= 0; i--)
                {
                    pSum[i] = pSum[i + 1] + p[i + 1];
                }
            }
            // kの計算
            while (k-- > 0 && u[k] == v[k]) ;
            
            var flag_second = false;
            int mark_index = -1;

            for (int i = 0; i < Dimension; i += 2)
            {
                Binary n0 = new Binary(GetNeighbor(node1, i));
                Binary n1 = new Binary(GetNeighbor(node1, i + 1));

                // 目印の更新
                if (u[i] == 1 && v[i] == 1)
                {
                    mark[(i >> 1) + 1] = u[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1 ? 1 : -1;
                }
                else
                {
                    mark[(i >> 1) + 1] = mark[i >> 1];
                }

                if (k / 2 > i / 2)
                {
                    // 0のはずが1になる場合
                    if (u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                               || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1])
                    {
                        r[i] = 1;
                        r[i + 1] = 1;
                    }
                    // 1のはずが0になる場合
                    else if (n0[i] == 1 && v[i] == 1 && (n0[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                               || n0[i] == 0 && v[i] == 0 && n0[i + 1] == v[i + 1])
                    {
                        r[i] = -1;
                        r[i + 1] = mark[i >> 1];
                    }
                    else if (n1[i] == 1 && v[i] == 1 && (n1[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                              || n1[i] == 0 && v[i] == 0 && n1[i + 1] == v[i + 1])
                    {
                        r[i] = mark[i >> 1];
                        r[i + 1] = -1;
                    }
                    else
                    {
                        r[i] = mark[i >> 1];
                        r[i + 1] = r[i];
                    }
                }
                else if (k >> 1 == i >> 1)
                {
                    if (p[i / 2] == 1)
                    {
                        r[k] = flag_second ? mark[mark_index >> 1] : -1;
                        r[k ^ 1] = 1;
                    }
                    else
                    {
                        r[i] = -1;
                        r[i + 1] = -1;
                    }
                }
                else
                {
                    if (p[k >> 1] == 1)
                    {
                        r[i] = 1;
                        r[i + 1] = 1;
                    }
                    else
                    {
                        r[i] = mark[k >> 1];
                        r[i + 1] = mark[k >> 1];
                    }
                }

                // フラグの更新
                if (p[i / 2] > 0)
                {
                    flag_second = (v[i + 1] ^ v[i]) == u[i + 1];
                    mark_index = i;
                }
            }
            return r;
        }

        public int[] Rho(uint node1, uint node2)
        {
            Binary u = new Binary(node1), v = new Binary(node2);
            int[] p = new int[(Dimension + 1) >> 1];
            int sum = 0;

            for (int i = Dimension - 2 + (Dimension & 1); i >= 0; i -= 2) // Set i to right bit index in leftmost bitpair
            {
                int pi = i >> 1;    // bitpair index
                if (sum == 0)
                {
                    p[pi] = (int)((u[i + 1] ^ v[i + 1]) + (u[i] ^ v[i]));
                    sum = p[pi];
                }
                else
                {
                    if (!(u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (sum & 1) == 1)
                            || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1]))
                    {
                        p[pi] = 1;
                        sum += 1;
                    }
                }
            }
            return p;
        }
        

        public int ChangRouting(uint node1, uint node2, int timeoutLimit, bool detour)
        {
            var current = node1;
            var preview = node1;
            var step = 0;

            while (current != node2)
            {
                // タイムアウト判定
                if (step >= timeoutLimit) return -2;

                // 一個前方を見つける
                var rel = CalcRelativeDistance(current, node2);
                uint next = current;
                for (int i = 0; i < Dimension; i++)
                {
                    if (rel[i] == -1)
                    {
                        next = GetNeighbor(current, i);
                        break;
                    }
                }

                // 故障していた場合
                if (FaultFlags[next])
                {
                    // 迂回ありのとき
                    if (detour)
                    {
                        try
                        {
                            var sub = GetNeighbor(current)
                                        .First(x => !FaultFlags[x] && x != preview);
                            preview = current;
                            current = sub;
                        }
                        catch
                        {
                            // 非故障かつ後退しない頂点が見つからない場合は失敗
                            return -1;
                        }
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

        // 提案手法(仮)
        // para
        //      timeoutLimit : タイムアウトとみなすステップ数
        //      detour       : 迂回するか否か
        // ret
        //      候補がない   : -1
        //      タイムアウト : -2
        public int ProposedRouting(uint node1, uint node2, int timeoutLimit, bool detour)
        {
            var current = node1;
            var preview = node1;
            var step = 0;

            while (current != node2)
            {
                // タイムアウト判定
                if (step++ >= timeoutLimit) return -2;

                // 相対距離を計算
                var rel = CalcRelativeDistance(current, node2);

                // 候補頂点を探す
                uint forward = current, side = current, backward = current;
                for (int i = 0; i < Dimension; i++)
                {
                    var neighbor = GetNeighbor(current, i);
                    if (!FaultFlags[neighbor])
                    {
                        if (rel[i] == -1)
                        {
                            forward = neighbor;
                            break;
                        }
                        else if (rel[i] == 0)
                        {
                            side = neighbor;
                        }
                        else if (neighbor != preview)
                        {
                            backward = neighbor;
                        }
                    }
                }

                preview = current;

                // 前方が見つかっていれば前方へ
                if (forward != current)
                {
                    current = forward;
                }
                // 横方が見つかっていれば横方へ
                else if (side != current)
                {
                    current = side;
                }
                // 後方が見つかっていれば後方へ
                else if (backward != current)
                {
                    current = backward;
                }
                // 候補が全くなければ失敗
                else
                {
                    return -1;
                }
            }
            return step;
        }
    }
}
