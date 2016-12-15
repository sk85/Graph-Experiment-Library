using System;
using System.IO;
using System.Text;

using Graph.Core;
using Graph.Experiment;

namespace GraphExperimentLibraryForCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new LocallyTwistedCube(10, 0);

            test(graph, 10000);

            //graph.Debug_GenerateFaults();
            //graph.Debug_GetNodeRandom();
            //graph.Debug_IsConnected();
            //graph.Debug_GetConnectedNodeRandom();
            //graph.Debug_CalcDistance();
        }

        static void test(AGraph graph, int numOfTrials)
        {
            Result result = new Result();

            for (int faultRatio = 0; faultRatio < 10; faultRatio++)
            {
                Console.WriteLine("Fault ratio = {0,2}%", faultRatio * 10);
                for (int count = 0; count < numOfTrials; count++)
                {
                    Console.CursorLeft = 0;
                    Console.Write("{0} / {1}", count + 1 , numOfTrials);
                    graph.GenerateFaults(faultRatio * 10);

                    Node node1, node2;
                    do
                    {
                        node1 = graph.GetNodeRandom();
                        node2 = graph.GetConnectedNodeRandom(node1);
                    } while (node1.ID == node2.ID);

                    result.Add(faultRatio, graph.Routing(node1, node2, graph.SimpleGetNext));
                }
                Console.Write(" ...end\n");
            }

            string path = @"..\..\output\" + graph.Name + graph.Dimension.ToString("00") + ".csv";
            result.SaveToCSV(path);
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
