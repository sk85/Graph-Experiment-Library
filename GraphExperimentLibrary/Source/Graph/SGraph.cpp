#include <Graph\SGraph.h>

SGraph::~SGraph()
{
	delete[] Faults;
}

void SGraph::SetDimension(int _dim)
{
	// メンバの初期化
	this->Dimension = _dim;
	this->NodeNum = this->CalcNodeNum();

	// Faults
	if (this->Faults != nullptr) {
		delete[] this->Faults;
	}
	this->Faults = new bool[this->NodeNum];
	this->GenerateFaults(0);
}

int SGraph::GetDimension()
{
	return this->Dimension;
}

uint32_t SGraph::GetNodeNum()
{
	return this->NodeNum;
}

void SGraph::SetRandSeed(int seed)
{
	MT.seed(seed);
}

bool SGraph::IsFault(uint32_t node)
{
	return this->Faults[node];
}

bool SGraph::IsConnected(uint32_t node1, uint32_t node2)
{
	int *length = this->CalcAllDistanceBFS(node1);
	bool ret = length[node2] > 0;
	delete[] length;
	return ret;
}



int SGraph::CalcDistance(uint32_t node1, uint32_t node2) {
	int* distanceTable = CalcAllDistanceBFS(node1);
	uint32_t distance = distanceTable[node2];
	delete[] distanceTable;
	return distance;
}

int SGraph::CalcDistanceF(uint32_t node1, uint32_t node2) {
	int* distanceTable = CalcAllDistanceBFSF(node1);
	uint32_t distance = distanceTable[node2];
	delete[] distanceTable;
	return distance;
}

int* SGraph::CalcAllDistanceBFS(uint32_t node)
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
		for (int i = 0; i < this->GetDegree(current); i++)
		{
			uint32_t neighbor = GetNeighbor(current, i);
			if (distance[neighbor] > distance[current])
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

int* SGraph::CalcAllDistanceBFSF(uint32_t node)
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
		for (int i = 0; i < this->GetDegree(current); i++)
		{
			uint32_t neighbor = GetNeighbor(current, i);
			if (this->Faults[neighbor])
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



void SGraph::GenerateFaults(int faultRatio)
{
	this->FaultRatio = faultRatio;
	this->FaultNum = (uint32_t)(this->NodeNum * ((double)faultRatio / 100));

	// 初期化
	for (uint32_t i = 0; i < this->NodeNum; ++i)
	{
		this->Faults[i] = false;
	}

	// ランダムに故障を作る
	for (uint32_t i = 0; i < this->FaultNum; ++i)
	{
		uint32_t rand = this->MT() % (this->NodeNum - i);

		uint32_t index = 0, count = 0;
		while (count <= rand)
		{
			if (!this->Faults[index++])
			{
				count++;
			}
		}
		this->Faults[index - 1] = true;
	}
}

uint32_t SGraph::GetNodeRandom()
{
	uint32_t unfaultNum = this->NodeNum - this->FaultNum;
	uint32_t rand = this->MT() % unfaultNum;
	uint32_t index = 0, count = 0;
	while (count <= rand)
	{
		if (!this->Faults[index++])
		{
			count++;
		}
	}
	return index - 1;
}

uint32_t SGraph::GetConnectedNodeRandom(uint32_t node) 
{
	int* length = CalcAllDistanceBFSF(node);

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
		uint32_t rand = this->MT() % count;

		// 選んだノードは何か
		count = 0;
		while (count <= rand)
		{
			if (length[index++] > 0)
			{
				count++;
			}
		}
	}

	delete[] length;

	if (index == 0)
	{
		return node;
	}
	else
	{
		return index - 1;
	}
}
