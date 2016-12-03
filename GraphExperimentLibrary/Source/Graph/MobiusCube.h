#pragma once

#include "SGraph.h"

class MobiusCube : public SGraph {
private:
	// 0-mobiusか1-mobiusかを表すタイプ
	int type;

	class Expansion
	{
	private:
		uint32_t Bin;
		uint32_t Term[2];
		int Dimension;

		void InitializeGreedy()
		{
			Term[0] = 0;
			Term[1] = 0;
			uint32_t bin = Bin;

			for (int i = Dimension - 1; i >= 1; i--)
			{
				if ((bin >> i) & 1)
				{
					if ((bin >> (i - 1)) & 1)
					{
						Set(0, i);
						bin ^= 1 << i;
					}
					else
					{
						Set(1, i);
						bin ^= (1 << (i + 1)) - 1;
					}
				}
			}
		}

	public:
		// Greedyに初期化
		Expansion(uint32_t bin, int dim)
		{
			Bin = bin;
			Dimension = dim;
			InitializeGreedy();
		}

		void Set(uint32_t type, int index)
		{
			Term[type] |= 1 << index;
		}

		bool IsExist(uint32_t type, int index)
		{
			return ((Term[type] >> index) & 1) == 1;
		}
	};

	int GetUpbitIndex(uint32_t bin, int index);

public:
	// タイプをセット
	void SetType(int _type);



	// SGraphより継承
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;

	//virtual int CalcDistance(uint32_t s, uint32_t d) override;

	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
	uint32_t MobiusCube::ExactRoute(uint32_t node1, uint32_t node2);
};