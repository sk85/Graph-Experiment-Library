using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graph.Experiment;

namespace Graph.Core
{
    partial class SpinedCube : AGraph
    {
        class Expansion
        {
            UInt32[] Ary;
            public int Count { get; private set; }

            public Expansion()
            {
                Ary = new UInt32[4];
                Count = 0;
            }

            public Expansion Add(UInt32 type, int index)
            {
                Ary[type] |= (UInt32)1 << index;
                Count++;
                return this;
            }

            public Expansion Marge(Expansion exp2)
            {
                Ary[0] |= exp2.Ary[0];
                Ary[1] |= exp2.Ary[1];
                Ary[2] |= exp2.Ary[2];
                Ary[3] |= exp2.Ary[3];
                return this;
            }
        }

        public class Binary
        {
            public UInt32 Bin { get; set; }

            public Binary(UInt32 bin)
            {
                Bin = bin;
            }

            public UInt32 this[int i]
            {
                get
                {
                    return (Bin >> i) & 1;
                }
                set
                {
                    if (value == 1)
                    {
                        Bin |= ((UInt32)1 << i);
                    }
                    else
                    {
                        Bin &= ~((UInt32)1 << i);
                    }
                }
            }

            public Binary Sub(int index, int length)
            {
                return new Binary((Bin >> (index - length + 1)) & (UInt32)((1 << length) - 1));
            }

            public static Binary operator ^ (Binary b1, Binary b2)
            {
                return new Binary(b1.Bin ^ b2.Bin);
            }

            public static Binary operator ^ (Binary b1, UInt32 bits)
            {
                return new Binary(b1.Bin ^ bits);
            }

            public static bool operator == (Binary b, int bits)
            {
                return b.Bin == bits;
            }

            public static bool operator != (Binary b, int bits)
            {
                return b.Bin != bits;
            }

            public override string ToString()
            {
                return Tools.UIntToBinStr(Bin, 32, 4);
            }
        }

        public int Dist(Node node1, Node node2)
        {
            UInt32 type1 = node1.ID & 0b11, type2 = node2.ID & 0b11;
            Binary n1 = new Binary(node1.ID), n2 = new Binary(node2.ID);

            if ((type1 ^ type2) == 0b01)    // 第0ビットが異なる
            {
                if (type1 == 0b00 || type1 == 0b01) // 00 -> 01  or  01 -> 00
                {
                    int min = CalcMinimalExpansionTypeAll(n1, n2).Count + 3;
                    int tmp = CalcMinimalExpansionType0001(n1, n2).Count + 1;
                    if (tmp < min) min = tmp;
                    return min;
                }
                else
                {
                    return -1;
                }
            }
            else if ((type1 ^ type2) == 0b10)    // 第1ビットが異なる
            {
                if (type1 == 0b00 || type1 == 0b10) // 00 -> 10  or  10 -> 00
                {
                    int min = CalcMinimalExpansionTypeAll(n1, n2).Count + 3;
                    int tmp = CalcMinimalExpansionType0010(n1, n2).Count + 1;
                    if (tmp < min) min = tmp;
                    return min;
                }
                else // 01 -> 11  or  11 -> 01
                {
                    return -1;
                    int min = CalcMinimalExpansionTypeAll(n1, n2).Count + 3;
                    int tmp = CalcMinimalExpansionType0111(n1, n2).Count + 1;
                    if (tmp < min) min = tmp;
                    return min;
                }
            }
            else
            {
                return -1;
            }
        }

        public void test()
        {
            var a = CalcMinimalExpansionTypeAll(new Binary(0), new Binary(686));
            for (Node node1 = new Node(0); node1.ID < NodeNum; node1.ID++)
            {
                int[] ary = CalcAllDistanceBFS(node1);
                for (Node node2 = new Node(0); node2.ID < NodeNum; node2.ID++)
                {
                    int dist = Dist(node1, node2);
                    if (dist > 0)
                    {
                        Console.Write("d({0,4}, {1,4}) = {2}\t", node1.ID, node2.ID, dist);
                        if (dist != ary[node2.ID])
                        {
                            Console.WriteLine("{0}", ary[node2.ID]);
                            Console.WriteLine("{0,4} = {1}", node1.ID, Tools.UIntToBinStr(node1.ID, Dimension, 4));
                            Console.WriteLine("{0,4} = {1}", node2.ID, Tools.UIntToBinStr(node2.ID, Dimension, 4));
                            Console.WriteLine("   c = {1}", node1.ID ^ node2.ID, Tools.UIntToBinStr(node1.ID ^ node2.ID, Dimension, 4));
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("ok");
                        }
                    }
                }
            }
        }


        // CalcMinimalExpansionシリーズは、タイプを移動するもの以外をすべて含んだExpansionを返す

        private Expansion CalcMinimalExpansionTypeSingle(Node node1, Node node2)
        {
            UInt32 type = node1.ID ^ 0b11;
            UInt32 c = node1.ID ^ node2.ID;
            Expansion exp = new Expansion();

            // ～第2ビット
            for (int i = Dimension - 1; i >= 2; --i)
            {
                if (((c >> i) & 1) == 1)
                {
                    c ^= DecisionBinary[type, i];
                    exp.Add(type, i);
                }
            }

            // 第1ビット
            if (c == 0b100)
                return null;
            else
                return exp;
        }

        // 済み
        private Expansion CalcMinimalExpansionType0001(Binary node1, Binary node2)
        {
            Binary c = node1 ^ node2;
            Expansion exp = new Expansion();

            // n-1 ～ 4th bit
            for (int i = Dimension - 1; i >= 4; --i)
            {
                if (c[i] == 1)
                {
                    int type = c[i - 2] == 1 ? 0b01 : 0b00;
                    c ^= DecisionBinary[type, i];
                    exp.Add((UInt32)type, i);
                }
            }

            // 3rd, 2nd bit
            var bits = c.Sub(3, 2);
            if (bits == 0b11)
            {
                c ^= DecisionBinary[0b01, 3];
                exp.Add(0b01, 3);
            }
            else if (bits == 0b10)
            {
                c ^= DecisionBinary[0b00, 3];
                exp.Add(0b00, 3);
            }
            else if (bits == 0b01)
            {
                c ^= DecisionBinary[0b00, 2];
                exp.Add(0b00, 2);
            }

            return exp;
        }

        // 済み
        private Expansion CalcMinimalExpansionType0010(Binary node1, Binary node2)
        {
            Binary c = node1 ^ node2;
            Expansion exp = new Expansion();

            // n-1 ～ 4th bit
            for (int i = Dimension - 1; i >= 4; --i)
            {
                if (c[i] == 1)
                {
                    int type = c[i - 1] == 1 ? 0b10 : 0b00;
                    c ^= DecisionBinary[type, i];
                    exp.Add((UInt32)type, i);
                }
            }

            // 3rd, 2nd bit
            if (c[3] == 1)
            {
                c ^= DecisionBinary[0b00, 3];
                exp.Add(0b00, 3);
            }
            if (c[2] == 1)
            {
                c ^= DecisionBinary[0b00, 2];
                exp.Add(0b00, 2);
            }

            return exp;
        }

        // 
        private Expansion CalcMinimalExpansionType0111(Binary node1, Binary node2)
        {
            Binary c = node1 ^ node2;
            Expansion exp = new Expansion();
            int i = Dimension - 1;

            // a/1aを比較するサブルーチン
            void func1(Expansion expX, Expansion expY)
            {
                // 長さが6未満ならばa <= 1aが言える
                if (i < 5)
                {
                    exp.Marge(expX);
                }

                // 長さが6以上のとき
                if (c[i] == 1)
                {
                    if (c[i - 1] == 1)  // abcdef = 11cdef...の場合
                    {
                        Expansion expA = new Expansion().Add(0b11, i + 1).Marge(expY);
                        Expansion expB = new Expansion().Add(0b01, i).Marge(expX);
                        Expansion expC = new Expansion().Add(0b11, i).Marge(expX);
                        c ^= (UInt32)0b11 << (i - 1);
                        i -= 2;
                        func2(expA, expB, expC);
                    }
                    else	// abcdef = 10cdef...の場合
                    {
                        exp.Marge(expX);
                    }
                }
                else if (c[i - 1] == 1)
                {
                    if (c[i - 2] == 1)
                    {
                        if (c[i - 3] == 1)	// abcdef = 0111ef...の場合
                        {
                            exp.Marge(expX);
                        }
                        else if (c[i - 4] == 1) // abcdef = 01101f...の場合
                        {
                            exp.Marge(expX);
                        }
                        else if (i < 7)	// abcdef = 01100f...の場合かつ長さ7以下
                        {
                            exp.Marge(expX);
                        }
                        else	// abcdef = 01100f...の場合かつ長さ7より大きい
                        {
                            Expansion expA = new Expansion().Add(0b11, i + 1).Add(0b01, i).Marge(expY);
                            Expansion expB = new Expansion().Add(0b11, i).Add(0b01, i - 3).Marge(expX);
                            Expansion expC = new Expansion().Add(0b11, i).Add(0b11, i - 3).Marge(expX);
                            c ^= (UInt32)0b11 << (i - 2);
                            i -= 5;
                            func2(expA, expB, expC);
                        }
                    }
                    else    // abcdef = 010def...の場合
                    {
                        Expansion expA = new Expansion().Add(0b01, i + 1).Marge(expY);
                        Expansion expB = new Expansion().Add(0b01, i - 1).Marge(expX);
                        Expansion expC = new Expansion().Add(0b11, i - 1).Marge(expX);
                        c ^= (UInt32)1 << (i - 1);
                        i -= 3;
                        func2(expA, expB, expC);
                    }
                }
                else if (c[i - 2] == 1)
                {
                    if (c[i - 3] == 1)
                    {
                        if (c[i - 4] == 1)  // abcdef = 00111f...の場合
                        {
                            exp.Marge(expX);
                        }
                        else if (c[i - 5] == 1) // abcdefg = 001101g...の場合
                        {
                            exp.Marge(expX);
                        }
                        else if (i < 8) // abcdefg = 001100g...の場合かつ長さが8以下
                        {
                            exp.Marge(expX);
                        }
                        else // abcdefg = 001100g...の場合かつ長さが8より大きい
                        {
                            Expansion expA = new Expansion().Add(0b01, i + 1).Add(0b11, i - 1).Marge(expY);
                            Expansion expB = new Expansion().Add(0b11, i - 2).Add(0b01, i - 3).Marge(expX);
                            Expansion expC = new Expansion().Add(0b11, i - 2).Add(0b11, i - 3).Marge(expX);
                            c ^= (UInt32)0b11 << (i - 3);
                            i -= 6;
                            func2(expA, expB, expC);
                        }
                    }
                    else	// abcdef = 0010ef...の場合
                    {
                        exp.Marge(expX);
                    }
                }
                else	// abcdef = 000def...の場合
                {
                    exp.Marge(expX);
                }
            }

            // a/A,1Aを比較するサブルーチン
            void func2(Expansion expA, Expansion expB, Expansion expC)
            {
                // 長さが4以下のとき
                if (i == 3)
                {
                    if (c[3] == 1)
                    {
                        c ^= 0b1000;
                        exp.Marge(expB);
                    }
                    else
                    {
                        exp.Marge(expA);
                    }
                }
                else if (i < 3)
                {
                    exp.Marge(expA);
                }
                // それ以上
                else if (c[i] == 1)
                {
                    if (c[i - 1] == 1)
                    {
                        if (c[i - 2] == 1)   // abcd=...111d...の場合
                        {
                            Expansion expX = new Expansion().Add(0b11, i).Marge(expA);
                            Expansion expY = new Expansion().Add(0b11, i).Marge(expB);
                            Expansion expZ = new Expansion().Add(0b01, i).Marge(expB);
                            c ^= DecisionBinary[0b11, i];
                            i -= 3;
                            func2(expX, expY, expZ);
                        }
                        else   // abcd=...110d...の場合
                        {
                            Expansion expX = new Expansion().Add(0b01, i + 1).Marge(expC);
                            Expansion expY = new Expansion().Add(0b01, i).Marge(expB);
                            Expansion expZ = new Expansion().Add(0b11, i).Marge(expB);
                            c ^= DecisionBinary[0b10, i];
                            i -= 3;
                            func2(expX, expY, expZ);
                        }
                    }
                    else   // abcd=...10cd...の場合
                    {
                        c ^= (UInt32)1 << i;
                        i -= 1;
                        func1(expB, expA);
                    }
                }
                else
                {
                    if (c[i - 1] == 1)
                    {
                        if (c[i - 2] == 1)   // abcd=...011d...の場合
                        {
                            Expansion expX = new Expansion().Add(0b11, i).Marge(expB);
                            Expansion expY = new Expansion().Add(0b11, i).Marge(expA);
                            Expansion expZ = new Expansion().Add(0b01, i).Marge(expA);
                            c ^= (UInt32)0b11 << (i - 3);
                            i -= 3;
                            func2(expX, expY, expZ);
                        }
                        else  // abcd=...010d...の場合
                        {
                            Expansion expX = new Expansion().Add(0b11, i).Marge(expC);
                            Expansion expY = new Expansion().Add(0b01, i).Marge(expA);
                            Expansion expZ = new Expansion().Add(0b11, i).Marge(expA);
                            c ^= (UInt32)0b1 << i;
                            i -= 3;
                            func2(expX, expY, expZ);
                        }
                    }
                    else   // abcd=...00cd...の場合
                    {
                        i -= 1;
                        func1(expA, expB);
                    }
                }

            }

            // n-2 ~ 5th bit
            while (i >= 5)
            {
                if (c[i] == 1)
                {
                    if (c.Sub(i, 4) == 0b1000 && i < Dimension - 1)    // ..1000...
                    {
                        Expansion expA = new Expansion().Add(0b01, i + 1).Add(0b11, i + 1);
                        Expansion expB = new Expansion().Add(0b01, i).Add(0b01, i - 2);
                        Expansion expC = new Expansion().Add(0b01, i).Add(0b11, i - 2);
                        c = c ^ DecisionBinary[0b01, i + 1] ^ DecisionBinary[0b11, i + 1];
                        i -= 4;
                        func2(expA, expB, expC);
                    }
                    else    //..11... or ..101... or ..1001...
                    {
                        UInt32 type = (UInt32)(c[i - 1] == 1 ? 0b11 : 0b01);
                        Expansion expA = new Expansion().Add(type, i);
                        Expansion expB = new Expansion().Add(type ^ 0b10, i);
                        c ^= DecisionBinary[type, i];
                        i -= 2;
                        func1(expA, expB);
                    }
                }
            }

            // 4 ~ 2nd bit
            //if (c[4])

            return exp;
        }

        // 済み
        private Expansion CalcMinimalExpansionTypeAll(Binary node1, Binary node2)
        {
            Binary c = node1 ^ node2;
            Expansion exp = new Expansion();

            // n-1 ～ 4th bit
            for (int i = Dimension - 1; i >= 4; --i)
            {
                if (c[i] == 1)
                {
                    var type = c.Sub(i - 1, 2).Bin;
                    c ^= DecisionBinary[type, i];
                    exp.Add(type, i);
                }
            }

            // 3rd bit
            var bits = c.Sub(3, 2);
            if (bits == 0b11)   // 11xx
            {
                c ^= DecisionBinary[0b01, 3];
                exp.Add(0b01, 3);
            }
            else if (bits == 0b10)  // 10xx
            {
                c ^= DecisionBinary[0b00, 3];
                exp.Add(0b00, 3);
            }

            // 2nd bit
            if (c[2] == 1)
            {
                UInt32 type1 = node1.Sub(1, 2).Bin, type2 = node2.Sub(1, 2).Bin;
                if ((type1 == 0b01 && type2 == 0b11) || (type1 == 0b11 && type2 == 0b01))
                {
                    exp.Add(0b00, 2);
                }
            }

            return exp;
        }
    }
}
