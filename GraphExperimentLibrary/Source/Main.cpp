#include <iostream>
#include <fstream>

#include "..\Header\Common.h"
#include "..\Header\SpinedCube\SpinedCube.h"
#include "..\Header\Sample.h"

#include <chrono>

using namespace std;


int* GetParameter1(bool *faults, SGraph *g)
{
	uint32_t nodeNum = g->GetNodeNum();
	int* parameter = new int[nodeNum];
	for (size_t node = 0; node < nodeNum; node++)
	{
		parameter[node] = 0;
		for (int index = 0; index < g->GetDimension(); index++)
		{
			uint32_t neighbor = g->GetNeighbor(node, index);
			if (!faults[neighbor])
			{
				parameter[node]++;
			}
		}
	}
	return parameter;
}

int* GetParameter2(bool *faults, int* param, SGraph *g)
{
	uint32_t nodeNum = g->GetNodeNum();
	int* parameter = new int[nodeNum];
	for (size_t node = 0; node < nodeNum; node++)
	{
		parameter[node] = 0;
		for (int index = 0; index < g->GetDimension(); index++)
		{
			uint32_t neighbor = g->GetNeighbor(node, index);
			if (!faults[neighbor])
			{
				parameter[node] += param[node];
			}
		}
	}
	return parameter;
}

// 最短経路しか認めないルーティング
// "非故障かつ前方"な頂点が見つからなかったら失敗
bool Routing1(Experiment *exp, SGraph *g)
{
	uint32_t current = exp->node1;
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

		// "非故障かつ前方"が見つからなかった場合失敗
		if (next == current)
		{
			return false;
		}

		current = next;
	}
	return true;
}

// 距離が変わらない経路も認めるルーティング
// １．前方があればそれを選ぶ
// ２．横方向があればそれを選ぶ
// ３. 無ければ失敗
bool Routing2(Experiment *exp, SGraph *g)
{
	uint32_t current = exp->node1;
	int distance = g->CalcDistance(exp->node1, exp->node2);
	uint32_t nodeNum = g->GetNodeNum();
	bool* visited = new bool[nodeNum];
	for (size_t i = 0; i < nodeNum; i++)
	{
		visited[i] = false;
	}

	while (distance > 0)
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
				return false;
			}
			// 横方向は見つかればそちらへ
			else
			{
				current = yoko;
			}
		}
	}
	delete[] visited;
	return true;
}

// パラメタを意識したルーティング
// １．前方があれば、その中からパラメタが最大のものを選ぶ
// ２．横方向があれば、その中からパラメタが最大のものを選ぶ
// ３. 無ければ失敗
bool Routing3(Experiment *exp, int* param, SGraph *g)
{
	uint32_t current = exp->node1;
	int distance = g->CalcDistance(exp->node1, exp->node2);
	uint32_t nodeNum = g->GetNodeNum();
	bool* visited = new bool[nodeNum];
	for (size_t i = 0; i < nodeNum; i++)
	{
		visited[i] = false;
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
			if (exp->Faults[neighbor] || visited[neighbor])
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


		// 前方があればそちらへ
		if (mae != current)
		{
			current = mae;
			distance--;
		}
		else
		{
			// 前方も横も見つからなければ失敗
			if (yoko == current)
			{
				delete[] visited;
				return false;
			}
			// 横方向は見つかればそちらへ
			else
			{
				current = yoko;
			}
		}
	}
	delete[] visited;
	return true;
}


int main(void)
{
	SpinedCube sq;
	sq.SetDimension(10);

	int count[4][10];
	for (size_t i = 0; i < 10; i++)
	{
		count[0][i] = 0;
		count[1][i] = 0;
		count[2][i] = 0;
		count[3][i] = 0;
	}

	for (size_t i = 1; i <= 1000; i++)
	{
		printf_s("%5d / 1000\r", i);
		for (int j = 0; j < 10; j++)
		{
			Experiment exp(&sq, j * 10);
			if (Routing1(&exp, &sq))
			{
				count[0][j]++;
			}
			if (Routing2(&exp, &sq))
			{
				count[1][j]++;
			}
			int* param1 = GetParameter1(exp.Faults, &sq);
			if (Routing3(&exp, param1, &sq))
			{
				count[2][j]++;
			}
			int* param2 = GetParameter2(exp.Faults, param1, &sq);
			if (Routing3(&exp, param2, &sq))
			{
				count[3][j]++;
			}
			delete[] param1;
			delete[] param2;
		}
	}

	ofstream of("kekka2.csv");
	for (size_t i = 0; i < 4; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of << count[i][j] << ',';
		}
		of << endl;
	}
	of.close();

	cout << endl << "end" << endl;
	
	getchar();
}

