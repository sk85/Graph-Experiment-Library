#include <iostream>
#include <fstream>
#include <chrono>

#include <Graph\SpinedCube.h>


using namespace std;


// void SGraph::GenerateFaults(int faultRatio)�̃e�X�g�֐�
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
			printf_s("�̏ᗦ:%d, ���̏ᐔ:%d, ���_�̏ᐔ:%d\n", i, count, faultNum);
		}
	}
}

// void SGraph::GetNodeRandom(SNode* node)�̃e�X�g�֐�
void check_GetNodeRandom(SGraph *g, SNode *n)
{
	for (int i = 0; i < 100; i += 10)
	{
		g->GenerateFaults(i);

		g->GetNodeRandom(n);

		if (g->IsFault(n))
		{
			printf_s("�̏ᗦ:%d, �I���m�[�h:%d\n", i, n->GetIndex());
		}
	}
}

// void SGraph::GetConnectedNode(SNode *node1, SNode *node2)�̃e�X�g�֐�
void check_GetConnectedNode(SGraph *g, SNode *n1, SNode *n2)
{
	for (int i = 0; i < 100; i += 10)
	{
		g->GenerateFaults(i);

		bool b;
		do
		{
			g->GetNodeRandom(n1);
			b = g->GetConnectedNode(n1, n2);

		}while (!b);

		bool b1 = g->IsFault(n2);
		bool b2 = g->IsConnected(n1, n2);
		if (b1 || !b2)
		{
			printf_s("�̏ᗦ:%d, %d, %d", i, n1->GetIndex(), n2->GetIndex());
		}
	}
}

int main(void)
{
	SpinedCube sq;
	sq.SetDimension(10);
	CubeNode cn1, cn2;
	for (size_t i = 0; i < 10; i++)
	{
		check_GetConnectedNode(&sq, &cn1, &cn2);
	}
	
	//Sample::RoutingExperiment(&sq, 10000, "k.csv");

	cout << endl << "end" << endl;
	

	getchar();
}



