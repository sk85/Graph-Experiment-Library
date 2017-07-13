﻿#define DEBUG
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
            // test(new CrossedCube(10, 0), true);
            var g = new CrossedCube(1, 0);
            Debug.Check_CalcDistance(g, 2, 16);
        }

        // クロスとキューブの色々を表示
        static void test(AGraph g, bool f)
        {
            var rand = new Random();

            do
            {
                uint node1 = (uint)rand.Next((int)g.NodeNum);
                uint node2 = (uint)rand.Next((int)g.NodeNum);
                int[] forward = g.GetForwardNeighbor(node1, node2);
                Console.WriteLine(" u  = {0}", Debug.UintToBinaryString(node1, g.Dimension, f ? 2 : 32));
                Console.WriteLine(" v  = {0}", Debug.UintToBinaryString(node2, g.Dimension, f ? 2 : 32));
                Console.WriteLine("u^v = {0}", Debug.UintToBinaryString(node1 ^ node2, g.Dimension, f ? 2 : 32));
                Console.Write(" fn = ");
                for (int i = g.Dimension - 1; i >= 0; i--)
                {
                    Console.Write(forward[i]);
                    if (f && (i & 1) == 0) Console.Write(' ');
                }
                Console.WriteLine();
                Console.WriteLine("  距離  = {0}", g.CalcDistance(node1, node2));
                Console.WriteLine(" ﾊﾐﾝｸﾞ  = {0}", (uint)Debug.PopCount(node1^node2));
                Console.WriteLine(" 前方数 = {0}", forward.Count(x => x == 1));
                Console.WriteLine("---------------------------------------------------");
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
