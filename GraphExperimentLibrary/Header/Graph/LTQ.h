#pragma once

#include <Score.h>
#include "SGraph.h"

class LTQ : public SGraph
{
public:
	// SGraphÇÊÇËåpè≥
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
	virtual uint32_t GetForwardNeighbor(uint32_t s, uint32_t d);

	Score& CalcCapability1();
	Score& CalcCapability2();
	int GetDiameter();

	int Routing_Takano1603(uint32_t s, uint32_t d);
};