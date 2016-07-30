#pragma once

#include <iostream>
#include "SGraph.h"

#define MAX_N 50
#define MAX_R 50

class PancakeGraph : public SGraph
{
private:
	int n, r;
	unsigned a[MAX_R][MAX_N];
	unsigned InitPerm(int nn, int rr);
	unsigned PermToNum(int*vec);
	void NumToPerm(int*vec, unsigned num);

	uint32_t CalcNodeNum() override;

public:
	int GetDegree(uint32_t node) override;

	uint32_t GetNeighbor(uint32_t node, int index) override;
};