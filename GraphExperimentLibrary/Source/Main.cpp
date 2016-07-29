#include <iostream>
#include <fstream>

#include "..\Header\Common.h"
#include <Graph\SpinedCube.h>
#include <Routing.h>

#include <chrono>

using namespace std;

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
	const int routingNum = 4;	// ルーティングの種類の数
	char routingName[][4] = {"SR", "NR1", "NR2", "ER"};

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
			// Simple
			result = Routing::SimpleRouting(&sq, node1, node2);
			if (result > 0)
			{
				success[0][j]++;
				step[0][j] += result;
			}
			else
			{
				fstep[0][j] += -result;
			}

			// Normal 1
			result = Routing::NormalRouting1(&sq, node1, node2);
			if (result > 0)
			{
				success[1][j]++;
				step[1][j] += result;
			}
			else
			{
				fstep[1][j] += -result;
			}

			// Normal 2
			result = Routing::NormalRouting2(&sq, node1, node2);
			if (result > 0)
			{
				success[2][j]++;
				step[2][j] += result;
			}
			else
			{
				fstep[2][j] += -result;
			}

			// Extra
			result = Routing::ExtraRouting(&sq, node1, node2, param);
			if (result > 0)
			{
				success[3][j]++;
				step[3][j] += result;
			}
			else
			{
				fstep[3][j] += -result;
			}

			// ゴミ掃除
			delete[] param;
		}
		printf_s("%5d / %d\r", i, trials);	// 進捗の表示
	}
	std::cout << endl;

	// 結果の出力
	ofstream of("result.csv");
	of << "成功率,0%,10%,20%,30%,40%,50%,60%,70%,80%,90%,\n";
	for (size_t i = 0; i < routingNum; i++)
	{
		of << routingName[i] << ',';
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)success[i][j] / trials) << ',';
		}
		of << endl;
	}
	of << "\n平均経路長,0%,10%,20%,30%,40%,50%,60%,70%,80%,90%,\n";
	for (size_t i = 0; i < routingNum; i++)
	{
		of << routingName[i] << ',';
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)step[i][j] / success[i][j]) << ',';
		}
		of << endl;
	}
	of << "\n失敗までの平均ステップ数,0%,10%,20%,30%,40%,50%,60%,70%,80%,90%,\n";
	for (size_t i = 0; i < routingNum; i++)
	{
		of << routingName[i] << ',';
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)fstep[i][j] / (trials - success[i][j])) << ',';
		}
		of << endl;
	}
	of.close();

	std::cout << "FInish.\nPress enter.";
	getchar();
	return 0;
}

