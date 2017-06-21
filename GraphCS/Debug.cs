using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// <summary>
    /// CalcDistanceの動作確認
    /// </summary>
    /// <param name="g">グラフ</param>
    public static void Check_CalcDistance(AGraph g)
    {
        for (uint node1 = 0; node1 < g.NodeNum; node1++)
        {
            for (uint node2 = 0; node2 < g.NodeNum; node2++)
            {
                int d1 = g.CalcDistanceBFS(node1, node2);
                int d2 = g.CalcDistance(node1, node2);
                Console.WriteLine(
                    "d({0},{1}) = {2,2} / {3,2}",
                    UintToBinaryString(node1, 16, 4),
                    UintToBinaryString(node2, 16, 4),
                    d1,
                    d2
                );
                if (d1 != d2) Console.ReadKey();
            }
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

    // 2頂点間の距離を列挙
    public static void test2(AGraph g)
    {
    }
}
