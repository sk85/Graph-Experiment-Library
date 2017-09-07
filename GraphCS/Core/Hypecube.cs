using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    class Hypercube : AGraph
    {
        public Hypercube(int dim, int randSeed) : base(dim, randSeed)
        {
        }

        public override string Name
        {
            get { return "Hypercube"; }
        }

        public override int GetDegree(uint Node)
        {
            return Dimension;
        }

        public override uint GetNeighbor(uint node, int index)
        {
            return node ^ ((uint)1 << index);
        }

        protected override uint CalcNodeNum()
        {
            return (uint)1 << Dimension;
        }

        public override int CalcDistance(uint node1, uint node2)
        {
            int c = (int)(node1 ^ node2);
            c = (c & 0x55555555) + (c >> 1 & 0x55555555);
            c = (c & 0x33333333) + (c >> 2 & 0x33333333);
            c = (c & 0x0f0f0f0f) + (c >> 4 & 0x0f0f0f0f);
            c = (c & 0x00ff00ff) + (c >> 8 & 0x00ff00ff);
            return (c & 0x0000ffff) + (c >> 16 & 0x0000ffff);
        }
    }
}
