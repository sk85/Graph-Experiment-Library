#include <iostream>
#include <fstream>
#include <chrono>

#include "..\Header\Common.h"
#include <Test.h>

#include <Graph\LTQ.h>
#include <Graph\SpinedCube.h>


using namespace std;



int main(void)
{
	SpinedCube ltq;

	Test::e160720(&ltq, "result2.csv");

	return 0;
	
	
	
	Test::e160731();
	
	getchar();
	return 0;
}

