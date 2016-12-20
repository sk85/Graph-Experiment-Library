using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using Graph.Core;
using Graph.Experiment;

namespace GraphExperimentLibraryForCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new Hypercube(10, 0);

            //graph.SaveCapability();

            Experiment(graph, 500, new Func<Node, Node, int>[] { graph.Routing_Simple, graph.Routing_Capability });
            

            //graph.Debug_GenerateFaults();
            //graph.Debug_GetNodeRandom();
            //graph.Debug_IsConnected();
            //graph.Debug_GetConnectedNodeRandom();
            //graph.Debug_CalcDistance();
        }

        static void Experiment(AGraph graph, int numOfTrials, Func<Node,Node,int>[] RoutingMethods)
        {
            Result[] result = new Result[RoutingMethods.Length];
            for (int i = 0; i < RoutingMethods.Length; i++)
                result[i] = new Result();

            for (int faultRatio = 0; faultRatio < 10; faultRatio++)
            {
                Console.WriteLine("Fault ratio = {0,2}%", faultRatio * 10);
                Stopwatch sw = Stopwatch.StartNew();
                for (int trialCount = 0; trialCount < numOfTrials; trialCount++)
                {
                    // 進捗状況の表示
                    Console.CursorLeft = 0;
                    Console.Write("{0} / {1}", trialCount + 1, numOfTrials);

                    // 故障・出発頂点・目的頂点の設定
                    graph.GenerateFaults(faultRatio * 10);
                    // 故障の形的に，連結な頂点ペアが一切存在しないことがあり得るので，なんとかしないといけない
                    Node node1, node2;
                    do
                    {
                        node1 = graph.GetNodeRandom();
                        node2 = graph.GetConnectedNodeRandom(node1);
                    } while (node1.ID == node2.ID);

                    // それぞれのルーティング結果を保存
                    int[] a = new int[2];
                    for (int i = 0; i < RoutingMethods.Length; i++)
                    {
                        a[i] = RoutingMethods[i](node1, node2);
                        result[i].Add(faultRatio, a[i]);
                    }
                }

                sw.Stop();
                Console.Write(" \t[{0}]\n", sw.Elapsed);
            }

            for (int i = 0; i < RoutingMethods.Length; i++)
            {
                string path = @"..\..\output\" + graph.Name + graph.Dimension.ToString("00") + "-" + i.ToString() + ".csv";
                result[i].SaveToCSV(path);
            }
        }
    }

    /// <summary>
    /// Routingの結果をまとめるためのクラス。
    /// </summary>
    class Result
    {
        private int[] Count;
        private int[] SuccessCount;
        private int[] SuccessSteps;
        private int[] FaultSteps;

        public Result()
        {
            Count = new int[10];
            SuccessCount = new int[10];
            SuccessSteps = new int[10];
            FaultSteps = new int[10];
        }

        /// <summary>
        /// Routingの結果を追加します。
        /// </summary>
        /// <param name="faultRatio">故障率</param>
        /// <param name="step">Routingの返り値をそのまま渡してください。</param>
        public void Add(int faultRatio, int step)
        {
            Count[faultRatio]++;
            if (step > 0)
            {
                SuccessCount[faultRatio]++;
                SuccessSteps[faultRatio] += step;
            }
            else
            {
                FaultSteps[faultRatio] += -step;
            }
        }

        /// <summary>
        /// 結果をCSVで出力します
        /// </summary>
        /// <param name="path">出力先のパス</param>
        public void SaveToCSV(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.GetEncoding("shift_jis")))
            {
                sw.Write(",0%,10%,20%,30%,40%,50%,60%,70%,80%,90%");
                sw.Write("\nSuccessCount / Count,");
                for (int i = 0; i < 10; i++) sw.Write("{0},", (double)SuccessCount[i] / Count[i]);
                sw.Write("\nSuccessSteps / SuccessCount,");
                for (int i = 0; i < 10; i++) sw.Write("{0},", (double)SuccessSteps[i] / SuccessCount[i]);
            }
        }
    }
}
