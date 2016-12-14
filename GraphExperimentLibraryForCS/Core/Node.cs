using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Core
{
    /// <summary>
    /// ノードを表すクラスです。
    /// <para>
    /// <para>バイナリ系、順列系、符号付き順列系など、それぞれ継承したクラスを作って使うといいです。</para>
    /// 各ノードに0以上ノード数未満の一意なIDを与えます。
    /// バイナリ系(キューブ系)はそのままなので直感的ですが、順列系だと順列に辞書番号を付ける必要があります。
    /// </para>
    /// </summary>
    class Node
    {
        /// <summary>
        /// 0以上ノード数未満の一意なID
        /// </summary>
        public virtual UInt32 ID { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">ノードID</param>
        public Node(UInt32 id)
        {
            ID = id;
        }
    }
}
