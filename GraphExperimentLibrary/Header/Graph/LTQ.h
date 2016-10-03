#pragma once

#include "SGraph.h"

class LTQ : public SGraph
{
public:
	// SGraphÇÊÇËåpè≥
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;

//private:
	int GetExpansionSingle(uint32_t s, uint32_t d);
	int GetExpansionDouble(uint32_t s, uint32_t d);
};