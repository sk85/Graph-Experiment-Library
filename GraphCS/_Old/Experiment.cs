using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using GraphCS.Old.Core;

namespace GraphCS.Old
{
    static class Experiment3
    {
        /// <summary>
        /// 基本的なルーティング．いつやったかは忘れた
        /// </summary>
        /// <param name="g"></param>
        /// <param name="trials"></param>
        /// <param name="path"></param>
        public static void 基本的なルーティングの実験(AGraph g, int trials, string path)
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
                    g.GetExperimentParam(out uint node1, out uint node2);

                    // ルーティングを実行
                    var step = new int[]
                    {
                        g.Routing(node1, node2, g.GetNext_Simple),
                        g.Routing(node1, node2, g.GetNext_Normal)
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

        // 17/09/06
        // 色々なグラフの平均経路長と直径を計算
        // 各グラフはCalcDistanceをオーバーライドしておかないと計算量が大きい
        public static void 平均経路長と直径(AGraph[] gs, int minDim, int maxDim)
        {
            // 出力先フォルダを作成
            string date = System.Text.RegularExpressions.Regex.Replace(
                DateTime.Now.ToString(),
                @"(?:\d\d)?(?<year>\d\d)/(?<month>\d\d?)/(?<day>\d\d?)\s(?<hour>\d\d?):(?<minute>\d\d?):(?<second>\d\d?)",
               "${year}${month}${day}_${hour}_${minute}_${second}");
            string path = $"../../Output/DistanseaverageAndDiameter_{date}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 計算と出力
            for (int i = 0; i < gs.Length; i++)
            {
                Console.WriteLine(gs[i].Name);
                for (int dim = minDim; dim <= maxDim; dim++)
                {
                    var start = DateTime.Now;
                    Console.Write($"  n = {dim,2}   {DateTime.Now}");
                    // 計算
                    gs[i].Dimension = dim;
                    gs[i].CalcDistanceaverageAndDiameter(out var distance, out var diameter);

                    // ファイルに出力
                    var sw = new StreamWriter(
                        $"{path}/{gs[i].Name}.csv",
                        true,
                        Encoding.GetEncoding("shift_jis"));
                    sw.WriteLine($"{dim},{distance},{diameter}");
                    sw.Close();

                    Console.WriteLine($"   {(DateTime.Now - start).TotalMilliseconds}msec.");
                }
            }
        }
        
        /// <summary>
        /// 17/09/14
        /// クロストキューブにおいて，候補頂点を増やすことによる効果
        /// (プロジェクトのホーム)/Candidate_(日付)/...csv に出力
        /// </summary>
        /// <param name="g"></param>
        /// <param name="timeoutLimit"></param>
        /// <param name="trials"></param>
        public static void 候補頂点を増やすことによる効果(CrossedCube g, int timeoutLimit, int trials)
        {
            // 出力先パス
            // 以下にcsvで出力
            var date = System.Text.RegularExpressions.Regex.Replace(
                DateTime.Now.ToString(),
                @"(?:\d\d)?(?<year>\d\d)/(?<month>\d\d?)/(?<day>\d\d?)\s(?<hour>\d\d?):(?<minute>\d\d?):(?<second>\d\d?)",
               "${year}${month}${day}_${hour}_${minute}_${second}");
            var dir = $"../../Output/Candidate_{g.Dimension}dim_{date}";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // 結果を保持する配列
            var result = new int[4, 11, 3]; // [種類], [故障率], [総ステップ数，-1の数，-2の数]

            // 実験本体
            for (int faultRatio = 0; faultRatio <= 10; faultRatio++)
            {
                // ルーティング実行部
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
                    g.GetExperimentParam(out uint node1, out uint node2);

                    // ルーティングを実行
                    var steps = new int[]
                    {
                        g.ChangRouting(node1, node2, timeoutLimit, false),
                        g.ChangRouting(node1, node2, timeoutLimit, true),
                        g.ProposedRouting(node1, node2, timeoutLimit, false),
                        g.ProposedRouting(node1, node2, timeoutLimit, true),
                    };

                    // 結果を保持
                    for (int k = 0; k < steps.Length; k++)
                    {
                        if (steps[k] > 0)
                        {
                            result[k, faultRatio, 0] += steps[k];
                        }
                        else
                        {
                            result[k, faultRatio, -steps[k]]++;
                        }
                    }
                }

                // 結果をファイルに出力1
                for (int k = 0; k < 4; k++)
                {
                    using (var sw = new StreamWriter($"{dir}/case{k}.csv", true, Encoding.GetEncoding("shift_jis")))
                    {
                        sw.WriteLine($"{faultRatio},{result[k, faultRatio, 0]},{result[k, faultRatio, 1]},{result[k, faultRatio, 2]}");
                    }
                }

                // 進捗の表示
                Console.CursorLeft = 21;
                Console.WriteLine("100%");
            }

            // 結果をファイルに出力2
            using (var sw = new StreamWriter($"{dir}/total.csv", false, Encoding.GetEncoding("shift_jis")))
            {
                sw.WriteLine($"trials:,{trials},");
                for (int i = 0; i < 3; i++)
                {
                    sw.WriteLine(",0%,1%,2%,3%,4%,5%,6%,7%,8%,9%,10%,");
                    for (int j = 0; j < 4; j++)
                    {
                        sw.Write($"case{j},");
                        for (int k = 0; k <= 10; k++)
                        {
                            sw.Write($"{result[j, k, i]},");
                        }
                        sw.WriteLine();
                    }
                    sw.WriteLine();
                }
            }
        }
        
        // 17/09/19
        // クロストキューブの対称性を検証
        // 頂点ごとの平均経路長を計算
        public static void 頂点ごとの平均経路長(AGraph g)
        {
            int maxDim = 16;

            for (int dim = 2; dim <= maxDim; dim++)
            {
                Console.WriteLine($"n = {dim,2}");
                g.Dimension = dim;
                var mat = new TriangleMatrix((int)g.NodeNum);
                for (uint node1 = 0; node1 < g.NodeNum; node1++)
                {
                    for (uint node2 = node1 + 1; node2 < g.NodeNum; node2++)
                    {
                        mat[node1, node2] = (char)g.CalcDistance(node1, node2);
                    }
                }

                for (uint node1 = 0; node1 < g.NodeNum; node1++)
                {
                    int sum = 0;
                    for (uint node2 = 0; node2 < g.NodeNum; node2++)
                    {
                        if (node1 != node2) sum += mat[node1, node2];
                    }
                    Console.Write($"{sum} ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 17/10/16
        /// ルーティング実験
        /// (プロジェクトのホーム)/Routing_(日付)/...csv に出力
        /// </summary>
        /// <param name="g">グラフ</param>
        /// <param name="timeoutLimit">タイムアウト上限</param>
        /// <param name="trials">試行回数</param>
        public static void RoutingExperiment(AGraph g, Func<uint, uint, int, bool, int>[] routings, int timeoutLimit)
        {
            g.GetExperimentParam(out uint node1, out uint node2);
            foreach (var routing in routings)
            {
                var step = routing(node1, node2, timeoutLimit, false);
            }
        }

        class TriangleMatrix
        {
            private char[] Ary;
            private int Size;

            public TriangleMatrix(int size)
            {
                Size = size;
                Ary = new char[(size + 1) * size / 2];
            }
            
            public char this[uint i, uint j]
            {
                get
                {
                    if (i == j)
                    {
                        throw new IndexOutOfRangeException("同じ数字を引数にしてはだめ");
                    }
                    if (i > j)
                    {
                        var tmp = i;
                        i = j;
                        j = tmp;
                    }
                    return Ary[i * (Size * 2 - (i - 1)) / 2 + j - i - 1];
                }
                set
                {
                    Ary[i * (Size * 2 - (i - 1)) / 2 + j - i - 1] = value;
                }
            }
        }
    }
}
