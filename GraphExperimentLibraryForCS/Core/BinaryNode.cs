using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    /// <summary>
    /// バイナリ系のノードクラス。
    /// バイナリ系はIDとアドレスが一致するのであまり使わないかもしれないが、
    /// インデクサや演算子のオーバーロードは便利かもしれない。
    /// </summary>
    class BinaryNode : Node
    {
        private UInt32 __Addr;
        private UInt32 __ID;

        /// <summary>
        /// ノードアドレス。IDと連動
        /// </summary>
        public UInt32 Addr
        {
            get { return __Addr; }
            set
            {
                __Addr = value;
                __ID = __Addr;
            }
        }

        /// <summary>
        /// ノードID。アドレスと連動
        /// </summary>
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
        
        public int this[int i]
        {
            get
            {
                return (int)((Addr >> i) & 1);
            }
            set
            {
                Addr ^= ((UInt32)1 << value);
            }
        }
    }
}
