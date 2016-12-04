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



        public List<BinaryNode> GetFowardNeighbor2(BinaryNode node1, BinaryNode node2)
        {
            int count = 0;  // 上位に存在する枝候補の数
            BinaryNode tmp = null;
            List<BinaryNode> set = new List<BinaryNode>();

            void addTmp()
            {
                if(tmp != null)
                {
                    set.Add(tmp);
                    tmp = null;
                }
            }

            // iはビットペアの左側が第何ビットかを表す
            for (int i = Dimension - 1; i > 1; i--)
            {
                if (node1[i] != node2[i])
                {
                    // 奇数(左側)
                    if ((i & 1) == 1)
                    {
                        if (node1[i - 1] == 1)
                        {
                            if (count > 0)
                            {
                                addTmp();
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }


            return null;
        }

        public IEnumerable<BinaryNode> GetFowardNeighbor3(BinaryNode node1, BinaryNode node2)
        {
            int count = 0;  // 上位に存在する枝候補の数
            BinaryNode tmp = null;

            // iはビットペアの左側が第何ビットかを表す
            for (int i = Dimension + (Dimension & 1); i > 1; i -= 2)
            {
                if (node1[i] != node2[i])
                {
                    if (node1[i - 1] == 1)
                    {
                        // x1/Xy かつ上に候補が有る
                        if (count > 0)
                        {
                            if (tmp != null)
                            {
                                yield return tmp;
                                tmp = null;
                            }
                        }
                        else
                        {
                            // x1/X1 かつ上に候補がない
                            if (node2[i - 1] == 1)
                            {
                                tmp = (BinaryNode)GetNeighbor(node1, i);
                                count += 1;
                            }
                            // x1/X0 かつ上に候補がない
                            else
                            {
                                yield return (BinaryNode)GetNeighbor(node1, i);
                                yield return (BinaryNode)GetNeighbor(node1, i - 1);
                                count += 2;
                            }
                        }
                    }
                    else
                    {
                        if (node2[i - 1] == 1)
                        {
                            // x0/X1 かつ候補が複数
                            if (count > 1)
                            {
                                yield return (BinaryNode)GetNeighbor(node1, i);
                                yield return (BinaryNode)GetNeighbor(node1, i - 1);
                                if (tmp != null)
                                {
                                    yield return tmp;
                                    tmp = null;
                                }
                                count += 2;
                            }
                            // x0/X1 かつ候補がすくない
                            else
                            {
                                yield return (BinaryNode)GetNeighbor(node1, i - 1);
                                if (tmp != null)
                                {
                                    yield return tmp;
                                    tmp = null;
                                }
                                count += 1;
                            }
                        }
                        else
                        {
                            // x0/X0 かつ候補が存在する
                            if (count > 0)
                            {
                                yield return (BinaryNode)GetNeighbor(node1, i);
                                if (tmp != null)
                                {
                                    yield return tmp;
                                    tmp = null;
                                }
                                count += 1;
                            }
                            // x0/X0 かつ候補が存在しない
                            else
                            {
                                tmp = (BinaryNode)GetNeighbor(node1, i);
                                count += 1;
                            }
                        }
                    }
                }
                else
                {
                    if (node1[i - 1] == 1 && node2[i - 1] == 0)
                    {
                        if (count > 1)
                        {
                            yield return (BinaryNode)GetNeighbor(node1, i - 1);
                            count += 1;

                        }
                        else if (count == 1)
                        {
                            yield return (BinaryNode)GetNeighbor(node1, i - 1);
                            if (tmp != null)
                            {
                                tmp = null;
                                count -= 1;
                            }
                            count += 1;
                        }
                        else
                        {
                            tmp = (BinaryNode)GetNeighbor(node1, i - 1);
                            count += 1;
                        }
                    }
                    else if (node1[i - 1] == 0 && node2[i - 1] == 1)
                    {
                        if (count > 0)
                        {
                            yield return (BinaryNode)GetNeighbor(node1, i - 1);
                            count += 1;

                        }
                        else
                        {
                            if (tmp != null)
                            {
                                yield return tmp;
                                tmp = null;
                                count += 1;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<int> GetFowardNeighbor(BinaryNode node1, BinaryNode node2)
        {
            // 出発頂点と目的頂点が同じなら何もなし
            if (node1.ID == node2.ID) yield break;

            int count = 0;              // 上位に存在する枝候補の数
            int tmp = -1;
            int index = Dimension | 1;  // ビットペアの左側が第何ビットかを表す

            // 最左な異なるビットを探す
            while (node1[index] == node2[index]) index--;

            // 左かつ両方揃える場合
            if ((index & 1) == 1 && node1[index - 1] != node2[index - 1])
            {
                yield return index;
                yield return index - 1;
                count += 2;
            }
            // 片方しか揃えない場合
            else
            {
                tmp = index;
            }

            // 一番右まで
            while (index-- > 0)
            {
                // 普通パターン
                // 左 かつ 異なる かつ 下は同じ
                // 右 かつ
                if (
                    (index & 1) == 1 && node1[index] != node2[index] && node1[index - 1] != node2[index - 1] ||
                    (index & 1) == 0 
                    )
                {
                    yield return index;
                    if (count == 0) yield return tmp;
                }
                // node1の右が１ かつ 少なくとも左は揃える
                else if (node1[index - 1] == 1 && node1[index] != node2[index])
                {
                    //if (count == 0) yield return tmp;
                }
                index -= 2;
            }

            /*
            {
                for (int i = Dimension | 1; i > 1; i -= 2)
                {
                    if (node1[i - 1] == 1)
                    {
                        // 01/00か11/10
                        if (node1[i] == node2[i] && node1[i - 1] != node2[i - 1])
                        {

                        }
                        // 01/11か11/01
                        else if (node1[i] != node2[i] && node1[i - 1] == node2[i - 1])
                        {

                        }
                        // 01/10か11/00
                        else if (node1[i] != node2[i] && node1[i - 1] != node2[i - 1])
                        {

                        }
                    }
                    else
                    {
                        // 00/01か10/11
                        if (node1[i] == node2[i] && node1[i - 1] != node2[i - 1])
                        {

                        }
                        // 00/10か10/00
                        else if (node1[i] != node2[i] && node1[i - 1] == node2[i - 1])
                        {

                        }
                        // 00/11か10/01
                        else if (node1[i] != node2[i] && node1[i - 1] != node2[i - 1])
                        {

                        }
                    }
                }
            }
            */
        }
    }
}
