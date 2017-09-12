using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    /// <summary>
    /// グラフを表す抽象クラス
    /// since 2017/06/01
    /// </summary>
    abstract partial class AGraph
    {
        public abstract string Name { get; }

        private int __Dimension;
        /// <summary>
        /// グラフの次元数
        /// </summary>
        public int Dimension
        {
            get { return __Dimension; }
            set
            {
                __Dimension = value;
                NodeNum = CalcNodeNum();
                FaultFlags = new bool[NodeNum];
            }
        }

        /// <summary>
        /// ノード数
        /// </summary>
        public uint NodeNum { get; private set; }

        private double __FaultRatio;
        /// <summary>
        /// グラフの故障率(0~1)
        /// </summary>
        public Double FaultRatio
        {
            get { return __FaultRatio; }
            set
            {
                __FaultRatio = value;
                FaultNodeNum = (uint)(NodeNum * __FaultRatio);
            }
        }

        /// <summary>
        /// 故障ノード数
        /// </summary>
        public uint FaultNodeNum { get; private set; }

        /// <summary>
        /// 第iノードが故障か否か
        /// </summary>
        public bool[] FaultFlags { get; protected set; }

        /// <summary>
        /// 乱数
        /// </summary>
        protected Random Rand;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dim">次元数</param>
        /// <param name="randSeed">乱数のシード</param>
        protected AGraph(int dim, int randSeed)
        {
            Dimension = dim;
            Rand = new Random(randSeed);
        }

        /// <summary>
        /// グラフのノード数を計算する．
        /// </summary>
        /// <returns>ノード数</returns>
        protected abstract uint CalcNodeNum();

        /// <summary>
        /// nodeの次数を返す
        /// </summary>
        /// <param name="Node">ノードアドレス</param>
        /// <returns>次数</returns>
        public abstract int GetDegree(uint Node);

        /// <summary>
        /// 第index隣接ノードを返す
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <param name="index">隣接ノードの番号</param>
        /// <returns>第index隣接ノード</returns>
        public abstract uint GetNeighbor(uint node, int index);

        /// <summary>
        /// 第i隣接ノードが前方かどうか
        /// ary[i] = 前方なら1、そうでないなら0
        /// </summary>
        /// <param name="node1">現在のノード</param>
        /// <param name="node2">目的ノード</param>
        /// <returns>第i隣接ノードが前方かどうかを示す配列</returns>
        public virtual int[] CalcForwardNeighbor(uint node1, uint node2)
        {
            int distance = CalcDistance(node1, node2);
            var ary = new int[Dimension];
            for (int i = 0; i < Dimension; i++)
            {
                ary[i] = CalcDistance(GetNeighbor(node1, i), node2) < distance ? 1 : 0;
            }
            return ary;
        }

        /// <summary>
        /// nodeの隣接ノードをすべて返す．
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <returns>隣接ノードの列挙体</returns>
        public virtual IEnumerable<uint> GetNeighbor(uint node)
        {
            for (int i = 0; i < GetDegree(node); i++)
            {
                yield return GetNeighbor(node, i);
            }
        }

        /// <summary>
        /// 2ノード間の距離を幅優先探索で求める
        /// </summary>
        /// <param name="node1">出発ノードのアドレス</param>
        /// <param name="node2">目的ノードのアドレス</param>
        /// <returns>距離</returns>
        public virtual int CalcDistanceBFS(uint node1, uint node2)
        {
            if (node1 == node2) return 0;

            Queue<uint> que = new Queue<uint>();
            int[] distance = new int[NodeNum];
            for (int i = 0; i < NodeNum; i++)
            {
                distance[i] = 100000;
            }

            // 探索本体
            que.Enqueue(node1);
            distance[node1] = 0;
            while (que.Count > 0)
            {
                uint current = que.Dequeue();
                foreach (var neighbor in GetNeighbor(current))
                {
                    if (distance[neighbor] == 100000)
                    {
                        if (neighbor == node2) return distance[current] + 1;
                        distance[neighbor] = distance[current] + 1;
                        que.Enqueue(neighbor);
                    }
                }
            }

            throw new Exception("2頂点が非連結?");
        }

        /// <summary>
        /// 2ノード間の距離を求める．適宜オーバーライドする．
        /// </summary>
        /// <param name="node1">出発ノードのアドレス</param>
        /// <param name="node2">目的ノードのアドレス</param>
        /// <returns>距離</returns>
        public virtual int CalcDistance(uint node1, uint node2)
        {
            return CalcDistanceBFS(node1, node2);
        }


        // Returns rerative distances
        public virtual int[] CalcRelativeDistance(uint node1, uint node2)
        {
            var ary = new int[GetDegree(node1)];
            var d = CalcDistance(node1, node2);
            for (int i = 0; i < GetDegree(node1); i++)
            {
                ary[i] = CalcDistance(GetNeighbor(node1, i), node2) - d;
            }
            return ary;
        }

        /// <summary>
        /// Calculate distance average and diameter.
        /// </summary>
        /// <param name="distance">variable to receive distance</param>
        /// <param name="diameter">variable to receive diameter</param>
        public void CalcDistanceaverageAndDiameter(out double distance, out int diameter)
        {
            int diam = 0;
            double sum = 0;
            double num = 0;

            for (uint node1 = 0; node1 < NodeNum; node1++)
            {
                for (uint node2 = node1 + 1; node2 < NodeNum; node2++)
                {
                    int d = CalcDistance(node1, node2);
                    if (diam < d) diam = d;
                    sum += d;
                    num++;
                }
            }

            distance = sum / num;
            diameter = diam;
        }


        // ルーティング


    }
}
