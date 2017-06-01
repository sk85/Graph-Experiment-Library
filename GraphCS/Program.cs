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
            Debug.Check_CalcDistance(new LocallyTwistedCube(10, 0));
        }
    }
}
