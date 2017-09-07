using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    class MobiusCube : AGraph
    {
        public MobiusCube(int dim, int randSeed) : base(dim, randSeed)
        {
            Type = 0;
        }

        /// <summary>
        /// Type of MobiusCube.
        /// Defalt type is 0;
        /// </summary>
        public int Type { get; set; }

        public override string Name
        {
            get { return "MobiusCube"; }
        }

        public override int GetDegree(uint Node)
        {
            return Dimension;
        }

        public override uint GetNeighbor(uint node, int index)
        {
            int type = index == Dimension - 1
                ? Type
                : (int)((node >> (index + 1)) & 1);

            if (type == 0)
            {
                return node ^ ((uint)1 << index);    // 100...000
            }
            else
            {
                return node ^ (((uint)1 << (index + 1)) - 1);  // 111...111
            }
        }

        protected override uint CalcNodeNum()
        {
            return (uint)1 << Dimension;
        }

        public override int CalcDistance(uint node1, uint node2)
        {
            // とりあえずminimal expansionの要素数を数える？
            Binary c = new Binary(node1 ^ node2);
            Binary n1 = new Binary(node1);
            int i = Dimension - 1;
            int count = 0;
            bool bad = false;

            while (i > 0)
            {
                if (c[i] == 1)
                {
                    if (n1[i + 1] != c[i - 1])
                    {
                        bad = true;
                    }
                    if (c[i - 1] == 1)
                    {
                        c.Bin = ~c.Bin;
                    }
                    i -= 2;
                    count++;
                }
                else
                {
                    i--;
                }
            }
            if (c[0] == 1)
            {
                count++;
            }

            return count + (bad ? 1 : 0);

            /*
            // →動かない
            Binary c = new Binary(node1);
            Binary d = new Binary(node2);
            int i = Dimension - 1;
            int count = 0;

            while (i >= 0)
            {
                if (c[i] != d[i])
                {
                    c.Bin = GetNeighbor(c.Bin, i);
                    count++;
                }
                i--;
            }
            return count;*/
        }
    }
}
