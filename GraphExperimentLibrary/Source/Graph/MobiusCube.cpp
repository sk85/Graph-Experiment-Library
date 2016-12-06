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

int MobiusCube::GetUpbitIndex(uint32_t bin, int index)
{
	for ( ; index >= 0; --index)
	{
		if (bin >> index) return index;
	}
	return -1;
}

uint32_t MobiusCube::ExactRoute(uint32_t node1, uint32_t node2)
{
	if (node1 == node2) return 0;
	uint32_t sum = node1 ^ node2;

	// Most left different bit ��T��
	int index = GetUpbitIndex(sum, Dimension - 1);

	int term = (sum >> (index - 1)) & 1;
	int edge = (node1 >> (index + 1)) & 1;

	// ���ݒn�_��E����bad�ł���
	if (term == 1 && term != edge)
	{
		// ����������good�ł���
		int term_ = (sum >> (index - 2)) & 1;
		int edge_ = (node1 >> (index)) & 1;
		
	}
	else
	{
		return GetNeighbor(node1, index);
	}

	/*
	// t_i = E_i ���� t_i��X�ɑ΂���good
	// ���Ȃ킿�AX_(i-1) != Y_(i-1) ���� X_(i+1) = 1
	if (a == 1 && b == 1)
	{
		// ���̕��� good �� term ��T��
		sum ^= (1 << (index + 1)) - 1;
		int index2 = index;
		while ((index2 = GetUpbitIndex(sum, index2 - 1)) > 0)
		{
			if (((sum >> (index2 - 1)) & 1) == ((node1 >> (index2 + 1)) & 1))
			{
				return GetNeighbor(node1, index2);
			}
		}
	}
	// e �̂Ƃ�
	else
	{
		return GetNeighbor(node1, index);
	}*/
}