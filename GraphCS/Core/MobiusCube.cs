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

        public MobiusCube(int dim, int randSeed, int type) : base(dim, randSeed)
        {
            Type = type;
        }


        /// <summary>
        /// Type of MobiusCube.
        /// Defalt type is 0;
        /// </summary>
        public int Type { get; set; }

        public override string Name
        {
            get { return $"{Type}-MobiusCube"; }
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
    }
}
