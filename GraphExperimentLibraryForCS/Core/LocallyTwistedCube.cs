﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    /// <summary>
    /// ローカリーツイステッドキューブのクラスです。
    /// </summary>
    class LocallyTwistedCube : AGraph
    {
        /************************************************************************
        * 
        *  基底クラスのメソッドのオーバーライドなど
        *  
        ************************************************************************/

        /// <summary>
        /// グラフの名前
        /// </summary>
        public override string Name
        {
            get { return "LocallyTwistedCube"; }
        }

        /// <summary>
        /// AGraphのコンストラクタを呼びます。
        /// </summary>
        /// <param name="dim">次元数</param>
        /// <param name="randSeed">乱数の初期シード</param>
        public LocallyTwistedCube(int dim, int randSeed) : base(dim, randSeed) { }

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
            if (index < 2)
            {
                return new BinaryNode(node.ID ^ ((UInt32)0b1 << index));
            }
            else
            {
                return new BinaryNode(node.ID ^ ((0b10 + (node.ID & 0b1)) << (index - 1)));
            }
        }

        /// <summary>
        /// ２頂点間の距離を返します．
        /// </summary>
        /// <param name="node1">頂点１</param>
        /// <param name="node2">頂点２</param>
        /// <returns>距離</returns>
        public override int CalcDistance(Node node1, Node node2)
        {
            UInt32 c1 = node1.ID ^ node2.ID, c2 = c1, type = 0b10 + (node1.ID & 1);
            int count1 = 0, count2 = 0;

            for (int i = Dimension - 1; i > 1; --i)
            {
                if ((c1 >> i) == 1)
                {
                    c1 ^= type << (i - 1);
                    count1++;
                }
                if ((c2 >> i) == 1)
                {
                    c2 ^= (c2 >> (i - 1)) << (i - 1);
                    count2++;
                }
            }
            count1 += (int)((c1 >> 1) + ((c1 & 1) << 10));
            count2 += 2 + (int)((c2 >> 1) - (c2 & 1));

            return count1 < count2 ? count1 : count2;
        }
    }
}
