using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    /// <summary>
    /// ノードを表す抽象クラスです。
    /// <para>ここを抽象化することで、バイナリ系でも順列系でも対応できるようになってます。</para>
    /// <para>
    /// 各ノードに0以上ノード数未満の一意なIDを与えます。
    /// バイナリ系(キューブ系)はそのままなので直感的ですが、順列系だと順列に辞書番号を付ける必要があります。
    /// </para>
    /// </summary>
    abstract class INode
    {
        /// <summary>
        /// 0以上ノード数未満の一意なID
        /// </summary>
        public abstract UInt32 ID { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ノードID</param>
        public INode(UInt32 id)
        {
            ID = id;
        }
    }
}
