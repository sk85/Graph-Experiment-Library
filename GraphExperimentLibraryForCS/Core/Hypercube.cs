using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph.Core
{
    class Hypercube : AGraph
    {
        /************************************************************************
         * 
         *  基底クラスのメソッドのオーバーライドなど
         *  
         ************************************************************************/

        /// <summary>
        /// AGraphのコンストラクタを呼びます。
        /// </summary>
        /// <param name="dim">次元数</param>
        /// <param name="randSeed">乱数の初期シード</param>
        public Hypercube(int dim, int randSeed) : base(dim, randSeed) { }

        /// <summary>
        /// グラフの名前
        /// </summary>
        public override string Name
        {
            get { return "Hypercube"; }
        }

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
            return new BinaryNode(node.ID ^ ((UInt32)0b1 << index));
        }

        /// <summary>
        /// ２頂点間の距離を返します．
        /// </summary>
        /// <param name="node1">頂点１</param>
        /// <param name="node2">頂点２</param>
        /// <returns>距離</returns>
        public override int CalcDistance(Node node1, Node node2)
        {
            return Experiment.Tools.GetPopCount(node1.ID ^ node2.ID);
        }

        /// <summary>
        /// 前方隣接頂点の列挙体を返します。
        /// </summary>
        /// <param name="node1">出発頂点</param>
        /// <param name="node2">目的頂点</param>
        /// <returns>前方隣接頂点</returns>
        public override IEnumerable<Node> CalcForwardNeighbor(Node node1, Node node2)
        {
            UInt32 c = node1.ID ^ node2.ID;
            for (int i = Dimension - 1; i >= 0; i--)
            {
                if (((c >> i) & 1) == 1) yield return GetNeighbor(node1, i);
            }
        }




        /*
         ************************************************************************
         * 
         *  ルーティングに関する固有のメンバ
         *  
         ************************************************************************
        */
        
        /// <summary>
        /// Routing Capablilityを計算します
        /// </summary>
        /// <returns>Routing Capablility</returns>
        private bool[,] CalcCapability()
        {
            bool[,] capability = new bool[NodeNum, Dimension];

            for (UInt32 nodeID = 0; nodeID < NodeNum; nodeID++)
            {
                capability[nodeID, 0] = !FaultFlags[nodeID];
            }

            for (int k = 1; k < Dimension; k++)
            {
                for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
                {
                    capability[node.ID, k] = GetNeighbor(node).Count(n => capability[n.ID, k - 1]) > Dimension - (k + 1);
                }
            }

            return capability;
        }

        /// <summary>
        /// Routing Probabilityを計算します
        /// </summary>
        /// <returns>Routing Probability</returns>
        private double[,] CalcProbability()
        {
            double[,] probability = new double[NodeNum, Dimension];

            // k = 1のときは故障頂点の割合
            for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
            {
                int count = 0;
                foreach (var neighbor in GetNeighbor(node))
                {
                    if (FaultFlags[neighbor.ID]) count++;
                }
                probability[node.ID, 0] = (double)count / Dimension;
            }

            // k >= 2のとき
            for (int k = 1; k < Dimension; k++)
            {
                for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
                {
                    probability[node.ID, k] = 1.0;
                    foreach (var neighbor in GetNeighbor(node))
                    {
                        if (!FaultFlags[neighbor.ID])
                        {
                            probability[node.ID, k] *= (1 - (1 - probability[neighbor.ID, k - 1]) * k / Dimension);
                        }
                    }
                }
            }

            for (int k = 0; k < Dimension; k++)
            {
                for (UInt32 nodeID = 0; nodeID < NodeNum; nodeID++)
                {
                    probability[nodeID, k] = 1 - probability[nodeID, k];
                }
            }

            return probability;
        }

        // 実験中
        public double[,] CalcCapability2()
        {
            double[,] capability2 = new double[NodeNum, Dimension];

            for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
            {
                capability2[node.ID, 0] = FaultFlags[node.ID] ? 0 : 1;
                    // (double)(GetNeighbor(node).Count(n => !FaultFlags[n.ID])) / Dimension;
            }

            for (int k = 1; k < Dimension; k++)
            {
                for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
                {
                    double exp = GetNeighbor(node).Sum(n => capability2[n.ID, k - 1]);
                    if (exp > Dimension - (k + 1))
                    {
                        capability2[node.ID, k] = 1.0;
                    }
                    else
                    {
                        double tmp = 1.0;
                        for (int i = 0; i < k; i++)
                        {
                            tmp *= (Dimension - exp - i) / (Dimension - i);
                        }
                        capability2[node.ID, k] = 1 - (tmp > 0 ? tmp : 0);
                    }
                }
            }

            return capability2;
        }

        public void SaveScore(string name)
        {
            for (int faultRatio = 0; faultRatio < 100; faultRatio += 10)
            {
                GenerateFaults(faultRatio);
                double[,] sd = CalcCapability2();
                var sw = new System.IO.StreamWriter(
                    @"..\..\output\" + name + faultRatio.ToString("00") + ".csv",
                    false, 
                    System.Text.Encoding.GetEncoding("shift_jis"));

                for (UInt32 i = 0; i < NodeNum; i++)
                {
                    for (int j = 0; j < Dimension; j++)
                    {
                        sw.Write("{0},", sd[i, j]);
                    }
                    sw.Write("\n");
                }

                sw.Close();
            }
        }

        /// <summary>
        /// Capabilityを用いたルーティングです。
        /// 成功ならば正の数でかかったステップ数、失敗ならば負の数で失敗までのステップ数を返します。
        /// </summary>
        /// <param name="node1">出発ノード</param>
        /// <param name="node2">目的ノード</param>
        /// <returns>かかったステップ数(負の数なら失敗時のステップ数)</returns>
        public int Routing_Capability(Node node1, Node node2)
        {
            bool[,] capability = CalcCapability();
            return RoutingBase(node1, node2, GetNext, capability);
        }

        /// <summary>
        /// Probabilityを用いたルーティングです。
        /// 成功ならば正の数でかかったステップ数、失敗ならば負の数で失敗までのステップ数を返します。
        /// </summary>
        /// <param name="node1">出発ノード</param>
        /// <param name="node2">目的ノード</param>
        /// <returns>かかったステップ数(負の数なら失敗時のステップ数)</returns>
        public int Routing_Probability(Node node1, Node node2)
        {
            double[,] probability = CalcProbability();
            return RoutingBase(node1, node2, GetNext, probability);
        }

        /// <summary>
        /// Capability2を用いたルーティングです。
        /// 成功ならば正の数でかかったステップ数、失敗ならば負の数で失敗までのステップ数を返します。
        /// </summary>
        /// <param name="node1">出発ノード</param>
        /// <param name="node2">目的ノード</param>
        /// <param name="getNext">移動先のノードを決める関数</param>
        /// <returns>かかったステップ数(負の数なら失敗時のステップ数)</returns>
        public int Routing_Capability2(Node node1, Node node2)
        {
            double[,] capability2 = CalcCapability2();
            return RoutingBase(node1, node2, GetNext, capability2);
        }

        public int Routing_Greedy(Node node1, Node node2)
        {
            Node current = node1;
            Stack<Node> stack = new Stack<Node>();

            stack.Push(current);
            while(stack.Count > 0)
            {
                current = stack.Pop();
                if (current.ID == node2.ID) return 1;

                foreach (var node in CalcForwardNeighbor(current, node2).Where(n => !FaultFlags[n.ID]))
                {
                    stack.Push(node);
                }
            }
            return -1;
        }
    }
}
