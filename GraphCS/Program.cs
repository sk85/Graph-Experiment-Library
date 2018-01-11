#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using GraphCS.Core;
using GraphCS.Graphs;

namespace GraphCS
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int dim = 15; dim < 21; dim++)
            {
                Test.Test180111(dim);
            }
        }
    }
}
