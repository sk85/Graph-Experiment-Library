using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    class LocallyTwistedCube : AGraph
    {
        uint[,] ADB =
            {
                {
                    0b0000000000000001,
                    0b0000000000000010,
                    0b0000000000000100,
                    0b0000000000001000,
                    0b0000000000010000,
                    0b0000000000100000,
                    0b0000000001000000,
                    0b0000000010000000,
                    0b0000000100000000,
                    0b0000001000000000,
                    0b0000010000000000,
                    0b0000100000000000,
                    0b0001000000000000,
                    0b0010000000000000,
                    0b0100000000000000,
                    0b1000000000000000
                },
                {
                    0b0000000000000001,
                    0b0000000000000010,
                    0b0000000000000110,
                    0b0000000000001100,
                    0b0000000000011000,
                    0b0000000000110000,
                    0b0000000001100000,
                    0b0000000011000000,
                    0b0000000110000000,
                    0b0000001100000000,
                    0b0000011000000000,
                    0b0000110000000000,
                    0b0001100000000000,
                    0b0011000000000000,
                    0b0110000000000000,
                    0b1100000000000000
                }
            };
        public LocallyTwistedCube(int dim, int randSeed) : base(dim, randSeed)
        {
        }

        public override int GetDegree(uint Node)
        {
            return Dimension;
        }

        public override uint GetNeighbor(uint node, int index)
        {
            return node ^ ADB[node & 0b1, index];
        }
        protected override uint CalcNodeNum()
        {
            return (uint)1 << Dimension;
        }

        public override int CalcDistance(uint node1, uint node2)
        {
            UInt32 c1 = node1 ^ node2, c2 = c1, type = 0b10 + (node1 & 1);
            uint t = node1 & 1;
            int count1 = 0, count2 = 0;

            for (int i = Dimension - 1; i > 1; --i)
            {
                if ((c1 >> i) == 1)
                {
                    c1 ^= ADB[t,i];
                    count1++;
                }
                if ((c2 >> i) == 1)
                {
                    c2 ^= (c2 >> (i - 1)) << (i - 1);
                    count2++;
                }
            }
            count1 += (int)((c1 >> 1) + ((c1 & 1) << 10));
            count2 += 2 + (int)((c2 >> 1) - (c2 & 1));

            return count1 < count2 ? count1 : count2;
            /*
            UInt32 c1 = node1 ^ node2, c2 = c1, type = 0b10 + (node1 & 1);
            int count1 = 0, count2 = 0;

            for (int i = Dimension - 1; i > 1; --i)
            {
                if ((c1 >> i) == 1)
                {
                    c1 ^= type << (i - 1);
                    count1++;
                }
                if ((c2 >> i) == 1)
                {
                    c2 ^= (c2 >> (i - 1)) << (i - 1);
                    count2++;
                }
            }
            count1 += (int)((c1 >> 1) + ((c1 & 1) << 10));
            count2 += 2 + (int)((c2 >> 1) - (c2 & 1));

            return count1 < count2 ? count1 : count2;*/
        }
    }
}
