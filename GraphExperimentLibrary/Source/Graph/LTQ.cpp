#include <Graph\LTQ.h>
#include "../../Header/Common.h"
#include <chrono>

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
	for (size_t dim = 2; dim < 16; dim++)
	{
		SetDimension(dim);

		auto start = std::chrono::system_clock::now();      // �v���X�^�[�g������ۑ�
		printf_s("n = %d�J�n\n", dim);

		for (uint32_t s = 0; s < GetNodeNum(); s++)
		{
			for (uint32_t d = 0; d < GetNodeNum(); d++)
			{
				int count1 = GetPreferredNeighbor(s, d);
				int count2 = ttt(s, d);

				// �\��
				if (count1 != count2)
				{
					printf_s("(%d, %d)  count1 = %d, count2 = %d", s, d, count1, count2);
					getchar();
				}
			}
		}
		auto end = std::chrono::system_clock::now();       // �v���I��������ۑ�
		auto dur = end - start;        // �v�������Ԃ��v�Z
		auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
		printf_s("...ok. ���v���� : %dmsec.\n", msec);
	}
}

int LTQ::ttt(uint32_t s, uint32_t d)
{

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

	if (count_single < count_double) return ary_single.GetCount();

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

	int countt = 0;
	for (size_t i = 0; i < ary.GetCount(); i++)
	{
		if (ary.Get(i).GetType() == (s & 1)) countt++;
	}

	return countt;
}

int LTQ::GetPreferredNeighbor(uint32_t s, uint32_t d)
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
	DBary ary_single;
	DBary ary;
	DBary subAry;
	uint32_t type_s = s & 1;

	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c_single >> i)
		{
			DecisionBinary db = DecisionBinary(type_s, i);
			ary_single.Add(db);
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

	bool flag = false;
	for (int i = this->Dimension - 1; i > 1; --i)
	{
		if (c >> i)
		{
			uint32_t type = (c >> (i - 1)) & 1;

			c ^= (0b10 + type) << (i - 1);
			// c &= (1 << (i - 1)) - 1;

			if (type == type_s) ary.Add(type, i);
			a ^= ((type ^ type_s) ^ 1) << i;

			if (flag)
			{
				if (type)
				{
					if (type == type_s) subAry.Add(type, i + 1);
					a_sub ^= ((type ^ type_s) ^ 1) << (i + 1);
				}
				else
				{
					ary.Marge(&subAry);
					a |= a_sub;

					if (type != type_s) ary.Add(type ^ 1, i + 1);
					a ^= (type ^ type_s) << (i + 1);

					subAry.Reset();
					a_sub = 0;
					flag = false;
				}
			}

			if (type != type_s) subAry.Add(type ^ 1, i);
			a_sub ^= (type ^ type_s) << i;
			flag = true;

			--i;
		}
		else
		{
			subAry.Reset();
			a_sub = 0;
			flag = false;
		}
	}



	if (c & 0b100)	// c = 0001ab
	{
		if (c & 0b10)	// c = 00011b
		{
			ary.Marge(&subAry);
			a |= a_sub;
		}

		c ^= c & 0b110;
		ary.Add((c >> 1) & 1, 2);
		a ^= ((c >> 1) ^ type_s ^ 1) << 2;
	}
	else
	{
		if (c & 0b10)	// c = 00001b
		{
			c ^= c & 0b10;
			ary.Add(s & 1, 1);
			a ^= 0b10;
			if (flag)
			{
				if (type_s == 1) ary.Add(1, 2);
				a ^= type_s << 2;


				ary.Marge(&subAry);
				a |= a_sub;
			}
		}
	}

	////// NOW
	if (ary.GetCount() != __popcnt(a))
	{
		printf_s("a");
	}

	subAry.Reset();

	// ��1�r�b�g�̏���
	if (c_single >> 1)
	{
		ary_single.Add(type_s, 1);
		a_single ^= 0b10;
	}
	if (c_single2 >> 1) a_single2 ^= 0b10;
	if (c_double >> 1) a_double[s & 1] ^= 0b10;

	// ��0�r�b�g�̏����Ƃ�
	int count_single = __popcnt(a_single) + ((c_single & 1) << 10);	// s��d���ʃ^�C�v�Ȃ�+��
	int count_double = __popcnt(a_double[0]) + __popcnt(a_double[1]) + 2 - ((s ^ d) & 1);	// �^�C�v�̈ړ���ǉ�

	if (count_single < count_double) 
	{
		return count_single;
	}
	else
	{

	}
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
	int count_single2 = __popcnt(a_single2) + 2 - ((s ^ d) & 1);	// �^�C�v�̈ړ���ǉ�

																		// ��0bit
																		// �����^�C�v�̏ꍇ���Ⴄ�^�C�v��1�ȏ�Ȃ炢���Ȃ�s����OK
																		// �Ⴄ�^�C�v�̏ꍇ�������^�C�v��0�܂��͂����Ȃ�s���Ă������Ȃ�OK
	if (
		!((s ^ d) & 1) || ((s ^ d) & 1) && (count == 0 || count_double == count_single2)
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

	int countt = 0;
	for (size_t i = 0; i < ary.GetCount(); i++)
	{
		if (ary.Get(i).GetType() == (s & 1)) countt++;
	}

	/*
	if (s == 1 && d == 14)
	{
		printf_s("-------------------------------------\n");
		for (size_t i = 0; i < ary.GetCount(); i++)
		{
			showBinary(ary.Get(i).GetBinary());
		}
		printf_s("-------------------------------------\n");
	}*/

	return countt;
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