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
	
	LTQ ltq;
	ltq.SetDimension(10);
	ltq.GenerateFaults(10);
	auto a = ltq.CalcCapability1();
	ofstream ofs("a.csv");
	for (size_t i = 0; i < ltq.GetNodeNum(); i++)
	{
		for (size_t j = 0; j < 6; j++)
		{
			ofs << a->Get(i, j) << ",";
		}
		ofs << endl;
	}
	ofs.close();


	test(10, 100, "test.csv");


	return 0;
}

void test(int dim, int trials, char* filename)
{
	LTQ ltq;
	ltq.SetDimension(dim);
	const int RoutingNum = 4;

	char routingName[][30] = { "ŽG1", "ŽG2", "‚–ì‘²˜_(‰¼)", "‚–ì‘²˜_(‰¼2)" };
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
			ltq.GenerateFaults(faultRatio * 10);	// ŒÌá‚ð”­¶‚³‚¹‚é

			uint32_t node1, node2;		// o”­ƒm[ƒh‚Æ–Ú“Iƒm[ƒh
			do
			{
				node1 = ltq.GetNodeRandom();
				node2 = ltq.GetConnectedNodeRandom(node1);
			} while (node2 == node1);	// ˜AŒ‹‚ÈŒó•â‚ªŒ©‚Â‚©‚é‚Ü‚Åƒ‹[ƒv

			// ŽG1
			int id = 0;
			{
				int step = ltq.Routing_Stupid(node1, node2);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
			}
			// ŽG2
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
				// ‚–ì‚­‚ñ‚Ì‘²˜_
				id = 2;
				int step = ltq.Routing_TakanoSotsuron(node1, node2, score);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
				// ‚–ì‚­‚ñ‚Ì‘²˜_‰ü
				id = 3;
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

	ofs << "ŒÌá—¦,";
	for (size_t i = 0; i < 10; i++)
	{
		ofs << i * 10 << "%,";
	}
	ofs << endl;

	for (size_t i = 0; i < RoutingNum; i++)
	{
		ofs << routingName[i] << "(“ž’B—¦),";
		for (size_t j = 0; j < 10; j++)
		{
			ofs << (double)successCount[i][j] / trials << ",";
		}
		ofs << endl;
	}

	for (size_t i = 0; i < RoutingNum; i++)
	{
		ofs << routingName[i] << "(Œo˜H’·),";
		for (size_t j = 0; j < 10; j++)
		{
			ofs << (double)totalSteps[i][j] / trials << ",";
		}
		ofs << endl;
	}
	ofs << endl;

	ofs.close();
}