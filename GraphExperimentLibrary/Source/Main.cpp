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

	char routingName[][30] = { "�G1", "�G2", "���쑲�_(��)", "���쑲�_(��2)", "TM-Mk2" };
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
			ltq.GenerateFaults(faultRatio * 10);	// �̏�𔭐�������

			uint32_t node1, node2;		// �o���m�[�h�ƖړI�m�[�h
			do
			{
				node1 = ltq.GetNodeRandom();
				node2 = ltq.GetConnectedNodeRandom(node1);
			} while (node2 == node1);	// �A���Ȍ�₪������܂Ń��[�v

			// �G1
			int id = 0;
			{
				int step = ltq.Routing_Stupid(node1, node2);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
			}
			// �G2
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
				// ���삭��̑��_
				id = 2;
				int step = ltq.Routing_TakanoSotsuron(node1, node2, score);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
				// ���삭��̑��_��
				id = 3;
				step = ltq.Routing_TakanoSotsuronKai(node1, node2, score);
				if (step > 0)
				{
					successCount[id][faultRatio]++;
					totalSteps[id][faultRatio] += step;
				}
				delete score;
				// ���삭��̑��_����
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

	ofs << "�̏ᗦ,";
	for (size_t i = 0; i < 10; i++)
	{
		ofs << i * 10 << "%,";
	}
	ofs << endl;

	for (size_t i = 0; i < RoutingNum; i++)
	{
		ofs << routingName[i] << "(���B��),";
		for (size_t j = 0; j < 10; j++)
		{
			ofs << (double)successCount[i][j] / trials << ",";
		}
		ofs << endl;
	}

	for (size_t i = 0; i < RoutingNum; i++)
	{
		ofs << routingName[i] << "(�o�H��),";
		for (size_t j = 0; j < 10; j++)
		{
			ofs << (double)totalSteps[i][j] / trials << ",";
		}
		ofs << endl;
	}
	ofs << endl;

	ofs.close();
}