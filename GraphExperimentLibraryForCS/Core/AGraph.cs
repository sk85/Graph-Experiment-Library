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
    abstract partial class AGraph
    {

        /************************************************************************
         * 
         *  基本的なメンバ
         *      ノード数、故障、故障率など
         *  
         ************************************************************************/

        private int __Dimension;
        private int __FaultRatio;

        /// <summary>
        /// グラフの次元数です。
        /// <para>更新すると勝手にNodeNumや、それに関わるもろもろも更新されます。</para>
        /// </summary>
        public int Dimension
        {
            get { return __Dimension; }
            set
            {
                __Dimension = value;
                NodeNum = CalcNodeNum(Dimension);
                FaultFlags = new bool[NodeNum];
            }
        }

        /// <summary>
        /// グラフの名前です。
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// グラフのノード数です。
        /// </summary>
        public UInt32 NodeNum { get; private set; }

        /// <summary>
        /// グラフのノード故障率(％)です。
        /// <para>GenerateFaultsを使うと更新されます。</para>
        /// </summary>
        public int FaultRatio
        {
            get { return __FaultRatio; }
            private set
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
        /// FaultFlags[i] = 第iノードが故障か否か
        /// </summary>
        public bool[] FaultFlags { get; protected set; }

        /// <summary>
        /// 乱数オブジェクト。
        /// <para>使うたびに生成するのは無駄な気がしてフィールドに含めてみたけど、まずそうなら消します。</para>
        /// </summary>
        protected Random Rand;








        /************************************************************************
         * 
         *  基本的なメソッド
         *      コンストラクタ、グラフの状態を取得・設定するなど
         *  
         ************************************************************************/

        /// <summary>
        /// コンストラクタ。
        /// とりあえずは次元数の設定とRandの初期化くらい
        /// </summary>
        protected AGraph(int dim, int randSeed)
        {
            Dimension = dim;
            SetRandSeed(randSeed);
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
        public abstract int GetDegree(Node node);

        /// <summary>
        /// nodeの第indexエッジと接続する隣接ノードを返します。
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <param name="index">エッジの番号</param>
        /// <returns>隣接ノードのアドレス</returns>
        public abstract Node GetNeighbor(Node node, int index);

        /// <summary>
        /// 2頂点が連結かどうかを返します。
        /// </summary>
        /// <param name="node1">頂点1</param>
        /// <param name="node2">頂点2</param>
        /// <returns>頂点1と頂点2が連結か否か</returns>
        public bool IsConnected(Node node1, Node node2)
        {
            //return CalcAllDistanceBFSF(node1)[node2.ID] > 0;
            
            // 深さ優先に書き直す
            bool[] unvisited = new bool[NodeNum];
            Stack<Node> stack = new Stack<Node>();  // 探索用のキュー
            
            for (UInt32 i = 0; i < NodeNum; i++) unvisited[i] = true;

            // 探索本体
            stack.Push(node1);
            unvisited[node1.ID] = false;
            while (stack.Count > 0)
            {
                Node current = stack.Pop();
                for (int i = 0; i < GetDegree(current); i++)
                {
                    Node neighbor = GetNeighbor(current, i);
                    if (unvisited[neighbor.ID] && !FaultFlags[neighbor.ID])
                    {
                        if (neighbor.ID == node2.ID) return true;
                        unvisited[neighbor.ID] = false;
                        stack.Push(neighbor);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// FaultFlagsを指定の故障率で設定
        /// </summary>
        /// <param name="faultRatio">故障率(%)</param>
        public void GenerateFaults(int faultRatio)
        {
            FaultRatio = faultRatio;

            // 初期化
            for (UInt32 i = 0; i < NodeNum; i++) FaultFlags[i] = false;

            // ランダムに故障の生成
            for (UInt32 i = 0; i < FaultNodeNum; i++)
            {
                UInt32 rand = (UInt32)(Rand.NextDouble() * (NodeNum - i));
                UInt32 index = 0, count = 0;

                while (count <= rand)
                {
                    if (!FaultFlags[index++]) count++;
                }
                FaultFlags[index - 1] = true;
            }
        }






        /************************************************************************
         * 
         *  距離に関するメソッド
         *  
         ************************************************************************/

        /// <summary>
        /// nodeから他のノードへの距離を幅優先探索により計算して返します。
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <returns>nodeからの距離の表</returns>
        public int[] CalcAllDistanceBFS(Node node)
        {
            int[] array = new int[NodeNum];         // 距離の表
            Queue<Node> que = new Queue<Node>();  // 探索用のキュー

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
                Node current = que.Dequeue();
                for (int i = 0; i < GetDegree(node); i++)
                {
                    Node neighbor = GetNeighbor(current, i);
                    if (array[neighbor.ID] > array[current.ID])
                    {
                        array[neighbor.ID] = array[current.ID] + 1;
                        que.Enqueue(neighbor);
                    }
                }
            }

            return array;
        }

        /// <summary>
        /// nodeから他のノードへの距離を幅優先探索により計算して返します。
        /// <para>こちらは故障を考慮します。到達不可能ならば-1</para>
        /// </summary>
        /// <param name="node">ノードアドレス</param>
        /// <returns>nodeからの距離の表</returns>
        public int[] CalcAllDistanceBFSF(Node node)
        {
            int[] array = new int[NodeNum];         // 距離の表
            const int inf = 100000;
            Queue<Node> que = new Queue<Node>();  // 探索用のキュー

            // 距離の初期値は∞
            for (UInt32 i = 0; i < NodeNum; i++)
            {
                array[i] = inf;
            }

            // 探索本体
            que.Enqueue(node);
            array[node.ID] = 0;
            while (que.Count > 0)
            {
                Node current = que.Dequeue();
                for (int i = 0; i < GetDegree(node); i++)
                {
                    Node neighbor = GetNeighbor(current, i);
                    if (!FaultFlags[neighbor.ID] && array[neighbor.ID] > array[current.ID])
                    {
                        array[neighbor.ID] = array[current.ID] + 1;
                        que.Enqueue(neighbor);
                    }
                }
            }

            // ∞を-1に
            for (UInt32 i = 0; i < NodeNum; i++)
            {
                if (array[i] == inf) array[i] = -1;
            }

            return array;
        }

        /// <summary>
        /// 2頂点間の距離を返します．
        /// デフォルトでは幅優先探索メソッドのラッパなので，適宜オーバーライドしてください．
        /// </summary>
        /// <param name="node1">頂点１</param>
        /// <param name="node2">頂点２</param>
        /// <returns>node1とnode2の距離</returns>
        public virtual int CalcDistance(Node node1, Node node2)
        {
            return CalcAllDistanceBFS(node1)[node2.ID];
        }


        





        /************************************************************************
         * 
         *  実験に便利なメソッド
         *      ランダムにノードを取得する
         *      連結なノードを取得する
         *      など
         *  
         ************************************************************************/
        
        /// <summary>
        /// 非故障なノードをランダムに取得
        /// </summary>
        /// <returns>ノード</returns>
        public Node GetNodeRandom()
        {
            UInt32 unfaultNum = NodeNum - FaultNodeNum;
            UInt32 rand = (UInt32)(Rand.NextDouble() * unfaultNum);
            UInt32 index = 0, count = 0;
            while (count <= rand)
            {
                if (!FaultFlags[index++])
                {
                    count++;
                }
            }
            return new Node(index - 1);
        }

        /// <summary>
        /// あるノードと連結かつ非故障なノードをランダムに取得。
        /// 存在しなければ元のノードを返します。
        /// </summary>
        /// <param name="node">ノード</param>
        /// <returns>連結なノード</returns>
        public Node GetConnectedNodeRandom(Node node)
        {
            {
                bool f = true;
                for (int i = 0; i < GetDegree(node); i++)
                {
                    if (!FaultFlags[GetNeighbor(node, i).ID]) f = false;
                }
                if (f) return node;
            }
            
            Node node2;
            do
            {
                node2 = GetNodeRandom();
            } while (node.ID == node2.ID || !IsConnected(node, node2));
            return node2;

            /*
            int[] distance = CalcAllDistanceBFSF(node);
            UInt32 count = 0;   // nodeと連結なノードの数
            UInt32 index = 0;

            // 連結なノード数を数える
            for (UInt32 i = 0; i < NodeNum; ++i)
            {
                if (distance[i] > 0) count++;
            }

            // 先に連結なノードを列挙してから選ぶ形にすれば効率的かもかも
            if (count > 0)
            {
                UInt32 rand = (UInt32)(Rand.NextDouble() * count);  // 何番目の連結ノードを選ぶか

                // 選んだノードは何か
                count = 0;
                while (count <= rand)
                {
                    if (distance[index++] > 0) count++;
                }
            }
            else  // 連結なノードが存在しないならば失敗
            {
                return node;
            }

            return new Node(index - 1);
            */
        }






        /************************************************************************
         * 
         *  ルーティングメソッド
         *  
         ************************************************************************/
        

        public Node SimpleGetNext(Node node1, Node node2)
        {
            /*
            int[] distance = CalcAllDistanceBFS(node2);
            for (int i = 0; i < GetDegree(node1); i++)
            {
                Node neighbor = GetNeighbor(node1, i);
                if (!FaultFlags[neighbor.ID] && distance[neighbor.ID] < distance[node1.ID]) return neighbor;
            }
            */
            int currentDistance = CalcDistance(node1, node2);
            for (int i = 0; i < GetDegree(node1); i++)
            {
                Node neighbor = GetNeighbor(node1, i);
                int nextDistance = CalcDistance(neighbor, node2);
                if (!FaultFlags[neighbor.ID] && nextDistance < currentDistance) return neighbor;
            }
            return node1;
        }

        /// <summary>
        /// ルーティングメソッドです。
        /// node1からnode2まで、getNextに従ってルーティングを行います。
        /// <para>
        /// Node getNext(Node c, Node d);
        /// cからdへのルーティングで次のノードを返してください。
        /// 行けるノードがない場合はcを返してください。
        /// </para>
        /// </summary>
        /// <param name="node1">出発ノード</param>
        /// <param name="node2">目的ノード</param>
        /// <param name="getNext">移動先のノードを決める関数</param>
        /// <returns>かかったステップ数(負の数なら失敗時のステップ数)</returns>
        public int Routing(Node node1, Node node2, Func<Node, Node, Node> getNext)
        {
            Node current = new Node(node1.ID);
            Node preview = new Node(node1.ID);

            int step = 0;

            while (current.ID != node2.ID)
            {
                Node next = getNext(current, node2);

                // 1つ前に戻る or 行けるノードがないなら失敗
                if (next.ID == preview.ID || next.ID == current.ID)
                {
                    return -step;
                }
                else
                {
                    step++;
                    preview = current;
                    current = next;
                }
            }

            return step;
        }
    }
}
