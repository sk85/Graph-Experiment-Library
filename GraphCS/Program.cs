#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphCS.Core;

namespace GraphCS
{
    class Program
    {
        static void Main(string[] args)
        {
            //Debug.Check_CalcDistance(new LocallyTwistedCube(10, 0));
            int dim = 5;
            var g = new LocallyTwistedCube(dim, 0);
            for (uint node1 = 0; node1 < g.NodeNum; node1++)
            {
                for (uint node2 = 0; node2 < g.NodeNum; node2++)
                {
                    var distance = g.CalcDistance(node1, node2);
                    if (distance <= 1) continue;
                    foreach (var n in g.Test(node1, node2))
                    {
                        if (g.CalcDistance(node1, n) >= distance)
                        {
                            Console.WriteLine($"({Debug.UintToBinaryString(node1, dim, dim)}, {Debug.UintToBinaryString(node2, dim, dim)})");
                            Console.WriteLine($"{Debug.UintToBinaryString(n, dim, dim)}");
                        }
                    }
                }
            }
        }
    }
}
