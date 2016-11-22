#include "MobiusCube.h"

void MobiusCube::SetType(int _type)
{
	this->type = _type;
}

uint32_t MobiusCube::GetNeighbor(uint32_t s, int index)
{
	int flag;
	if (index == this->Dimension - 1)
		flag = type;
	else
		flag = s & (1 << (index + 1));

	if (flag)
	{
		return s ^ ((1 << (index + 1)) - 1);	// 111...111
	}
	else
	{
		return s ^ (1 << index);	// 100...000
	}
}

/*
int MobiusCube::CalcDistance(uint32_t s, uint32_t d)
{
	// TODO
	return 0;
}*/

int MobiusCube::GetDegree(uint32_t node)
{
	return this->Dimension;
}

uint32_t MobiusCube::CalcNodeNum()
{
	return 1 << this->Dimension;
}