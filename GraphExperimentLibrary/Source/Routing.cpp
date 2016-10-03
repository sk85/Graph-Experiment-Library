
#include <Routing.h>


int* Routing::CreateZeroParameter(SGraph *g)
{
	int* parameter = new int[g->GetNodeNum()];
	for (size_t node = 0; node < g->GetNodeNum(); node++)
	{
		parameter[node] = 0;
	}
	return parameter;
}

int* Routing::CreateParameter(SGraph *g)
{
	int* parameter = new int[g->GetNodeNum()];
	for (uint32_t node = 0; node < g->GetNodeNum(); node++)
	{
		parameter[node] = 0;
		for (int index = 0; index < g->GetDegree(node); index++)
		{
			uint32_t neighbor = g->GetNeighbor(node, index);
			if (!g->IsFault(neighbor))
			{
				parameter[node]++;
			}
		}
	}
	return parameter;
}

int* Routing::CreateParameter(SGraph *g, int *param)
{
	int* parameter = new int[g->GetNodeNum()];
	for (uint32_t node = 0; node < g->GetNodeNum(); node++)
	{
		parameter[node] = 0;
		for (int index = 0; index < g->GetDegree(node); index++)
		{
			uint32_t neighbor = g->GetNeighbor(node, index);
			if (!g->IsFault(neighbor))
			{
				parameter[node] += param[neighbor];
			}
		}
	}
	return parameter;
}

int Routing::SimpleRouting(SGraph *g, const uint32_t node1, const uint32_t node2)
{
	int distance = g->CalcDistance(node1, node2);
	uint32_t current = node1;
	int step = 0;

	while (current != node2)
	{
		uint32_t mae = current;
		for (int index = 0; index < g->GetDegree(node1); index++)
		{
			uint32_t neighbor = g->GetNeighbor(current, index);
			if (!g->IsFault(neighbor))
			{
				int neighborDistance = g->CalcDistance(neighbor, node2);
				if (neighborDistance < distance)
				{
					mae = neighbor;
					distance = neighborDistance;
					break;
				}
			}
		}

		// "��̏Ⴉ�O��"��������Ȃ������ꍇ���s
		if (mae == current)
		{
			return -step;
		}

		step++;
		current = mae;
	}
	return step;
}

int Routing::NormalRouting1(SGraph *g, uint32_t node1, uint32_t node2)
{
	bool* visited = new bool[g->GetNodeNum()];
	for (size_t i = 0; i < g->GetNodeNum(); i++)
	{
		visited[i] = false;
	}

	int distance = g->CalcDistance(node1, node2);
	uint32_t current = node1;
	uint32_t preview = node1;
	int step = 0;
	while (current != node2)
	{
		uint32_t mae = current;
		uint32_t yoko = current;
		int maeParam = -1;
		int yokoParam = -1;
		visited[current] = true;

		for (int index = 0; index < g->GetDegree(current); index++)
		{
			uint32_t neighbor = g->GetNeighbor(current, index);

			// �̏�or�ʉߍς݂Ȃ�X�L�b�v
			if (g->IsFault(neighbor) || visited[neighbor])
			{
				continue;
			}

			int neighborDistance = g->CalcDistance(neighbor, node2);

			// �O���Ȃ炻���I��
			if (neighborDistance < distance)
			{
				mae = neighbor;
				break;
			}
			// �������Ȃ�L�����Ă���
			else if (neighborDistance == distance)
			{
				yoko = neighbor;
			}
		}

		// �O��������΂������
		if (mae != current)
		{
			current = mae;
			distance--;
		}
		else
		{
			// �������͌�����΂������
			if (yoko != current)
			{
				current = yoko;
			}
			// �ǂ�����Ȃ���Ύ��s
			else
			{
				delete[] visited;
				return -step;
			}
		}
		step++;
	}

	delete[] visited;
	return step;
}

int Routing::NormalRouting2(SGraph *g, uint32_t node1, uint32_t node2)
{
	bool* visited = new bool[g->GetNodeNum()];
	for (size_t i = 0; i < g->GetNodeNum(); i++)
	{
		visited[i] = false;
	}

	int distance = g->CalcDistance(node1, node2);
	uint32_t current = node1;
	uint32_t preview = node1;
	int step = 0;
	while (current != node2)
	{
		uint32_t mae = current;
		uint32_t yoko = current;
		visited[current] = true;

		for (int index = 0; index < g->GetDegree(current); index++)
		{
			uint32_t neighbor = g->GetNeighbor(current, index);

			// �̏�or���O�Œʂ��Ă���Ƃ��̓X�L�b�v
			if (g->IsFault(neighbor) || neighbor == preview)
			{
				continue;
			}

			int neighborDistance = g->CalcDistance(neighbor, node2);

			// �O���Ȃ炻���I��
			if (neighborDistance < distance)
			{
				mae = neighbor;
				distance = neighborDistance;
				break;
			}
			// �������Ȃ�L�����Ă���
			else if (neighborDistance == distance)
			{
				yoko = neighbor;
			}
		}

		// prev�X�V
		preview = current;

		// �O��������΂������
		if (mae != current)
		{
			current = mae;
			if (visited[mae])	// �O���ʂ������Ƃ����Ȃ玸�s
			{
				delete[] visited;
				return -step;
			}
		}
		else
		{
			// �O��������������Ȃ�or���̌�₪�ʉߍς݂Ȃ玸�s
			if (visited[yoko] || yoko == current)
			{
				delete[] visited;
				return -step;
			}
			// �������͌�����΂������
			else
			{
				current = yoko;
			}
		}

		// 1�X�e�b�v�ǉ�
		step++;
	}

	delete[] visited;
	return step;
}

int Routing::ExtraRouting(SGraph *g, uint32_t node1, uint32_t node2, int* param)
{
	bool* visited = new bool[g->GetNodeNum()];
	for (size_t i = 0; i < g->GetNodeNum(); i++)
	{
		visited[i] = false;
	}

	int distance = g->CalcDistance(node1, node2);
	uint32_t current = node1;
	uint32_t preview = node1;
	int step = 0;
	while (current != node2)
	{
		uint32_t mae = current;
		uint32_t yoko = current;
		int maeParam = -1;
		int yokoParam = -1;
		visited[current] = true;

		for (int index = 0; index < g->GetDegree(current); index++)
		{
			uint32_t neighbor = g->GetNeighbor(current, index);

			// �̏�or���O�Œʂ��Ă���Ƃ��̓X�L�b�v
			if (g->IsFault(neighbor) || neighbor == preview)
			{
				continue;
			}

			int neighborDistance = g->CalcDistance(neighbor, node2);

			// �O���ŁA�p�����^���傫�����mae���X�V
			if (neighborDistance < distance)
			{
				if (param[neighbor] > maeParam)
				{
					mae = neighbor;
					maeParam = param[neighbor];
				}
			}
			// �������ŁA�p�����^���傫�����yoko���X�V
			else if (neighborDistance == distance)
			{
				if (param[neighbor] > yokoParam)
				{
					yoko = neighbor;
					yokoParam = param[neighbor];
				}
			}
		}

		preview = current;

		// �O��������΂������
		if (mae != current)
		{
			if (visited[mae])
			{
				delete[] visited;
				return -step;
			}
			current = mae;
			distance--;
		}
		else
		{
			// �������͌�����΂������
			if (yoko != current)
			{
				if (visited[yoko])
				{
					delete[] visited;
					return -step;
				}
				current = yoko;
			}
			// �ǂ�����Ȃ���Ύ��s
			else
			{
				delete[] visited;
				return -step;
			}
		}
		step++;
	}

	delete[] visited;
	return step;
}
