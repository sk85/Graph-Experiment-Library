using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphExperimentLibraryForCS
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph.Core.AGraph CQ = new Graph.Core.LocallyTwistedCube(5);
            for (UInt32 node1 = 0; node1 < CQ.NodeNum; node1++)
            {
                int[] array = CQ.CalcAllDistanceBFS(node1);
                for (UInt32 node2 = 0; node2 < CQ.NodeNum; node2++)
                {
                    Console.WriteLine("d({0,2}, {1,2}) = {2}", node1, node2, array[node2]);
                }
            }
            Console.ReadKey();
        }

        static string UIntToBinStr(UInt32 bin, int length, int interval)
        {
            string str = "";
            for (int i = 0; i < length; i++)
            {
                if (i % interval == 0) str = " " + str;
                str = ((bin >> i) & 1).ToString() + str;
            }
            return str;
        }
    }
}
