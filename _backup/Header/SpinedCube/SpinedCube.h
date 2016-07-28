#pragma once

#include "..\Common.h"
#include "..\SGraph.h"
#include "SPR.h"


class SpinedCube : public SGraph {
public:
	int CalcDiameter();

	ulong GetNeighbor(addr s, int index);

	int CalcDistance(uint32_t s, uint32_t d);
};