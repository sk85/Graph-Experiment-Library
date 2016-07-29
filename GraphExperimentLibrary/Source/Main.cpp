#include <iostream>
#include <fstream>

#include "..\Header\Common.h"
#include <Graph\SpinedCube.h>
#include <Routing.h>

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
	// 実験の条件を設定
	SpinedCube sq;				// 対象のグラフを宣言
	sq.SetDimension(10);		// 次元数をセット
	const int trials = 1000;	// 試行回数
	const int routingNum = 1;	// ルーティングの種類の数

	// 結果を保持する領域の確保と初期化
	int success[routingNum][10];	// ルーティング成功数
	int step[routingNum][10];		// ステップ数の和
	int fstep[routingNum][10];		// 失敗時ステップ数の和
	for (size_t i = 0; i < routingNum; i++)	// 初期化
	{
		for (size_t j = 0; j < 10; j++)
		{
			success[i][j] = 0;
			step[i][j] = 0;
			fstep[i][j] = 0;
		}
	}

	// 実験本体
	for (uint32_t i = 1; i <= trials; i++)
	{
		for (int j = 0; j < 10; j++)
		{
			sq.GenerateFaults(j * 10);	// 故障を発生させる

			uint32_t node1, node2;		// 出発ノードと目的ノード
			do
			{
				node1 = sq.GetNodeRandom();
				node2 = sq.GetConnectedNodeRandom(node1);
			} while (node2 == node1);	// 連結な候補が見つかるまでループ

			// パラメタを生成
			int *param = Routing::CreateZeroParameter(&sq);

			int result;
			// ルーティング
			result = Routing::ExtraRouting(&sq, node1, node2, param);
			if (result > 0)
			{
				success[0][j]++;
				step[0][j] += result;
			}
			else
			{
				fstep[0][j] += -result;
			}

			// ゴミ掃除
			delete[] param;
		}
		printf_s("%5d / %d\r", i, trials);	// 進捗の表示
	}

	// 結果の出力
	ofstream of("result.csv");
	of << "成功率" << endl;
	for (size_t i = 0; i < routingNum; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)success[i][j] / trials) << ',';
		}
		of << endl;
	}
	of << "平均経路長" << endl;
	for (size_t i = 0; i < routingNum; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)step[i][j] / success[i][j]) << ',';
		}
		of << endl;
	}
	of << "失敗までの平均ステップ数" << endl;
	for (size_t i = 0; i < routingNum; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)fstep[i][j] / (trials - success[i][j])) << ',';
		}
		of << endl;
	}
	of.close();



	std::cout << "end1";
	getchar();
	return 0;

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

	ofstream of2("kekka2.csv");
	for (size_t i = 0; i < 4; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of2 << count[i][j] << ',';
		}
		of2 << endl;
	}
	of2.close();

	cout << endl << "end" << endl;
	
	getchar();
}

