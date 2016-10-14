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

	int GetPreferredNeighbor(uint32_t s, uint32_t d);
	void GetPreferredNeighborSub(uint32_t &c, int index, int &Count, DecisionBinary *Ary, DecisionBinary db1, DecisionBinary db2);
	int GetExpansionSizeSingle(uint32_t s, uint32_t d);
	int GetExpansionSizeDouble(uint32_t s, uint32_t d);
};