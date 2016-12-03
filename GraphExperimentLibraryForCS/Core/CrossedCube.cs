using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    /// <summary>
    /// クロスドキューブのクラスです。
    /// <para>現在うまく動かない</para>
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
        public override int GetDegree(uint node)
        {
            return Dimension;
        }

        /// <summary>
        /// nodeの第indexエッジと接続する隣接ノードを返します。
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <param name="index">エッジの番号</param>
        /// <returns>隣接ノードのアドレス</returns>
        public override UInt32 GetNeighbor(UInt32 node, int index)
        {
            const UInt32 mask1 = 0x55555555;            // 01010101....01
            UInt32 mask2 = ((UInt32)1 << index) - 1;    // 00...0111...11
            UInt32 mask = ((node & mask1 & mask2) << 1) | ((UInt32)1 << index);
            return node ^ mask;
        }
    }
}
