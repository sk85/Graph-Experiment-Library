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
    class CrossedCube : AGraph
    {
        /// <summary>
        /// AGraphのコンストラクタを呼びます。
        /// <para>現状ではRandomオブジェクトの初期化とDimensionフィールドの初期化だけ。</para>
        /// </summary>
        /// <param name="dim">次元数</param>
        public CrossedCube(int dim) : base(dim) { }

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
        public override int GetDegree(INode node)
        {
            return Dimension;
        }

        /// <summary>
        /// nodeの第indexエッジと接続する隣接ノードを返します。
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <param name="index">エッジの番号</param>
        /// <returns>隣接ノードのアドレス</returns>
        public override INode GetNeighbor(INode node, int index)
        {
            BinaryNode binNode = (BinaryNode)node;
            const UInt32 mask1 = 0x55555555;            // 01010101....01
            UInt32 mask2 = ((UInt32)1 << index) - 1;    // 00...0111...11
            UInt32 mask = ((binNode.Addr & mask1 & mask2) << 1) | ((UInt32)1 << index);
            return binNode ^ mask;
        }

        public IEnumerable<int> GetFowardNeighbor2(BinaryNode node1, BinaryNode node2)
        {
            UInt32 u = node1.Addr, v = node2.Addr, diff = u ^ v;

            // 出発頂点と目的頂点が同じなら前方は存在しない
            if (u == v) yield break;

            int count = 0;  // 現在見つかっている前方隣接頂点の数
            int tmp = -1;   // 前方かわからない隣接頂点のindex
            bool flag = true;   // まだ一度も揃えるべきビットペアに行き当たっていない

            // iは現在見ているビットペアの左側のindex
            for (int i = (Dimension - 1) | 1; i > 0; i -= 2)
            {
                UInt32 pair = (diff >> (i - 1)) & 0b11;
                // 揃える必要のないビットペアならスキップ
                if (pair == 0) continue;

                // 揃えるべき最左のビットペアのとき
                if (flag)
                {
                    // ペアの両方を揃えるとき
                    if (pair == 0b11)
                    {
                        yield return i;
                        yield return i - 1;
                        count += 2;
                    }
                    // ペアのどちらか片方のみを揃えるとき
                    else
                    {
                        tmp = pair == 0b10 ? i : i - 1;
                        count++;
                    }
                    flag = false;
                }

                // まだ前方隣接頂点が1つも見つかっていない
                else if ((count & 1) == 0)
                {
                    // ペアを両方揃えたい場合
                    if (pair == 0b11)
                    {
                        // a1 / A0　：　上だけやる
                        if (node1[i - 1] == 1)
                        {
                            yield return tmp;
                        }
                        // a0 / A1　：　右だけやる
                        else
                        {
                            // 上に丁度2個でおしまいのときこまる
                            tmp = i - 1;
                        }
                        count++;
                    }

                    // ペアの左だけ揃えたい場合
                    else if (pair == 0b10)
                    {
                        // a1 / A1　：　上に一個しかないのでできない。上は未確定
                        // a0 / A0　：　上もここもできる
                        if (node1[i - 1] == 0)
                        {
                            yield return tmp;
                            yield return i;
                            count += 2;
                        }
                    }

                    // ペアの右だけ揃えたい場合
                    else
                    {
                        // a0 / a1　：　上に一個しかないのでできない。上は未確定
                        // a1 / a0　：　上に一個しかないので先にやるが、下次第
                        if (node1[i - 1] == 1)
                        {
                            tmp = i - 1;
                        }
                    }
                }
            }
        }


        public IEnumerable<int> GetFowardNeighbor(BinaryNode node1, BinaryNode node2)
        {
            UInt32 u = node1.Addr, v = node2.Addr, diff = u ^ v;

            // 出発頂点と目的頂点が同じなら前方は存在しない
            if (u == v) yield break;

            int count = 0;      // 現在見つかっている枝の数
            List<int> tmp = new List<int>();

            for (int i = Dimension | 1; i > 0; i -= 2)
            {
                // Case1
                if (node1[i] == node2[i] && node1[i - 1] == node2[i - 1])
                {
                    // 確定
                    // u^iもu^(i-1)も含まれない
                }
                // Case2
                else if (node1[i] == node2[i] && node1[i - 1] == 0 && node2[i - 1] == 1)
                {
                    // 確定
                    // u^iは含まれない

                    // 確認中
                    // iより大きいのが偶数ならばu^(i-1)を含む
                    if ((count & 1) == 0)
                    {
                        yield return i - 1;
                    }
                    // 第(i-1)辺はいずれ現れるのでカウントを進める
                    count++;
                }
                // Case3
                else if (node1[i] == node2[i] && node1[i - 1] == 1 && node2[i - 1] == 0)
                {
                    // 確定
                    // u^iは含まれない

                    // 確認中
                    // u^(i-1)を含む
                    if (count == 0)
                    {
                        tmp.Add(i - 1);
                    }
                    else if (count == 1)
                    {
                        tmp.Clear();
                        yield return i - 1;
                    }
                    else
                    {
                        foreach (int j in tmp) yield return j;
                        yield return i - 1;
                    }
                    
                    // 第(i-1)辺はいずれ現れるのでカウントを進める
                    count++;
                }
                // Case4
                else if (node1[i] == 0 && node2[i] == 1 && node1[i - 1] == node2[i - 1])
                {
                    // 確定
                    // u^(i-1)は含まれない

                    // 確認中
                    // u^iを含む
                    tmp.Add(i);

                    // 第(i-1)辺はいずれ現れるのでカウントを進める
                    count++;
                }
                // Case5
                else if (node1[i] == 1 && node2[i] == 0 && node1[i - 1] == node2[i - 1])
                {
                    // 確定
                    // u^(i-1)は含まれない

                    // 確認中
                    // iより大きいのが偶数ならばu^iを含む
                    if ((count & 1) == 0)
                    {
                        yield return i;
                    }
                }
                // Case6
                else if (node1[i] != node2[i] && node1[i - 1] == 0 && node2[i - 1] == 1)
                {
                    // iより大きいのが存在しなければu^iを含む
                    if (count == 0)
                    {
                        yield return i;
                        count++;
                    }
                    // iより大きいのが奇数ならu^(i-1)を含む
                    if ((count & 1) == 1)
                    {
                        yield return i - 1;
                    }
                    // 第(i-1)辺はいずれ現れるのでカウントを進める
                    count++;
                }
                // Case7
                else if (node1[i] != node2[i] && node1[i - 1] == 1 && node2[i - 1] == 0)
                {
                    // iより大きいのが存在しなければu^iもu^(i-1)も含む
                    if (count == 0)
                    {
                        yield return i - 1;
                        yield return i;
                        count += 2; ;
                    }
                }
            }

        }

        public void Check_GetFowardNeighbor()
        {
            for (UInt32 node2ID = 0; node2ID < NodeNum; node2ID++)
            {
                BinaryNode node2 = new BinaryNode(node2ID);
                int[] distance = CalcAllDistanceBFS(node2);

                for (UInt32 node1ID = 0; node1ID < NodeNum; node1ID++)
                {
                    BinaryNode node1 = new BinaryNode(node1ID);

                    Console.WriteLine("d({0}, {1}) = {2}", node1ID, node2ID, distance[node1ID]);
                    Console.WriteLine("  u   = {0}", Debug.Tools.UIntToBinStr(node1.Addr, Dimension, 2));
                    Console.WriteLine("  v   = {0}", Debug.Tools.UIntToBinStr(node2.Addr, Dimension, 2));
                    Console.WriteLine("s ^ d = {0}\n", Debug.Tools.UIntToBinStr((node1 ^ node2).Addr, Dimension, 2));

                    var FN = GetFowardNeighbor(node1, node2);

                    bool flag = false;
                    foreach (var neighborIndex in FN)
                    {
                        UInt32 neighborID = ((BinaryNode)GetNeighbor(node1, neighborIndex)).Addr;
                        if (distance[neighborID] >= distance[node1ID])
                        {
                            Console.Write(" u^{0}  = {1}", neighborIndex, Debug.Tools.UIntToBinStr(neighborID, Dimension, 2));
                            Console.WriteLine("\td({0}, {1}) = {2}", neighborID, node2ID, distance[neighborID]);
                            flag = true;
                        }
                    }
                    Console.WriteLine("------------------------------");
                    if (flag) Console.ReadKey();
                }
            }
        }
    }
}
