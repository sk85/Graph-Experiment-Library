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
