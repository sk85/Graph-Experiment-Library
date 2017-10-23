using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphCS.NEW.Core;

namespace GraphCS.NEW
{
    class CrossedCube : AGraph<BinaryNode>
    {
        public CrossedCube(int dim) : base(dim) { }

        public override string Name => "CrossedCube";

        public override int GetDegree(BinaryNode node) => Dimension;

        public override BinaryNode GetNeighbor(BinaryNode node, int i)
        {
            const int mask1 = 0x55555555;            // 01010101....01
            int mask2 = (1 << i) - 1;    // 00...0111...11
            int mask = ((node.Addr & mask1 & mask2) << 1) | (1 << i);
            return node ^ mask;
        }

        protected override int CalcNodeNum() => 1 << Dimension;

        public override int CalcDistance(BinaryNode node1, BinaryNode node2)
        {
            int score = 0;
            for (int i = Dimension - (Dimension & 1); i >= 0; i -= 2) // iをdouble bit の右側に合わせる
            {
                if (score == 0)
                {
                    score = (int)((node1[i + 1] ^ node2[i + 1]) + (node1[i] ^ node2[i]));
                }
                else
                {
                    if (!(node1[i] == 1 && node2[i] == 1 && (node1[i + 1] == node2[i + 1] ^ (score & 1) == 1)
                            || node1[i] == 0 && node2[i] == 0 && node1[i + 1] == node2[i + 1]))
                    {
                        score += 1;
                    }
                }
            }
            return score;
        }
    }
}
