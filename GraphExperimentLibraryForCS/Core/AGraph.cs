using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    /// <summary>
    /// グラフを表す抽象クラスです。
    /// 任意のグラフ用のクラスに継承して用います。
    /// <para>ノード数が2^32未満(キューブ系ならn＜32)であるという前提で実装しています。</para>
    /// </summary>
    abstract class AGraph
    {
        private int __Dimension;
        private int __FaultRatio;

        /// <summary>
        /// グラフの次元数です。
        /// <para>更新すると勝手にNodeNumも更新されます。</para>
        /// </summary>
        public int Dimension
        {
            get { return __Dimension; }
            set
            {
                __Dimension = value;
                NodeNum = CalcNodeNum(Dimension);
            }
        }
        
        /// <summary>
        /// グラフのノード数です。
        /// </summary>
        public UInt32 NodeNum { get; private set; }

        /// <summary>
        /// グラフのノード故障率(％)です。
        /// <para>0 ≦ FaultRatio ≦ 100です。</para>
        /// <para>更新すると勝手にFaultNodeNumも更新されます。</para>
        /// </summary>
        public int FaultRatio
        {
            get { return __FaultRatio; }
            set
            {
                __FaultRatio = value;
                FaultNodeNum = (UInt32)(NodeNum * ((double)__FaultRatio / 100));
            }
        }

        /// <summary>
        /// グラフの故障ノード数です。
        /// </summary>
        public UInt32 FaultNodeNum { get; private set; }

        /// <summary>
        /// ノードが故障しているかを示します。
        /// 第iノードが故障ならば
        /// </summary>
        protected bool[] FaultFlags;

        /// <summary>
        /// 乱数オブジェクト。
        /// <para>使うたびに生成するのは無駄な気がしてフィールドに含めてみたけど、まずそうなら消します。</para>
        /// </summary>
        protected Random Rand;

        /// <summary>
        /// コンストラクタ。
        /// とりあえずは
        /// </summary>
        protected AGraph(int dim)
        {
            Dimension = dim;
            SetRandSeed(0);
        }

        /// <summary>
        /// 現在の次元数からノード数を計算して返します。
        /// </summary>
        /// <param name="dim">次元数</param>
        /// <returns>ノード数</returns>
        protected abstract UInt32 CalcNodeNum(int dim);

        /// <summary>
        /// Randのシード値を書き換えます。
        /// </summary>
        /// <param name="seed">シード値</param>
        public void SetRandSeed(int seed)
        {
            Rand = new Random(seed);
        }

        /// <summary>
        /// nodeの次数を返します。
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <returns>nodeの次数</returns>
        public abstract int GetDegree(INode node);

        /// <summary>
        /// nodeの第indexエッジと接続する隣接ノードを返します。
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <param name="index">エッジの番号</param>
        /// <returns>隣接ノードのアドレス</returns>
        public abstract INode GetNeighbor(INode node, int index);

        /// <summary>
        /// nodeから他のノードへの距離を幅優先探索により計算して返します。
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <returns>nodeからの距離の表</returns>
        public int[] CalcAllDistanceBFS(INode node)
        {
            int[] array = new int[NodeNum];         // 距離の表
            Queue<INode> que = new Queue<INode>();  // 探索用のキュー

            // 距離の初期値は∞
            for (UInt32 i = 0; i < NodeNum; i++)
            {
                array[i] = 100000;
            }

            // 探索本体
            que.Enqueue(node);
            array[node.ID] = 0;
            while(que.Count > 0)
            {
                INode current = que.Dequeue();
                for (int i = 0; i < GetDegree(node); i++)
                {
                    INode neighbor = GetNeighbor(current, i);
                    if (array[neighbor.ID] > array[current.ID])
                    {
                        array[neighbor.ID] = array[current.ID] + 1;
                        que.Enqueue(neighbor);
                    }
                }
            }

            return array;
        }
    }
}
