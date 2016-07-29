#include <fstream>
#include <iostream>

#include <Graph\SpinedCube.h>
#include <Test.h>
#include <Routing.h>

using namespace std;

namespace Test
{
	void e160720()
	{
		// 実験の条件を設定
		SpinedCube sq;				// 対象のグラフを宣言
		sq.SetDimension(10);		// 次元数をセット
		const int trials = 1000;	// 試行回数
		const int routingNum = 6;	// ルーティングの種類の数
		char routingName[][4] = { "SR", "NR1", "NR2", "ER1", "ER2", "ER3" };

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
				int *param1 = Routing::CreateParameter(&sq);
				int *param2 = Routing::CreateParameter(&sq, param1);
				int *param3 = Routing::CreateParameter(&sq, param2);

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

				// Extra 1
				result = Routing::ExtraRouting(&sq, node1, node2, param1);
				if (result > 0)
				{
					success[3][j]++;
					step[3][j] += result;
				}
				else
				{
					fstep[3][j] += -result;
				}

				// Extra 2
				result = Routing::ExtraRouting(&sq, node1, node2, param2);
				if (result > 0)
				{
					success[4][j]++;
					step[4][j] += result;
				}
				else
				{
					fstep[4][j] += -result;
				}

				// Extra 3
				result = Routing::ExtraRouting(&sq, node1, node2, param3);
				if (result > 0)
				{
					success[5][j]++;
					step[5][j] += result;
				}
				else
				{
					fstep[5][j] += -result;
				}

				// ゴミ掃除
				delete[] param1;
				delete[] param2;
				delete[] param3;
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
	}
}