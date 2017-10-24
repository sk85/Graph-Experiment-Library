using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphCS.Old.Core
{
    /// <summary>
    /// Methods for experiment
    /// </summary>
    abstract partial class AGraph
    {
        /// <summary>
        /// Initialize FaultFlags randomly according to faultRatio.
        /// </summary>
        /// <param name="faultRatio">Fault ratio in [0, 1]</param>
        public void GenerateFaults(double faultRatio)
        {
            FaultRatio = faultRatio;

            // Set all flags false
            for (uint i = 0; i < NodeNum; i++) FaultFlags[i] = false;

            if (faultRatio < 0.5)
            {
                // 故障に当たったら乱数を発生し直す
                // 故障率が低いならこちらが有利
                for (uint i = 0; i < FaultNodeNum;)
                {
                    uint rand = (uint)(Rand.NextDouble() * FaultNodeNum);
                    if (!FaultFlags[rand])
                    {
                        FaultFlags[rand] = true;
                        i++;
                    }
                }
            }
            else
            {
                // 常に一定時間で終わる
                for (uint i = 0; i < FaultNodeNum; i++)
                {
                    uint rand = (uint)(Rand.NextDouble() * (NodeNum - i));
                    uint index = 0, count = 0;

                    while (count <= rand)
                    {
                        if (!FaultFlags[index++]) count++;
                    }
                    FaultFlags[index - 1] = true;
                }
            }
        }

        /// <summary>
        /// Returns node1 and node2 are connected or not.
        /// </summary>
        /// <param name="node1">Node</param>
        /// <param name="node2">Node</param>
        /// <returns>Node1 and node2 are connected or not</returns>
        public bool IsConnected(uint node1, uint node2)
        {
            // Depth first search
            var visited = new bool[NodeNum];
            var stack = new Stack<uint>();
            stack.Push(node1);
            visited[node1] = true;
            while (stack.Count > 0)
            {
                uint current = stack.Pop();
                foreach (var neighbor in GetNeighbor(current))
                {
                    if (!visited[neighbor] && !FaultFlags[neighbor])
                    {
                        if (neighbor == node2) return true;
                        visited[neighbor] = true;
                        stack.Push(neighbor);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Retuns unfault node randomly.
        /// </summary>
        /// <returns>Node</returns>
        public uint GetArbitaryNode()
        {
            uint unfaultNum = NodeNum - FaultNodeNum;
            uint rand = (uint)(Rand.NextDouble() * unfaultNum);
            uint index = 0, count = 0;
            while (count <= rand)
            {
                if (!FaultFlags[index++])
                {
                    count++;
                }
            }
            return index - 1;
        }

        /// <summary>
        /// Returns unfault node that is connected with argument node randomly.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Node (If no connected nodes, returns argument node)</returns>
        public uint GetArbitaryConnectedNodes(uint node)
        {
            // If all neighbor nodes are fault, it is failure.
            if (GetNeighbor(node).Any(n => !FaultFlags[n]))
            {
                uint node2;
                do
                {
                    node2 = GetArbitaryNode();
                }
                while (node == node2 || !IsConnected(node, node2));
                return node2;
            }
            else
            {
                return node;
            }
        }

        /// <summary>
        /// Returns ExperimentParametor
        /// </summary>
        /// <returns>ExperimentParametors</returns>
        public void GetExperimentParam(out uint node1, out uint node2)
        {
            do
            {
                node1 = GetArbitaryNode();
                node2 = GetArbitaryConnectedNodes(node1);
            } while (node1 == node2);
        }
        
        /// <summary>
        /// Generates faults then get two connected unfault nodes.
        /// </summary>
        /// <param name="faultRatio"></param>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        public void ExperimentPreparation(double faultRatio, out uint node1, out uint node2)
        {
            GenerateFaults(faultRatio);
            do
            {
                node1 = GetArbitaryNode();
                node2 = GetArbitaryConnectedNodes(node1);
            } while (node1 == node2);
        }

        /// <summary>
        /// 前方隣接頂点が1つしか見つけられないルーティング
        /// </summary>
        /// <param name="node1">出発頂点</param>
        /// <param name="node2">目的頂点</param>
        /// <param name="timeoutLimit">タイムアウト上限</param>
        /// <param name="detour">迂回をするか否か</param>
        /// <returns>ステップ数(袋小路:-1, タイムアウト:-2)</returns>
        public int Routing_ForwardSingle(uint node1, uint node2, int timeoutLimit, bool detour)
        {
            var current = node1;
            var preview = node1;
            var step = 0;

            while (current != node2)
            {
                // タイムアウト判定
                if (step >= timeoutLimit) return -2;

                // ランダムに一個前方を見つける
                uint next;
                {
                    var rel = CalcRelativeDistance(current, node2);
                    var count = rel.Count(x => x == -1);
                    var ii = Rand.Next(count);
                    int i;
                    for (i = 0; ii > 0; i++)
                    {
                        if (rel[i] == -1) ii--;
                    }
                    next = GetNeighbor(current, i);
                }

                // 故障していた場合
                if (FaultFlags[next])
                {
                    // 迂回ありのとき
                    if (detour)
                    {
                        foreach (var x in GetNeighbor(current))
                        {
                            if (!FaultFlags[x] && x != preview)
                            {
                                preview = current;
                                current = x;
                                continue;
                            }
                        }
                        // 非故障かつ後退しない頂点が見つからない = 袋小路
                        return -1;
                    }
                    // 迂回なしのとき
                    else
                    {
                        return -1;
                    }
                }
                // 故障していない場合
                else
                {
                    preview = current;
                    current = next;
                }

                step++;
            }

            return step;
        }

        /// <summary>
        /// ToString method for debug.
        /// </summary>
        /// <returns>Information text</returns>
        public override string ToString()
        {
            string str = string.Format(
                    "{0,5}|{1,16}|{2,5}|\n",
                    "ID",
                    "Addr",
                    "IsFault"
                );
            for (uint i = 0; i < NodeNum; i++)
            {
                str += string.Format(
                    "{0,5}|{1,16}|{2,7}|\n",
                    i,
                    Debug.UintToBinaryString(i, 16, 16),
                    FaultFlags[i]
                );
            }
            return str;
        }
    }
}
