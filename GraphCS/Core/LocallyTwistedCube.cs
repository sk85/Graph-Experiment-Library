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
        static readonly uint[,] ADB =
            {
                {
                    0b0000000000000001,
                    0b0000000000000010,
                    0b0000000000000100,
                    0b0000000000001000,
                    0b0000000000010000,
                    0b0000000000100000,
                    0b0000000001000000,
                    0b0000000010000000,
                    0b0000000100000000,
                    0b0000001000000000,
                    0b0000010000000000,
                    0b0000100000000000,
                    0b0001000000000000,
                    0b0010000000000000,
                    0b0100000000000000,
                    0b1000000000000000
                },
                {
                    0b0000000000000001,
                    0b0000000000000010,
                    0b0000000000000110,
                    0b0000000000001100,
                    0b0000000000011000,
                    0b0000000000110000,
                    0b0000000001100000,
                    0b0000000011000000,
                    0b0000000110000000,
                    0b0000001100000000,
                    0b0000011000000000,
                    0b0000110000000000,
                    0b0001100000000000,
                    0b0011000000000000,
                    0b0110000000000000,
                    0b1100000000000000
                }
            };
        public LocallyTwistedCube(int dim, int randSeed) : base(dim, randSeed)
        {
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

        public IEnumerable<uint> Test(uint node1, uint node2)
        {
            Binary c = new Binary(node1 ^ node2);
            uint type = node1 & 1;

            if (c[0] == 1) yield break;

            // 片方のサブグラフしか通らない場合
            for (int k = Dimension - 1; k >= 1; k--)
            {
                if (c[k] == 1)
                {
                    yield return GetNeighbor(node1, k);
                    c.Bin ^= ADB[type, k];
                }
            }
        }
    }
}
