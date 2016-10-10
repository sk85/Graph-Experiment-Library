#include <iostream>
#include <fstream>
#include <chrono>

#include "..\Header\Common.h"
#include <Test.h>

#include <Graph\LTQ.h>
#include <Graph\SpinedCube.h>
#include <Graph\MobiusCube.h>


using namespace std;



int main(void)
{
	SpinedCube sq;
	LTQ ltq;
	MobiusCube mq;

	mq.SetType(0);
	Test::e160720(&mq, "0-mq.csv", 500);
	mq.SetType(1);
	Test::e160720(&mq, "1-mq.csv", 500);
	Test::e160720(&ltq, "ltq.csv", 500);
	Test::e160720(&sq, "sq.csv", 500);

	return 0;
	
	
	
	Test::e160731();
	
	getchar();
	return 0;
}

