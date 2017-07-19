using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    class CrossedCube : AGraph
    {
        public CrossedCube(int dim, int randSeed) : base(dim, randSeed)
        {
        }

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

        public void CalcRelativeDistance(uint node1, uint node2)
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
