using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using GraphCS.Core;
using GraphCS.Graphs;

#if true

namespace GraphCS.Graphs
{
    partial class CrossedCube : AGraph<BinaryNode>
    {
        /// <summary>
        /// EfeのCalcDistanceをデバッグ
        /// </summary>
        public void DEBUG_Efe_GetNext()
        {
            int minDim = 2;
            int maxDim = 10;

            Console.WriteLine("Debug 'Efe_GetNext'");

            for (int dim = minDim; dim <= maxDim; dim++)
            {
                Console.WriteLine("> dim = {0}", dim);

                Dimension = dim;
                var u = new BinaryNode(0);
                var v = new BinaryNode(0);
                var rand = new Random();
                for (u.Addr = 0; u.Addr < NodeNum; u.Addr++)
                {
                    for (v.Addr = u.Addr + 1; v.Addr < NodeNum; v.Addr++)
                    {
                        var d1 = CalcDistance(u, v);

                        var d2 = 0;
                        var current = new BinaryNode(u);
                        while (current != v)
                        {
                            Efe_GetNext(current, v, out var n1, out var n2);
                            current = 
                                n2 == null                  ? n1 
                                : (rand.Next() & 1) == 0    ? n1 
                                : n2;
                            d2++;
                        }

                        if (d1 != d2)
                        {
                            Console.WriteLine($"({u.Addr},{v.Addr}) [{d1} / {d2}]");
                            Console.ReadKey();
                        }
                    }
                }
            }
        }
    }
}

#endif