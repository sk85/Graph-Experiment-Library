using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graph.Core;
using Graph.Experiment;

namespace GraphExperimentLibraryForCS
{
    class Program
    {
        static void Main(string[] args)
        {
            // オブジェクトの生成

            new MobiusCube(0).CollectStatisticsToCSV(2, 14, "MQ");
            new CrossedCube(0).CollectStatisticsToCSV(2, 14, "CQ");
            // MobiusCUbe クラスを実装して、動作が正しいか試してる
        }
    }

}
