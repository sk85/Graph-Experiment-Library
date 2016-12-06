using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graph.Core;
using Graph.Debug;

namespace Graph.Experiment
{
    static class Experiment
    {
        public static void ShowFowardNeighbor(AGraph graph, BinaryNode node1, BinaryNode node2)
        {
            int[] distance = graph.CalcAllDistanceBFS(node2);
            Console.WriteLine("d({0}, {1}) = {2}", node1.ID, node2.ID, distance[node1.ID]);
            Console.WriteLine("  s   = {0}", Tools.UIntToBinStr(node1.Addr, graph.Dimension, 2));
            Console.WriteLine("  d   = {0}", Tools.UIntToBinStr(node2.Addr, graph.Dimension, 2));
            Console.WriteLine("s ^ d = {0}\n", Tools.UIntToBinStr((node1 ^ node2).Addr, graph.Dimension, 2));
            for (int i = 0; i < graph.GetDegree(node1); i++)
            {
                UInt32 neighborID = ((BinaryNode)graph.GetNeighbor(node1, i)).Addr;
                if (distance[neighborID] < distance[node1.ID])
                    Console.WriteLine("node{1} = {0}", Tools.UIntToBinStr(neighborID, graph.Dimension, 2), i);
            }
        }

        public static void ShowAllFowardNeighbor(AGraph graph)
        {
            for (UInt32 node2ID = 0; node2ID < graph.NodeNum; node2ID++)
            {
                Console.WriteLine(node2ID);
                BinaryNode node2 = new BinaryNode(node2ID);
                int[] distance = graph.CalcAllDistanceBFS(node2);

                for (UInt32 node1ID = 0; node1ID < graph.NodeNum; node1ID++)
                {
                    BinaryNode node1 = new BinaryNode(node1ID);
                    Console.WriteLine("d({0}, {1}) = {2}", node1ID, node2ID, distance[node1ID]);
                    Console.WriteLine("  s   = {0}", Tools.UIntToBinStr(node1.Addr, graph.Dimension, 2));
                    Console.WriteLine("  d   = {0}", Tools.UIntToBinStr(node2.Addr, graph.Dimension, 2));
                    Console.WriteLine("s ^ d = {0}\n", Tools.UIntToBinStr((node1 ^ node2).Addr, graph.Dimension, 2));
                    for (int i = 0; i < graph.GetDegree(node1); i++)
                    {
                        UInt32 neighborID = ((BinaryNode)graph.GetNeighbor(node1, i)).Addr;
                        if (distance[neighborID] < distance[node1ID])
                            Console.WriteLine("node{1} = {0}", Tools.UIntToBinStr(neighborID, graph.Dimension, 2), i);
                    }
                    Console.WriteLine("------------------------------");
                    Console.ReadKey();
                }
            }
        }
    }
}
