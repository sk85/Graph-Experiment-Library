#include "Score.h"

Score::Score(uint32_t nodeNum, int size)
{
	NodeNum = nodeNum;
	Size = size;
	Array = new int*[NodeNum];
	for (size_t i = 0; i < NodeNum; i++)
	{
		Array[i] = new int[Size];
	}
}

Score::~Score()
{
	if (Array != nullptr)
	{
		for (size_t i = 0; i < NodeNum; i++)
		{
			delete[] Array[i];
		}
		delete[] Array;
	}
}

int Score::Get(uint32_t node, int index)
{
	return Array[node][index];
}

void Score::Set(uint32_t node, int index, int score)
{
	Array[node][index] = score;
}
