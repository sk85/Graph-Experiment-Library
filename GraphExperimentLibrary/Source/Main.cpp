#include <iostream>
#include <string>

#include "Common.h"
#include "Experiment\Test.h"
#include "Graph\MobiusCube.h"


using namespace std;

int main(void)
{

	// Test::e161122(10, 100, "new.csv");

	MobiusCube mq;

	mq.SetDimension(10);

	for (uint32_t node2 = 0; node2 < mq.GetNodeNum(); node2++)
	{
		cout << "node2 = " << node2 << endl;
		int* distanceAry = mq.CalcAllDistanceBFS(node2);
		for (uint32_t node1 = 0; node1 < mq.GetNodeNum(); node1++)
		{
			if (node1 == node2) continue;
			uint32_t foward = mq.ExactRoute(node1, node2);

			if (distanceAry[foward] >= distanceAry[node1])
			{
				printf_s("d(%4d, %4d) = %d / ", node1, node2, distanceAry[node1]);
				printf_s("d(%4d, %4d) = %d \n", foward, node2, distanceAry[foward]);
			}
		}
		getchar();
		delete[] distanceAry;
	}

	return 0;
}

