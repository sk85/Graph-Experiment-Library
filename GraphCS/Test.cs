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
    static class Test
    {
        // CQでルーティング実験
        public static void T171027()
        {
            var CQ = new CrossedCube(2);
            var exp = new Experiment<BinaryNode>(CQ, 0);
            Test1<BinaryNode>(100000, 0.01, 0.10, 0.01, exp);
        }

        /// <summary>
        /// Routing_171025でルーティングを行い、
        /// 故障率,成功率,平均経路長,成功回数,合計経路長,タイムアウト回数,袋小路回数
        /// をcsvで出力
        /// </summary>
        public static void Test1<NodeType>
            (int trials, double minRatio, double maxRatio, double ratioInterval, Experiment<NodeType> exp)
            where NodeType : ANode, new()
        {
            // 出力先パス
            var dir = $"../../Output/Routing/";
            var path = dir + $"{DateTime.Now.ToString("yyMMddHHmmss")}[{exp.G.Dimension}]{exp.G.Name}.csv";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            Console.CursorVisible = false;
            Console.WriteLine($"Start Routing_171025");
            Console.WriteLine($"> {exp.G.Dimension}-dimensional {exp.G.Name}");

            var sw = new StreamWriter(path, true, Encoding.GetEncoding("shift_jis"));
            sw.WriteLine($"試行回数,{trials}");
            sw.WriteLine($"故障率,成功率,平均経路長,成功回数,合計経路長,タイムアウト回数,袋小路回数,");
            for (double faultRatio = minRatio; faultRatio <= maxRatio; faultRatio += ratioInterval)
            {
                Console.WriteLine($"> Fault ratio = {faultRatio}");

                exp.FaultRatio = faultRatio;
                int successCount = 0, timeoutCount = 0, deadCount = 0, totalStep = 0;
                for (int i = 0; i < trials; i++)
                {
                    Console.CursorLeft = 4;
                    Console.Write($"{i + 1} / {trials}");
                    exp.Next();
                    var step = exp.Routing(exp.G.Dimension);

                    // 結果の保存
                    if (step > 0)
                    {
                        successCount++;
                        totalStep += step;
                    }
                    else if (step == -1)
                    {
                        deadCount++;
                    }
                    else if (step == -2)
                    {
                        timeoutCount++;
                    }
                }

                // 結果の出力
                sw.WriteLine("{0},{1},{2},{3},{4},{5},{6},"
                    ,faultRatio, (double)successCount / trials, (double)totalStep / successCount
                    ,successCount, totalStep, timeoutCount, deadCount);

                Console.WriteLine(); ;
            }
            sw.Close();
            Console.CursorVisible = true;
        }

        /// <summary>
        /// クロストキューブ相対距離の計算アルゴリズムの実行速度を確認。
        /// 本当にO(n)でできているか
        /// </summary>
        /// <returns></returns>
        public static void Test171026()
        {
            var g = new CrossedCube(2);
            int N = 100000;
            var sw1 = new System.Diagnostics.Stopwatch();
            var sw2 = new System.Diagnostics.Stopwatch();

            var stream = new StreamWriter("../../相対距離計算時間.csv", false);
            stream.WriteLine($"{N}回分の実行時間(ms.)");
            Console.WriteLine($"{N}回分の実行時間(ms.)");
            stream.WriteLine($"次元数,提案手法,比較対象");
            Console.WriteLine($"次元数,提案手法,比較対象");

            for (int dim = 2; dim < 31; dim++)
            {
                // 頂点ペアをN個作る
                var nodes1 = new BinaryNode[N];
                var nodes2 = new BinaryNode[N];
                var rand = new Random();
                for (int i = 0; i < N; i++)
                {
                    nodes1[i].Addr = rand.Next(g.NodeNum);
                    nodes2[i].Addr = rand.Next(g.NodeNum);
                }

                // 比較対象
                sw1.Start();
                for (int i = 0; i < N; i++)
                {
                    var relDis = new int[g.Dimension];
                    for (int k = 0; k < g.Dimension; k++)
                    {
                        relDis[k] = g.CalcDistance(g.GetNeighbor(nodes1[i], k), nodes2[i]);
                    }
                }
                sw1.Stop();

                // 提案手法
                sw2.Start();
                for (int i = 0; i < N; i++)
                {
                    g.CalcRelativeDistance(nodes1[i], nodes2[i]);
                }
                sw2.Stop();

                // 出力
                stream.WriteLine($"{dim},{sw2.ElapsedMilliseconds},{sw1.ElapsedMilliseconds}");
                Console.WriteLine($"{dim},{sw2.ElapsedMilliseconds},{sw1.ElapsedMilliseconds}");
            }
        }

        /// <summary>
        /// Efeらの手法を含めた実験。
        /// 迂回の有無、ランダムの有無も比較する。
        /// </summary>
        public static void Test171104(int dim)
        {
            int trials = 100;
            double minRatio = 0.01;
            double maxRatio = 0.20;
            double ratioInteval = 0.01;
            int cases = 7;

            // 実験オブジェクト
            var exp = new Experiment<BinaryNode>(new CrossedCube(dim), 0);

            // 出力先パス
            var dir = $"../../Output/CQ_Routing/";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var path = dir + $"{DateTime.Now.ToString("yyMMddHHmmss")}_{exp.G.Dimension}_{minRatio}-{maxRatio}.csv";

            // 結果の一時保存先
            var steps = new int[cases, 20, exp.G.Dimension * 2 + 3];
            var success = new int[cases];

            // 実験本体
            double faultRatio = 0.01;
            for (int f = 0; faultRatio < maxRatio + ratioInteval; faultRatio = minRatio + ratioInteval * f)
            {
                Console.WriteLine($"> Fault ratio = {faultRatio}");
                exp.FaultRatio = faultRatio;

                for (int i = 0; i < trials; i++)
                {
                    // 進捗の表示
                    Console.CursorLeft = 4;
                    Console.Write($"{i + 1} / {trials}");

                    // パラメタの更新
                    exp.Next();

                    int step;

                    step = exp.Routing_NoDetour();
                    steps[0, (int)faultRatio * 100 - 1, step + 2]++;

                    step = exp.Routing_NoBackTrack_NoRandom();
                    steps[1, (int)faultRatio * 100 - 1, step + 2]++;

                    step = exp.Routing_NoBackTrack(dim * 2);
                    steps[2, (int)faultRatio * 100 - 1, step + 2]++;

                    step = exp.Routing_NoRandom();
                    steps[3, (int)faultRatio * 100 - 1, step + 2]++;

                    step = exp.Routing(dim * 2);
                    steps[4, (int)faultRatio * 100 - 1, step + 2]++;

                    step = ((CrossedCube)exp.G).Efe_Routing_NoDetour(exp.SourceNode, exp.DestinationNode, exp.FaultFlags);
                    steps[5, (int)faultRatio * 100 - 1, step + 2]++;

                    step = ((CrossedCube)exp.G).Efe_Routing(exp.SourceNode, exp.DestinationNode, exp.FaultFlags, dim * 2);
                    steps[6, (int)faultRatio * 100 - 1, step + 2]++;
                }
                Console.WriteLine();
            }

            // csvに保存
            var names = new string[]
            {
                "Routing_NoDetour",
                "Routing_NoBackTrack_NoRandom",
                "Routing_NoBackTrack",
                "Routing_NoRandom",
                "Routing",
                "Efe_Routing_NoDetour",
                "Efe_Routing",
            };
            using (var sw = new StreamWriter(path, false, Encoding.GetEncoding("Shift-jis")))
            {

                for (int i = 0; i < names.Length; i++)
                {
                    sw.WriteLine(names[i]);
                    sw.Write("fault ratio,timeout,fault,");
                    for (int j = 0; j < dim * 2 + 2; j++) sw.Write("{0},", j);
                    sw.WriteLine();
                    for (int j = 0; j < 20; j++)
                    {
                        sw.Write("{0}%,", j + 1);
                        for (int k = 0; k < dim * 2 + 2; k++)
                        {
                            sw.Write("{0},", steps[i, j, k]);
                        }
                        sw.WriteLine();
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}
