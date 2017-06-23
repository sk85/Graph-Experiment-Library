using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using GraphCS.Core;

static class Debug
{
    /// <summary>
    /// uintを二進数列に変換
    /// </summary>
    /// <param name="u">uint</param>
    /// <param name="length">二進数列の長さ</param>
    /// <param name="interval">何文字おきに空白を入れるか</param>
    /// <returns>二進数列</returns>
    public static string UintToBinaryString(uint u, int length, int interval)
    {
        string str = "";
        for (int i = length - 1; i >= 0; i--)
        {
            str += $"{(u & (1 << i)) >> i}";
            if (i > 0 && i % interval == 0) str += ' ';
        }
        return str;
    }

    /// <summary>
    /// GetForwardNeighborの動作確認。
    /// すべての頂点ペアをチェック
    /// </summary>
    /// <param name="g"></param>
    public static void Check_GetForwardNeighbor1(AGraph g)
    {
        for (uint node1 = 0; node1 < g.NodeNum; node1++)
        {
            for (uint node2 = 0; node2 < g.NodeNum; node2++)
            {
                // 基準距離を計算
                var distance = g.CalcDistance(node1, node2);

                // 正しい解を計算
                var ary1 = new int[g.Dimension];
                for (int i = 0; i < g.Dimension; i++)
                {
                    ary1[i] = g.CalcDistance(g.GetNeighbor(node1, i), node2) < distance ? 1 : 0;
                }

                // GetForwardNeighbor
                var ary2 = g.GetForwardNeighbor(node1, node2);

                // 間違っていたら情報を表示して停止
                if (ary1.SequenceEqual(ary2))
                {
                    Console.WriteLine($"({UintToBinaryString(node1, g.Dimension, 32)},{UintToBinaryString(node2, g.Dimension, 32)})...ok");
                }
                else
                {
                    Console.WriteLine($"d(u, v) = {distance}");
                    Console.WriteLine($" u  : {UintToBinaryString(node1, g.Dimension, 32)}");
                    Console.WriteLine($" v  : {UintToBinaryString(node2, g.Dimension, 32)}");
                    Console.WriteLine($"u^v : {UintToBinaryString(node1 ^ node2, g.Dimension, 32)}");
                    Console.Write("ans :");
                    for (int i = g.Dimension - 1; i >= 0; i--) Console.Write(ary1[i]);
                    Console.Write("\n    :");
                    for (int i = g.Dimension - 1; i >= 0; i--) Console.Write(ary2[i]);
                }
            }
        }
    }

    public static void Check_GetForwardNeighbor2(AGraph g, Func<uint, uint, int[]> f)
    {
        var sw1 = new Stopwatch();
        var sw2 = new Stopwatch();
        for (int dim = 2; dim <= 13; dim++)
        {
            g.Dimension = dim;
            for (uint node1 = 0; node1 < g.NodeNum; node1++)
            {
                Console.WriteLine($"n = {dim}, node1 = {UintToBinaryString(node1, dim, dim)}");
                for (uint node2 = 0; node2 < g.NodeNum; node2++)
                {
                    // GetForwardNeighbor
                    f(node1, node2);

                    sw1.Start();
                    var ary1 = g.GetForwardNeighbor(node1, node2);
                    sw1.Stop();

                    sw2.Start();
                    var ary2 = f(node1, node2);
                    sw2.Stop();

                    // 間違っていたら情報を表示して停止
                    if (!ary1.SequenceEqual(ary2))
                    {
                        Console.WriteLine($"d(u, v) = {g.CalcDistance(node1, node2)}");
                        Console.WriteLine($" u  : {UintToBinaryString(node1, g.Dimension, 32)}");
                        Console.WriteLine($" v  : {UintToBinaryString(node2, g.Dimension, 32)}");
                        Console.WriteLine($"u^v : {UintToBinaryString(node1 ^ node2, g.Dimension, 32)}");
                        Console.Write("ans :");
                        for (int i = g.Dimension - 1; i >= 0; i--) Console.Write(ary1[i]);
                        Console.Write("\n    :");
                        for (int i = g.Dimension - 1; i >= 0; i--) Console.Write(ary2[i]);
                        Console.WriteLine("\n");
                    }
                }
            }
        }

        Console.WriteLine($"元々の : {sw1.Elapsed}");
        Console.WriteLine($"新しい : {sw2.Elapsed}");

        Console.WriteLine($"　差　 : {sw1.Elapsed - sw2.Elapsed}");
    }

    /// <summary>
    /// CalcDistanceの動作確認
    /// </summary>
    /// <param name="g">グラフ</param>
    public static void Check_CalcDistance(AGraph g, int minDim, int maxDim)
    {
        for (int dim = minDim; dim < maxDim ; dim++)
        {
            g.Dimension = dim;
            Console.Write($"n = {dim}");
            for (uint node1 = 0; node1 < g.NodeNum; node1++)
            {
                for (uint node2 = 0; node2 < g.NodeNum; node2++)
                {
                    int d1 = g.CalcDistanceBFS(node1, node2);
                    int d2 = g.CalcDistance(node1, node2);
                    if (d1 != d2)
                    {
                        Console.WriteLine(
                            "\nd({0},{1}) = {2,2} / {3,2}",
                            UintToBinaryString(node1, 16, 4),
                            UintToBinaryString(node2, 16, 4),
                            d1,
                            d2
                        );
                    }
                }
            }
            Console.WriteLine("...ok");
        }
    }

    /// <summary>
    /// ２ノード間の距離を列挙
    /// </summary>
    /// <param name="g">グラフ</param>
    /// <param name="dim">次元数</param>
    public static void ShowDistance(AGraph g, int dim)
    {
        for (uint node1 = 0; node1 < g.NodeNum; node1++)
        {
            for (uint node2 = 0; node2 < g.NodeNum; node2++)
            {
                Console.WriteLine(
                    "d({0},{1}) = {2,3}",
                    UintToBinaryString(node1, dim, dim),
                    UintToBinaryString(node2, dim, dim),
                    g.CalcDistanceBFS(node1, node2)
                );
                //Console.ReadKey();
            }
        }
    }

    public static int PopCount(uint bits)
    {
        bits = (bits & 0x55555555) + (bits >> 1 & 0x55555555);
        bits = (bits & 0x33333333) + (bits >> 2 & 0x33333333);
        bits = (bits & 0x0f0f0f0f) + (bits >> 4 & 0x0f0f0f0f);
        bits = (bits & 0x00ff00ff) + (bits >> 8 & 0x00ff00ff);
        return (int)((bits & 0x0000ffff) + (bits >> 16 & 0x0000ffff));
    }

    // 隣接頂点を列挙
    public static void test1(AGraph g)
    {
        for (uint node = 0; node < g.NodeNum; node++)
        {
            Console.WriteLine($" u  = {UintToBinaryString(node, 16, 4)}");
            for (int i = 0; i < g.GetDegree(node); i++)
            {
                Console.WriteLine($"u^{i} = {UintToBinaryString(g.GetNeighbor(node, i), 16, 4)}");
            }
            Console.WriteLine("-------------------------------");
            Console.ReadKey();
        }
    }
}
