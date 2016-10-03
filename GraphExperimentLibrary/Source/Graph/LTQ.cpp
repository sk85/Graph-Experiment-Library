#include <Graph\LTQ.h>

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
	int dist1 = GetExpansionSingle(s, d);
	int dist2 = GetExpansionDouble(s, d);
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

int LTQ::GetExpansionSingle(uint32_t s, uint32_t d)
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

int LTQ::GetExpansionDouble(uint32_t s, uint32_t d)
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