using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graph.Core;

namespace Graph.Debug
{
    /// <summary>
    /// デバッグ用に表示したりするコード類をここにまとめてあります。
    /// ほんとは基本グラフクラスに内蔵しておいて#ifdef DEBUGでくくる、
    /// みたいにした方がいいとは思うんだけどとりあえずで。
    /// </summary>
    static class TestCodes
    {
        /// <summary>
        /// グラフの各頂点の隣接頂点をコンソールに出力します。
        /// キューブ用。
        /// </summary>
        /// <param name="graph">対象のグラフ</param>
        public static void ShowNeighbors(AGraph graph)
        {
            Console.WriteLine("グラフの各頂点の隣接頂点をコンソールに出力します。");
            Console.WriteLine("1つの出発頂点ごとに止まるので、何かキーを押して進めてください。");
            BinaryNode node = new BinaryNode(0);
            for (UInt32 nodeID = 0; nodeID < graph.NodeNum; nodeID++)
            {
                node.ID = nodeID;
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine(Tools.UIntToBinStr(node.Addr, 32, 4) + '\n');
                for (int i = 0; i < graph.GetDegree(node); i++)
                {
                    Console.WriteLine(Tools.UIntToBinStr(((BinaryNode)graph.GetNeighbor(node, i)).Addr, 32, 4));
                }
                Console.ReadKey();
            }
        }

        /// <summary>
        /// グラフの各頂点からの距離をコンソールに出力します。
        /// キューブ用。
        /// </summary>
        /// <param name="graph">対象のグラフ</param>
        public static void ShowAllDistances(AGraph graph)
        {
            Console.WriteLine("グラフの各頂点からの距離をコンソールに出力します。");
            Console.WriteLine("1つの出発頂点ごとに止まるので、何かキーを押して進めてください。");
            for (UInt32 node1 = 0; node1 < graph.NodeNum; node1++)
            {
                int[] array = graph.CalcAllDistanceBFS(new BinaryNode(node1));
                for (UInt32 node2 = 0; node2 < graph.NodeNum; node2++)
                {
                    Console.WriteLine("d({0,2}, {1,2}) = {2}", node1, node2, array[node2]);
                    Console.ReadKey();
                }
                Console.ReadKey();
            }
        }
    }
}
