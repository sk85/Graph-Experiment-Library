﻿using System;
using System.Collections.Generic;

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
        public override IEnumerable<Node> CalcPrefferedNeighbor(Node node1, Node node2)
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

        private int[,] Capability;

        private void CalcCapability()
        {
            Capability = new int[NodeNum, Dimension];

            for (UInt32 nodeID = 0; nodeID < NodeNum; nodeID++)
            {
                Capability[nodeID, 0] = FaultFlags[nodeID] ? 0 : 1;
            }

            for (int k = 1; k < Dimension; k++)
            {
                for (Node node = new Node(0); node.ID < NodeNum; node.ID++)
                {
                    int count = 0;
                    if (FaultFlags[node.ID])
                    {
                        Capability[node.ID, k] = 0;
                    }
                    else
                    {
                        foreach (var neighbor in GetNeighbor(node))
                            count += Capability[neighbor.ID, k - 1];
                        Capability[node.ID, k] = (count > Dimension - (k + 1)) ? 1 : 0;
                    }
                }
            }
        }

        private Node GetNext_Capability(Node node1, Node node2)
        {
            Node next = null;
            foreach (var prNeighbor in CalcPrefferedNeighbor(node1, node2))
            {
                if (prNeighbor.ID == node2.ID)
                    return prNeighbor;
                else if (!FaultFlags[prNeighbor.ID])
                {
                    if (Capability[prNeighbor.ID, CalcDistance(prNeighbor, node2) - 1] == 1)
                        return prNeighbor;
                    else
                    {
                        if (next == null) next = prNeighbor;
                    }
                }
            }
            return next == null ? node1 : next;
        }

        public int Routing_Capability(Node node1, Node node2)
        {
            CalcCapability();
            int step = RoutingBase(node1, node2, GetNext_Capability);
            if (Capability[node1.ID, CalcDistance(node1, node2) - 1] == 1)
            {
                if (step < 0)
                {
                    Console.Write("a");
                    Console.ReadKey();
                }
            }
            return step;
        }

        public void SaveCapability()
        {
            for (int faultRatio = 0; faultRatio < 100; faultRatio += 10)
            {
                GenerateFaults(faultRatio);
                CalcCapability();
                var sw = new System.IO.StreamWriter(
                    @"..\..\output\" + faultRatio.ToString("00") + ".csv",
                    false, 
                    System.Text.Encoding.GetEncoding("shift_jis"));

                for (UInt32 i = 0; i < NodeNum; i++)
                {
                    for (int j = 0; j < Dimension; j++)
                    {
                        sw.Write("{0},", Capability[i,j]);
                    }
                    sw.Write("\n");
                }

                sw.Close();
            }
        }
    }
}
