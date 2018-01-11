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
        public static void Test171104()
        {
            // 実験パラメタ
            int dim = 15;
            int timeoutLimit = 20;
            double minRatio = 0.00;
            double ratioInterval = 0.01;
            int ratioNum = 21;
            int trialCount = 10000;

            // 実験オブジェクト
            var cq = new CrossedCube(dim);
            var exp = new Experiment<BinaryNode>(cq, 0);

            // 出力先パス
            var dir = $"../../Output/CQ_Routing/";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var path = dir + $"{DateTime.Now.ToString("yyMMddHHmmss")}_{exp.G.Dimension}_{minRatio}-{ratioNum}.csv";

            // 結果の一時保存先
            var result = new int[6, ratioNum, timeoutLimit + 1];

            // 実験本体
            for (int ratio = 0; ratio < ratioNum; ratio++)
            {
                exp.FaultRatio = minRatio + ratioInterval * ratio;
                Console.WriteLine($"> Fault ratio = {exp.FaultRatio:0.00}");

                for (int trial = 0; trial < trialCount; trial++)
                {
                    // 進捗の表示
                    Console.CursorLeft = 4;
                    Console.Write($"{trial + 1} / {trialCount}");

                    // パラメタの更新
                    exp.Next();

                    int step;
                    // 迂回なし２種
                    {
                        step = cq.Efe_Routing_NoDetour(exp.SourceNode, exp.DestinationNode, exp.FaultFlags);
                        if (step > 0) result[0, ratio, 0]++;

                        step = exp.Routing_NoDetour();
                        if (step > 0) result[1, ratio, 0]++;
                    }

                    // 迂回ありランダムなし２種， 迂回ありランダムあり２種
                    {
                        var s = new int[]
                        {
                            cq.Efe_Routing_NoRandom
                                (exp, timeoutLimit),
                            exp.Routing_NoRandom(timeoutLimit),
                            cq.Efe_Routing
                                (exp, timeoutLimit),
                            exp.Routing(timeoutLimit),
                        };
                        for (int type = 0; type < 4; type++)
                        {
                            result[type + 2, ratio, s[type] > 0 ? s[type] - 1 : timeoutLimit]++;
                        }
                    }
                }
                Console.WriteLine();
            }

            // 出力
            var str = $"Trial count, {trialCount},\n";

            for (int ratio = 0; ratio < ratioNum; ratio++) str += $",{minRatio + ratioInterval * ratio}";
            for (int type = 0; type < 2; type++)
            {
                str += $"\nCase{type + 1},";
                for (int ratio = 0; ratio < ratioNum; ratio++)
                    str += $"{(double)result[type, ratio, 0] / trialCount},";
            }

            for (int type = 2; type < 6; type++)
            {
                str += $"\n";
                for (int ratio = 0; ratio < ratioNum; ratio++)
                    str += $",{minRatio + ratioInterval * ratio}";

                for (int step = 0; step < timeoutLimit + 1; step++)
                {
                    str += $"\n{step + 1},";
                    for (int ratio = 0; ratio < ratioNum; ratio++)
                    {
                        str += $"{result[type, ratio, step]},";
                    }
                }
            }

            File.AppendAllText(path, str);
        }


        public static void Test171119()
        {
            // 実験パラメタ
            int dim = 15;
            int timeoutLimit = 20;
            double minRatio = 0.00;
            double ratioInterval = 0.01;
            int ratioNum = 21;
            int trialCount = 10000;

            // 実験オブジェクト
            var cq = new CrossedCube(dim);
            var exp = new Experiment<BinaryNode>(cq, 0);

            // 出力先パス
            var dir = $"../../Output/CQ_Routing/";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var path = dir + $"{DateTime.Now.ToString("yyMMddHHmmss")}_{exp.G.Dimension}_{minRatio}-{ratioNum}.csv";

            // 結果の一時保存先
            var result = new int[5, ratioNum, timeoutLimit + 1];

            // 実験本体
            for (int ratio = 0; ratio < ratioNum; ratio++)
            {
                exp.FaultRatio = minRatio + ratioInterval * ratio;
                Console.WriteLine($"> Fault ratio = {exp.FaultRatio:0.00}");

                for (int trial = 0; trial < trialCount; trial++)
                {
                    // 進捗の表示
                    Console.CursorLeft = 4;
                    Console.Write($"{trial + 1} / {trialCount}");

                    // パラメタの更新
                    exp.Next();

                    // 距離を計算
                    int distance = exp.CalcDistance();

                    // ルーティング実験部
                    var s = new int[]
                    {
                        cq.Efe_Routing_NoRandom(exp, timeoutLimit),
                        cq.Efe_Routing_NoRandom2(exp, timeoutLimit),
                        exp.Routing_NoRandom(timeoutLimit),
                        cq.Efe_Routing(exp, timeoutLimit),
                        exp.Routing(timeoutLimit),
                    };
                    for (int type = 0; type < 5; type++)
                    {
                        result[type, ratio, s[type] > 0 ? s[type] - distance : timeoutLimit]++;
                    }
                }
                Console.WriteLine();
            }

            // 出力
            var str = $"Trials, {trialCount}\n";
            for (int type = 0; type < 5; type++)
            {
                for (int ratio = 0; ratio < ratioNum; ratio++)
                    str += $",{minRatio + ratioInterval * ratio}";

                for (int step = 0; step < timeoutLimit + 1; step++)
                {
                    str += $"\n{step},";
                    for (int ratio = 0; ratio < ratioNum; ratio++)
                    {
                        str += $"{result[type, ratio, step]},";
                    }
                }
                str += $"\n";
            }

            File.AppendAllText(path, str);
        }

        // 前方を見つけられる確率
        // 前方が見つからない場合はランダムで、最短経路で到達できる確率
        public static void Test180111(int dim)
        {
            // 故障率は0.00~0.20まで0.01刻み、0.20~0.90まで0.10刻み
            List<double> frList = new List<double>();
            for (int i = 0; i < 20; i++) frList.Add(0.01 * i);
            for (int i = 0; i < 8; i++) frList.Add(0.20 + 0.1 * i);
            int seed = 0;
            int trials = 10000;
            int timeout = 20;

            var cq = new CrossedCube(dim);
            var exp = new Experiment<BinaryNode>(cq, seed);
            var result = new int[2, frList.Count];


            Console.WriteLine($"{dim}-dimension, {trials} trials");
            int b = trials / 100;
            for (int j = 0; j < frList.Count; j++)
            {
                exp.FaultRatio = frList[j];

                Console.Write($"{frList[j]:0.00}...");
                for (int i = 0; i < trials; i++)
                {
                    if (i % b == 0)
                    {
                        Console.CursorLeft = 8;
                        Console.Write($"{i / (double)trials:p0}");
                    }
                    exp.Next();
                    int step;
                    step = cq.Efe_Routing_NoDetour(exp.SourceNode, exp.DestinationNode, exp.FaultFlags);
                    if (step > 0) result[0, j]++;

                    step = exp.Routing_NoBackTrack(timeout);
                    if (step > 0) result[1, j]++;
                }
                Console.CursorLeft = 8;
                Console.Write($"100%\n");
            }

            // 結果の出力
            // 出力先パス
            var dir = $"../../Output/Exp1/";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var path = dir + $"{DateTime.Now.ToString("yyMMddHHmmss")}_{exp.G.Dimension}.csv";

            var str = $"Trials, {trials}\n";
            for (int i = 0; i < frList.Count; i++)
            {
                str += $",{frList[i]}";
            }
            str += $",\n";
            for (int i = 0; i < 2; i++)
            {
                str += $"case {i}";
                for (int j = 0; j < frList.Count; j++)
                {
                    str += $",{result[i,j]}";
                }
                str += $",\n";
            }

            File.AppendAllText(path, str);
        }
    }
}
