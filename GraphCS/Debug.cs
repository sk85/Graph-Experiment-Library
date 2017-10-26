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
        public string CmpSpeed_CalcRelativeDistance()
        {
            Console.WriteLine("Compare the speed CalcRelativeDistance");
            Console.WriteLine($"> {Dimension}-dimensional {Name}");

            Stopwatch sw1 = new Stopwatch(), sw2 = new Stopwatch();
            BinaryNode u = new BinaryNode(), v = new BinaryNode();
            var rand = new Random();

            for (int i = 0; i < 100000; i++)
            {
                u.Addr = rand.Next(NodeNum);
                v.Addr = rand.Next(NodeNum);

                sw1.Start();
                base.CalcRelativeDistance(u, v);
                sw1.Stop();

                sw2.Start();
                CalcRelativeDistance(u, v);
                sw2.Stop();
            }
            return $"{sw1.ElapsedMilliseconds},{sw2.ElapsedMilliseconds}";
        }
    }
}

#endif