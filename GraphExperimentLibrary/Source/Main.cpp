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
	// ltq.test();

	int dim = 13;
	{
		printf_s("GetPreferredNeighborのチューニング\n");
		printf_s("-----------------------------------------\n");
		Test::e161024(2, 10);
		printf_s("-----------------------------------------\n");
		printf_s("速度比較開始\n");

		ltq.SetDimension(dim);

		auto start = std::chrono::system_clock::now();      // 計測スタート時刻を保存
		int count = 0;

		for (uint32_t s = 0; s < ltq.GetNodeNum(); s++)
		{
			for (uint32_t d = 0; d < ltq.GetNodeNum(); d++)
			{
				count += ltq.ttt(s, d);
			}
		}
		auto end = std::chrono::system_clock::now();       // 計測終了時刻を保存
		auto dur = end - start;        // 要した時間を計算
		auto before = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
		printf_s("旧 : %10dmsec. %d\n", before, count);


		start = std::chrono::system_clock::now();      // 計測スタート時刻を保存
		count = 0;
		for (uint32_t s = 0; s < ltq.GetNodeNum(); s++)
		{
			for (uint32_t d = 0; d < ltq.GetNodeNum(); d++)
			{
				count += ltq.GetPreferredNeighbor(s, d);
			}
		}
		end = std::chrono::system_clock::now();       // 計測終了時刻を保存
		dur = end - start;        // 要した時間を計算
		auto after = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
		printf_s("新 : %10dmsec. %d\n", after, count);

		printf_s("%0.00lf%%", (double)after / before * 100);
	}

	getchar();
	return 0;

	// 明日確認する
	int maxDim = 13;
	{
		printf_s("GetPreferredNeighbor\n");
		for (size_t dim = 2; dim <= maxDim; dim++)
		{
			ltq.SetDimension(dim);

			auto start = std::chrono::system_clock::now();      // 計測スタート時刻を保存
			printf_s("n = %2d開始", dim);

			for (uint32_t s = 0; s < ltq.GetNodeNum(); s++)
			{
				for (uint32_t d = 0; d < ltq.GetNodeNum(); d++)
				{
					int count = ltq.GetPreferredNeighbor(s, d);
				}
			}
			auto end = std::chrono::system_clock::now();       // 計測終了時刻を保存
			auto dur = end - start;        // 要した時間を計算
			auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
			printf_s("...ok. 所要時間 : %10dmsec.\n", msec);
		}
	}
	{
		printf_s("総当り\n");

		for (size_t dim = 2; dim <= maxDim; dim++)
		{
			ltq.SetDimension(dim);

			auto start = std::chrono::system_clock::now();      // 計測スタート時刻を保存
			printf_s("n = %2d開始", dim);

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
			auto end = std::chrono::system_clock::now();       // 計測終了時刻を保存
			auto dur = end - start;        // 要した時間を計算
			auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
			printf_s("...ok. 所要時間 : %10dmsec.\n", msec);
		}
	}

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

