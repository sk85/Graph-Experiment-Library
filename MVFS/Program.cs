using System.Collections.Generic;
using System;

namespace MVFS
{
    // VSS  ：頂点集合集合
    // VS   ：頂点集合
    // FVS  ：フィードバック頂点集合

    class Program
    {
        static void Main(string[] args)
        {
            int Num = 6, Dim = 3;

            int[] distAry = new int[Num];
            for (int i = 1; i < Num; i++)
            {
                distAry[i] = 1;
            }
            distAry[0] = 0;

            bool f = true;
            while (f)
            {
                for (int i = 0; i < Num; i++)
                {
                    if (distAry[i] < Dim)
                    {
                        distAry[i]++;
                        break;
                    }
                    else
                    {
                        distAry[i] = 1;
                    }
                }

                for (int i = 0; i < Num; i++) Console.Write(distAry[i]);
                Console.Write('\n');
                Console.ReadKey();
                for (int i = 0; i < Num; i++) f = f && (distAry[i] == Dim);
                f = !f;
            }

            return;
            // とりあえずn = 5まで
            for (int dim = 2; dim < 5; dim++)
            {
                VertexSetSet VSS = new VertexSetSet(dim);
                while (!VSS.IsContainsFVS())
                {
                    VSS = VSS.GetNext();
                    Console.WriteLine("n = {0,2}, |VS| = {1}, |VSS| = {2}", dim, VSS.Num, VSS.Count);
                }
                Console.WriteLine("n = {0,2}, |VS| = {1}, |VSS| = {2}, MFVS発見", dim, VSS.Num, VSS.Count);
            }
        }
    }

    class VertexSet
    {
        public int Dim; // グラフの次元数
        public int Num; // 要素数
        public short[] Array;

        // コンストラクタ(要素数１)
        public VertexSet(int dim)
        {
            // 要素数1の状態に初期化
            Dim = dim;
            Num = 1;
            Array = new short[1];
            Array[0] = 0;
        }

        // 次のサイズを作るためのコンストラクタ
        public VertexSet(VertexSet old, short value)
        {
            Dim = old.Dim;
            Num = old.Num + 1;
            Array = new short[Num];
            for (int i = 0; i < old.Num; i++)
            {
                Array[i] = old.Array[i];
            }
            Array[old.Num] = value;
        }

        // 引数の条件のノードアドレスを返す(0なら失敗)
        private short DistAryToAddr(int[] distAry)
        {

            return 0; // TODO
        }

        // FVSであるかどうか
        public bool IsFVS()
        {
            // TODO
            // 幅優先及び深さ優先探索でループの存在を確認
            return false;
        }

        // このVSから派生する次の要素数のVS群を返す
        public IEnumerable<VertexSet> GetNextVSs()
        {
            int[] distAry = new int[Num];
            for (int i = 1; i < Num; i++) distAry[i] = 1;
            distAry[0] = 0;

            bool f = true;
            while (f)
            {
                for (int i = 0; i < Num; i++)
                {
                    if (distAry[i] < Dim)
                    {
                        distAry[i]++;
                        break;
                    }
                    else
                    {
                        distAry[i] = 1;
                    }
                }

                short newAddr = DistAryToAddr(distAry);
                if (newAddr != 0) yield return new VertexSet(this, newAddr);

                for (int i = 0; i < Num; i++) f = f && (distAry[i] == Dim);
                f = !f;
            }
            yield return null;
        }
    }

    class VertexSetSet : List<VertexSet>
    {
        // 以下の"要素数"はVSの要素数のこと(VSSのことではない)

        public int Dim; // グラフの次元数
        public int Num; // VSの要素数

        public VertexSetSet(int dim)
        {
            // 要素数1の状態に初期化
            Dim = dim;
            Num = 1;
            var VS = new VertexSet(dim);
            Add(VS);
        }

        // FVSを含んでいるか
        public bool IsContainsFVS()
        {
            foreach(VertexSet VS in this)
            {
                if (VS.IsFVS()) return true;
            }
            return false;
        }

        // 要素数が一個増えたときのVSSを取得
        public VertexSetSet GetNext()
        {
            Num++;
            VertexSetSet VSS = new VertexSetSet(Dim + 1);
            foreach(VertexSet VS in this)
            {
                var newVSs = VS.GetNextVSs();
                foreach(VertexSet newVS in newVSs)
                {
                    VSS.Add(newVS);
                }
            }
            return VSS;
        }
    }
}
