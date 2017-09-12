﻿#define DEBUG
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
            var graphs = new AGraph[] {
                // new Hypercube(0, 0),
                // new CrossedCube(0, 0),
                new LocallyTwistedCube(0, 0),
                new MobiusCube(0, 0, 0),
                new MobiusCube(0, 0, 1),
                new SpinedCube(0, 0), };
            

            Experiment.E平均経路長と直径(graphs, 1, 15);
        }

        // 実験
        static void Experimentt(AGraph g, int trials, string path)
        {
            var pattern = 2;
            var successCount = new int[pattern, 10];
            var successCount2 = new int[10];
            var totalPathLength = new int[pattern, 10];
            var totalPathLength2 = new int[10];

            for (int faultRatio = 0; faultRatio < 10; faultRatio++)
            {
                Console.Write($"Fault ratio = {faultRatio,2}% ...");
                for (int i = 0; i < trials; i++)
                {
                    // 進捗の表示
                    if ((i * 100) % trials == 0)
                    {
                        Console.CursorLeft = 21;
                        Console.Write($"{(double)(i + 1) / trials:###%}");
                    }

                    // 故障率に従って故障をランダムに発生
                    g.GenerateFaults((double)faultRatio / 100);

                    // 連結な2ノードを取得
                    var p = g.GetExperimentParam();

                    // ルーティングを実行
                    var step = new int[] 
                    {
                        g.Routing(p.Node1, p.Node2, g.GetNext_Simple),
                        g.Routing(p.Node1, p.Node2, g.GetNext_Normal)
                    };

                    // 結果の保存
                    if (step[0] > 0)
                    {
                        successCount[0, faultRatio]++;
                        totalPathLength[0, faultRatio] += step[0];
                        if (step[1] > 0)
                        {
                            successCount2[faultRatio]++;
                            totalPathLength2[faultRatio] += step[1];

                        }
                    }
                    if (step[1] > 0)
                    {
                        successCount[1, faultRatio]++;
                        totalPathLength[1, faultRatio] += step[1];
                    }
                }
                // 進捗の表示
                Console.CursorLeft = 21;
                Console.WriteLine("100%");
            }

            // csv形式の文字列を作成
            var str = ",1%,2%,3%,4%,5%,6%,7%,8%,9%,10%,";
            for (int i = 0; i < pattern; i++)
            {
                str += $"\n到達率[{i}],";
                for (int j = 0; j < 10; j++)
                {
                    str += $"{(double)successCount[i, j] / trials},";
                }
            }
            for (int i = 0; i < pattern; i++)
            {
                str += $"\n平均経路長[{i}],";
                for (int j = 0; j < 10; j++)
                {
                    str += $"{(double)totalPathLength[i, j] / successCount[i, j]},";
                }
            }
            str += $"\n到達率[],";
            for (int j = 0; j < 10; j++)
            {
                str += $"{(double)successCount2[j] / successCount[0, j]},";
            }
            str += $"\n平均経路長[],";
            for (int j = 0; j < 10; j++)
            {
                str += $"{(double)totalPathLength2[j] / successCount[0, j]},";
            }

            // テキストファイルに書き出し
            File.WriteAllText(path, str, Encoding.GetEncoding("shift_jis"));
        }

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
        
    }
}
