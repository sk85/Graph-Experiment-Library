#include <Score.h>

Score::Score(uint32_t nodeNum, int size)
{
	NodeNum = nodeNum;
	Size = size;
	Array = (int**)new int[nodeNum * size];
}

Score::~Score()
{
	delete[] Array;
}

int Score::Get(uint32_t node, int index)
{
	return Array[node][index];
}

void Score::Set(uint32_t node, int index, int score)
{
	Array[node][index] = score;
}
