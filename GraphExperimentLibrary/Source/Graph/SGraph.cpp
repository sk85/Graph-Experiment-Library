#include <Graph\SGraph.h>

void SGraph::SetDimension(int _dim) {
	this->Dimension = _dim;
	this->NodeNum = 1 << this->Dimension;
}

int SGraph::GetDimension() {
	return this->Dimension;
}

void SGraph::SetRandSeed(int seed)
{
	mt.seed(seed);
}

int SGraph::CalcDistance(uint32_t node1, uint32_t node2) {
	int* distanceTable = CalcAllDistanceBFS(node1);
	uint32_t distance = distanceTable[node2];
	delete[] distanceTable;
	return distance;
}

int SGraph::CalcDistance(const bool *faults, uint32_t node1, uint32_t node2) {
	int* distanceTable = CalcAllDistanceBFS(faults, node1);
	uint32_t distance = distanceTable[node2];
	delete[] distanceTable;
	return distance;
}

uint32_t SGraph::GetNodeNum()
{
	return this->NodeNum;
}



int* SGraph::CalcAllDistanceBFS(uint32_t node)
{
	bool *faults = CreateFaults(0);
	int *distance = CalcAllDistanceBFS(faults, node);
	delete[] faults;
	return distance;
}

int* SGraph::CalcAllDistanceBFS(const bool *faults, uint32_t node)
{
	// 距離の表の準備
	int *distance = new int[this->NodeNum];
	for (size_t i = 0; i < this->NodeNum; i++)
	{
		distance[i] = 100000;
	}
	// キューの準備
	std::queue<uint32_t> que;

	// 探索本体
	que.push(node);	// 始点をキューに入れる
	distance[node] = 0;
	while (!que.empty())
	{
		uint32_t current = que.front();	// キューから1つ取り出してcurrentとする
		que.pop();
		for (int i = 0; i < this->Dimension; i++)
		{
			uint32_t neighbor = GetNeighbor(current, i);
			if (faults[neighbor])
			{
				continue;
			}
			else if (distance[neighbor] > distance[current])
			{
				distance[neighbor] = distance[current] + 1;
				que.push(neighbor);
			}
		}
	}

	for (size_t i = 0; i < this->NodeNum; i++)
	{
		if (distance[i] == 100000)
		{
			distance[i] = -1;
		}
	}

	return distance;
}

bool* SGraph::CreateFaults(int faultRatio)
{
	uint32_t faultNum = (uint32_t)(this->NodeNum * ((double)faultRatio / 100));

	// 故障ノード保存用配列の準備
	bool *faults = new bool[this->NodeNum];
	for (uint32_t i = 0; i < this->NodeNum; ++i)
	{
		faults[i] = false;
	}

	// ランダムに故障を作る
	for (uint32_t i = 0; i < faultNum; ++i)
	{
		uint32_t rand = this->mt() % (this->NodeNum - i);

		uint32_t index = 0, count = 0;
		while (count <= rand)
		{
			if (!faults[index++])
			{
				count++;
			}
		}
		faults[index - 1] = true;
	}

	return faults;
}

uint32_t SGraph::GetNodeRandom(bool *faults, uint32_t faultRatio)
{
	uint32_t unfaultNum = (uint32_t)(this->NodeNum * ((double)(100 - faultRatio) / 100));
	uint32_t rand = this->mt() % unfaultNum;
	uint32_t index = 0, count = 0;
	while (count <= rand)
	{
		if (!faults[index++])
		{
			count++;
		}
	}
	return index - 1;
}

uint32_t SGraph::GetConnectedNode(bool *faults, uint32_t node) 
{
	int* length = CalcAllDistanceBFS(faults, node);

	// 連結なノード数を数える
	uint32_t count = 0;
	for (uint32_t i = 0; i < this->NodeNum; ++i)
	{
		if (length[i] > 0) 
		{
			count++;
		}
	}

	uint32_t index = 0;
	if (count > 0)	// 連結なノードが存在しないならば失敗
	{
		// 何番目の連結ノードを選ぶか
		uint32_t rand = this->mt() % count;

		// 選んだノードは何か
		count = 0;
		while (count <= rand)
		{
			if (length[index++] > 0)
			{
				count++;
			}
		}

		delete[] length;
	}
	return index - 1;
}
