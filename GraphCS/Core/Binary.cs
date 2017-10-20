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

namespace aaa
{
    public class Binary
    {
        public int Bin { get; set; }
        public Binary(int bin)
        {
            Bin = bin;
        }
        
        #region 演算子のオーバーロード

        public static Binary operator &(Binary u, Binary v)
        {
            return new Binary(u.Bin & v.Bin);
        }
        public static Binary operator &(Binary u, int v)
        {
            return new Binary(u.Bin & v);
        }

        public static Binary operator |(Binary u, Binary v)
        {
            return new Binary(u.Bin | v.Bin);
        }
        public static Binary operator |(Binary u, int v)
        {
            return new Binary(u.Bin | v);
        }

        public static Binary operator ^(Binary u, Binary v)
        {
            return new Binary(u.Bin | v.Bin);
        }
        public static Binary operator ^(Binary u, int v)
        {
            return new Binary(u.Bin | v);
        }

        public static Binary operator <<(Binary u, int i)
        {
            return new Binary(u.Bin << i);
        }

        public static Binary operator >>(Binary u, int i)
        {
            return new Binary(u.Bin >> i);
        }

        public static Binary operator ~(Binary u)
        {
            return new Binary(~u.Bin);
        }
        
        public override bool Equals(object obj)
        {
            // objがnullか、型が違うときは、等価でない
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return (Bin == ((Binary) obj).Bin);
        }

        // Equalsがtrueを返すときに同じ値を返す
        public override int GetHashCode()
        {
            return Bin;
        }
        #endregion
    }
}
