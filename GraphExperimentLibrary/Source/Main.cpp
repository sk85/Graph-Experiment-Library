#include <iostream>
#include <fstream>
#include <chrono>

#include "..\Header\Common.h"
#include <Test.h>

#include <Graph\LTQ.h>
#include <Graph\SpinedCube.h>
#include <Graph\MobiusCube.h>
#include <chrono>


using namespace std;



int main(void)
{
	LTQ ltq;

	for (int dim = 2; dim < 18; dim++)
	{
		ltq.SetDimension(dim);
		int diam0 = 0;
		int diam1 = 0;
		for (size_t i = 2; i < ltq.GetNodeNum(); i++)
		{
			int next;
			if ((next = ltq.CalcDistance(0, i)) > diam0) diam0 = next;
			if ((next = ltq.CalcDistance(1, i)) > diam1) diam1 = next;
		}
		printf_s("dim = %d, diam0 = %d, diam1 = %d\n", dim, diam0, diam1);
	}



	//Test::e161024(2, 14);
	getchar();
	return 0;

	SpinedCube sq;
	// LTQ ltq;
	MobiusCube mq;

	return 0;
}

