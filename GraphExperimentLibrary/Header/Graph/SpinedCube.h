#pragma once

#include "SGraph.h"
#include "SpinedCube_shortestpathrouting\SPR.h"
#include "Node\CubeNode.h"


class SpinedCube : public SGraph
{
private:
	uint32_t CalcNodeNum();

public:
	// SGraphÇ©ÇÁåpè≥
	virtual SNode* GetNeighbor(SNode* node, int index) override;
	int CalcDistance(SNode* s, SNode* d) override;
	virtual int GetDegree(SNode* node) override;
};