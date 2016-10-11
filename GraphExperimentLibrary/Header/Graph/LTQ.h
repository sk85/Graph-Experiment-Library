#pragma once

#include "SGraph.h"

#define TYPE_0 0b10
#define TYPE_1 0b11

class LTQ : public SGraph
{
public:
	// SGraph‚æ‚èŒp³
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
	void test();

private:
	int GetExpansionsCount(uint32_t s, uint32_t d);
	int GetExpansionSizeSingle(uint32_t s, uint32_t d);
	int GetExpansionSizeDouble(uint32_t s, uint32_t d);

	// —×ÚŒˆ’èƒoƒCƒiƒŠ
	class DecisionBinary
	{
	public:
		__readonly int Index;
		__readonly uint32_t Type;

		DecisionBinary() {};

		DecisionBinary(uint32_t type, int index)
		{
			Index = index;
			Type = type;
		}

		uint32_t GetBinary()
		{
			if (Index < 2)
				return 0b01 << Index;
			else
				return Type << (Index - 1);
		}
	};

	// MinimalExpansion‚Ì—v‘f‚ÌŒó•â‚ğŠi”[‚·‚éƒNƒ‰ƒX
	class MinExCandidates
	{
	private:
		int (*AbsSet)[2];
		int (*RelSet)[4];
		int CountAbs0, CountAbs1, CountRel;
		int Dimension;
	public:
		MinExCandidates(int dimension)
		{
			Dimension = dimension;
			AbsSet = new int[Dimension][2];
			RelSet = new int[Dimension][4];	// type1, index1, type2, index2
			CountAbs0 = CountAbs1 = CountRel = 0;
		}

		~MinExCandidates()
		{
			delete[] AbsSet;
		}

		void AddAbs(uint32_t type, int index)
		{
			AbsSet[CountAbs0++][type] = index;
		}

		void AddRel(uint32_t type1, int index1, uint32_t type2, int index2)
		{
			RelSet[CountRel][0] = type1;
			RelSet[CountRel][1] = index1;
			RelSet[CountRel][2] = type2;
			RelSet[CountRel++][3] = index2;
		}

		int GetMinExCount()
		{
			// TODO
			return 0;
		}
	};
};