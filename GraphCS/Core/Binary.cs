using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    public class Binary
    {
        public uint Bin { get; set; }

        public Binary(uint bin)
        {
            Bin = bin;
        }

        /// <summary>
        /// 第iビットにアクセス。
        /// 書き込むときは0以外の値は1と考える
        /// </summary>
        /// <param name="i">ビットの添字</param>
        /// <returns>ビットの値</returns>
        public uint this[int i]
        {
            set
            {
                if (value == 0)
                {
                    Bin &= 0xFFFFFFFF - ((uint)1 << i);
                }
                else
                {
                    Bin |= (uint)1 << i;
                }
            }
            get
            {
                return ((Bin >> i) & 1);
            }
        }

        /// <summary>
        /// サブストリングを取得/設定。
        /// 第jビットから第iビットまで。
        /// </summary>
        /// <param name="i">サブストリングの左端</param>
        /// <param name="j">サブストリングの右端</param>
        /// <returns>サブストリング</returns>
        public uint this[int i, int j]
        {
            get
            {
                if (j >= i)
                {
                    throw new IndexOutOfRangeException("jはi未満の値でなくてはいけません。");
                }
                if (j > 31 || j < 0 || i > 31 || i < 0)
                {
                    throw new IndexOutOfRangeException("i, jは0～31で指定してください。");
                }
                return (Bin << (31 - i)) >> (31 - i + j);
            }
            set
            {
                if (j >= i)
                {
                    throw new IndexOutOfRangeException("jはi未満の値でなくてはいけません。");
                }
                if (j > 31 || j < 0 || i > 31 || i < 0)
                {
                    throw new IndexOutOfRangeException("i, jは0～31で指定してください。");
                }
                if (value >= (1 << (i - j + 1)))
                {
                    throw new ArgumentOutOfRangeException("設定する値はiとjの間におさめてください。");
                }
                Bin = (Bin & ((0xFFFFFFFF << (i + 1)) | (0xFFFFFFFF >> (31 - j))) | (value << j));
            }
        }

        /// <summary>
        /// 長さを指定して文字列で返す
        /// </summary>
        /// <param name="length">長さ</param>
        /// <returns>2進数列の文字列</returns>
        public string ToString(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("長さは正の数でなくてはいけません。");
            }
            string str = "";
            for (int i = length - 1; i >= 0; i--)
            {
                str += $"{(Bin & (1 << i)) >> i}";
            }
            return str;
        }
    }
}
