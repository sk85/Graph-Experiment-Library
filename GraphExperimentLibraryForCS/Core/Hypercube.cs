using System;

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




        /************************************************************************
         * 
         *  ルーティングに関する固有のメソッド
         *  
         ************************************************************************/
        
        public int[,] CalcCapability()
        {
            int[,] capability = new int[NodeNum, Dimension + 1];


            for (UInt32 nodeID = 0; nodeID < NodeNum; nodeID++)
            {
                if (!FaultFlags[nodeID]) capability[nodeID, 0] = 1;
            }

            for (int k = 1; k <= Dimension; k++)
            {
                for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
                {
                    int count = 0;
                    for (int i = 0; i < GetDegree(node); i++)
                        count += capability[node.ID, k - 1];
                    if (count > Dimension - k) capability[node.ID, k] = 1;
                }
            }

            return capability;
        }
    }
}
