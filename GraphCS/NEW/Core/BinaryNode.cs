using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphCS.NEW.Core;

namespace GraphCS.NEW.Core
{
    /// <summary>
    /// Binary node.
    /// </summary>
    class BinaryNode : ANode
    {
        /// <summary>
        /// Binary address.
        /// </summary>
        public override int Addr { get; set; }

        /// <summary>
        /// Initialize new Node class with 0.
        /// </summary>
        public BinaryNode() : base() { }

        /// <summary>
        /// Initialize new instance of BinaryNode with the integer address.
        /// </summary>
        /// <param name="addr">Address</param>
        public BinaryNode(int addr) : base(addr) { }

        /// <summary>
        /// Copies the node to new instance.
        /// </summary>
        /// <param name="addr">Address</param>
        public BinaryNode(ANode node) : base(node) { }

        /// <summary>
        /// Read or write at i-th bit.
        /// </summary>
        /// <param name="i">Bit index</param>
        /// <returns>i-th bit value(0 or 1)</returns>
        public int this[int i]
        {
            set
            {
                if (value == 0)
                {
                    Addr &= ~0 - (1 << i);
                }
                else
                {
                    Addr |= 1 << i;
                }
            }
            get
            {
                return ((Addr >> i) & 1);
            }
        }

        public override bool Equals(object obj)
        {
            // It is not equal if obj is null or different type
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return (Addr == ((BinaryNode)obj).Addr);
        }

        public override int GetHashCode()
        {
            // Returns same value when Equals is true
            return Addr;
        }

        #region Operator overloading

        public static BinaryNode operator &(BinaryNode u, BinaryNode v)
        {
            return new BinaryNode(u.Addr & v.Addr);
        }
        public static BinaryNode operator &(BinaryNode u, int v)
        {
            return new BinaryNode(u.Addr & v);
        }

        public static BinaryNode operator |(BinaryNode u, BinaryNode v)
        {
            return new BinaryNode(u.Addr | v.Addr);
        }
        public static BinaryNode operator |(BinaryNode u, int v)
        {
            return new BinaryNode(u.Addr | v);
        }

        public static BinaryNode operator ^(BinaryNode u, BinaryNode v)
        {
            return new BinaryNode(u.Addr ^ v.Addr);
        }
        public static BinaryNode operator ^(BinaryNode u, int v)
        {
            return new BinaryNode(u.Addr ^ v);
        }

        public static BinaryNode operator <<(BinaryNode u, int i)
        {
            return new BinaryNode(u.Addr << i);
        }

        public static BinaryNode operator >>(BinaryNode u, int i)
        {
            return new BinaryNode(u.Addr >> i);
        }

        public static BinaryNode operator ~(BinaryNode u)
        {
            return new BinaryNode(~u.Addr);
        }

        public static bool operator ==(BinaryNode a, BinaryNode b)
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

        public static bool operator !=(BinaryNode a, BinaryNode b)
        {
            return !(a == b);
        }
        #endregion
    }
}
