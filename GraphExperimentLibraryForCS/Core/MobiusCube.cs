using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    class MobiusCube : AGraph
    {
        public int Type { get; set; }

        /// <summary>
        /// AGraphのコンストラクタを呼びます。
        /// <para>現状ではRandomオブジェクトの初期化とDimensionフィールドの初期化だけ。</para>
        /// </summary>
        /// <param name="dim">次元数</param>
        public MobiusCube(int dim, int randSeed) : base(dim, randSeed) { }

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
        /// <param name="node">ノードアドレス</param>
        /// <param name="index">エッジの番号</param>
        /// <returns>隣接ノードのアドレス</returns>
        public override Node GetNeighbor(Node node, int index)
        {
            BinaryNode binNode = (BinaryNode)node;
            UInt32 mask;
            int type = index == Dimension - 1
                ? Type
                : (int)((binNode.Addr >> (index + 1)) & 1);

            if (type == 0)
            {
                mask = (UInt32)1 << index;    // 100...000
            }
            else
            {
                mask = ((UInt32)1 << (index + 1)) - 1;  // 111...111
            }
            return new BinaryNode(binNode.Addr ^ mask);
        }
    }
}
