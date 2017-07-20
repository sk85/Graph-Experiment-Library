using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    class CrossedCube : AGraph
    {
        // Constructor
        public CrossedCube(int dim, int randSeed) : base(dim, randSeed)
        {
        }


        // Abstract members
        public override int GetDegree(uint Node)
        {
            return Dimension;
        }

        public override uint GetNeighbor(uint node, int index)
        {
            const UInt32 mask1 = 0x55555555;            // 01010101....01
            UInt32 mask2 = ((UInt32)1 << index) - 1;    // 00...0111...11
            UInt32 mask = ((node & mask1 & mask2) << 1) | ((UInt32)1 << index);
            return node ^ mask;
        }

        protected override uint CalcNodeNum()
        {
            return (uint)1 << Dimension;
        }

        public override int CalcDistance(uint node1, uint node2)
        {
            Binary u = new Binary(node1), v = new Binary(node2);
            int score = 0;
            for (int i = Dimension - (Dimension & 1); i >= 0; i -= 2) // iをdouble bit の右側に合わせる
            {
                if (score == 0)
                {
                    score = (int)((u[i + 1] ^ v[i + 1]) + (u[i] ^ v[i]));
                }
                else
                {
                    if (!(u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (score & 1) == 1)
                            || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1]))
                    {
                        score += 1;
                    }
                }
            }
            return score;
        }

        public int[] R(uint node1, uint node2)
        {
            Binary u = new Binary(node1), v = new Binary(node2);
            var p = Rho(node1, node2);
            var pSum = new int[Dimension];
            for (int i = (Dimension - 1) / 2 - 1; i >= 0; i--)
            {
                pSum[i] = pSum[i + 1] + p[i + 1];
            }
            var d = CalcDistance(node1, node2);
            int k = Dimension;
            while (k-- > 0 && u[k] == v[k]) ;
            int flag = 0;

            var r = new int[Dimension];
            for (int i = 0; i < Dimension; i++) r[i] = 9;
            for (int i = 0; i < 3/*TODO*/; i += 2)
            {
                Binary n0 = new Binary(GetNeighbor(node1, i));
                Binary n1 = new Binary(GetNeighbor(node1, i + 1));
                if (k / 2 > i / 2)
                {
                    // 0のはずが1になる場合
                    if (u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                               || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1])
                    {
                        r[i] = 1;
                        r[i + 1] = 1;
                    }
                    // 1のはずが0になる場合
                    else if (n0[i] == 1 && v[i] == 1 && (n0[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                               || n0[i] == 0 && v[i] == 0 && n0[i + 1] == v[i + 1])
                    {
                        r[i] = -1;
                        r[i + 1] = 0;
                    }
                    else if (n1[i] == 1 && v[i] == 1 && (n1[i + 1] == v[i + 1] ^ (pSum[i >> 1] & 1) == 1)
                              || n1[i] == 0 && v[i] == 0 && n1[i + 1] == v[i + 1])
                    {
                        r[i] = 0;
                        r[i + 1] = -1;
                    }
                    else
                    {
                        r[i] = 0;
                        r[i + 1] = 0;
                    }
                }
            }
            return r;
        }

        public bool Show2(uint node1, uint node2)
        {
            Binary u = new Binary(node1), v = new Binary(node2);
            int k = Dimension;
            while (k-- > 0 && u[k] == v[k]) ;

            // u, vの表示
            Console.WriteLine(" u  = {0}", Debug.UintToBinaryString(node1, Dimension, 2));
            Console.WriteLine(" v  = {0}", Debug.UintToBinaryString(node2, Dimension, 2));
            Console.WriteLine("u^v = {0}", Debug.UintToBinaryString(node1 ^ node2, Dimension, 2));

            // もとのpの表示
            var p = Rho(node1, node2);
            Console.Write(" p  =");
            for (int i = (Dimension - 1) >> 1; i >= 0; i--)
            {
                Console.Write($"  {p[i]}");
            }
            Console.WriteLine();

            // 今見ているpの表示
            p = Rho(GetNeighbor(node1, 2), node2);
            Console.Write(" p2 =");
            for (int i = (Dimension - 1) >> 1; i >= 0; i--)
            {
                Console.Write($"  {p[i]}");
            }
            Console.WriteLine();

            // kの表示
            Console.WriteLine($" k  = {k}");
            Console.WriteLine("------------------------------");

            for (int i = Dimension - 1; i >= 0; i--) Console.Write($" {i,2}");
            Console.WriteLine();

            var p1 = CalcRelativeDistance(node1, node2);
            for (int i = Dimension - 1; i >= 0; i--) Console.Write($" {p1[i],2}");
            Console.WriteLine();

            var p2 = R(node1, node2);
            for (int i = Dimension - 1; i >= 0; i--) Console.Write($" {p2[i],2}");
            Console.WriteLine();

            Console.WriteLine("------------------------------");

            for (int i = Dimension - 1; i >= 0; i--)
            {
                if (p2[i] != 9 && p1[i] != p2[i]) return false;
            }
            return true;
        }


        // 色々表示
        public void Show(uint node1, uint node2)
        {
            Binary u = new Binary(node1), v = new Binary(node2);
            int k = Dimension;
            while (k-- > 0 && u[k] == v[k]) ;

            var p1 = Rho(node1, node2);
            Console.WriteLine(" u  = {0}", Debug.UintToBinaryString(node1, Dimension, 2));
            Console.WriteLine(" v  = {0}", Debug.UintToBinaryString(node2, Dimension, 2));
            Console.WriteLine("u^v = {0}", Debug.UintToBinaryString(node1 ^ node2, Dimension, 2));
            Console.WriteLine($" k  = {k}");
            Console.WriteLine("------------------------------");
            Console.Write(" p  =");
            for (int i = (Dimension - 1) >> 1; i >= 0; i--)
            {
                Console.Write($"  {p1[i]}");
            }
            Console.WriteLine();
            Console.WriteLine("------------------------------");

            for (int i = 0; i < GetDegree(node1); i++)
            {
                var neighbor = GetNeighbor(node1, i);
                var p2 = Rho(neighbor, node2);
                Console.WriteLine("u^{0} = {1}", i, Debug.UintToBinaryString(neighbor, Dimension, 2));
                Console.Write(" p{0} =",i);
                for (int j = (Dimension - 1) >> 1; j >= 0; j--)
                {
                    Console.Write($"  {p2[j]}");
                }
                Console.WriteLine();
                Console.WriteLine("------------------------------");
            }
        }

        public int[] Rho(uint node1, uint node2)
        {
            Binary u = new Binary(node1), v = new Binary(node2);
            int[] p = new int[Dimension >> 1];
            int sum = 0;

            for (int i = Dimension - ((Dimension & 1) == 1 ? 1 : 2); i >= 0; i -= 2) // iをdouble bit の右側に合わせる
            {
                if (sum == 0)
                {
                    p[i / 2] = (int)((u[i + 1] ^ v[i + 1]) + (u[i] ^ v[i]));
                    sum = p[i / 2];
                }
                else
                {
                    if (!(u[i] == 1 && v[i] == 1 && (u[i + 1] == v[i + 1] ^ (sum & 1) == 1)
                            || u[i] == 0 && v[i] == 0 && u[i + 1] == v[i + 1]))
                    {
                        p[i / 2] = 1;
                        sum += 1;
                    }
                }
            }
            return p;
        }
    }
}
