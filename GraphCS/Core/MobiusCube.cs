using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Core
{
    class MobiusCube : AGraph
    {
        public MobiusCube(int dim, int randSeed) : base(dim, randSeed)
        {
            Type = 0;
        }

        public MobiusCube(int dim, int randSeed, int type) : base(dim, randSeed)
        {
            Type = type;
        }


        /// <summary>
        /// Type of MobiusCube.
        /// Defalt type is 0;
        /// </summary>
        public int Type { get; set; }

        public override string Name
        {
            get { return $"{Type}-MobiusCube"; }
        }

        public override int GetDegree(uint Node)
        {
            return Dimension;
        }

        public override uint GetNeighbor(uint node, int index)
        {
            int type = index == Dimension - 1
                ? Type
                : (int)((node >> (index + 1)) & 1);

            if (type == 0)
            {
                return node ^ ((uint)1 << index);    // 100...000
            }
            else
            {
                return node ^ (((uint)1 << (index + 1)) - 1);  // 111...111
            }
        }

        protected override uint CalcNodeNum()
        {
            return (uint)1 << Dimension;
        }


        // メビウスキューブで色々表示
        public void Test()
        {
            while (true)
            {
                var u = new Binary2((int)(Rand.NextDouble() * NodeNum));
                var v = new Binary2((int)(Rand.NextDouble() * NodeNum));
                //var u = new Binary2(0b1000010000);
                //var v = new Binary2(0b1000010110);

                Console.WriteLine(" u  = {0}", u.ToString(Dimension, 1));
                Console.WriteLine(" v  = {0}", v.ToString(Dimension, 1));
                Console.WriteLine("u^v = {0}", (u ^ v).ToString(Dimension, 1));

                var d = CalcDistance((uint)u.Bin, (uint)v.Bin);
                var relDis = new int[Dimension];
                Console.Write("     ");
                for (int i = Dimension - 1; i >= 0; i--)
                {
                    relDis[i] = CalcDistance(GetNeighbor((uint)u.Bin, i), (uint)v.Bin) - d;
                    Console.Write(" {0}", (char)('B' + relDis[i]));
                }
                Console.WriteLine();

                // k の計算
                // kより上に前方はない．
                int k = Dimension - 1;
                for (; k > 0 && u[k] == v[k]; k--) { }
                Console.CursorLeft = (Dimension - k) * 2 + 4;
                Console.WriteLine(k);

                Console.Write(u[k + 1] == (u ^ v)[k - 1] ? "Good " : "Bad ");
                Console.WriteLine((u ^ v)[k - 1] == 1 ? "E" : "e");

                // Good eは常にOK
                if (u[k + 1] == (u ^ v)[k - 1] && (u ^ v)[k - 1] == 0 && relDis[k] != -1)
                {
                    Console.ReadKey();
                }
                // Good E
                else if (u[k + 1] == (u ^ v)[k - 1] && (u ^ v)[k - 1] == 1)
                {
                    Console.WriteLine(relDis[k] == -1 ? "OK" : "NG");
                    if (u[k] == 0 && u[k - 1] == 1)
                    Console.ReadKey();
                }

                Console.WriteLine("--------------------------------------------");
            }
        }

        class Binary2
        {
            public int Bin { get; private set; }

            public Binary2(int bin)
            {
                Bin = bin;
            }

            #region 演算子系
            public int this[int i]
            {
                get
                {
                    return (Bin >> i) & 1;
                }
                set
                {
                    if (value == 0)
                    {
                        Bin &= (int)(0xFFFFFFFF - (1 << i));
                    }
                    else
                    {
                        Bin |= 1 << i;
                    }
                }
            }

            public static Binary2 operator^(Binary2 u, Binary2 v)
            {
                return new Binary2(u.Bin ^ v.Bin);
            }
            public static Binary2 operator ~(Binary2 u)
            {
                return new Binary2(~u.Bin);
            }

            #endregion

            public string ToString(int length, int interval)
            {
                string str = "";
                for (int i = length - 1; i >= 0; i--)
                {
                    str += this[i];
                    if (i % interval == 0) str += ' ';
                }
                return str;
            }
        }
    }
}
