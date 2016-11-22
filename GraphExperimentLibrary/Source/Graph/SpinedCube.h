#pragma once

#include "SGraph.h"

class SpinedCube : public SGraph {
public:
	// SGraph‚æ‚èŒp³
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
};