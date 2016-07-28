#include <iostream>
#include <fstream>
#include <chrono>

#include <Graph\SpinedCube.h>


using namespace std;


// void SGraph::GenerateFaults(int faultRatio)のテスト関数
void check_GenerateFaults(SGraph* g)
{
	for (int i = 0; i < 100; i += 10)
	{
		g->GenerateFaults(i);
		int count = 0;
		for (size_t j = 0; j < g->GetNodeNum(); j++)
		{
			if (g->IsFault(&CubeNode(j)))
			{
				count++;
			}
		}
		int faultNum = (int)(g->GetNodeNum() * (double)i / 100);
		if (faultNum != count)
		{
			printf_s("故障率:%d, 実故障数:%d, 理論故障数:%d\n", i, count, faultNum);
		}
	}
}

// void SGraph::GetNodeRandom(SNode* node)のテスト関数
void check_GetNodeRandom(SGraph *g, SNode *n)
{
	for (int i = 0; i < 100; i += 10)
	{
		g->GenerateFaults(i);

		g->GetNodeRandom(n);

		if (g->IsFault(n))
		{
			printf_s("故障率:%d, 選択ノード:%d\n", i, n->GetIndex());
		}
	}
}

// void SGraph::GetConnectedNodeRandom(SNode *node1, SNode *node2)のテスト関数
void check_GetConnectedNodeRandom(SGraph *g, SNode *n1, SNode *n2)
{
	for (int i = 0; i < 100; i += 10)
	{
		g->GenerateFaults(i);

		bool b;
		do
		{
			g->GetNodeRandom(n1);
			b = g->GetConnectedNodeRandom(n1, n2);

		}while (!b);

		bool b1 = g->IsFault(n2);
		bool b2 = g->IsConnected(n1, n2);
		if (b1 || !b2)
		{
			printf_s("故障率:%d, %d, %d", i, n1->GetIndex(), n2->GetIndex());
		}
	}
}

int main(void)
{
	SpinedCube sq;
	sq.SetDimension(10);
	int count = 1000;
	CubeNode cn1, cn2;
	for (size_t i = 1; i <= count; i++)
	{
		cn2.SetAddr(i);
		printf_s("%5d / %d\r", i, count);
		auto *n = sq.CalcAllDistanceBFS(&cn2);
		delete n;
	}
	
	//Sample::RoutingExperiment(&sq, 10000, "k.csv");

	cout << endl << "end" << endl;
	

	getchar();
}



