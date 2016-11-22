
#include "Routing.h"


int* Routing::CreateZeroParameter(SGraph *g)
{
	int* parameter = new int[g->GetNodeNum()];
	for (size_t node = 0; node < g->GetNodeNum(); node++)
	{
		parameter[node] = 0;
	}
	return parameter;
}

int* Routing::CreateParameter(SGraph *g)
{
	int* parameter = new int[g->GetNodeNum()];
	for (uint32_t node = 0; node < g->GetNodeNum(); node++)
	{
		parameter[node] = 0;
		for (int index = 0; index < g->GetDegree(node); index++)
		{
			uint32_t neighbor = g->GetNeighbor(node, index);
			if (!g->IsFault(neighbor))
			{
				parameter[node]++;
			}
		}
	}
	return parameter;
}

int* Routing::CreateParameter(SGraph *g, int *param)
{
	int* parameter = new int[g->GetNodeNum()];
	for (uint32_t node = 0; node < g->GetNodeNum(); node++)
	{
		parameter[node] = 0;
		for (int index = 0; index < g->GetDegree(node); index++)
		{
			uint32_t neighbor = g->GetNeighbor(node, index);
			if (!g->IsFault(neighbor))
			{
				parameter[node] += param[neighbor];
			}
		}
	}
	return parameter;
}

int Routing::SimpleRouting(SGraph *g, const uint32_t node1, const uint32_t node2)
{
	uint32_t current = node1;
	int step = 0;

	while (current != node2)
	{
		uint32_t candidate = current;

		// 前方隣接頂点の中で非故障なノードがあればそこへルーティング
		uint32_t forward = g->GetForwardNeighbor(current, node2);
		for (int index = 0; index < g->GetDegree(node1); index++)
		{
			if (forward & (1 << index))
			{
				uint32_t neighbor = g->GetNeighbor(current, index);
				if (!g->IsFault(neighbor))
				{
					candidate = neighbor;
					break;
				}
			}
		}

		// ルーティングができない(＝非故障かつ前方が見つからない)ならば失敗
		if (candidate == current)
		{
			return -step;
		}

		step++;
		current = candidate;
	}
	return step;
}

int Routing::NormalRouting1(SGraph *g, uint32_t node1, uint32_t node2)
{
	bool* visited = new bool[g->GetNodeNum()];
	for (size_t i = 0; i < g->GetNodeNum(); i++)
	{
		visited[i] = false;
	}

	int distance = g->CalcDistance(node1, node2);
	uint32_t current = node1;
	uint32_t preview = node1;
	int step = 0;
	while (current != node2)
	{
		uint32_t mae = current;
		uint32_t yoko = current;
		int maeParam = -1;
		int yokoParam = -1;
		visited[current] = true;

		for (int index = 0; index < g->GetDegree(current); index++)
		{
			uint32_t neighbor = g->GetNeighbor(current, index);

			// 故障or通過済みならスキップ
			if (g->IsFault(neighbor) || visited[neighbor])
			{
				continue;
			}

			int neighborDistance = g->CalcDistance(neighbor, node2);

			// 前方ならそれを選ぶ
			if (neighborDistance < distance)
			{
				mae = neighbor;
				break;
			}
			// 横方向なら記憶しておく
			else if (neighborDistance == distance)
			{
				yoko = neighbor;
			}
		}

		// 前方があればそちらへ
		if (mae != current)
		{
			current = mae;
			distance--;
		}
		else
		{
			// 横方向は見つかればそちらへ
			if (yoko != current)
			{
				current = yoko;
			}
			// どちらもなければ失敗
			else
			{
				delete[] visited;
				return -step;
			}
		}
		step++;
	}

	delete[] visited;
	return step;
}

int Routing::NormalRouting2(SGraph *g, uint32_t node1, uint32_t node2)
{
	bool* visited = new bool[g->GetNodeNum()];
	for (size_t i = 0; i < g->GetNodeNum(); i++) visited[i] = false;

	uint32_t current = node1;
	uint32_t preview = node1;
	int step = 0;
	while (current != node2)
	{
		uint32_t candidate = current;
		visited[current] = true;

		// 前方隣接頂点の中で非故障なノードがあればそこへルーティング
		uint32_t forward = g->GetForwardNeighbor(current, node2);
		for (int index = 0; index < g->GetDegree(node1); index++)
		{
			if (forward & (1 << index))
			{
				uint32_t neighbor = g->GetNeighbor(current, index);
				if (!g->IsFault(neighbor))
				{
					candidate = neighbor;
					break;
				}
			}
		}

		// 前方にルーティングできなかった場合
		if (candidate == current)
		{
			// 横方隣接頂点の中で非故障なノードがあればそこへルーティング
			uint32_t side = g->GetSideNeighbor(current, node2);
			for (int index = 0; index < g->GetDegree(node1); index++)
			{
				if (side & (1 << index))
				{
					uint32_t neighbor = g->GetNeighbor(current, index);
					if (!g->IsFault(neighbor) && neighbor != preview)
					{
						candidate = neighbor;
						break;
					}
				}
			}
		}

		// ルーティングができない(＝非故障かつ前方も横方も見つからない)
		// or 通ったことのある頂点に到達(ループ)ならば失敗
		if (candidate == current || visited[candidate])
		{
			step = -step;
			break;
		}

		step++;
		preview = current;
		current = candidate;
	}

	delete[] visited;
	return step;
}

int Routing::ExtraRouting(SGraph *g, uint32_t node1, uint32_t node2, int* param)
{
	bool* visited = new bool[g->GetNodeNum()];
	for (size_t i = 0; i < g->GetNodeNum(); i++)
	{
		visited[i] = false;
	}

	int distance = g->CalcDistance(node1, node2);
	uint32_t current = node1;
	uint32_t preview = node1;
	int step = 0;
	while (current != node2)
	{
		uint32_t mae = current;
		uint32_t yoko = current;
		int maeParam = -1;
		int yokoParam = -1;
		visited[current] = true;

		for (int index = 0; index < g->GetDegree(current); index++)
		{
			uint32_t neighbor = g->GetNeighbor(current, index);

			// 故障or一つ手前で通っているときはスキップ
			if (g->IsFault(neighbor) || neighbor == preview)
			{
				continue;
			}

			int neighborDistance = g->CalcDistance(neighbor, node2);

			// 前方で、パラメタが大きければmaeを更新
			if (neighborDistance < distance)
			{
				if (param[neighbor] > maeParam)
				{
					mae = neighbor;
					maeParam = param[neighbor];
				}
			}
			// 横方向で、パラメタが大きければyokoを更新
			else if (neighborDistance == distance)
			{
				if (param[neighbor] > yokoParam)
				{
					yoko = neighbor;
					yokoParam = param[neighbor];
				}
			}
		}

		preview = current;

		// 前方があればそちらへ
		if (mae != current)
		{
			if (visited[mae])
			{
				delete[] visited;
				return -step;
			}
			current = mae;
			distance--;
		}
		else
		{
			// 横方向は見つかればそちらへ
			if (yoko != current)
			{
				if (visited[yoko])
				{
					delete[] visited;
					return -step;
				}
				current = yoko;
			}
			// どちらもなければ失敗
			else
			{
				delete[] visited;
				return -step;
			}
		}
		step++;
	}

	delete[] visited;
	return step;
}

