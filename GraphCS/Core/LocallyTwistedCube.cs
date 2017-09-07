#define Optimized

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    class LocallyTwistedCube : AGraph
    {
        // Adjacent decided binary
        static readonly uint[,] ADB =
            {
                {
                    0b00000000000000000000000000000001,
                    0b00000000000000000000000000000010,
                    0b00000000000000000000000000000100,
                    0b00000000000000000000000000001000,
                    0b00000000000000000000000000010000,
                    0b00000000000000000000000000100000,
                    0b00000000000000000000000001000000,
                    0b00000000000000000000000010000000,
                    0b00000000000000000000000100000000,
                    0b00000000000000000000001000000000,
                    0b00000000000000000000010000000000,
                    0b00000000000000000000100000000000,
                    0b00000000000000000001000000000000,
                    0b00000000000000000010000000000000,
                    0b00000000000000000100000000000000,
                    0b00000000000000001000000000000000,
                    0b00000000000000010000000000000000,
                    0b00000000000000100000000000000000,
                    0b00000000000001000000000000000000,
                    0b00000000000010000000000000000000,
                    0b00000000000100000000000000000000,
                    0b00000000001000000000000000000000,
                    0b00000000010000000000000000000000,
                    0b00000000100000000000000000000000,
                    0b00000001000000000000000000000000,
                    0b00000010000000000000000000000000,
                    0b00000100000000000000000000000000,
                    0b00001000000000000000000000000000,
                    0b00010000000000000000000000000000,
                    0b00100000000000000000000000000000,
                    0b01000000000000000000000000000000,
                    0b10000000000000000000000000000000,
                },
                {
                    0b00000000000000000000000000000001,
                    0b00000000000000000000000000000010,
                    0b00000000000000000000000000000110,
                    0b00000000000000000000000000001100,
                    0b00000000000000000000000000011000,
                    0b00000000000000000000000000110000,
                    0b00000000000000000000000001100000,
                    0b00000000000000000000000011000000,
                    0b00000000000000000000000110000000,
                    0b00000000000000000000001100000000,
                    0b00000000000000000000011000000000,
                    0b00000000000000000000110000000000,
                    0b00000000000000000001100000000000,
                    0b00000000000000000011000000000000,
                    0b00000000000000000110000000000000,
                    0b00000000000000001100000000000000,
                    0b00000000000000011000000000000000,
                    0b00000000000000110000000000000000,
                    0b00000000000001100000000000000000,
                    0b00000000000011000000000000000000,
                    0b00000000000110000000000000000000,
                    0b00000000001100000000000000000000,
                    0b00000000011000000000000000000000,
                    0b00000000110000000000000000000000,
                    0b00000001100000000000000000000000,
                    0b00000011000000000000000000000000,
                    0b00000110000000000000000000000000,
                    0b00001100000000000000000000000000,
                    0b00011000000000000000000000000000,
                    0b00110000000000000000000000000000,
                    0b01100000000000000000000000000000,
                    0b11000000000000000000000000000000,
                }
            };

        // Constructor
        public LocallyTwistedCube(int dim, int randSeed) : base(dim, randSeed)
        {
        }

        // Abstract menbers
        public override string Name
        {
            get { return "LocallyTwistedCube"; }
        }

        public override int GetDegree(uint Node)
        {
            return Dimension;
        }

        public override uint GetNeighbor(uint node, int index)
        {
            return node ^ ADB[node & 0b1, index];
        }

        protected override uint CalcNodeNum()
        {
            return (uint)1 << Dimension;
        }

        public override int CalcDistance(uint node1, uint node2)
        {
#if Optimized
            // わかりやすさ度外視で効率よく書いたコード
            uint c1 = node1 ^ node2;
            uint c2 = c1;
            uint t = node1 & 1;
            int count1 = 0;
            int count2 = 0;

            for (int i = Dimension - 1; i > 1; --i)
            {
                if ((c1 >> i) == 1)
                {
                    c1 ^= ADB[t, i];
                    count1++;
                }
                if ((c2 >> i) == 1)
                {
                    c2 ^= ADB[(c2 >> (i - 1)) & 1, i];
                    count2++;
                }
            }
            count1 += (int)((c1 >> 1) + ((c1 & 1) << 10));
            count2 += 2 + (int)((c2 >> 1) - (c2 & 1));

            return count1 < count2 ? count1 : count2;
#else
            // わかりやすく書いたコード
            
            int count1 = 0;  // サブグラフを移動する場合の最短経路長
            int count2 = 0;  // サブグラフを移動しない場合の最短経路長

            // サブグラフを移動する場合
            {
                Binary c = new Binary(node1 ^ node2);  // 出発頂点と目的頂点の排他的論理和
                int i = Dimension - 1;  // 現在第何ビットをに注目しているか。左端から第1ビットまで
                while (i > 0)
                {
                    if (c[i] == 1)
                    {
                        count1++;
                        i -= 2;
                    }
                    else
                    {
                        i--;
                    }
                }

                // 同じサブグラフに属する場合
                if ((node1 & 1) == (node2 & 1))
                {
                    count1 += 2; // サブグラフ間の枝が2本
                }
                // 異なるサブグラフに属する場合
                else
                {
                    count1 += 1; // サブグラフ間の枝が1本
                }
            }

            // サブグラフを移動しない場合
            if ((node1 & 1) == (node2 & 1))
            {
                Binary c = new Binary(node1 ^ node2);  // 出発頂点と目的頂点の排他的論理和
                uint type = node1 & 1;  // どちらのサブグラフに属するか
                int i = Dimension - 1;  // 現在第何ビットをに注目しているか。左端から第1ビットまで
                while (i > 0)
                {
                    if (c[i] == 1)
                    {
                        c[i] = 0;
                        c[i - 1] = c[i - 1] ^ type;
                        count2++;
                    }
                    i--;
                }
            }
            else
            {
                count2 = 100;
            }

            // count1とcount2の小さい方を採用
            return count1 < count2 ? count1 : count2;
#endif
        }

        public override int[] CalcForwardNeighbor(uint node1, uint node2)
        {
            var x1 = new Binary(node1 ^ node2);
            var x2 = new Binary(x1.Bin);
            var x3 = new Binary(x1.Bin);
            var x4 = new Binary(x1.Bin);
            var s1 = new Binary(0);
            var s4 = new Binary(0);
            var _s4 = new Binary(0);
            int c1 = 0;
            int c2 = 0;
            int c3 = 0;
            int type = (int)node1 & 1;
            var flag = false;

            for (int k = Dimension - 1; k >= 1; k--)
            {
                if (x1[k] == 1)
                {
                    s1[k] = 1;
                    x1.Bin ^= ADB[type, k];
                    c1++;
                }
                if (x2[k] == 1)
                {
                    x2.Bin ^= ADB[type ^ 1, k];
                    c2++;
                }
                if (x3[k] == 1)
                {
                    x3.Bin ^= ADB[x3[k - 1], k];
                    c3++;
                }
            }

            int j = Dimension - 1;
            while (j > 1)
            {
                if (x4[j] == 1)
                {

                    if (x4[j - 1] == 0)
                    {
                        s4.Bin |= _s4.Bin;

                        if (type == 0)
                        {
                            s4[j] = 1;
                        }
                        else
                        {
                            _s4[j] = 1;
                            if (flag)
                            {
                                s4[j + 1] = 1;
                            }
                        }
                    }
                    else
                    {
                        if (type == 0)
                        {
                            _s4[j] = 1;
                        }
                        else
                        {
                            s4[j] = 1;
                            if (flag)
                            {
                                _s4[j + 1] = 1;
                            }
                        }

                    }
                    x4.Bin ^= ADB[x4[j - 1], j];
                    flag = true;
                    j -= 2;
                }
                else
                {
                    _s4.Bin = 0;
                    flag = false;
                    j--;
                }
            }
            if (j == 1 && x4[1] == 1)
            {
                s4.Bin |= _s4.Bin;
                s4[1] = 1;
                if (flag && type == 1)
                {
                    s4[2] = 1;
                }
            }

            var a = new int[Dimension];
            if (((node1 ^ node2) & 1) == 0) // 同じサブグラフの場合
            {
                c3 += 2;    // サブグラフの移動分

                if (c1 < c3)    // 移動しないほうが早い場合
                {
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = (int)s1[i];
                    }
                }
                else if (c1 == c3)  // 移動してもしなくても同じ場合
                {
                    s1.Bin |= s4.Bin;
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = (int)s1[i];
                    }
                    a[0] = 1;
                }
                else // 移動したほうが早い場合
                {
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = (int)s4[i];
                    }
                    a[0] = 1;
                }
            }
            else // 違うサブグラフの場合
            {
                for (int i = 0; i < Dimension; i++)
                {
                    a[i] = (int)s4[i];
                }
                if (c2 == c3)
                {
                    a[0] = 1;
                }
            }
            return a;
        }

        public int[] GetForwardNeighbor2(uint node1, uint node2)
        {
            var x1 = new Binary(node1 ^ node2);
            var x2 = new Binary(x1.Bin);
            var x3 = new Binary(x1.Bin);
            var x4 = new Binary(x1.Bin);
            var s1 = new Binary(0);
            var s4 = new Binary[] { new Binary(0), new Binary(0) };
            int c1 = 0;
            int c2 = 0;
            int c3 = 0;
            int type = (int)node1 & 1;
            var flag = false;

            for (int k = Dimension - 1; k >= 1; k--)
            {
                if (x1[k] == 1)
                {
                    s1[k] = 1;
                    x1.Bin ^= ADB[type, k];
                    c1++;
                }
                if (x2[k] == 1)
                {
                    x2.Bin ^= ADB[type ^ 1, k];
                    c2++;
                }
                if (x3[k] == 1)
                {
                    x3.Bin ^= ADB[x3[k - 1], k];
                    c3++;
                }
            }

            int j = Dimension - 1;
            while (j > 1)
            {
                if (x4[j] == 1)
                {
                    if (x4[j - 1] == 0) s4[0].Bin |= s4[1].Bin;

                    if (type == 1 && flag) s4[x4[j - 1]][j + 1] = 1;

                    s4[x4[j - 1] ^ type][j] = 1;

                    x4.Bin ^= ADB[x4[j - 1], j];
                    flag = true;
                    j -= 2;
                }
                else
                {
                    s4[1].Bin = 0;
                    flag = false;
                    j--;
                }
            }
            if (j == 1 && x4[1] == 1)
            {
                s4[0].Bin |= s4[1].Bin;
                s4[0][1] = 1;
                if (flag && type == 1)
                {
                    s4[0][2] = 1;
                }
            }

            var a = new int[Dimension];
            if (((node1 ^ node2) & 1) == 0) // 同じサブグラフの場合
            {
                c3 += 2;    // サブグラフの移動分

                if (c1 < c3)    // 移動しないほうが早い場合
                {
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = (int)s1[i];
                    }
                }
                else if (c1 == c3)  // 移動してもしなくても同じ場合
                {
                    s1.Bin |= s4[0].Bin;
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = (int)s1[i];
                    }
                    a[0] = 1;
                }
                else // 移動したほうが早い場合
                {
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = (int)s4[0][i];
                    }
                    a[0] = 1;
                }
            }
            else // 違うサブグラフの場合
            {
                for (int i = 0; i < Dimension; i++)
                {
                    a[i] = (int)s4[0][i];
                }
                if (c2 == c3)
                {
                    a[0] = 1;
                }
            }
            return a;
        }

        // int[i]はd(u, v) - d(n(u, i), v)
        public int[] Test(uint node1, uint node2)
        {
            // 1 : node1のタイプのexpansion
            // 2 : node1のタイプの逆のexpansion
            // 3 : expansion
            // 4 : 両方通る場合の相対距離
            Binary x1 = new Binary(node1 ^ node2);
            Binary x2 = new Binary(node1 ^ node2);
            Binary x3 = new Binary(node1 ^ node2);
            Binary x4 = new Binary(node1 ^ node2);
            int[] a1 = new int[Dimension];
            int[] a2 = new int[Dimension];
            int[] a4 = new int[Dimension];
            int[] a = new int[Dimension];
            int c1 = 0;
            int c2 = 0;
            int c3 = 2;    // タイプの移動の分
            uint type = node1 & 1;
            
            for (int k = Dimension - 1; k >= 1; k--)
            {
                if (x1[k] == 1)
                {
                    a1[k] = 1;
                    x1.Bin ^= ADB[type, k];
                    c1++;
                }
                if (x2[k] == 1)
                {
                    a2[k] = 1;
                    x2.Bin ^= ADB[type ^ 1, k];
                    c2++;
                }
                if (x3[k] == 1)
                {
                    x3.Bin ^= ADB[x3[k - 1], k];
                    c3++;
                }
            }

            int j = Dimension - 1;
            var f = new Stack<int>();
            var s = new Stack<int>();
            var tmp1 = new Stack<int>();
            var tmp2 = new Stack<int>();
            var flag = false;
            while (j > 1)
            {
                if (x4[j] == 1)
                {
                    if (x4[j - 1] == 0)
                    {
                        while (tmp1.Count > 0) f.Push(tmp1.Pop());
                        while (tmp2.Count > 0) s.Push(tmp2.Pop());

                        if (type == 0)
                        {
                            f.Push(j);
                        }
                        else
                        {
                            tmp1.Push(j);
                            if (j < Dimension - 1 && flag) f.Push(j + 1);
                        }

                        if (j < Dimension - 1) s.Push(j + 1);
                    }
                    else
                    {
                        if (type == 0)
                        {
                            tmp1.Push(j);
                        }
                        else
                        {
                            f.Push(j);
                            if (j < Dimension - 1 && flag) tmp1.Push(j + 1);
                        }

                        if (j < Dimension - 1) tmp2.Push(j + 1);
                    }
                    flag = true;
                    x4[j] = 0;
                    x4[j - 1] = 0;
                    j -= 2;
                }
                else
                {
                    if (flag)
                    {
                        while (tmp1.Count > 0) s.Push(tmp1.Pop());
                        tmp2.Clear();
                        if (type == 0)
                        {
                            tmp2.Push(j);
                            s.Push(j + 1);
                        }
                        else
                        {
                            tmp2.Push(j + 1);
                        }
                        flag = false;
                    }
                    j--;
                }
            }
            
            if (j == 1)
            {
                while (tmp1.Count > 0) f.Push(tmp1.Pop());
                //while (tmp2.Count > 0) s.Push(tmp2.Pop());
                //f.Push(1);
                if (flag)
                {
                    if (type == 1)
                    {
                        //f.Push(2);

                    }
                    else
                    {

                       // s.Push(2);
                    }
                }
            }
            else
            {
                //while (tmp1.Count > 0) s.Push(tmp1.Pop());
                if (flag)
                {
                    s.Push(1);
                }
            }
            

            if (((node1 ^ node2) & 1) == 0)
            {
                // 1の前方以外は後方
                if (c1 + 1 < c3)
                {
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = a1[i] == 1 ? 0 : 2;
                    }
                    return a;
                }
                // 4の前方は横方
                else if (c1 + 1 == c3)
                {
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = a1[i] == 1 ? 0 : 2;
                    }
                    foreach (var k in f)
                    {
                        if (a[k] == 2) a[k] = 1;
                    }
                    a[0] = 1;
                    return a;
                }
                else if (c1 == c3)
                {
                    for (int i = 0; i < Dimension; i++)
                    {
                        a[i] = a1[i] == 1 ? 0 : 2;
                    }
                    foreach (var k in f)
                    {
                        a[k] = 0;
                    }
                    foreach (var k in s)
                    {
                        if (a[k] != 0) a[k] = 1;
                    }
                    a[0] = 0;
                    //return a;
                }
            }
            return null;
        }
    }
}


