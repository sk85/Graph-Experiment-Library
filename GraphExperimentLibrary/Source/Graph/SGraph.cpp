#include <Graph\SGraph.h>

SGraph::SGraph()
{
	this->Faults = nullptr;
}

SGraph::~SGraph()
{
	delete[] Faults;
}

void SGraph::SetDimension(int _dim)
{
	this->Dimension = _dim;
	this->NodeNum = CalcNodeNum();

	if (this->Faults != nullptr)
	{
		delete[] this->Faults;
	}
	this->Faults = new bool[this->NodeNum];

	GenerateFaults(0);
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

bool SGraph::IsFault(SNode *node)
{
	return this->Faults[node->GetIndex()];
}

bool SGraph::IsConnected(SNode* node1, SNode* node2)
{
	int *length = this->CalcAllDistanceBFS(node1);
	bool ret = length[node2->GetIndex()] > 0;
	delete[] length;
	return ret;
}


// ok
int SGraph::CalcDistance(SNode* node1, SNode* node2)
{
	int* distanceTable = CalcAllDistanceBFS(node1);
	uint32_t distance = distanceTable[node2->GetIndex()];
	delete[] distanceTable;
	return distance;
}
// ok
int* SGraph::CalcAllDistanceBFS(SNode* node)
{
	// 距離の表の準備
	int *distance = new int[this->NodeNum];
	for (size_t i = 0; i < this->NodeNum; i++)
	{
		distance[i] = 100000;
	}
	// キューの準備
	std::queue<SNode*> que;

	// 探索本体
	que.push(node);	// 始点をキューに入れる
	distance[node->GetIndex()] = 0;
	while (!que.empty())
	{
		SNode* current = que.front();	// キューから1つ取り出してcurrentとする
		que.pop();
		for (int i = 0; i < this->GetDegree(current); i++)
		{
			SNode* neighbor = GetNeighbor(current, i);
			if (this->Faults[neighbor->GetIndex()])
			{
				delete neighbor;
				continue;
			}
			else if (distance[neighbor->GetIndex()] > distance[current->GetIndex()])
			{
				distance[neighbor->GetIndex()] = distance[current->GetIndex()] + 1;
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
// ok
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
// ok
void SGraph::GetNodeRandom(SNode* node)
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
	node->SetAddr(index - 1);
}
// ok
bool SGraph::GetConnectedNodeRandom(SNode *node1, SNode *node2)
{
	int* length = CalcAllDistanceBFS(node1);

	// 連結なノード数を数える
	uint32_t count = 0;
	for (uint32_t i = 0; i < this->NodeNum; ++i)
	{
		if (length[i] > 0) 
		{
			count++;
		}
	}

	// 連結なノードが存在しないならば失敗
	if (count == 0)
	{
		delete[] length;
		return false;
	}

	// 何番目の連結ノードを選ぶか
	uint32_t rand = this->MT() % count;

	// 選んだノードは何か
	uint32_t index = 0;
	count = 0;
	while (count <= rand)
	{
		if (length[index++] > 0)
		{
			count++;
		}
	}
	node2->SetAddr(index - 1);

	delete[] length;

	return true;
}

bool SGraph::GetConnectedNodesRandom(SNode *node1, SNode *node2)
{
	int count = 0;
	do
	{
		if (count++ > 100) return false;
		this->GetNodeRandom(node1);
	} while (!this->GetConnectedNodeRandom(node1, node2));
	return true;
}
