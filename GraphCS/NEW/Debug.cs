using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphCS.NEW.Core;

namespace GraphCS.NEW
{
    class Debug
    {
        /// <summary>
        /// Checking Agraph.CalcDistance
        /// </summary>
        /// <typeparam name="NodeType">Node type of the graph</typeparam>
        /// <param name="g">Graph object</param>
        /// <param name="minDim">Minimum dimension for checking</param>
        /// <param name="maxDim">Maximum dimension for checking</param>
        /// <param name="stop">Stop or not when error appears</param>
        public static void Check_CalcDistance<NodeType>
            (AGraph<NodeType> g, int minDim, int maxDim, bool stop) where NodeType : ANode, new()
        {
            for (int dim = minDim; dim < maxDim; dim++)
            {
                g.Dimension = dim;
                Console.Write($"n = {dim,2}");
                for (var node1 = new NodeType(); node1.Addr < g.NodeNum; node1.Addr++)
                {
                    if (node1.Addr % 10 == 0)
                    {
                        Console.CursorLeft = 7;
                        Console.Write($"{(double)(node1.Addr + 1) / g.NodeNum:###%}");
                    }
                    var node2 = new NodeType();
                    for (node2.Addr = node1.Addr + 1; node2.Addr < g.NodeNum; node2.Addr++)
                    {
                        int d1 = g.CalcDistanceBFS(node1, node2);
                        int d2 = g.CalcDistance(node1, node2);
                        if (d1 != d2)
                        {
                            Console.WriteLine($"\nd({node1},{node2}) = {d1,2} / {d2,2}");

                            if (stop)
                            {
                                Console.ReadKey();
                            }
                        }
                    }
                }
                Console.CursorLeft = 7;
                Console.WriteLine($"100%");
            }
        }
    }
}
