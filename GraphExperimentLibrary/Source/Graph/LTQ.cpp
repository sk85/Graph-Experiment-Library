#include "LTQ.h"
#include "..\Common.h"

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

uint32_t LTQ::GetForwardNeighbor(uint32_t s, uint32_t d)
{
	uint32_t c_single = s ^ d;
	uint32_t c_single2 = c_single;
	uint32_t c_double = c_single;
	uint32_t c = c_single;
	uint32_t a_single = 0;
	uint32_t a_single2 = 0;
	uint32_t a_double[2] = {0, 0};
	uint32_t a = 0;
	uint32_t a_sub = 0;
	uint32_t type_s = s & 1;
	bool flag = false;

	// �����^�C�v�݂̂�ʂ�ꍇ�Ȃ�
	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c_single >> i)
		{
			a_single ^= 1 << i;
			c_single ^= (0b10 + type_s) << (i - 1);
		}
		if (c_single2 >> i)
		{
			a_single2 ^= 1 << i;
			c_single2 ^= (0b10 + type_s ^ 1) << (i - 1);
		}
		if (c_double >> i)
		{
			uint32_t t = (c_double >> (i - 1)) & 1;
			a_double[t] ^= 1 << i;
			c_double ^= (0b10 + t) << (i - 1);
		}
	}

	// ��1�r�b�g�̏���
	if (c_single >> 1) a_single ^= 0b10;
	if (c_single2 >> 1) a_single2 ^= 0b10;
	if (c_double >> 1) a_double[type_s] ^= 0b10;

	// ��0�r�b�g�̏����Ƃ�
	int count_single = __popcnt(a_single) + ((c_single & 1) << 10);	// s��d���ʃ^�C�v�Ȃ�+��
	int count_double = __popcnt(a_double[0]) + __popcnt(a_double[1]) + 2 - ((s ^ d) & 1);	// �^�C�v�̈ړ���ǉ�

	// �P��^�C�v�̕����������Ȃ炻��DB
	if (count_single < count_double)
	{
		return a_single;
	}
	else
	{
		// ��4���܂�
		int index = this->Dimension - 1;
		while (index > 1)
		{
			if (c >> index)
			{
				uint32_t type = (c >> (index - 1)) & 1;

				c ^= (0b10 + type) << (index - 1);
				a ^= ((type ^ type_s) ^ 1) << index;

				if (flag)
				{
					if (type)
					{
						a_sub ^= ((type ^ type_s) ^ 1) << (index + 1);
					}
					else
					{
						a |= a_sub;
						a ^= (type ^ type_s) << (index + 1);
						a_sub = 0;
					}
				}
				a_sub ^= (type ^ type_s) << index;
				flag = true;
				index -= 2;
			}
			else
			{
				a_sub = 0;
				flag = false;
				index -= 1;
			}
		}

		// ��3������
		if (c & 0b100)	// c = 0001ab
		{
			if (c & 0b10)	// c = 00011b
			{
				a |= a_sub;
			}

			c ^= c & 0b110;
			a ^= type_s << 2;
		}
		else
		{
			if (c & 0b10)	// c = 00001b
			{
				c ^= c & 0b10;
				a ^= 0b10;
				if (flag)
				{
					a ^= type_s << 2;
					a |= a_sub;
				}
			}
		}

		// ����ŕʃ^�C�v�Ɉړ�����P�[�X
			// ��0bit��	�����^�C�v�̏ꍇ�������Ȃ�s����OK
			//			�Ⴄ�^�C�v�̏ꍇ�������Ȃ�s���Ă������Ȃ�OK
		int count_single2 = __popcnt(a_single2) + 2 - ((s ^ d) & 1);	// �^�C�v�̈ړ���ǉ�
		if (!((s ^ d) & 1) || count_double == count_single2) a ^= 1;

		// �P��^�C�v�o�[�W�����ƕ����^�C�v�o�[�W�����������Ȃ�P��^�C�v�o�[�W�������}�[�W
		if (count_single == count_double) a |= a_single;

		return a;
	}
}

Score* LTQ::CalcCapability1()
{
	int diameter = this->GetDiameter();

	Score *c = new Score(this->NodeNum, diameter + 1);

	// c_0��������
	for (uint32_t node = 0; node < this->GetNodeNum(); node++)
	{
		if (this->IsFault(node))
			c->Set(node, 0, 0);
		else
			c->Set(node, 0, 1);
	}

	// c_1�`��������
	for (int k = 1; k <= diameter; k++)
	{
		for (uint32_t node = 0; node < this->GetNodeNum(); node++)
		{
			int tmp = 0;
			for (int index = 1; index < this->GetDegree(node); index++)
			{
				tmp += c->Get(this->GetNeighbor(node, index), k - 1);
			}
			if (tmp > this->Dimension - 1 - k)
				c->Set(node, k, 1);
			else
				c->Set(node, k, 0);
		}
	}
	return c;
}

Score* LTQ::CalcCapability2()
{
	int diameter = this->GetDiameter();

	Score *c = new Score(this->NodeNum, diameter + 1);

	// c_1��������
	for (uint32_t node = 0; node < this->GetNodeNum(); node++)
	{
		if (this->IsFault(node))
			c->Set(node, 1, 0);
		else
			c->Set(node, 1, 1);
	}

	// c_2�`��������
	for (int k = 2; k <= diameter; k++)
	{
		for (uint32_t node = 0; node < this->GetNodeNum(); node++)
		{
			int tmp = 0;
			for (int index = 0; index < this->GetDegree(node); index++)
			{
				tmp += c->Get(this->GetNeighbor(node, index), k - 1);
			}
			if (tmp > this->Dimension - k)
				c->Set(node, k, 1);
			else
				c->Set(node, k, 0);
		}
	}
	return c;
}

int LTQ::GetDiameter()
{
	if (this->Dimension < 5)
		return (this->Dimension + 2) / 2;
	else
		return (this->Dimension + 4) / 2;
}

uint32_t LTQ::CalcInnerForward(uint32_t node1, uint32_t node2)
{
	uint32_t c1 = node1 ^ node2, c2 = c1;
	int count1 = 0, count2 = 0, countE = 0;
	uint32_t type_node1 = 0b10 + (node1 & 1);
	uint32_t innerFoward1 = 0, innerFoward2= 0;

	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c1 >> i)
		{
			c1 ^= type_node1 << (i - 1);
			innerFoward1 |= (1 << i);
			count1++;
		}
		if (c2 >> i)
		{
			uint32_t type = (c2 >> (i - 1));
			if (type == type_node1) innerFoward2 |= (1 << i);
			c2 ^= type << (i - 1);
			count2++;
		}
	}
	count1 += (c1 >> 1) + ((c1 & 1) << 10);
	count2 += 2 + (c2 >> 1) - (c2 & 1);
	innerFoward1 |= c1;
	innerFoward2 |= c2 & 0b10;

	if (count1 <= count2)
		return innerFoward1;
	else
		return innerFoward2;
}

int LTQ::Routing_Stupid(uint32_t node1, uint32_t node2)
{
	uint32_t current = node1;
	int step = 0;

	while (current != node2)
	{
		uint32_t innerFoward = CalcInnerForward(current, node2);

		// ���ԖړI���_�܂ł̃��[�e�B���O
		while (innerFoward != 0)
		{
			int nextIndex = -1;

			// �����O������ړ������T��
			int i = GetDegree(current);
			while ((i = GetNextBitIndex(innerFoward, i - 1)) >= 0)
			{
				if (!IsFault(GetNeighbor(current, i)))
				{
					nextIndex = i;
					break;
				}
			}

			// �ړ����₪�Ȃ���Ύ��s
			if (nextIndex < 0) return -step;

			// �ړ����₪����΂������
			current = GetNeighbor(current, nextIndex);
			innerFoward ^= 1 << nextIndex;
			step++;
		}

		// �T�u�O���t��n��
		if (current != node2)
		{
			uint32_t neighbor = GetNeighbor(current, 0);

			// �n�����悪�̏�Ȃ玸�s
			if (IsFault(neighbor)) return -step;

			// ��̏�Ȃ�ړ�
			step++;
			current = neighbor;
		}
	}
	return step;
}

int LTQ::Routing_TakanoSotsuron(uint32_t node1, uint32_t node2, Score* score)
{
	uint32_t current = node1;
	int step = 0;

	while (current != node2)
	{
		uint32_t innerFoward = CalcInnerForward(current, node2);
		uint32_t intermediate = current;

		// ���ԖړI���_��ݒ�
		int i = GetDegree(current);
		while ((i = GetNextBitIndex(innerFoward, i - 1)) >= 0)
		{
			intermediate = GetNeighbor(intermediate, i);
		}

		// ���ԖړI���_�܂ł̃��[�e�B���O
		while (current != intermediate)
		{
			int nextIndex1 = -1;
			int nextIndex2 = -1;

			// [�����O������Capable]��[�����O������̏�]��T��
			int i = GetDegree(current);
			while ((i = GetNextBitIndex(innerFoward, i - 1)) >= 0)
			{
				uint32_t neighbor = GetNeighbor(current, i);
				int distance = CalcDistance(neighbor, intermediate);
				if (score->Get(neighbor, distance) == 1)
				{
					nextIndex1 = i;
					break;
				}
				if (!IsFault(neighbor))
				{
					nextIndex2 = i;
				}
			}

			// [�����O������Capable]������΂�����փ��[�e�B���O
			if (nextIndex1 >= 0)
			{
				innerFoward ^= 1 << nextIndex1;
				current = GetNeighbor(current, nextIndex1);
				step++;
			}
			// [�����O������̏�]������΂�����փ��[�e�B���O
			else if (nextIndex2 >= 0)
			{
				innerFoward ^= 1 << nextIndex2;
				current = GetNeighbor(current, nextIndex2);
				step++;
			}
			// �ړ����₪�Ȃ���Ύ��s
			else
			{
				return -step;
			}
		}

		// �T�u�O���t��n��
		if (current != node2)
		{
			uint32_t neighbor = GetNeighbor(current, 0);

			// �n�����悪�̏�Ȃ玸�s
			if (IsFault(neighbor)) return -step;

			// ��̏�Ȃ�ړ�
			step++;
			current = neighbor;
		}
	}
	return step;
}

int LTQ::Routing_TakanoSotsuronKai(uint32_t node1, uint32_t node2, Score* score)
{
	uint32_t current = node1;
	int step = 0;

	while (current != node2)
	{
		uint32_t innerFoward = CalcInnerForward(current, node2);
		uint32_t intermediate = current;

		// ���ԖړI���_��ݒ�
		int i = GetDegree(current);
		while ((i = GetNextBitIndex(innerFoward, i - 1)) >= 0)
		{
			intermediate = GetNeighbor(intermediate, i);
		}

		// ���ԖړI���_�ɓ��B���Ă��Ȃ��ꍇ
		if (current != intermediate)
		{
			int nextIndex1 = -1;

			// [�����O������Capable]��T��
			i = GetDegree(current);
			while ((i = GetNextBitIndex(innerFoward, i - 1)) >= 0)
			{
				uint32_t neighbor = GetNeighbor(current, i);
				int distance = CalcDistance(neighbor, intermediate);
				if (score->Get(neighbor, distance) == 1)
				{
					nextIndex1 = i;
					break;
				}
			}

			// [�����O������Capable]������΂�����փ��[�e�B���O
			if (nextIndex1 >= 0)
			{
				innerFoward ^= 1 << nextIndex1;
				current = GetNeighbor(current, nextIndex1);
				step++;
				continue;
			}

			// [�O������̏�]��T��
			uint32_t foward = GetForwardNeighbor(current, node2);
			i = GetDegree(current);
			while ((i = GetNextBitIndex(foward, i - 1)) >= 0)
			{
				uint32_t neighbor = GetNeighbor(current, i);
				if (!IsFault(neighbor))
				{
					nextIndex1 = i;
					break;
				}
			}

			// [�O������̏�]������΂�����փ��[�e�B���O
			if (nextIndex1 >= 0)
			{
				innerFoward = CalcInnerForward(current, node2);
				current = GetNeighbor(current, nextIndex1);
				step++;
				continue;
			}

			// �ړ����₪�Ȃ���Ύ��s
			return -step;
		}

		// ���ԖړI���_�ɓ��B���Ă���ꍇ
		// ���ԖړI���_ != �ړI���_�Ȃ�T�u�O���t��n��
		if (current != node2)
		{
			uint32_t neighbor = GetNeighbor(current, 0);

			// �n�����悪�̏�Ȃ玸�s
			if (IsFault(neighbor)) return -step;

			// ��̏�Ȃ�ړ�
			step++;
			current = neighbor;
		}
	}
	return step;
}