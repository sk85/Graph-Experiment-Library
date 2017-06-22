#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphCS.Core;

namespace GraphCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var g = new LocallyTwistedCube(2, 0);
            Debug.Check_GetForwardNeighbor2(g, g.GetForwardNeighbor2);
        }

        static void ShowBitAndReldis(AGraph g)
        {
            var rand = new Random();
            do
            {
                uint node1 = (uint)rand.Next((int)g.NodeNum);
                uint node2 = (uint)rand.Next((int)g.NodeNum);
                var distance = g.CalcDistance(node1, node2);
                Console.WriteLine(Debug.UintToBinaryString(node1, g.Dimension, 32));
                Console.WriteLine(Debug.UintToBinaryString(node2, g.Dimension, 32));
                Console.WriteLine(Debug.UintToBinaryString(node1 ^ node2, g.Dimension, 32));
                for (int i = g.Dimension - 1; i >= 0; i--)
                {
                    Console.Write(g.CalcDistance(g.GetNeighbor(node1, i), node2) - distance + 1);
                }
                Console.WriteLine("\n-----------------");
                Console.ReadKey();

            } while (true);
        }
        

        static void T1(AGraph g)
        {
            var rand = new Random();
            do
            {
                // ノードをランダムに設定して表示
                uint node1 = 0b0000101;
                uint node2 = 0b1001101;
                //uint node1 = (uint)rand.Next((int)g.NodeNum);
                //uint node2 = (uint)rand.Next((int)g.NodeNum);
                Console.WriteLine(Debug.UintToBinaryString(node1, g.Dimension, 32));
                Console.WriteLine(Debug.UintToBinaryString(node2, g.Dimension, 32));
                Console.WriteLine(Debug.UintToBinaryString(node1 ^ node2, g.Dimension, 32));

                // 基準距離を計算
                var distance = g.CalcDistance(node1, node2);
                Console.WriteLine(distance);

                // 相対距離ベクトルを計算して表示
                var relVec1 = new int[g.Dimension];
                for (int i = g.Dimension - 1; i >= 0; i--)
                {
                    relVec1[i] = g.CalcDistance(g.GetNeighbor(node1, i), node2) - distance + 1;
                    Console.Write(relVec1[i]);
                }
                Console.WriteLine();

                // テスト
                var relVec2 = ((LocallyTwistedCube)g).Test(node1, node2);

                // 結果の表示
                if (relVec2 == null)
                {
                    Console.WriteLine("\n-----------------");
                    continue;
                }
                for (int i = g.Dimension - 1; i >= 0; i--)
                {
                    Console.Write(relVec2[i]);
                }
                Console.WriteLine("\n-----------------");

                // 結果が間違っていたら停止
                if (!relVec1.SequenceEqual(relVec2))
                {
                    Console.ReadKey();
                }
            } while (true);
        }
    }
}
