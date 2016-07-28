#pragma once

#include "SGraph.h"

class SpinedCube : public SGraph {
public:
	int CalcDiameter();

	uint32_t GetNeighbor(uint32_t s, int index);

	int CalcDistance(uint32_t s, uint32_t d);
};