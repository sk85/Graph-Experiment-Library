using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using GraphCS.Core;

namespace GraphCS
{
    static class Test
    {
        // Routing_171025でルーティングの実験を行う。
        // 特殊なことはしてないのでだいたいコレのコピペで行ける
        public static void Test171024<NodeType>
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
                    var step = exp.Routing_171025(exp.G.Dimension);

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
    }
}
