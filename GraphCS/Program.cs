#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GraphCS.Core;
using GraphCS.Graphs;

namespace GraphCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var g = new CrossedCube(2);
            

            // 時間の計測
            using(var sw = new StreamWriter("../../test2.csv", false))
            {
                for (int dim = 2; dim < 31; dim++)
                {
                    g.Dimension = dim;
                    var str = g.CmpSpeed_CalcRelativeDistance();
                    Console.WriteLine(str);
                    sw.WriteLine(str);
                }
            }
            Console.ReadKey();
        }

        /*
        static void Check_CalcRelativeDistance(AGraph g, int minDim, int maxDim)
        {
            for (int dim = minDim; dim <= maxDim; dim++)
            {
                g.Dimension = dim;
                for (uint node1 = 0; node1 < g.NodeNum; node1++)
                {
                    if (node1 % 10 == 0)
                    {
                        Console.CursorLeft = 7;
                        Console.Write($"{(double)(node1 + 1) / g.NodeNum:###%}");
                    }
                    for (uint node2 = 0; node2 < g.NodeNum; node2++)
                    {
                        // 正解を計算
                        var r1 = new int[dim];
                        var d = g.CalcDistance(node1, node2);
                        for (int i = 0; i < dim; i++)
                            r1[i] = g.CalcDistance(g.GetNeighbor(node1, i), node2) - d;

                        // 計算
                        var r2 = g.CalcRelativeDistance(node1, node2);

                        // 比較して異なっていたら表示
                        if (!r1.SequenceEqual(r2))
                        {
                            Console.WriteLine($"-----------------------------");
                            Console.WriteLine(" u  = {0}", Debug.UintToBinaryString(node1, g.Dimension, 2));
                            Console.WriteLine(" v  = {0}", Debug.UintToBinaryString(node2, g.Dimension, 2));

                            Console.Write(" r1 = ");
                            for (int i = dim - 1; i >= 0; i--) Console.Write($" {r1[i],2}");
                            Console.WriteLine();

                            Console.Write(" r2 = ");
                            for (int i = dim - 1; i >= 0; i--) Console.Write($" {r2[i],2}");
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        // クロスとキューブの色々を表示
        static void test(AGraph g, bool f)
        {
            var rand = new Random();

            do
            {
                uint node1 = (uint)rand.Next((int)g.NodeNum);
                uint node2 = (uint)rand.Next((int)g.NodeNum);
                int[] forward = g.CalcForwardNeighbor(node1, node2);
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
        */
    }
}
