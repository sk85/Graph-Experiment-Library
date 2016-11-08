#include <Graph\LTQ.h>
#include "../../Header/Common.h"

#include <chrono>
#include <vector>

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

uint32_t LTQ::GetPreferredNeighbor(uint32_t s, uint32_t d)
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

	// 同じタイプのみを通る場合など
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

	// 第1ビットの処理
	if (c_single >> 1) a_single ^= 0b10;
	if (c_single2 >> 1) a_single2 ^= 0b10;
	if (c_double >> 1) a_double[type_s] ^= 0b10;

	// 第0ビットの処理とか
	int count_single = __popcnt(a_single) + ((c_single & 1) << 10);	// sとdが別タイプなら+∞
	int count_double = __popcnt(a_double[0]) + __popcnt(a_double[1]) + 2 - ((s ^ d) & 1);	// タイプの移動を追加

	// 単一タイプの方が小さいならそのDB
	if (count_single < count_double)
	{
		return a_single;
	}
	else
	{
		// 下4桁まで
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

		// 下3桁から
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

		// 初手で別タイプに移動するケース
			// 第0bitが	同じタイプの場合→いきなり行ってOK
			//			違うタイプの場合→いきなり行っても同等ならOK
		int count_single2 = __popcnt(a_single2) + 2 - ((s ^ d) & 1);	// タイプの移動を追加
		if (!((s ^ d) & 1) || count_double == count_single2) a ^= 1;

		// 単一タイプバージョンと複数タイプバージョンが同等なら単一タイプバージョンをマージ
		if (count_single == count_double) a |= a_single;

		return a;
	}
}

void LTQ::CalcCapability()
{
	std::vector<std::vector<int>> capability(this->GetNodeNum());

	for (size_t node = 0; node < this->GetNodeNum(); node++)
	{
		capability[node][0] = this->IsFault(node) ? 0 : 1;
	}

	for (size_t i = 0; i <= this->GetDiameter(); i++)
	{
		for (size_t node = 1; node < this->GetNodeNum(); node++)
		{
			if (this->IsFault(node))
				capability[node][i] = 0;
			else
				capability[node][i] = 1;
			
		}
	}
}

int LTQ::GetDiameter()
{
	if (this->Dimension < 5)
		return (this->Dimension + 2) / 2;
	else
		return (this->Dimension + 4) / 2;
}