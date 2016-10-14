#include <Graph\LTQ.h>
#include "../../Header/Common.h"
#include <chrono>
#define TEST

uint32_t LTQ::GetNeighbor(uint32_t s, int index)
{
	if (index < 2)
	{
		return s ^ (1 << index);
	}
	else
	{
		uint32_t diff = (0b10 + (s & 1)) << (index - 1);
		return s ^ diff;
	}
}

int LTQ::CalcDistance(uint32_t s, uint32_t d)
{
	uint32_t c1 = s ^ d;
	uint32_t c2 = c1;
	int count1 = 0;
	int count2 = 0;
	uint32_t type = 0b10 + (s & 1);

	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c1 >> i)
		{
			c1 ^= type << (i - 1);
			count1++;
		}
		if (c2 >> i)
		{
			c2 ^= (c2 >> (i - 1)) << (i - 1);
			count2++;
		}
	}
	count1 += (c1 >> 1) + ((c1 & 1) << 10);
	count2 += 2 + (c2 >> 1) - (c2 & 1);

	if (count1 < count2) return count1;
	else return count2;
}

int LTQ::GetDegree(uint32_t node)
{
	return this->Dimension;
}

uint32_t LTQ::CalcNodeNum()
{
	return 1 << this->Dimension;
}

int LTQ::GetExpansionSizeSingle(uint32_t s, uint32_t d)
{
	if ((s ^ d) & 1) return 100;

	uint32_t c = s ^ d;
	uint32_t type = s & 1;
	int dist = 0;

	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c & (1 << i))
		{
			c ^= (0b10 + type) << (i - 1);
			dist++;
		}
	}

	if (c == 0b10) dist++;

	return dist;
}

int LTQ::GetExpansionSizeDouble(uint32_t s, uint32_t d)
{
	uint32_t c = s ^ d;
	int dist = 0;

	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c & (1 << i))
		{
			c &= ~(0b11 << (i - 1));
			dist++;
		}
	}

	if ((s ^ d) & 1)
	{
		if (c & 0b10) dist += 2;
		else dist++;
	}
	else
	{
		if (c & 0b10) dist += 3;
		else dist += 2;
	}

	return dist;
}

void LTQ::test()
{
	SetDimension(10);

	printf_s("b\n");
#ifdef TESTa
	{
		SetDimension(14);
		auto start = std::chrono::system_clock::now();      // 計測スタート時刻を保存
		for (uint32_t s = 0; s < NodeNum; s++)
		{
			for (uint32_t d = 0; d < NodeNum; d++)
			{
				GetPreferredNeighbor(s, d);
			}
		}
		auto end = std::chrono::system_clock::now();       // 計測終了時刻を保存
		auto dur = end - start;        // 要した時間を計算
		auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
		// 要した時間をミリ秒（1/1000秒）に変換して表示
		std::cout << msec << " milli sec \n";
	}
	{
		auto start = std::chrono::system_clock::now();      // 計測スタート時刻を保存
		for (uint32_t s = 0; s < NodeNum; s++)
		{
			for (uint32_t d = 0; d < NodeNum; d++)
			{
				CalcDistance(s, d);
			}
		}
		auto end = std::chrono::system_clock::now();       // 計測終了時刻を保存
		auto dur = end - start;        // 要した時間を計算
		auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
		// 要した時間をミリ秒（1/1000秒）に変換して表示
		std::cout << msec << " milli sec \n";
	}
#endif

	for (uint32_t s = 0; s < NodeNum; s++)
	{
		for (uint32_t d = 0; d < NodeNum; d++)
		{
			int distance = CalcDistance(s, d);
			int count = 0;
			uint32_t correctAry[10];
			uint32_t testAry[10];

			printf_s("(%4u, %4u) 距離%d\n", s, d, distance);

			for (int i = 0; i < GetDegree(s); i++)
			{
				uint32_t neighbor = GetNeighbor(s, i);
				if (CalcDistance(neighbor, d) < distance) correctAry[count++] = neighbor;
			}

			int testCount = GetPreferredNeighbor(s, d);
			if (testCount != count)
			{
				printf_s("正解→\n");
				for (size_t i = 0; i < count; i++) showBinary(correctAry[i]);

				getchar();
			}
		}
	}
}

int LTQ::GetPreferredNeighbor(uint32_t s, uint32_t d)
{
	uint32_t c1 = s ^ d;
	uint32_t c2 = c1;
	uint32_t c3 = c1;
	int count1 = 0;
	int count2 = 0;
	int dbCount = 0;
	int ary1[32];
	int ary2[32];
	DecisionBinary dbAry[32];
	uint32_t type = 0b10 + (s & 1);

	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c1 >> i)
		{
			ary1[count1] = type << (i - 1);
			c1 ^= ary1[count1++];
		}
		if (c2 >> i)
		{
			ary2[count2] = (c2 >> (i - 1)) << (i - 1);
			c2 ^= ary2[count2++];
		}
		if (c3 >> i)
		{
			uint32_t tmp = c3 >> (i - 2);
			if (tmp == 0b100)
			{
				dbAry[dbCount] = DecisionBinary(TYPE_0, i);
				c3 ^= dbAry[dbCount++].GetBinary();
			}
			else if (tmp == 0b101)
			{
				dbAry[dbCount] = DecisionBinary(TYPE_0, i);
				c3 ^= dbAry[dbCount++].GetBinary();

				GetPreferredNeighborSub(c3, i + 1, dbCount, dbAry, DecisionBinary(TYPE_1, i), NULL);
			}
			else if (tmp == 0b110)
			{
				dbAry[dbCount] = DecisionBinary(TYPE_1, i);
				c3 ^= dbAry[dbCount++].GetBinary();
			}
		}
	}
	count1 += (c1 >> 1) + ((c1 & 1) << 10);	
	count2 += 2 + (c2 >> 1) - (c2 & 1);

	if (count1 < count2) return count1;	// 1タイプの方が最適ならその数が前方隣接頂点数

}

// 1cとcは互角なのか
void LTQ::GetPreferredNeighborSub(uint32_t &c, int index, int &Count, DecisionBinary *Ary, DecisionBinary db1, DecisionBinary db2)
{
	if (c >> index)	// c = 1...
	{
		if ((c >> (index - 1)) == 0b11)	// c = 11...
		{
			mainAry[count++] = DecisionBinary(TYPE_1, index);
		}
		else	// c = 10...
		{

		}
	}
	else	// c = 0...
	{
		count = 0;
	}
}