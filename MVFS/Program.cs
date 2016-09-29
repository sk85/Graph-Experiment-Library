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
            // とりあえずn = 5まで
            for (int dim = 2; dim < 5; dim++)
            {
                VertexSetSet VSS = new VertexSetSet();
                VSS.Initialize(dim);
                while (!VSS.IsContainsFVS())
                {
                    VSS = VSS.GetNext();
                    Console.WriteLine("n = {0,2}, |VS| = {1}, |VSS| = {2}", dim, VSS.Num, VSS.Count);
                }
                Console.WriteLine("n = {0,2}, |VS| = {1}, |VSS| = {2}, MFVS発見", dim, VSS.Num, VSS.Count);
            }
        }
    }

    class VertexSet : List<short>
    {
        // 要素数1の状態に初期化
        public void Initialize()
        {
            Add(0);
        }

        // 引数の条件のノードが存在するか
        public bool IsExist(short node)
        {
            return false; // TODO
        }

        // FVSであるかどうか
        public bool IsFVS()
        {
            // TODO
            // 幅優先及び深さ優先探索でループの存在を確認
            return false;
        }
    }

    class VertexSetSet : List<VertexSet>
    {
        // 以下の"要素数"はVSの要素数のこと(VSSのことではない)

        public int Num; // VSの要素数

        // 要素数1の状態に初期化
        public void Initialize(int dim)
        {
            Num = 1;
            var VS = new VertexSet();
            VS.Initialize();
            Add(VS);    // TODO
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
            VertexSetSet VSS = new VertexSetSet();
            foreach(VertexSet VS in this)
            {

            }
            return null;    //TODO
        }
    }
}
