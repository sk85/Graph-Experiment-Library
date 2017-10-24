using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Old.Core
{
    class SpinedCube : AGraph
    {
        public SpinedCube(int dim, int randSeed) : base(dim, randSeed)
        {
        }

        private readonly uint[,] DecisionBinary =
        {
            {
                0x00000001, 0x00000002, 0x00000004, 0x00000008,
                0x00000010, 0x00000020, 0x00000040, 0x00000080,
                0x00000100, 0x00000200, 0x00000400, 0x00000800,
                0x00001000, 0x00002000, 0x00004000, 0x00008000,
                0x00010000, 0x00020000, 0x00040000, 0x00080000,
                0x00100000, 0x00200000, 0x00400000, 0x00800000,
                0x01000000, 0x02000000, 0x04000000, 0x08000000,
                0x10000000, 0x20000000, 0x40000000, 0x80000000,

            },
            {
                0x00000001, 0x00000002, 0x00000006, 0x0000000C,
                0x00000014, 0x00000028, 0x00000050, 0x000000A0,
                0x00000140, 0x00000280, 0x00000500, 0x00000A00,
                0x00001400, 0x00002800, 0x00005000, 0x0000A000,
                0x00014000, 0x00028000, 0x00050000, 0x000A0000,
                0x00140000, 0x00280000, 0x00500000, 0x00A00000,
                0x01400000, 0x02800000, 0x05000000, 0x0A000000,
                0x14000000, 0x28000000, 0x50000000, 0xA0000000,
            },
            {
                0x00000001, 0x00000002, 0x00000004, 0x00000008,
                0x00000018, 0x00000030, 0x00000060, 0x000000C0,
                0x00000180, 0x00000300, 0x00000600, 0x00000C00,
                0x00001800, 0x00003000, 0x00006000, 0x0000C000,
                0x00018000, 0x00030000, 0x00060000, 0x000C0000,
                0x00180000, 0x00300000, 0x00600000, 0x00C00000,
                0x01800000, 0x03000000, 0x06000000, 0x0C000000,
                0x18000000, 0x30000000, 0x60000000, 0xC0000000,
            },
            {
                0x00000001, 0x00000002, 0x00000006, 0x0000000C,
                0x0000001C, 0x00000038, 0x00000070, 0x000000E0,
                0x000001C0, 0x00000380, 0x00000700, 0x00000E00,
                0x00001C00, 0x00003800, 0x00007000, 0x0000E000,
                0x0001C000, 0x00038000, 0x00070000, 0x000E0000,
                0x001C0000, 0x03800000, 0x00700000, 0x00E00000,
                0x01C00000, 0x03800000, 0x07000000, 0x0E000000,
                0x1C000000, 0x38000000, 0x70000000, 0xE0000000,

            }
        };

        public override string Name
        {
            get { return "SpinedCube"; }
        }

        public override int GetDegree(uint Node)
        {
            return Dimension;
        }

        public override uint GetNeighbor(uint node, int index)
        {
            return node ^ DecisionBinary[node & 0b11, index];
        }

        protected override uint CalcNodeNum()
        {
            return (uint)1 << Dimension;
        }
    }
}
