using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Debug
{
    static class Tools
    {
        /// <summary>
        /// 数値を2進数列の文字列に変換します。
        /// </summary>
        /// <param name="bin">数値</param>
        /// <param name="length">長さ</param>
        /// <param name="interval">スペースを入れる間隔</param>
        /// <returns>文字列化した二進数列</returns>
        public static string UIntToBinStr(UInt32 bin, int length, int interval)
        {
            string str = "";
            for (int i = 0; i < length; i++)
            {
                if (i % interval == 0) str = " " + str;
                str = ((bin >> i) & 1).ToString() + str;
            }
            return str;
        }
    }
}
