#include <Routing.h>


namespace Routing
{
	int SR(SGraph *g, SNode *node1, SNode *node2)
	{
		int step = 0;
		int distance = g->CalcDistance(node1, node2);

		SNode *current = node1, *next = node1;
		uint32_t goalIndex = node2->GetIndex();
		while (next->GetIndex() != goalIndex)
		{
			int degree = g->GetDegree(current);
			for (int index = 0; index < degree; index++)
			{
				SNode *neighbor = g->GetNeighbor(current, index);
				if (!(g->IsFault(neighbor)))
				{
					int neighborDistance = g->CalcDistance(neighbor, node2);
					if (neighborDistance < distance)
					{
						next = neighbor;
						distance = neighborDistance;
						break;
					}
				}
			}

			step++;

			// "��̏Ⴉ�O��"��������Ȃ������ꍇ���s
			if (next == current)
			{
				return -step;
			}

			current = next;
		}
		return step;
	}
}
/*
namespace Routing
{
	int SR(Experiment *exp, SGraph *g)
	{
		uint32_t current = exp->node1;
		int step = 0;
		int distance = g->CalcDistance(exp->node1, exp->node2);

		while (distance > 0)
		{
			uint32_t next = current;
			for (int index = 0; index < g->GetDimension(); index++)
			{
				uint32_t neighbor = g->GetNeighbor(current, index);
				if (!(exp->Faults[neighbor]))
				{
					int neighborDistance = g->CalcDistance(neighbor, exp->node2);
					if (neighborDistance < distance)
					{
						next = neighbor;
						distance = neighborDistance;
						break;
					}
				}
			}

			step++;

			// "��̏Ⴉ�O��"��������Ȃ������ꍇ���s
			if (next == current)
			{
				return -step;
			}

			current = next;
		}
		return step;
	}

	int NR1(Experiment *exp, SGraph *g)
	{
		uint32_t current = exp->node1;
		int distance = g->CalcDistance(exp->node1, exp->node2);
		int step = 0;
		bool* visited;
		{
			uint32_t nodeNum = g->GetNodeNum();
			visited = new bool[nodeNum];
			for (size_t i = 0; i < nodeNum; i++)
			{
				visited[i] = false;
			}
		}

		while (current != exp->node2)
		{
			visited[current] = true;
			uint32_t next = current;
			uint32_t yoko = current;

			for (int index = 0; index < g->GetDimension(); index++)
			{
				uint32_t neighbor = g->GetNeighbor(current, index);

				// �̏�or�ʉߍς݂Ȃ�X�L�b�v
				if (exp->Faults[neighbor] || visited[neighbor])
				{
					continue;
				}

				int neighborDistance = g->CalcDistance(neighbor, exp->node2);

				// �O���Ȃ炻���I��
				if (neighborDistance < distance)
				{
					next = neighbor;
					distance = neighborDistance;
					break;
				}
				// �������Ȃ�L�����Ă���
				else if (neighborDistance == distance)
				{
					yoko = neighbor;
				}
			}

			// 1�X�e�b�v�ǉ�
			step++;

			// �O��������΂������
			if (next != current)
			{
				current = next;
			}
			else
			{
				// �O��������������Ȃ���Ύ��s
				if (yoko == current)
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
		}
		delete[] visited;
		return step;
	}

	int NR2(Experiment *exp, SGraph *g)
	{
		uint32_t current = exp->node1;
		uint32_t prev = current;
		int distance = g->CalcDistance(exp->node1, exp->node2);
		int step = 0;
		bool* visited;
		{
			uint32_t nodeNum = g->GetNodeNum();
			visited = new bool[nodeNum];
			for (size_t i = 0; i < nodeNum; i++)
			{
				visited[i] = false;
			}
		}

		while (current != exp->node2)
		{
			visited[current] = true;
			uint32_t next = current;
			uint32_t yoko = current;

			for (int index = 0; index < g->GetDimension(); index++)
			{
				uint32_t neighbor = g->GetNeighbor(current, index);

				// �̏�or��O�ɒʂ����Ȃ�X�L�b�v
				if (exp->Faults[neighbor] || neighbor == prev)
				{
					continue;
				}

				int neighborDistance = g->CalcDistance(neighbor, exp->node2);

				// �O���Ȃ炻���I��
				if (neighborDistance < distance)
				{
					next = neighbor;
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
			prev = current;
			// 1�X�e�b�v�ǉ�
			step++;

			// �O��������΂������
			if (next != current)
			{
				current = next;
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
		}
		delete[] visited;
		return step;
	}

	int ER(Experiment *exp, int* param, SGraph *g)
	{
		uint32_t current = exp->node1;
		uint32_t prev = current;
		int distance = g->CalcDistance(exp->node1, exp->node2);
		int step = 0;
		bool* visited;
		{
			uint32_t nodeNum = g->GetNodeNum();
			visited = new bool[nodeNum];
			for (size_t i = 0; i < nodeNum; i++)
			{
				visited[i] = false;
			}
		}

		while (distance > 0)
		{
			visited[current] = true;
			uint32_t mae = current;
			uint32_t yoko = current;
			int maeParam = 0;
			int yokoParam = 0;

			for (int index = 0; index < g->GetDimension(); index++)
			{
				uint32_t neighbor = g->GetNeighbor(current, index);

				// �̏�or�ʉߍς݂Ȃ�X�L�b�v
				if (exp->Faults[neighbor] || neighbor == prev)
				{
					continue;
				}

				int neighborDistance = g->CalcDistance(neighbor, exp->node2);

				// �O���ŁA�p�����^���傫����΍X�V
				if (neighborDistance < distance)
				{
					if (param[neighbor] > maeParam)
					{
						mae = neighbor;
						maeParam = param[neighbor];
					}
				}
				// �������ŁA�p�����^���傫����΍X�V
				else if (neighborDistance == distance)
				{
					if (param[neighbor] > yokoParam)
					{
						yoko = neighbor;
						yokoParam = param[neighbor];
					}
				}
			}

			// prev�X�V
			prev = current;
			// 1�X�e�b�v�ǉ�
			step++;

			// �O��������΂������
			if (mae != current)
			{
				current = mae;
				distance--;
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
		}
		delete[] visited;
		return step;
	}
}

*/