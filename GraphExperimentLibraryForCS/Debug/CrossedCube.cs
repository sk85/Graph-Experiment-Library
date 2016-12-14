#define DEBUG

using System;
using Graph.Experiment;

namespace Graph.Core
{
#if DEBUG
    partial class CrossedCube : AGraph
    {
        /// <summary>
        /// IEnumerable<int> GetFowardNeighbor(BinaryNode node1, BinaryNode node2)の動作を確認します。
        /// <para>幅優先探索で求めた前方隣接頂点集合と解が一致するかを確認します。</para>
        /// </summary>
        public void Check1_GetFowardNeighbor()
        {
            for (BinaryNode node2 = new BinaryNode(0); node2.ID <= NodeNum - 1; node2.ID = node2.ID + 1)
            {
                Console.WriteLine("v = {0}", node2.ID);
                int[] distance = CalcAllDistanceBFS(node2);

                for (BinaryNode node1 = new BinaryNode(0); node1.ID <= NodeNum - 1; node1.ID = node1.ID + 1)
                {
                    UInt32 correctPattern = 0, answerPattern = 0;

                    // 前方隣接頂点集合のビットパターンを生成
                    for (int i = 0; i < GetDegree(node1); i++)
                    {
                        if (distance[GetNeighbor(node1, i).ID] < distance[node1.ID])
                        {
                            correctPattern |= (UInt32)1 << i;
                        }
                    }

                    // 求めた解のビットパターンを生成
                    var answer = GetFowardNeighbor(node1, node2);
                    foreach (var neighborIndex in answer)
                    {
                        answerPattern |= (UInt32)1 << neighborIndex;
                    }

                    // 2つのビットパターンが異なれば情報を表示
                    if (correctPattern != answerPattern)
                    {
                        Console.WriteLine("d({0}, {1}) = {2}", node1.ID, node2.ID, distance[node1.ID]);
                        Console.WriteLine("  u   = {0}", Tools.UIntToBinStr(node1.Addr, Dimension, 2));
                        Console.WriteLine("  v   = {0}", Tools.UIntToBinStr(node2.Addr, Dimension, 2));
                        Console.WriteLine("s ^ d = {0}\n", Tools.UIntToBinStr(node1.Addr ^ node2.Addr, Dimension, 2));
                        Console.WriteLine(" corr = {0}", Tools.UIntToBinStr(correctPattern, Dimension, 2));
                        Console.WriteLine(" answ = {0}", Tools.UIntToBinStr(answerPattern, Dimension, 2));
                        Console.WriteLine("------------------------------");
                        Console.ReadKey();
                    }
                }
            }
        }

        /// <summary>
        /// IEnumerable<int> GetFowardNeighbor(BinaryNode node1, BinaryNode node2)の動作を確認します。
        /// <para>幅優先探索で求めた前方隣接頂点集合に解が含まれるかを確認します。</para>
        /// </summary>
        public void Check2_GetFowardNeighbor()
        {
            for (BinaryNode node2 = new BinaryNode(0); node2.ID <= NodeNum - 1; node2.ID = node2.ID + 1)
            {
                Console.WriteLine("v = {0}", node2.ID);
                int[] distance = CalcAllDistanceBFS(node2);

                for (BinaryNode node1 = new BinaryNode(0); node1.ID <= NodeNum - 1; node1.ID = node1.ID + 1)
                {
                    // 求めた解が前方でなければ情報を表示
                    var answer = GetFowardNeighbor(node1, node2);
                    foreach (var neighborIndex in answer)
                    {
                        BinaryNode neighbor = (BinaryNode)(GetNeighbor(node1, neighborIndex));
                        if (distance[neighbor.ID] >= distance[node1.ID])
                        {
                            Console.WriteLine("d({0}, {1}) = {2}", node1.ID, node2.ID, distance[node1.ID]);
                            Console.WriteLine("  u   = {0}", Tools.UIntToBinStr(node1.Addr, Dimension, 2));
                            Console.WriteLine("  v   = {0}", Tools.UIntToBinStr(node2.Addr, Dimension, 2));
                            Console.WriteLine("s ^ d = {0}\n", Tools.UIntToBinStr(node1.Addr ^ node2.Addr, Dimension, 2));
                            Console.WriteLine("  u^{0} = {1}", neighborIndex, Tools.UIntToBinStr(neighbor.Addr, Dimension, 2));
                            Console.WriteLine("------------------------------");
                            Console.ReadKey();
                        }
                    }
                }
            }
        }

    }
#endif
}
