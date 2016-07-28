#include <Routing.h>


namespace Routing
{
	int SR(SGraph *g, SNode *node1, SNode *node2)
	{
		int step = 0;
		int distance = g->CalcDistance(node1, node2);

		SNode *current = node1, *next = node1;
		uint32_t goalIndex = node2->GetIndex();
		while (next->GetIndex() != goalIndex)
		{
			int degree = g->GetDegree(current);
			for (int index = 0; index < degree; index++)
			{
				SNode *neighbor = g->GetNeighbor(current, index);
				if (!(g->IsFault(neighbor)))
				{
					int neighborDistance = g->CalcDistance(neighbor, node2);
					if (neighborDistance < distance)
					{
						next = neighbor;
						distance = neighborDistance;
						break;
					}
				}
			}

			step++;

			// "非故障かつ前方"が見つからなかった場合失敗
			if (next == current)
			{
				return -step;
			}

			current = next;
		}
		return step;
	}
}
/*
namespace Routing
{
	int SR(Experiment *exp, SGraph *g)
	{
		uint32_t current = exp->node1;
		int step = 0;
		int distance = g->CalcDistance(exp->node1, exp->node2);

		while (distance > 0)
		{
			uint32_t next = current;
			for (int index = 0; index < g->GetDimension(); index++)
			{
				uint32_t neighbor = g->GetNeighbor(current, index);
				if (!(exp->Faults[neighbor]))
				{
					int neighborDistance = g->CalcDistance(neighbor, exp->node2);
					if (neighborDistance < distance)
					{
						next = neighbor;
						distance = neighborDistance;
						break;
					}
				}
			}

			step++;

			// "非故障かつ前方"が見つからなかった場合失敗
			if (next == current)
			{
				return -step;
			}

			current = next;
		}
		return step;
	}

	int NR1(Experiment *exp, SGraph *g)
	{
		uint32_t current = exp->node1;
		int distance = g->CalcDistance(exp->node1, exp->node2);
		int step = 0;
		bool* visited;
		{
			uint32_t nodeNum = g->GetNodeNum();
			visited = new bool[nodeNum];
			for (size_t i = 0; i < nodeNum; i++)
			{
				visited[i] = false;
			}
		}

		while (current != exp->node2)
		{
			visited[current] = true;
			uint32_t next = current;
			uint32_t yoko = current;

			for (int index = 0; index < g->GetDimension(); index++)
			{
				uint32_t neighbor = g->GetNeighbor(current, index);

				// 故障or通過済みならスキップ
				if (exp->Faults[neighbor] || visited[neighbor])
				{
					continue;
				}

				int neighborDistance = g->CalcDistance(neighbor, exp->node2);

				// 前方ならそれを選ぶ
				if (neighborDistance < distance)
				{
					next = neighbor;
					distance = neighborDistance;
					break;
				}
				// 横方向なら記憶しておく
				else if (neighborDistance == distance)
				{
					yoko = neighbor;
				}
			}

			// 1ステップ追加
			step++;

			// 前方があればそちらへ
			if (next != current)
			{
				current = next;
			}
			else
			{
				// 前方も横も見つからなければ失敗
				if (yoko == current)
				{
					delete[] visited;
					return -step;
				}
				// 横方向は見つかればそちらへ
				else
				{
					current = yoko;
				}
			}
		}
		delete[] visited;
		return step;
	}

	int NR2(Experiment *exp, SGraph *g)
	{
		uint32_t current = exp->node1;
		uint32_t prev = current;
		int distance = g->CalcDistance(exp->node1, exp->node2);
		int step = 0;
		bool* visited;
		{
			uint32_t nodeNum = g->GetNodeNum();
			visited = new bool[nodeNum];
			for (size_t i = 0; i < nodeNum; i++)
			{
				visited[i] = false;
			}
		}

		while (current != exp->node2)
		{
			visited[current] = true;
			uint32_t next = current;
			uint32_t yoko = current;

			for (int index = 0; index < g->GetDimension(); index++)
			{
				uint32_t neighbor = g->GetNeighbor(current, index);

				// 故障or一個前に通ったならスキップ
				if (exp->Faults[neighbor] || neighbor == prev)
				{
					continue;
				}

				int neighborDistance = g->CalcDistance(neighbor, exp->node2);

				// 前方ならそれを選ぶ
				if (neighborDistance < distance)
				{
					next = neighbor;
					distance = neighborDistance;
					break;
				}
				// 横方向なら記憶しておく
				else if (neighborDistance == distance)
				{
					yoko = neighbor;
				}
			}

			// prev更新
			prev = current;
			// 1ステップ追加
			step++;

			// 前方があればそちらへ
			if (next != current)
			{
				current = next;
			}
			else
			{
				// 前方も横も見つからないor横の候補が通過済みなら失敗
				if (visited[yoko] || yoko == current)
				{
					delete[] visited;
					return -step;
				}
				// 横方向は見つかればそちらへ
				else
				{
					current = yoko;
				}
			}
		}
		delete[] visited;
		return step;
	}

	int ER(Experiment *exp, int* param, SGraph *g)
	{
		uint32_t current = exp->node1;
		uint32_t prev = current;
		int distance = g->CalcDistance(exp->node1, exp->node2);
		int step = 0;
		bool* visited;
		{
			uint32_t nodeNum = g->GetNodeNum();
			visited = new bool[nodeNum];
			for (size_t i = 0; i < nodeNum; i++)
			{
				visited[i] = false;
			}
		}

		while (distance > 0)
		{
			visited[current] = true;
			uint32_t mae = current;
			uint32_t yoko = current;
			int maeParam = 0;
			int yokoParam = 0;

			for (int index = 0; index < g->GetDimension(); index++)
			{
				uint32_t neighbor = g->GetNeighbor(current, index);

				// 故障or通過済みならスキップ
				if (exp->Faults[neighbor] || neighbor == prev)
				{
					continue;
				}

				int neighborDistance = g->CalcDistance(neighbor, exp->node2);

				// 前方で、パラメタが大きければ更新
				if (neighborDistance < distance)
				{
					if (param[neighbor] > maeParam)
					{
						mae = neighbor;
						maeParam = param[neighbor];
					}
				}
				// 横方向で、パラメタが大きければ更新
				else if (neighborDistance == distance)
				{
					if (param[neighbor] > yokoParam)
					{
						yoko = neighbor;
						yokoParam = param[neighbor];
					}
				}
			}

			// prev更新
			prev = current;
			// 1ステップ追加
			step++;

			// 前方があればそちらへ
			if (mae != current)
			{
				current = mae;
				distance--;
			}
			else
			{
				// 前方も横も見つからないor横の候補が通過済みなら失敗
				if (visited[yoko] || yoko == current)
				{
					delete[] visited;
					return -step;
				}
				// 横方向は見つかればそちらへ
				else
				{
					current = yoko;
				}
			}
		}
		delete[] visited;
		return step;
	}
}

*/