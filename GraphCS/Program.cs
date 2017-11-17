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
            //g.DEBUG_Efe_GetNext();
            /*
            var cq = new CrossedCube(15);
            var exp = new Experiment<BinaryNode>(cq, 0);

            exp.FaultRatio = 0.20;
            var TL = 100;
            var trialCount = 10000;

            var steps = new int[TL + 1];
            for (int trial = 0; trial < trialCount; trial++)
            {
                // 進捗の表示
                Console.CursorLeft = 4;
                Console.Write($"{trial + 1} / {trialCount}");

                // パラメタの更新
                exp.Next();
                var step = 
                    cq.Efe_Routing(exp, TL);
                steps[step > 0 ? step - 1 : TL]++;
            }
            Console.WriteLine();
            for (int i = 0; i < TL; i++)
            {
                Console.WriteLine($"{i + 1,3} : {steps[i],4}");
            }
            Console.ReadKey();*/

            test();
        }

        static void test()
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
            var steps_NoDetour = new int[2, ratioNum];
            var success_NoDetour = new int[2, ratioNum];
            var steps_NoRandom = new int[2, ratioNum, timeoutLimit];
            var success_NoRandom = new int[2, ratioNum];
            var steps = new int[2, ratioNum, timeoutLimit];
            var success = new int[2, ratioNum];

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
                                (exp.SourceNode, exp.DestinationNode, exp.FaultFlags, timeoutLimit),
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
    }
}
