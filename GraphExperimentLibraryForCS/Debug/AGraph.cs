#define DEBUG

using System;
using System.IO;
using System.Text;
using Graph.Experiment;

#if DEBUG

namespace Graph.Core
{
    abstract partial class AGraph
    {
        /************************************************************************
        * 
        *  各メソッドのデバッグ用メソッド
        *  
        ************************************************************************/

        /// <summary>
        /// void GenerateFaults(int faultRatio)
        /// のデバッグ用メソッド．
        /// 実際に発生させた故障の数がおかしければ!が表示される．
        /// </summary>
        public void Debug_GenerateFaults()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Fault ratio = {0,2}%", i * 10);
                for (int j = 0; j < 50; j++)
                {
                    Console.WriteLine("{0,2}", j);
                    GenerateFaults(i * 10);
                    int count = 0;
                    for (int k = 0; k < NodeNum; k++)
                    {
                        if (FaultFlags[k]) count++;
                    }
                    if (count != FaultNodeNum)
                    {
                        Console.Write("!");
                        Console.ReadKey();
                    }
                }
                Console.Write("\n");
            }
        }

        /// <summary>
        /// void Node GetNodeRandom()
        /// のデバッグ用メソッド．
        /// 返り値が故障頂点ならば!が表示される．
        /// </summary>
        public void Debug_GetNodeRandom()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Fault ratio = {0,2}%", i * 10);
                for (int j = 0; j < 50; j++)
                {
                    GenerateFaults(i * 10);
                    Node node = GetNodeRandom();
                    Console.Write("{0}\t", node.ID);
                    if (FaultFlags[node.ID])
                    {
                        Console.WriteLine("is fault!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("is ok.");
                    }
                }
            }
        }

        /// <summary>
        /// void Node GetConnectedNodeRandom()
        /// のデバッグ用メソッド．
        /// 返り値が故障頂点ならば!が表示される．
        /// </summary>
        public void Debug_GetConnectedNodeRandom()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Fault ratio = {0,2}%", i * 10);
                for (int j = 0; j < 50; j++)
                {
                    GenerateFaults(i*10);
                    Node node1, node2;
                    do
                    {
                        node1 = GetNodeRandom();
                        node2 = GetConnectedNodeRandom(node1);
                    } while (node1.ID == node2.ID);

                    Console.Write("{0}\t", node2.ID);
                    if (FaultFlags[node2.ID])
                    {
                        Console.WriteLine("is fault!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("is ok.");
                    }
                }
            }
        }

        /// <summary>
        /// int CalcDistance(Node node1, Node node2)
        /// のデバッグ用メソッド．
        /// 幅優先探索の解と比較して間違っていたら!が表示される．
        /// </summary>
        public void Debug_CalcDistance()
        {
            for (Node node1 = new Node(0); node1.ID < NodeNum; node1.ID++)
            {
                int[] distance = CalcAllDistanceBFS(node1);
                for (Node node2 = new Node(0); node2.ID < NodeNum; node2.ID++)
                {
                    Console.Write("d({0}, {1}) = {2}...", node1.ID, node2.ID, distance[node2.ID]);
                    if (CalcDistance(node1, node2) != distance[node2.ID])
                    {
                        Console.WriteLine("NG!");
                        Console.WriteLine("{0,4} = {1}", node1.ID, Tools.UIntToBinStr(node1.ID, Dimension, 4));
                        Console.WriteLine("{0,4} = {1}", node2.ID, Tools.UIntToBinStr(node2.ID, Dimension, 4));
                        Console.WriteLine(" s^d = {1}", node1.ID ^ node2.ID, Tools.UIntToBinStr(node1.ID ^ node2.ID, Dimension, 4));
                        Console.WriteLine("wrong answer is = {0}", CalcDistance(node1, node2));
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("OK.");
                    }
                }
            }
        }

        /// <summary>
        /// bool IsConnected(Node node1, Node node2)
        /// のデバッグ用メソッド．
        /// 間違っていたら!が表示される．
        /// </summary>
        public void Debug_IsConnected()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Fault ratio = {0,2}%", i * 10);
                GenerateFaults(i * 10);
                for (int j = 0; j < 50; j++)
                {
                    Node node1 = GetNodeRandom();
                    Node node2 = GetNodeRandom();
                    Console.Write("({0}, {1})", node1.ID, node2.ID);
                    bool ans = CalcAllDistanceBFSF(node1)[node2.ID] > 0;
                    if (IsConnected(node1, node2) != ans)
                    {
                        Console.WriteLine("NG!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("OK.");
                    }
                }
            }
        }






        /// <summary>
        /// グラフの各頂点の隣接頂点をコンソールに出力します。
        /// キューブ用(2進数値で出力)。
        /// </summary>
        /// <param name="graph">対象のグラフ</param>
        public void ShowNeighborsBinary()
        {
            Console.WriteLine("グラフの各頂点の隣接頂点をコンソールに出力します。");
            Console.WriteLine("1つの出発頂点ごとに止まるので、何かキーを押して進めてください。");
            BinaryNode node = new BinaryNode(0);
            for (UInt32 nodeID = 0; nodeID < NodeNum; nodeID++)
            {
                node.ID = nodeID;
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine(Tools.UIntToBinStr(node.Addr, Dimension, 4) + '\n');
                for (int i = 0; i < GetDegree(node); i++)
                {
                    Console.WriteLine(Tools.UIntToBinStr(((BinaryNode)GetNeighbor(node, i)).Addr, Dimension, 4));
                }
                Console.ReadKey();
            }
        }

        /// <summary>
        /// グラフの各頂点からの距離をコンソールに出力します。
        /// キューブ用(2進数値で出力)。
        /// </summary>
        /// <param name="graph">対象のグラフ</param>
        public void ShowAllDistances()
        {
            Console.WriteLine("グラフの各頂点からの距離をコンソールに出力します。");
            Console.WriteLine("1つの出発頂点ごとに止まるので、何かキーを押して進めてください。");
            for (UInt32 node1 = 0; node1 < NodeNum; node1++)
            {
                int[] array = CalcAllDistanceBFS(new BinaryNode(node1));
                for (UInt32 node2 = 0; node2 < NodeNum; node2++)
                {
                    Console.WriteLine("d({0,2}, {1,2}) = {2}", node1, node2, array[node2]);
                    Console.ReadKey();
                }
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 前方隣接頂点を表示。
        /// キューブ用(2進数値で出力)。
        /// <para>出発頂点と目的頂点を指定する</para>
        /// </summary>
        /// <param name="node1">出発頂点</param>
        /// <param name="node2">目的頂点</param>
        public void ShowFowardNeighbor(Node node1, Node node2)
        {
            int[] distance = CalcAllDistanceBFS(node2);
            Console.WriteLine("d({0}, {1}) = {2}", node1.ID, node2.ID, distance[node1.ID]);
            Console.WriteLine("  u   = {0}", Tools.UIntToBinStr(node1.ID, Dimension, 2));
            Console.WriteLine("  v   = {0}", Tools.UIntToBinStr(node2.ID, Dimension, 2));
            Console.WriteLine("u ^ v = {0}\n", Tools.UIntToBinStr((node1.ID ^ node2.ID), Dimension, 4));
            for (int i = 0; i < GetDegree(node1); i++)
            {
                UInt32 neighborID = GetNeighbor(node1, i).ID;
                if (distance[neighborID] < distance[node1.ID])
                    Console.WriteLine("  u^{0} = {1}", i, Tools.UIntToBinStr(neighborID, Dimension, 4));
            }
        }

        /// <summary>
        /// 前方隣接頂点を表示。
        /// キューブ用(2進数値で出力)。
        /// <para>全部出てくる</para>
        /// </summary>
        public void ShowFowardNeighborBinary()
        {
            for (BinaryNode node2 = new BinaryNode(0); node2.ID < NodeNum; node2.ID = node2.ID + 1)
            {
                Console.WriteLine(node2.ID);

                int[] distance = CalcAllDistanceBFS(node2);

                for (BinaryNode node1 = new BinaryNode(0); node1.ID < NodeNum; node1.ID = node1.ID + 1)
                {
                    Console.WriteLine("d({0}, {1}) = {2}", node1.ID, node2.ID, distance[node1.ID]);
                    Console.WriteLine("  u   = {0}", Tools.UIntToBinStr(node1.Addr, Dimension, 4));
                    Console.WriteLine("  v   = {0}", Tools.UIntToBinStr(node2.Addr, Dimension, 4));
                    Console.WriteLine("u ^ v = {0}\n", Tools.UIntToBinStr(node1.Addr ^ node2.Addr, Dimension, 4));
                    for (int i = 0; i < GetDegree(node1); i++)
                    {
                        BinaryNode neighbor = (BinaryNode)GetNeighbor(node1, i);
                        if (distance[neighbor.ID] < distance[node1.ID])
                            Console.WriteLine("  u^{1} = {0}", Tools.UIntToBinStr(neighbor.Addr, Dimension, 4), i);
                    }
                    Console.WriteLine("------------------------------");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// 前方隣接頂点をビットパターンで表示。
        /// キューブ用(2進数値で出力)。
        /// </summary>
        public void ShowFowardNeighborPattern()
        {
            for (BinaryNode node2 = new BinaryNode(0); node2.ID < NodeNum; node2.ID += 1)
            {
                int[] distance = CalcAllDistanceBFS(node2);

                for (BinaryNode node1 = new BinaryNode(0); node1.ID < NodeNum; node1.ID += 1)
                {

                    UInt32 correctPattern = 0;

                    // 前方隣接頂点集合のビットパターンを生成
                    for (int i = 0; i < GetDegree(node1); i++)
                    {
                        if (distance[GetNeighbor(node1, i).ID] < distance[node1.ID])
                        {
                            correctPattern |= (UInt32)1 << i;
                        }
                    }
                    Console.WriteLine("d({0}, {1}) = {2}", node1.ID, node2.ID, distance[node1.ID]);
                    Console.WriteLine("  u   = {0}", Tools.UIntToBinStr(node1.Addr, Dimension, 2));
                    Console.WriteLine("  v   = {0}", Tools.UIntToBinStr(node2.Addr, Dimension, 2));
                    Console.WriteLine("s ^ d = {0}\n", Tools.UIntToBinStr(node1.Addr ^ node2.Addr, Dimension, 2));
                    Console.WriteLine(" corr = {0}", Tools.UIntToBinStr(correctPattern, Dimension, 2));
                    Console.WriteLine("------------------------------");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// ランダムな頂点ペアの前方隣接頂点を表示。
        /// キューブ用(2進数値で出力)。
        /// </summary>
        public void ShowFOwardNeighborBinaryRandom()
        {
            Random rand = new Random(0);
            while (true)
            {
                BinaryNode node1 = new BinaryNode((UInt32)(rand.Next() % NodeNum));
                BinaryNode node2 = new BinaryNode((UInt32)(rand.Next() % NodeNum));
                int[] dist2 = CalcAllDistanceBFS(node2);

                Console.WriteLine("d({0}, {1}) = {2}", node1.ID, node2.ID, dist2[node1.ID]);
                Console.WriteLine("  u   = {0}", Tools.UIntToBinStr(node1.Addr, Dimension, 2));
                Console.WriteLine("  v   = {0}", Tools.UIntToBinStr(node2.Addr, Dimension, 2));
                Console.WriteLine("u ^ v = {0}\n", Tools.UIntToBinStr(node1.Addr ^ node2.Addr, Dimension, 2));

                for (int i = 0; i < GetDegree(node1); i++)
                {
                    BinaryNode neighbor = (BinaryNode)(GetNeighbor(node1, i));
                    if (dist2[neighbor.ID] < dist2[node1.ID])
                    {
                        Console.Write("  u^{0} = {1}", i, Tools.UIntToBinStr(neighbor.Addr, Dimension, 2));
                        Console.WriteLine("\tv ^ u^{0} = {1}", i, Tools.UIntToBinStr(node2.Addr ^ neighbor.Addr, Dimension, 2));
                    }
                }
                Console.WriteLine("------------------------------");
                Console.ReadKey();
            }
        }

        public void CollectStatisticsToCSV(int minDim, int maxDim, string name)
        {
            // ストリームを開く
            string path = @"..\..\output\" + name + ".csv";
            StreamWriter sw1 = new StreamWriter(path, false, Encoding.GetEncoding("shift_jis"));

            

            for (int dim = minDim; dim <= maxDim; dim++)
            {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                Dimension = dim;
                int[][,] matrix = new int[3][,];
                matrix[0] = new int[10, Dimension + 1]; // matrix[0][i, j] = 距離iで前方の数がjである頂点ペアの数
                matrix[1] = new int[10, Dimension + 1]; // matrix[1][i, j] = 距離iで横方の数がjである頂点ペアの数
                matrix[2] = new int[10, Dimension + 1]; // matrix[2][i, j] = 距離iで後方の数がjである頂点ペアの数

                Console.Write("Dim = {0,2}", Dimension);

                // 全頂点ペアの距離を配列にまとめる
                int[][] distMatrix = new int[NodeNum][];
                for (BinaryNode node = new BinaryNode(0); node.ID < NodeNum; node.ID += 1)
                {
                    distMatrix[node.ID] = CalcAllDistanceBFS(node);
                }

                // ストリームを開く
                path = @"..\..\output\" + name + '_' + Dimension + ".csv";
                StreamWriter sw2 = new StreamWriter(path, false, Encoding.GetEncoding("shift_jis"));

                // 色々計算してファイルに書き込み
                sw2.WriteLine("src, dst, distance, foward, side, backward");
                for (BinaryNode node1 = new BinaryNode(0); node1.ID < NodeNum; node1.ID += 1)
                {
                    for (BinaryNode node2 = new BinaryNode(0); node2.ID < NodeNum; node2.ID += 1)
                    {
                        int countF = 0, countS = 0, countB = 0;
                        for (int i = 0; i < GetDegree(node1); i++)
                        {
                            BinaryNode neighbor = (BinaryNode)GetNeighbor(node1, i);
                            if (distMatrix[neighbor.ID][node2.ID] < distMatrix[node1.ID][node2.ID])
                            {
                                countF++;
                            }
                            else if (distMatrix[neighbor.ID][node2.ID] == distMatrix[node1.ID][node2.ID])
                                countS++;
                            else
                                countB++;
                        }
                        sw2.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", node1.ID, node2.ID, distMatrix[node1.ID][node2.ID], countF, countS, countB);
                        matrix[0][distMatrix[node1.ID][node2.ID], countF]++;
                        matrix[1][distMatrix[node1.ID][node2.ID], countS]++;
                        matrix[2][distMatrix[node1.ID][node2.ID], countB]++;
                    }
                }
                sw2.Close();
                sw1.WriteLine("dim = {0}", Dimension);
                string[] types = { "foward", "side", "backward" };
                for (int type = 0; type < 3; type++)
                {
                    sw1.Write("distance/{0}", types[type]);
                    for (int count = 0; count <= Dimension; count++)
                    {
                        sw1.Write(", {0}", count);
                    }
                    for (int distance = 0; distance < 10; distance++)
                    {
                        sw1.Write("\n{0}, ", distance);
                        for (int count = 0; count <= Dimension; count++)
                        {
                            sw1.Write("{0}, ", matrix[type][distance, count]);
                        }
                    }
                    sw1.Write("\n\n");
                }
                sw1.Write("\n\n");

                stopwatch.Stop();
                Console.WriteLine("\t{0,10}milli sec.", stopwatch.ElapsedMilliseconds);
            }
            sw1.Close();
        }
    }
}

#endif