using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulator
{
    public class Network
    {
        public Random rand { get; }
        public int Dim { get; }
        public uint NodeNum { get; }

        public Network()
        {
            rand = new Random();
        }

        // 通信先のノードアドレスをランダムに取得
        public uint GetDestRandom(uint startAddr)
        {
            uint destAddr = startAddr;
            while (destAddr == startAddr)
            {
                destAddr = BitConverter.ToUInt32(BitConverter.GetBytes(rand.Next()), 0);
                destAddr &= (((uint)1 << Dim) - 1);
            }
            return destAddr;
        }
    }

    public class Packet
    {
        public int Length;
    }

    public class Node
    {
        private uint Addr { get; }
        private Network NW { get; }

        private int Frequency { get; }  // 通信の発生頻度(％)
        private int MsgLen { get; }     // 発生するメッセージの長さ(パケット)
        private uint DestAddr { get; set; }  // 通信中のメッセージの目的ノード
        private int RemainedMsgLen { get; set; }    // 通信中のメッセージの残り(パケット)

        private Queue<Packet> ImportBuffer { get; }
        private Queue<Packet> ExportBuffer { get; }
        private int ImportBufferSize { get; }
        private int ExportBufferSize { get; }

        // TODO
        // コンストラクタ
        public Node(Network nw, uint addr)
        {
            Addr = addr;
            MsgLen = 0;
            NW = nw;
        }

        // TODO
        // 次のノードのアドレスを返す
        public uint Routing(uint destAddr)
        {
            return 0;
        }

        // TODO
        // クロックごとの動作
        public void OnClock()
        {
            // 通信発生
            if (NW.rand.Next(100) < Frequency)
            {
                // 通信内容の設定
                RemainedMsgLen = MsgLen;
                DestAddr = NW.GetDestRandom(Addr);

                if (ExportBuffer.Count > ExportBufferSize)
                {
                    // TODO
                    // バッファがいっぱいのとき
                }
            }

            // 通信本体
            if (--RemainedMsgLen == 0)
            {
            }
        }
    }
}
