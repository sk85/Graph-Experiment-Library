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
    abstract class AGraph
    {
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

        /// <summary>
        /// デバッグ用ToString
        /// </summary>
        /// <returns>デバッグ用テキスト</returns>
        public override string ToString()
        {
            string str = string.Format(
                    "{0,5}|{1,16}|{2,5}|\n",
                    "ID",
                    "Addr",
                    "IsFault"
                );
            for (uint i = 0; i < NodeNum; i++)
            {
                str += string.Format(
                    "{0,5}|{1,16}|{2,7}|\n",
                    i,
                    Debug.UintToBinaryString(i, 16, 16),
                    FaultFlags[i]
                );
            }
            return str;
        }
    }
}
