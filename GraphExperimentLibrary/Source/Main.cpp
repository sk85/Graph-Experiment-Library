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
	int maxDim = 10;
	// ltq.test();

	{
		printf_s("GetPreferredNeighbor\n");
		for (size_t dim = 2; dim <= maxDim; dim++)
		{
			ltq.SetDimension(dim);

			auto start = std::chrono::system_clock::now();      // �v���X�^�[�g������ۑ�
			printf_s("n = %2d�J�n", dim);

			for (uint32_t s = 0; s < ltq.GetNodeNum(); s++)
			{
				for (uint32_t d = 0; d < ltq.GetNodeNum(); d++)
				{
					int count = ltq.GetPreferredNeighbor(s, d);
				}
			}
			auto end = std::chrono::system_clock::now();       // �v���I��������ۑ�
			auto dur = end - start;        // �v�������Ԃ��v�Z
			auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
			printf_s("...ok. ���v���� : %10dmsec.\n", msec);
		}
	}

	{
		printf_s("������\n");

		for (size_t dim = 2; dim <= maxDim; dim++)
		{
			ltq.SetDimension(dim);

			auto start = std::chrono::system_clock::now();      // �v���X�^�[�g������ۑ�
			printf_s("n = %2d�J�n", dim);

			for (uint32_t s = 0; s < ltq.GetNodeNum(); s++)
			{
				for (uint32_t d = 0; d < ltq.GetNodeNum(); d++)
				{
					int count = 0;
					int distance = ltq.CalcDistance(s, d);
					for (int i = 0; i < ltq.GetDegree(s); i++)
					{
						if (ltq.CalcDistance(ltq.GetNeighbor(s, i), d) < distance) count++;
					}
				}
			}
			auto end = std::chrono::system_clock::now();       // �v���I��������ۑ�
			auto dur = end - start;        // �v�������Ԃ��v�Z
			auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
			printf_s("...ok. ���v���� : %10dmsec.\n", msec);
		}
	}

	getchar();

	//Test::e161024(2, 15);

	return 0;

	SpinedCube sq;
	// LTQ ltq;
	MobiusCube mq;

	mq.SetType(0);
	Test::e160720(&mq, "0-mq.csv", 500);
	mq.SetType(1);
	Test::e160720(&mq, "1-mq.csv", 500);
	Test::e160720(&ltq, "ltq.csv", 500);
	Test::e160720(&sq, "sq.csv", 500);

	return 0;
}

