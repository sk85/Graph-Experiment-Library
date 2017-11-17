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
            Console.WriteLine("Debug 'Efe_GetNext'");
            Console.WriteLine("> Checking distance");

            Console.WriteLine("> dim = {0}", Dimension);
            
            var u = new BinaryNode(0);
            var v = new BinaryNode(0);

            for (u.Addr = 0; u.Addr < NodeNum; u.Addr++)
            {
                for (v.Addr = u.Addr + 1; v.Addr < NodeNum; v.Addr++)
                {
                    var d1 = CalcDistance(u, v);

                    var d2 = 0;
                    var current = new BinaryNode(u);
                    while (current != v)
                    {
                        d2++;
                        Efe_GetNext(current, v, out var n1, out var n2);
                        current = n1;
                    }

                    if (d1 != d2)
                    {
                        Console.WriteLine($"({u.Addr},{v.Addr}) [{d1} / {d2}]");
                        Console.ReadKey();
                    }
                }
            }
        }

        public void DEBUG_Efe_GetNext2()
        {
            Console.WriteLine("Debug 'Efe_GetNext'");
            Console.WriteLine("> Checking if n1 and n2 are forward");

            Console.WriteLine("> dim = {0}", Dimension);
            
            var u = new BinaryNode(0);
            var v = new BinaryNode(0);

            for (u.Addr = 0; u.Addr < NodeNum; u.Addr++)
            {
                for (v.Addr = u.Addr + 1; v.Addr < NodeNum; v.Addr++)
                {
                    var d = CalcDistance(u, v);
                    Efe_GetNext(u, v, out var n1, out var n2);
                    if (CalcDistance(n1, v) >= d || n2 != null && CalcDistance(n2, v) >= d)
                    {
                        Console.WriteLine($"({u.Addr},{v.Addr})");
                        Console.ReadKey();
                    }
                }
            }
        }
    }
}

#endif