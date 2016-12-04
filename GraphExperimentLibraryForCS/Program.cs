using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graph.Core;
using Graph.Debug;

namespace GraphExperimentLibraryForCS
{
    class Program
    {
        static void Main(string[] args)
        {
            // Crossed Cube クラスを実装して、動作が正しいか試してる

            // オブジェクトの生成
            int dim = 6;
            CrossedCube graph = new CrossedCube(dim);

            // 隣接の定義が正しいか確認
            //Graph.Debug.TestCodes.ShowNeighbors(graph);

            // ちゃんと連結か確認(幅探索で距離を求めてる)
            //Graph.Debug.TestCodes.ShowAllDistances(graph);

            for (UInt32 node2ID = 0; node2ID < graph.NodeNum; node2ID++)
            {
                Console.WriteLine(node2ID);
                BinaryNode node2 = new BinaryNode(node2ID);
                int[] distance = graph.CalcAllDistanceBFS(node2);

                for (UInt32 node1ID = 0; node1ID < graph.NodeNum; node1ID++)
                {
                    BinaryNode node1 = new BinaryNode(node1ID);
                    var fs = graph.GetFowardNeighbor(node1, node2);
                    //Console.WriteLine("({0}, {1})", node1ID, node2ID);
                    foreach (var f in fs)
                    {
                        BinaryNode fn = (BinaryNode)graph.GetNeighbor(node1, f);
                        if (distance[fn.ID] >= distance[node1.ID])
                        {
                            Console.WriteLine("  s   = {0}", Tools.UIntToBinStr(node1.Addr, dim, 2));
                            Console.WriteLine("  d   = {0}", Tools.UIntToBinStr(node2.Addr, dim, 2));
                            Console.WriteLine("s ^ d = {0}\n", Tools.UIntToBinStr((node1 ^ node2).Addr, dim, 2));
                            Console.WriteLine("ne {1,2} = {0}", Tools.UIntToBinStr(((BinaryNode)graph.GetNeighbor(node1, f)).Addr, dim, 2), f);
                            Console.WriteLine("------------------------------");
                            Console.ReadKey();
                        }
                    }

                    //if (distance[node1.ID] > 1)
                    /*
                    if (false)
                    {
                        var fns = graph.GetFowardNeighbor(node1, node2);

                        Console.WriteLine("  s   = {0}", Tools.UIntToBinStr(node1.Addr, 4, 2));
                        Console.WriteLine("  d   = {0}", Tools.UIntToBinStr(node2.Addr, 4, 2));
                        Console.WriteLine("s ^ d = {0}\n", Tools.UIntToBinStr((node1 ^ node2).Addr, 4, 2));

                        Console.WriteLine("正解の前方隣接頂点→");
                        for (int i = graph.GetDegree(node1) - 1; i >= 0; --i)
                        {
                            BinaryNode neighbor = (BinaryNode)graph.GetNeighbor(node1, i);
                            if (distance[neighbor.ID] < distance[node1.ID])
                            {
                                Console.WriteLine("ne {1,2} = {0}", Tools.UIntToBinStr(neighbor.Addr, 4, 2), i);
                            }
                        }
                        Console.WriteLine("アルゴリズム(仮)→");
                        foreach (var fn in fns)
                        {
                            Console.WriteLine("ne {1,2} = {0}", Tools.UIntToBinStr(((BinaryNode)graph.GetNeighbor(node1, fn)).Addr, 4, 2), fn);
                        }

                        Console.WriteLine("------------------------------");
                        if (fns.Count() > 0) Console.ReadKey();
                    }
                    */
                }
            }
            Console.WriteLine("\nend");
            Console.ReadKey();
        }
    }

}
