using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Experiment
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

        public static int GetPopCount(UInt32 bin)
        {
            bin = (bin & 0x55555555) + (bin >> 1 & 0x55555555);
            bin = (bin & 0x33333333) + (bin >> 2 & 0x33333333);
            bin = (bin & 0x0f0f0f0f) + (bin >> 4 & 0x0f0f0f0f);
            bin = (bin & 0x00ff00ff) + (bin >> 8 & 0x00ff00ff);
            return (int)((bin & 0x0000ffff) + (bin >> 16 & 0x0000ffff));
        }
    }
}
