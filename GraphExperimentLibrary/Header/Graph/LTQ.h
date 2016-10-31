#pragma once

#include "SGraph.h"

class LTQ : public SGraph
{
public:
	// SGraph‚æ‚èŒp³
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
	int GetPreferredNeighbor(uint32_t s, uint32_t d);
	int ttt(uint32_t s, uint32_t d);

private:
	// —×ÚŒˆ’èƒoƒCƒiƒŠ
	class DecisionBinary
	{
	private:
		__readonly int Index;
		__readonly uint32_t Type;
	public:
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
				return (0b10 + Type) << (Index - 1);
		}

		uint32_t GetType() { return Type; }
		uint32_t GetIndex() { return Index; }
	};

	class DBary
	{
	private:
		DecisionBinary Ary[32];
		int Count;

	public:
		DBary()
		{
			Count = 0;
		}

		void Add(DecisionBinary db)
		{
			Ary[Count++] = db;
		}

		void Add(uint32_t type, int index)
		{
			Add(DecisionBinary(type, index));
		}

		void Reset()
		{
			Count = 0;
		}

		DecisionBinary Get(int index)
		{
			return Ary[index];
		}

		int GetCount()
		{
			return Count;
		}

		void Marge(DBary *sub)
		{
			for (int i = 0; i < sub->GetCount(); i++)
				Add(sub->Get(i));
		}
	};

	void GetPreferredNeighborSub(uint32_t *c, int index, DBary *Ary, DBary *subAry);
};