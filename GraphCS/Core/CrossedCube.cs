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
            if (node1 == node2) return 0;

            var u = new Binary(node1);
            var v = new Binary(node2);

            int score;
            int i = Dimension - 1;

            while (i >= 0 && u[i] == v[i]) { i--; }  // MSBを探す

            i -= i & 1; // double bitの右側に合わせる

            // j = i^* のとき
            score = (int)(u[i + 1] ^ v[i + 1]) + (int)(u[i] ^ v[i]);

            i -= 2;

            // j < i^* のとき
            while (i >= 0)
            {
                if (!((u[i + 1] == v[i + 1] && u[i] == 1 && v[i] == 1 && (score & 1) == 0) ||
                      (u[i + 1] != v[i + 1] && u[i] == 1 && v[i] == 1 && (score & 1) == 1) ||
                      (u[i + 1] == v[i + 1] && u[i] == 0 && v[i] == 0)))
                {
                    score += 1;
                }
                i -= 2;
            }
            return score;
        }
    }
}
