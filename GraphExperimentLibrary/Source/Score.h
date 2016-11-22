#pragma once

#include "Graph\SGraph.h"

class Score
{
private:
	int** Array;
	uint32_t NodeNum;
	int Size;
public:
	Score(uint32_t nodeNum, int size);

	~Score();

	int Get(uint32_t node, int index);
	void Set(uint32_t node, int index, int score);
};