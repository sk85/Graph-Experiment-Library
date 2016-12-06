using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graph.Core;
using Graph.Debug;

namespace GraphExperimentLibraryForCS
{
    class Program
    {
        static void Main(string[] args)
        {
            // Crossed Cube クラスを実装して、動作が正しいか試してる



            // 隣接の定義が正しいか確認 OK
            //Graph.Debug.TestCodes.ShowNeighbors(graph);

            // ちゃんと連結か確認(幅探索で距離を求めてる) OK
            //Graph.Debug.TestCodes.ShowAllDistances(graph);



            /*
             * 2頂点間の前方隣接頂点を表示する実験
             * 
             * 
             * 
             */

            // オブジェクトの生成
            int dim = 10;
            CrossedCube graph = new CrossedCube(dim);

            //Graph.Experiment.Experiment.ShowFowardNeighbor(graph, new BinaryNode(0b111001), new BinaryNode(0b000000));
            //Console.WriteLine("------------------------------");
            
            Graph.Experiment.Experiment.ShowAllFowardNeighbor(graph);
        }
    }

}
