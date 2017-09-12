using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using GraphCS.Core;

namespace GraphCS
{
    static class Experiment
    {

        // 17/09/06
        // 色々なグラフの平均経路長と直径を計算
        // 各グラフはCalcDistanceをオーバーライドしておかないと計算量が大きい
        public static void E平均経路長と直径(AGraph[] gs, int minDim, int maxDim)
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

    }
}
