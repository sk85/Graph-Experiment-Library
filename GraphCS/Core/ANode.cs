using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    /// <summary>
    /// Node class.
    /// </summary>
    abstract class ANode
    {
        /// <summary>
        /// Unique address.
        /// </summary>
        public abstract int Addr { get; set; }

        /// <summary>
        /// Initialize new Node class with 0.
        /// </summary>
        public ANode()
        {
            Addr = 0;
        }

        /// <summary>
        /// Initialize new Node class with addr.
        /// </summary>
        /// <param name="addr">Address</param>
        public ANode(int addr)
        {
            Addr = addr;
        }

        /// <summary>
        /// Copies the node to new instance.
        /// </summary>
        /// <param name="addr">Address</param>
        public ANode(ANode node)
        {
            Addr = node.Addr;
        }

        public override bool Equals(object obj)
        {
            // It is not equal if obj is null or different type
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return (Addr == ((ANode)obj).Addr);
        }

        public override int GetHashCode()
        {
            // Returns same value when Equals is true
            return Addr;
        }

        public static bool operator ==(ANode a, ANode b)
        {
            if ((object)a == null)
            {
                if ((object)b == null) return true;
                else return false;
            }
            else
            {
                if ((object)b == null) return false;
                else return a.Addr == b.Addr;
            }
        }

        public static bool operator !=(ANode a, ANode b)
        {
            return !(a == b);
        }
    }
}
