#include <Graph\LTQ.h>
#include "../../Header/Common.h"

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
	int dist1 = GetExpansionSizeSingle(s, d);
	int dist2 = GetExpansionSizeDouble(s, d);
	if (dist1 < dist2) return dist1;
	else return dist2;
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

	for (size_t s = 0; s < NodeNum; s++)
	{
		for (size_t d = 0; d < NodeNum; d++)
		{
			int distance = CalcDistance(s, d);
			int count = 0;
			for (int i = 0; i < GetDegree(s); i++)
			{
				size_t neighbor = GetNeighbor(s, i);
				if (CalcDistance(neighbor, d) < distance) count++;
			}
			printf_s("(%4u, %4u) ‹——£:%d, ‘O•û:%d, ‘O•û/‹——£:%.2lf",
				s, d, distance, count, (double)count / distance);
			getchar();
		}
	}
}

int LTQ::GetExpansionsCount(uint32_t s, uint32_t d)
{
	uint32_t c = s ^ d;
	MinExCandidates MEC(Dimension);

	for (int i = Dimension - 1; i >= 0; --i)
	{
		if (!(c >> i)) continue;

		switch (c >> (i - 2))
		{
		case 0b100:
			MEC.AddAbs(TYPE_0, i);
			break;
		case 0b101:
			break;
		case 0b110:
			MEC.AddAbs(TYPE_1, i);
			break;
		case 0b111:
			break;
		default:
			break;
		}
	}
	//TODO
	return 0;
}