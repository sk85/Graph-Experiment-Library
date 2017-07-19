#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GraphCS.Core;

namespace GraphCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var g = new CrossedCube(0, 0);
            for (int dim = 10; dim <= 15; dim++)
            {
                Console.WriteLine($"n = {dim}%");
                g.Dimension = dim;
                Experiment(g, 10000, $@"..\..\{dim}.csv");
                Console.WriteLine($"-----------------------------");
            }
        }

        // 実験
        static void Experiment(AGraph g, int trials, string path)
        {
            var successCount = new int[10];
            var totalPathLength = new int[10];

            for (int faultRatio = 0; faultRatio < 10; faultRatio++)
            {
                Console.Write($"Fault ratio = {faultRatio + 1,2}% ...");
                for (int i = 0; i < trials; i++)
                {
                    if ((i * 100) % trials == 0)
                    {
                        Console.CursorLeft = 21;
                        Console.Write($"{(double)(i + 1) / trials:###%}");
                    }
                    // 故障率に従って故障をランダムに発生
                    g.GenerateFaults((double)(faultRatio + 1) / 100);

                    // 連結な2ノードを取得
                    var p = g.GetExperimentParam();

                    // ルーティングを実行
                    var step = g.Routing(p.Node1, p.Node2, g.GetNext_Simple);

                    if (step > 0)
                    {
                        successCount[faultRatio]++;
                        totalPathLength[faultRatio] += step;
                    }
                }
                Console.CursorLeft = 21;
                Console.WriteLine("100%");
            }

            // csv形式の文字列を作成
            var str = ",1%,2%,3%,4%,5%,6%,7%,8%,9%,10%,";
            str += "\n到達率,";
            for (int i = 0; i < 10; i++)
            {
                str += $"{(double)successCount[i] / trials},";
            }
            str += "\n平均経路長,";
            for (int i = 0; i < 10; i++)
            {
                str += $"{(double)totalPathLength[i] / trials},";
            }

            // テキストファイルに書き出し
            File.WriteAllText(path, str, Encoding.GetEncoding("shift_jis"));
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
        
    }
}
