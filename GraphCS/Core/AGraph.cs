using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    /// <summary>
    /// Abstract class of graph.
    /// </summary>
    abstract class AGraph<NodeType> where NodeType : ANode, new()
    {
        #region Member variables / Properties

        /// <summary>
        /// Name of the graph
        /// </summary>
        public abstract string Name { get; }

        private int __Dimension;
        /// <summary>
        /// Dimension of the graph.
        /// When it is changed, NodeNum is also updated.
        /// </summary>
        public int Dimension
        {
            get { return __Dimension; }
            set
            {
                __Dimension = value;
                NodeNum = CalcNodeNum();
            }
        }

        /// <summary>
        /// Number of vertices.
        /// Updated only when Dimension is changed.
        /// </summary>
        public int NodeNum { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize the new graph instance with specified dimension
        /// </summary>
        /// <param name="dim">Dimension</param>
        protected AGraph(int dim)
        {
            Dimension = dim;
        }

        #endregion

        #region Graph definition and distance calculation
        /// <summary>
        /// Calculate current node number.
        /// </summary>
        /// <returns>ノード数</returns>
        protected abstract int CalcNodeNum();

        /// <summary>
        /// Returns a degree of the node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Degree</returns>
        public abstract int GetDegree(NodeType node);

        /// <summary>
        /// Returns i-th neighbor of the node.
        /// Each neighbors are different with i.
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="i">Identifier of neighbor</param>
        /// <returns>i-th neighbor of the node</returns>
        public abstract NodeType GetNeighbor(NodeType node, int i);

        /// <summary>
        /// Returns all neighbors of the node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Enumerated neighbor of the node</returns>
        public IEnumerable<NodeType> GetNeighbor(NodeType node)
        {
            for (int i = 0; i < GetDegree(node); i++)
            {
                yield return GetNeighbor(node, i);
            }
        }


        /// <summary>
        /// Calculate the distance between node1 and node2 by BFS
        /// (breadth first search)
        /// </summary>
        /// <param name="node1">Source node</param>
        /// <param name="node2">Destination node</param>
        /// <returns>Distance</returns>
        public int CalcDistanceBFS(NodeType node1, NodeType node2)
        {
            if (node1 == node2) return 0;

            var que = new Queue<NodeType>();
            var distance = new int[NodeNum];
            
            que.Enqueue(node1);
            distance[node1.Addr] = 1;
            while (que.Count > 0)
            {
                NodeType current = que.Dequeue();
                foreach (var neighbor in GetNeighbor(current))
                {
                    if (distance[neighbor.Addr] == 0)
                    {
                        if (neighbor == node2) return distance[current.Addr];
                        distance[neighbor.Addr] = distance[current.Addr] + 1;
                        que.Enqueue(neighbor);
                    }
                }
            }

            throw new Exception("It is suggested that node1 and node2 is UNCONNECTED.");
        }

        /// <summary>
        /// Calculate the distance between node1 and node2.
        /// It calls CalcDistanceBFS at default.
        /// </summary>
        /// <param name="node1">Source node</param>
        /// <param name="node2">Destination node</param>
        /// <returns>Distance</returns>
        public virtual int CalcDistance(NodeType node1, NodeType node2)
        {
            return CalcDistanceBFS(node1, node2);
        }

        /// <summary>
        /// Calculate relative distance. 
        /// </summary>
        /// <param name="current">Current node</param>
        /// <param name="destination">Destination node</param>
        /// <returns>rel[i] is relative distance of i-th neighbor of current node</returns>
        public virtual int[] CalcRelativeDistance(NodeType current, NodeType destination)
        {
            var rel = new int[Dimension];
            var dis = CalcDistance(current, destination);
            for (int i = 0; i < Dimension; i++)
            {
                rel[i] = CalcDistance(GetNeighbor(current, i), destination) - dis;
            }
            return rel;
        }
        #endregion

#if DEBUG
        /// <summary>
        /// Debug Agraph.CalcDistance
        /// </summary>
        public void DEBUG_CalcDistance()
        {
            Console.WriteLine("Debug \"AGraph.CalcDistance\"");
            Console.WriteLine("> {0}-dimensional {1}", Dimension, Name);
            Console.Write("> ");
            for (var node1 = new NodeType(); node1.Addr < NodeNum; node1.Addr++)
            {
                if ((node1.Addr & 0b1111) == 0)
                {
                    Console.CursorLeft = 2;
                    Console.Write($"{(double)(node1.Addr + 1) / NodeNum:###%}");
                }

                // node1から各ノードへの距離+1の表を作る
                var que = new Queue<NodeType>();
                var dis = new int[NodeNum];
                que.Enqueue(node1);
                dis[node1.Addr] = 1;
                while (que.Count > 0)
                {
                    NodeType current = que.Dequeue();
                    foreach (var neighbor in GetNeighbor(current))
                    {
                        if (dis[neighbor.Addr] == 0)
                        {
                            dis[neighbor.Addr] = dis[current.Addr] + 1;
                            que.Enqueue(neighbor);
                        }
                    }
                }

                // チェック
                var node2 = new NodeType();
                for (node2.Addr = node1.Addr + 1; node2.Addr < NodeNum; node2.Addr++)
                {
                    int d = CalcDistance(node1, node2);
                    if (d != dis[node2.Addr] - 1)
                    {
                        Console.WriteLine($"\nd({node1},{node2}) = {dis[node2.Addr] - 1,2} / {d,2}");
                        Console.WriteLine();
                        Console.WriteLine("> NG");
                        return;
                    }
                }
            }
            Console.CursorLeft = 0;
            Console.WriteLine("> 100%");
            Console.WriteLine("> OK");
        }

        /// <summary>
        /// Debug CalcRelativeDistance.
        /// CalcDistanceを都度呼び出しているので遅いかも
        /// </summary>
        public void DEBUG_CalcRelativeDistance()
        {
            Console.WriteLine("Debug \"AGraph.CalcRelativeDistance\"");
            Console.WriteLine("> {0}-dimensional {1}", Dimension, Name);
            Console.Write("> ");

            for (var node1 = new NodeType(); node1.Addr < NodeNum; node1.Addr++)
            {
                if ((node1.Addr & 0b1111) == 0)
                {
                    Console.CursorLeft = 2;
                    Console.Write($"{(double)(node1.Addr + 1) / NodeNum:###%}");
                }
                
                for (var node2 = new NodeType(); node2.Addr < NodeNum; node2.Addr++)
                {
                    var rel = CalcRelativeDistance(node1, node2);
                    var dis = CalcDistance(node1, node2);

                    for (int i = 0; i < Dimension; i++)
                    {
                        if (CalcDistance(GetNeighbor(node1, i), node2) - dis != rel[i])
                        {
                            Console.WriteLine("");
                            Console.WriteLine("> NG");
                            Console.WriteLine("> node1 = {0}", node1.Addr);
                            Console.WriteLine("> node2 = {0}", node2.Addr);
                            return;
                        }
                    }
                }
            }
            Console.CursorLeft = 0;
            Console.WriteLine("> 100%");
            Console.WriteLine("> OK");
        }

        /// <summary>
        /// Debug CalcRelativeDistance.
        /// 最初に距離の表を作るので、CalcDistanceの呼び出し回数が減っている(速いかは未確認)
        /// </summary>
        public void DEBUG_CalcRelativeDistance2()
        {
            Console.WriteLine("Debug \"AGraph.CalcRelativeDistance\"");
            Console.WriteLine("> {0}-dimensional {1}", Dimension, Name);
            Console.Write("> ");

            var disMat = new int[NodeNum, NodeNum];

            for (var node1 = new NodeType(); node1.Addr < NodeNum; node1.Addr++)
            {
                if ((node1.Addr & 0b1111) == 0)
                {
                    Console.CursorLeft = 2;
                    Console.Write($"{(double)(node1.Addr + 1) / NodeNum:###%}");
                }

                for (var node2 = new NodeType(); node2.Addr < NodeNum; node2.Addr++)
                {
                    var rel = CalcRelativeDistance(node1, node2);
                    var dis = CalcDistance(node1, node2);

                    for (int i = 0; i < Dimension; i++)
                    {
                        var neighbor = GetNeighbor(node1, i);
                        int n1, n2;
                        if (neighbor.Addr > node2.Addr)
                        {
                            n1 = node2.Addr; n2 = neighbor.Addr;
                        }
                        else
                        {
                            n1 = neighbor.Addr; n2 = node2.Addr;
                        }

                        if (disMat[n1, n2] == 0)
                        {
                            disMat[n1, n2] = CalcDistance(neighbor, node2);
                        }

                        if (disMat[n1, n2] - dis != rel[i])
                        {
                            Console.WriteLine("");
                            Console.WriteLine("> NG");
                            Console.WriteLine("> node1 = {0}", node1.Addr);
                            Console.WriteLine("> node2 = {0}", node2.Addr);
                            return;
                        }
                    }
                }
            }
            Console.CursorLeft = 0;
            Console.WriteLine("> 100%");
            Console.WriteLine("> OK");
        }

        /// <summary>
        /// 全ノードペア間の距離を幅優先探索で計算して表示。
        /// GetNeighborの確認などに用いる(非連結なら停止する)
        /// </summary>
        public void DEBUG_ShowAllPairDistance()
        {
            for (var node1 = new NodeType(); node1.Addr < NodeNum; node1.Addr++)
            {
                var node2 = new NodeType();
                for (node2.Addr = node1.Addr + 1; node2.Addr < NodeNum; node2.Addr++)
                {
                    Console.WriteLine(
                        "d({0},{1}) = {2,3}",
                        node1.Addr,
                        node2.Addr,
                        CalcDistanceBFS(node1, node2)
                    );
                    //Console.ReadKey();
                }
            }
        }
#endif
    }
}
