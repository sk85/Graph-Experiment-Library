#include <iostream>
#include <fstream>
#include <chrono>
#include <vector>

#include "Common.h"
#include "Routing.h"
#include "Test.h"

#include "Graph\LTQ.h"


using namespace std;
void test(int dim, int trials, char* filename);


int main(void)
{

	test(10, 1000, "test10-1000.csv");


	return 0;
}

void test(int dim, int trials, char* filename)
{
	LTQ ltq;
	ltq.SetDimension(dim);
	const int RoutingNum = 5;

	char routingName[][30] = { "雑1", "雑2", "高野卒論(仮)", "高野卒論(仮2)", "TM-Mk2" };
	int successCount[RoutingNum][10];
	int totalSteps[RoutingNum][10];
	for (size_t i = 0; i < RoutingNum; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			successCount[i][j] = 0;
			totalSteps[i][j] = 0;
		}
	}

	for (int i = 1; i <= trials; i++)
	{
		printf_s("\r%5d/%5d", i, trials);

		for (int faultRatio = 0; faultRatio < 10; faultRatio++)
		{
			ltq.GenerateFaults(faultRatio * 10);	// 故障を発生させる

			uint32_t node1, node2;		// 出発ノードと目的ノード
			do
			{
				node1 = ltq.GetNodeRandom();
				node2 = ltq.GetConnectedNodeRandom(node1);
			} while (node2 == node1);	// 連結な候補が見つかるまでループ

			// 雑1
			int id = 0;
			{
				int step = ltq.Routing_Stupid(node1, node2);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
			}
			// 雑2
			id = 1;
			{
				int step = ltq.Routing_Simple(node1, node2);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
			}

			{
				Score *score = ltq.CalcCapability1();
				// 高野くんの卒論
				id = 2;
				int step = ltq.Routing_TakanoSotsuron(node1, node2, score);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
				// 高野くんの卒論改
				id = 3;
				step = ltq.Routing_TakanoSotsuronKai(node1, node2, score);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
				delete score;
				// 高野くんの卒論改二
				score = ltq.CalcCapability2();
				id = 4;
				step = ltq.Routing_TakanoSotsuronKai(node1, node2, score);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
				delete score;
			}
		}
	}

	ofstream ofs(filename);

	ofs << "故障率,";
	for (size_t i = 0; i < 10; i++)
	{
		ofs << i * 10 << "%,";
	}
	ofs << endl;

	for (size_t i = 0; i < RoutingNum; i++)
	{
		ofs << routingName[i] << "(到達率),";
		for (size_t j = 0; j < 10; j++)
		{
			ofs << (double)successCount[i][j] / trials << ",";
		}
		ofs << endl;
	}

	for (size_t i = 0; i < RoutingNum; i++)
	{
		ofs << routingName[i] << "(経路長),";
		for (size_t j = 0; j < 10; j++)
		{
			ofs << (double)totalSteps[i][j] / trials << ",";
		}
		ofs << endl;
	}
	ofs << endl;

	ofs.close();
}