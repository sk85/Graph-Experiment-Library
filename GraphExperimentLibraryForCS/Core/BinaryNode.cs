using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    class BinaryNode : INode
    {
        private UInt32 __Addr;
        private UInt32 __ID;

        public UInt32 Addr
        {
            get { return __Addr; }
            set
            {
                __Addr = value;
                __ID = __Addr;
            }
        }

        public override UInt32 ID
        {
            get { return __ID; }
            set
            {
                __ID = value;
                __Addr = __ID;
            }
        }

        public BinaryNode(UInt32 id) : base(id) { }

        public static BinaryNode operator ^ (BinaryNode node1, BinaryNode node2)
        {
            return new BinaryNode(node1.Addr ^ node2.Addr);
        }

        public static BinaryNode operator ^ (BinaryNode node, UInt32 bin)
        {
            return new BinaryNode(node.Addr ^ bin);
        }

        public UInt32 this[int i]
        {
            get
            {
                return (Addr >> i) & 1;
            }
        }
    }
}
