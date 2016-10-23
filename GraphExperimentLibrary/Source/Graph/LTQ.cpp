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
	printf_s("b\n");
#ifdef TESTa
	{
		SetDimension(14);
		auto start = std::chrono::system_clock::now();      // �v���X�^�[�g������ۑ�
		for (uint32_t s = 0; s < NodeNum; s++)
		{
			for (uint32_t d = 0; d < NodeNum; d++)
			{
				GetPreferredNeighbor(s, d);
			}
		}
		auto end = std::chrono::system_clock::now();       // �v���I��������ۑ�
		auto dur = end - start;        // �v�������Ԃ��v�Z
		auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
		// �v�������Ԃ��~���b�i1/1000�b�j�ɕϊ����ĕ\��
		std::cout << msec << " milli sec \n";
	}
	{
		auto start = std::chrono::system_clock::now();      // �v���X�^�[�g������ۑ�
		for (uint32_t s = 0; s < NodeNum; s++)
		{
			for (uint32_t d = 0; d < NodeNum; d++)
			{
				CalcDistance(s, d);
			}
		}
		auto end = std::chrono::system_clock::now();       // �v���I��������ۑ�
		auto dur = end - start;        // �v�������Ԃ��v�Z
		auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
		// �v�������Ԃ��~���b�i1/1000�b�j�ɕϊ����ĕ\��
		std::cout << msec << " milli sec \n";
	}
#endif

	for (size_t dim = 2; dim < 16; dim++)
	{
		SetDimension(dim);

		printf_s("n = %d�J�n\n", dim);

		for (uint32_t s = 0; s < NodeNum; s++)
		{
			for (uint32_t d = 0; d < NodeNum; d++)
			{
				// �����̌v�Z
				int count = 0;
				int distance = CalcDistance(s, d);
				for (int i = 0; i < GetDegree(s); i++)
				{
					if (CalcDistance(GetNeighbor(s, i), d) < distance) count++;
				}

				// �v�Z
				LTQ::DBary ary = LTQ::ttt(s, d);
				int countTest = 0;
				for (int i = 0; i < ary.GetCount(); i++)
				{
					if (ary.Get(i).GetType() == (s & 1)) countTest++;
				}

				// �\��
				if (countTest != count)
				{
					printf_s("------------------------------------------------------\n");
					printf_s("d(%d, %d) = %d\n", s, d, distance);
					printf_s("%10d = ", d);
					showBinary(d);
					for (int i = 0; i < ary.GetCount(); i++)
					{
						DecisionBinary db = ary.Get(i);
						printf_s("d(%3d,%3d) : ", db.GetType(), db.GetIndex());
						showBinary(ary.Get(i).GetBinary());
					}
					printf_s("���� : %d\nttt  : %d\n", count, countTest);
					getchar();
				}
			}
		}
		printf_s("...ok\n");
	}
}

LTQ::DBary& LTQ::ttt(uint32_t s, uint32_t d)
{
	if (d == 10)
	{
		int a = 0;
	}
	uint32_t c_single = s ^ d;
	uint32_t c_single2 = c_single;
	uint32_t c_double = c_single;
	uint32_t c = c_single;
	DBary ary_single;
	DBary ary_single2;
	DBary ary_double;
	DBary ary;
	DBary subAry;
	uint32_t type_s = s & 1;

	for (int i = this->Dimension - 1; i > 1; --i)
	{
		// ���̑�
		{
			if (c_single >> i)
			{
				DecisionBinary db = DecisionBinary(type_s, i);
				ary_single.Add(db);
				c_single ^= db.GetBinary();
			}
			if (c_single2 >> i)
			{
				DecisionBinary db = DecisionBinary(type_s ^ 1, i);
				ary_single2.Add(db);
				c_single2 ^= db.GetBinary();
			}
			if (c_double >> i)
			{
				DecisionBinary db = DecisionBinary((c_double >> (i - 1)) & 1, i);
				ary_double.Add(db);
				c_double ^= db.GetBinary();
			}
		}

		if (c >> i)
		{
			uint32_t type = (c >> (i - 1)) & 1;
			DecisionBinary db = DecisionBinary(type, i);
			ary.Add(db);
			c ^= db.GetBinary();
			subAry.Reset();
			subAry.Add(DecisionBinary(type ^ 1, i));
			GetPreferredNeighborSub(&c, i - 2, &ary, &subAry);
		}
	}

	// ���̑�
	if (c_single >> 1) ary_single.Add(type_s, 1);
	if (c_single2 >> 1) ary_single2.Add(type_s ^ 1, 1);
	if (c_double >> 1) ary_double.Add(type_s, 1);

	int count_single = ary_single.GetCount() + ((c_single & 1) << 10);	// s��d���ʃ^�C�v�Ȃ�+��
	int count_double = ary_double.GetCount() + ((s ^ d) & 1 ? 1 : 2);	// �^�C�v�̈ړ���ǉ�

	if (count_single < count_double) return ary_single;

	int count = 0;
	int count2 = 0;
	for (int i = 0; i < ary.GetCount(); i++)
	{
		if (ary.Get(i).GetType() == type_s) count++;
		else count2++;
	}

	// ��1bit
	if (c >> 1)
	{
		DecisionBinary db = DecisionBinary(type_s, 1);
		ary.Add(db);
		c ^= db.GetBinary();
	}

	// �����Ȃ�ړ������ꍇ
	int count_single2 = ary_single2.GetCount() + ((s ^ d) & 1 ? 1 : 2);	// �^�C�v�̈ړ���ǉ�

	// ��0bit
	// �����^�C�v�̏ꍇ���Ⴄ�^�C�v��1�ȏ�Ȃ炢���Ȃ�s����OK
	// �Ⴄ�^�C�v�̏ꍇ�������^�C�v��0�܂��͂����Ȃ�s���Ă������Ȃ�OK
	if (
			(!((s ^ d) & 1) && (count2 > 0)) || ((s ^ d) & 1) && (count == 0 || count_double == count_single2)
		)
	{
		DecisionBinary db = DecisionBinary(type_s, 0);
		ary.Add(db);
		c ^= db.GetBinary();
	}

	// �����ꍇ
	if (count_single == count_double)
	{
		for (int i = 0; i < ary_single.GetCount(); i++)
		{
			uint32_t db = ary_single.Get(i).GetBinary();
			bool f = true;
			for (int j = 0; j < ary.GetCount(); j++)
			{
				if (ary.Get(j).GetBinary() == db)
				{
					f = false;
					break;
				}
			}
			if (f)
			{
				ary.Add(ary_single.Get(i));
			}
		}
	}

	return ary;
}

int LTQ::GetPreferredNeighbor(uint32_t s, uint32_t d)
{
	// c0 ��0�^�C�v��DB�̂�
	// c1 ��1�^�C�v��DB�̂�
	// c2 �����^�C�v��DB
	// c3 ���O���p
	uint32_t c2 = s ^ d;
	uint32_t c[2] = { c2 , c2 };
	uint32_t c3 = c2;
	DBary ary[2];
	DBary ary2;
	DBary ary3;
	DBary subAry;
	uint32_t type = s & 1;

	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c[0] >> i)
		{
			DecisionBinary db = DecisionBinary(0, i);
			ary[0].Add(db);
			c[0] ^= db.GetBinary();
		}
		if (c[1] >> i)
		{
			DecisionBinary db = DecisionBinary(1, i);
			ary[1].Add(db);
			c[1] ^= db.GetBinary();
		}
		if (c2 >> i)
		{
			DecisionBinary db = DecisionBinary((c2 >> (i - 1)) & 1, i);
			ary2.Add(db);
			c2 ^= db.GetBinary();
		}
		if (c3 >> i)
		{
			if (c3 >> (i - 1) == 0b10)
			{
				DecisionBinary db = DecisionBinary(0, i);
				ary3.Add(db);
				c3 ^= db.GetBinary();
				subAry.Reset();
				subAry.Add(DecisionBinary(1, i));
				GetPreferredNeighborSub(&c3, i - 2, &ary3, &subAry);
			}
			else
			{
				DecisionBinary db = DecisionBinary(1, i);
				ary3.Add(db);
				c3 ^= db.GetBinary();
				subAry.Reset();
				subAry.Add(DecisionBinary(0, i));
				GetPreferredNeighborSub(&c3, i - 2, &ary3, &subAry);
			}
		}
	}

	// 1�^�C�v(��)
	if (c[type] >> 1)
	{
		ary[type].Add(type, 1);
	}

	// 1�^�C�v(�t)
	ary[type ^ 1].Add(type, 0);
	if ((s ^ d) & 1)
	{
		ary[type ^ 1].Add(type ^ 1, 0);
	}
	if (c[type ^ 1] >> 1)
	{
		ary[type ^ 1].Add(type, 1);
	}

	// 2�^�C�v
	ary2.Add(type, 0);
	if (((s ^ d) & 1) == 0)
	{
		ary2.Add(type ^ 1, 0);
	}
	if (c2 >> 1)
	{
		ary2.Add(type, 1);
	}

	int count_single = ary[type].GetCount() + ((c[type] & 1) << 10);
	int count_double = ary2.GetCount();

	if (count_single < count_double)	// 1�^�C�v�̕����ŒZ
	{

		return count_single;
	}
	else if (count_single > count_double)	// 2�^�C�v�̕����ŒZ
	{
		int count = 0;
		uint32_t type = s & 1;
		for (int i = 0; i < ary3.GetCount(); i++)
		{
			if (ary3.Get(i).GetType() == type) count++;
		}
		if (count == 0) count++;	// ���^�C�v�Ŗ�����΁A�^�C�v��n��}��1��
		if (c3 & 0b10) count++;		// ��1bit
		if (count_double == (ary[type ^ 1].GetCount() + (c[type ^ 1] >> 1) + ((c[type ^ 1] & 1) << 10) + 2 - (c[type ^ 1] & 1)))
		{
			count++;
		}

		return count;
		// �����^�C�v�̂�S��
		// �オ0�Ȃ�^�C�v��ς���Ƃ�
		// ���Ƒ�2bit
	}
	else	// �ǂ�����ŒZ
	{
		// �S���{�^�C�v��ς���Ƃ��{��2bit
	}

}


// 1c��c�͌݊p�Ȃ̂�
// &c, index, ary, count, subAry, sc
//	c�̕��͂��ł�ary�ɓ����Ă�
//	1c�̕���subAry�ɓ����ĂāAary�ɓ��邩�ǂ���
void LTQ::GetPreferredNeighborSub(uint32_t *c, int index, DBary *Ary, DBary *subAry)
{
	if (index < 3)
	{
		if (index == 1)
		{
			if (*c >> 1)	// c = 000001. 
			{
				Ary->Marge(subAry);
				Ary->Add(1, 2);
			}
		}
		else if (index == 2)
		{
			if (*c >> 2)	// c = 000001.. 
			{
				uint32_t type = (*c >> 1) & 1;
				if (type == 0)
				{
					Ary->Marge(subAry);
					Ary->Add(1, index + 1);
				}
				DecisionBinary db = DecisionBinary(type, index);
				*c ^= db.GetBinary();
				Ary->Add(db);
			}
		}
		return;
	}

	uint32_t bits = *c >> (index - 1);
	if (bits == 0b10)	// c = 10...
	{
		DecisionBinary db = DecisionBinary(0, index);
		*c ^= db.GetBinary();
		Ary->Add(db);
		Ary->Add(1, index + 1);
		Ary->Marge(subAry);
		subAry->Reset();
		subAry->Add(1, index);
		GetPreferredNeighborSub(c, index - 2, Ary, subAry);
	}
	else if (bits == 0b11)	// c = 11...
	{
		DecisionBinary db = DecisionBinary(1, index);
		Ary->Add(db);
		*c ^= db.GetBinary();
		subAry->Add(0, index);
		subAry->Add(1, index + 1);
		GetPreferredNeighborSub(c, index - 2, Ary, subAry);
	}
	// c = 0...�Ȃ炻�̂܂�
}