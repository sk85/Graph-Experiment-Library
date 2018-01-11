using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using GraphCS.Core;
using GraphCS.Graphs;

#if true

namespace GraphCS.Graphs
{
    partial class CrossedCube : AGraph<BinaryNode>
    {
        /// <summary>
        /// EfeのCalcDistanceをデバッグ
        /// </summary>
        public void DEBUG_Efe_GetNext()
        {
            Console.WriteLine("Debug 'Efe_GetNext'");
            Console.WriteLine("> Checking distance");

            Console.WriteLine("> dim = {0}", Dimension);
            
            var u = new BinaryNode(0);
            var v = new BinaryNode(0);

            for (u.Addr = 0; u.Addr < NodeNum; u.Addr++)
            {
                for (v.Addr = u.Addr + 1; v.Addr < NodeNum; v.Addr++)
                {
                    var d1 = CalcDistance(u, v);

                    var d2 = 0;
                    var current = new BinaryNode(u);
                    while (current != v)
                    {
                        d2++;
                        Efe_GetNext(current, v, out var n1, out var n2);
                        current = n1;
                    }

                    if (d1 != d2)
                    {
                        Console.WriteLine($"({u.Addr},{v.Addr}) [{d1} / {d2}]");
                        Console.ReadKey();
                    }
                }
            }
        }

        public void DEBUG_Efe_GetNext2()
        {
            Console.WriteLine("Debug 'Efe_GetNext'");
            Console.WriteLine("> Checking if n1 and n2 are forward");

            Console.WriteLine("> dim = {0}", Dimension);
            
            var u = new BinaryNode(0);
            var v = new BinaryNode(0);

            for (u.Addr = 0; u.Addr < NodeNum; u.Addr++)
            {
                for (v.Addr = u.Addr + 1; v.Addr < NodeNum; v.Addr++)
                {
                    var d = CalcDistance(u, v);
                    Efe_GetNext(u, v, out var n1, out var n2);
                    if (CalcDistance(n1, v) >= d || n2 != null && CalcDistance(n2, v) >= d)
                    {
                        Console.WriteLine($"({u.Addr},{v.Addr})");
                        Console.ReadKey();
                    }
                }
            }
        }
    }
}

namespace GraphCS.Core
{
    abstract partial class AGraph<NodeType> where NodeType : ANode, new()
    {

        /// <summary>
        /// Debug Agraph.CalcDistance
        /// </summary>
        public void DEBUG_CalcDistance()
        {
            Console.WriteLine("Debug \"AGraph.CalcDistance\"");
            Console.WriteLine("> {0}-dimensional {1}", Dimension, Name);
            Console.Write("> ");
            for (var node1 = new NodeType(); node1.Addr < NodeNum; node1.Addr++)
            {
                if ((node1.Addr & 0b1111) == 0)
                {
                    Console.CursorLeft = 2;
                    Console.Write($"{(double)(node1.Addr + 1) / NodeNum:###%}");
                }

                // node1から各ノードへの距離+1の表を作る
                var que = new Queue<NodeType>();
                var dis = new int[NodeNum];
                que.Enqueue(node1);
                dis[node1.Addr] = 1;
                while (que.Count > 0)
                {
                    NodeType current = que.Dequeue();
                    foreach (var neighbor in GetNeighbor(current))
                    {
                        if (dis[neighbor.Addr] == 0)
                        {
                            dis[neighbor.Addr] = dis[current.Addr] + 1;
                            que.Enqueue(neighbor);
                        }
                    }
                }

                // チェック
                var node2 = new NodeType();
                for (node2.Addr = node1.Addr + 1; node2.Addr < NodeNum; node2.Addr++)
                {
                    int d = CalcDistance(node1, node2);
                    if (d != dis[node2.Addr] - 1)
                    {
                        Console.WriteLine($"\nd({node1},{node2}) = {dis[node2.Addr] - 1,2} / {d,2}");
                        Console.WriteLine();
                        Console.WriteLine("> NG");
                        return;
                    }
                }
            }
            Console.CursorLeft = 0;
            Console.WriteLine("> 100%");
            Console.WriteLine("> OK");
        }

        /// <summary>
        /// Debug CalcRelativeDistance.
        /// CalcDistanceを都度呼び出しているので遅いかも
        /// </summary>
        public void DEBUG_CalcRelativeDistance()
        {
            Console.WriteLine("Debug \"AGraph.CalcRelativeDistance\"");
            Console.WriteLine("> {0}-dimensional {1}", Dimension, Name);
            Console.Write("> ");

            for (var node1 = new NodeType(); node1.Addr < NodeNum; node1.Addr++)
            {
                if ((node1.Addr & 0b1111) == 0)
                {
                    Console.CursorLeft = 2;
                    Console.Write($"{(double)(node1.Addr + 1) / NodeNum:###%}");
                }

                for (var node2 = new NodeType(); node2.Addr < NodeNum; node2.Addr++)
                {
                    var rel = CalcRelativeDistance(node1, node2);
                    var dis = CalcDistance(node1, node2);

                    for (int i = 0; i < Dimension; i++)
                    {
                        if (CalcDistance(GetNeighbor(node1, i), node2) - dis != rel[i])
                        {
                            Console.WriteLine("");
                            Console.WriteLine("> NG");
                            Console.WriteLine("> node1 = {0}", node1.Addr);
                            Console.WriteLine("> node2 = {0}", node2.Addr);
                            return;
                        }
                    }
                }
            }
            Console.CursorLeft = 0;
            Console.WriteLine("> 100%");
            Console.WriteLine("> OK");
        }

        /// <summary>
        /// Debug CalcRelativeDistance.
        /// 最初に距離の表を作るので、CalcDistanceの呼び出し回数が減っている(速いかは未確認)
        /// </summary>
        public void DEBUG_CalcRelativeDistance2()
        {
            Console.WriteLine("Debug \"AGraph.CalcRelativeDistance\"");
            Console.WriteLine("> {0}-dimensional {1}", Dimension, Name);
            Console.Write("> ");

            var disMat = new int[NodeNum, NodeNum];

            for (var node1 = new NodeType(); node1.Addr < NodeNum; node1.Addr++)
            {
                if ((node1.Addr & 0b1111) == 0)
                {
                    Console.CursorLeft = 2;
                    Console.Write($"{(double)(node1.Addr + 1) / NodeNum:###%}");
                }

                for (var node2 = new NodeType(); node2.Addr < NodeNum; node2.Addr++)
                {
                    var rel = CalcRelativeDistance(node1, node2);
                    var dis = CalcDistance(node1, node2);

                    for (int i = 0; i < Dimension; i++)
                    {
                        var neighbor = GetNeighbor(node1, i);
                        int n1, n2;
                        if (neighbor.Addr > node2.Addr)
                        {
                            n1 = node2.Addr; n2 = neighbor.Addr;
                        }
                        else
                        {
                            n1 = neighbor.Addr; n2 = node2.Addr;
                        }

                        if (disMat[n1, n2] == 0)
                        {
                            disMat[n1, n2] = CalcDistance(neighbor, node2);
                        }

                        if (disMat[n1, n2] - dis != rel[i])
                        {
                            Console.WriteLine("");
                            Console.WriteLine("> NG");
                            Console.WriteLine("> node1 = {0}", node1.Addr);
                            Console.WriteLine("> node2 = {0}", node2.Addr);
                            return;
                        }
                    }
                }
            }
            Console.CursorLeft = 0;
            Console.WriteLine("> 100%");
            Console.WriteLine("> OK");
            Console.WriteLine();
        }

        /// <summary>
        /// 全ノードペア間の距離を幅優先探索で計算して表示。
        /// GetNeighborの確認などに用いる(非連結なら停止する)
        /// </summary>
        public void DEBUG_ShowAllPairDistance()
        {
            for (var node1 = new NodeType(); node1.Addr < NodeNum; node1.Addr++)
            {
                var node2 = new NodeType();
                for (node2.Addr = node1.Addr + 1; node2.Addr < NodeNum; node2.Addr++)
                {
                    Console.WriteLine(
                        "d({0},{1}) = {2,3}",
                        node1.Addr,
                        node2.Addr,
                        CalcDistanceBFS(node1, node2)
                    );
                    //Console.ReadKey();
                }
            }
        }
    }

    partial class Experiment<NodeType> where NodeType : ANode, new()
    {
        public void DEBUG_GenerateFaults()
        {
            Console.WriteLine("Debug \"GenerateFaults\"");

            for (double faultRatio = 0.0; faultRatio < 1.0; faultRatio += 0.1)
            {
                for (int i = 0; i < 100; i++)
                {
                    GenerateFaults(faultRatio);
                    var num = (int)(G.NodeNum * faultRatio);
                    for (int j = 0; j < G.NodeNum; j++)
                    {
                        if (FaultFlags[j]) num--;
                    }
                    if (num != 0)
                    {
                        Console.WriteLine("> NG");
                        return;
                    }
                }
            }
            Console.WriteLine("> OK");
        }
    }
}

#endif