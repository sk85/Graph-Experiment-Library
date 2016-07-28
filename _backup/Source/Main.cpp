#include <iostream>
#include <fstream>

#include "..\Header\Common.h"
#include "..\Header\SpinedCube\SpinedCube.h"
#include "..\Header\Sample.h"

#include <chrono>

using namespace std;


int* GetParameter1(bool *faults, SGraph *g)
{
	uint32_t nodeNum = g->GetNodeNum();
	int* parameter = new int[nodeNum];
	for (size_t node = 0; node < nodeNum; node++)
	{
		parameter[node] = 0;
		for (int index = 0; index < g->GetDimension(); index++)
		{
			uint32_t neighbor = g->GetNeighbor(node, index);
			if (!faults[neighbor])
			{
				parameter[node]++;
			}
		}
	}
	return parameter;
}

int* GetParameter2(bool *faults, int* param, SGraph *g)
{
	uint32_t nodeNum = g->GetNodeNum();
	int* parameter = new int[nodeNum];
	for (size_t node = 0; node < nodeNum; node++)
	{
		parameter[node] = 0;
		for (int index = 0; index < g->GetDimension(); index++)
		{
			uint32_t neighbor = g->GetNeighbor(node, index);
			if (!faults[neighbor])
			{
				parameter[node] += param[node];
			}
		}
	}
	return parameter;
}

// �ŒZ�o�H�����F�߂Ȃ����[�e�B���O
// "��̏Ⴉ�O��"�Ȓ��_��������Ȃ������玸�s
bool Routing1(Experiment *exp, SGraph *g)
{
	uint32_t current = exp->node1;
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

		// "��̏Ⴉ�O��"��������Ȃ������ꍇ���s
		if (next == current)
		{
			return false;
		}

		current = next;
	}
	return true;
}

// �������ς��Ȃ��o�H���F�߂郋�[�e�B���O
// �P�D�O��������΂����I��
// �Q�D������������΂����I��
// �R. ������Ύ��s
bool Routing2(Experiment *exp, SGraph *g)
{
	uint32_t current = exp->node1;
	int distance = g->CalcDistance(exp->node1, exp->node2);
	uint32_t nodeNum = g->GetNodeNum();
	bool* visited = new bool[nodeNum];
	for (size_t i = 0; i < nodeNum; i++)
	{
		visited[i] = false;
	}

	while (distance > 0)
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
				return false;
			}
			// �������͌�����΂������
			else
			{
				current = yoko;
			}
		}
	}
	delete[] visited;
	return true;
}

// �p�����^���ӎ��������[�e�B���O
// �P�D�O��������΁A���̒�����p�����^���ő�̂��̂�I��
// �Q�D������������΁A���̒�����p�����^���ő�̂��̂�I��
// �R. ������Ύ��s
bool Routing3(Experiment *exp, int* param, SGraph *g)
{
	uint32_t current = exp->node1;
	int distance = g->CalcDistance(exp->node1, exp->node2);
	uint32_t nodeNum = g->GetNodeNum();
	bool* visited = new bool[nodeNum];
	for (size_t i = 0; i < nodeNum; i++)
	{
		visited[i] = false;
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
			if (exp->Faults[neighbor] || visited[neighbor])
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


		// �O��������΂������
		if (mae != current)
		{
			current = mae;
			distance--;
		}
		else
		{
			// �O��������������Ȃ���Ύ��s
			if (yoko == current)
			{
				delete[] visited;
				return false;
			}
			// �������͌�����΂������
			else
			{
				current = yoko;
			}
		}
	}
	delete[] visited;
	return true;
}


int main(void)
{
	SpinedCube sq;
	sq.SetDimension(10);

	int count[4][10];
	for (size_t i = 0; i < 10; i++)
	{
		count[0][i] = 0;
		count[1][i] = 0;
		count[2][i] = 0;
		count[3][i] = 0;
	}

	for (size_t i = 1; i <= 1000; i++)
	{
		printf_s("%5d / 1000\r", i);
		for (int j = 0; j < 10; j++)
		{
			Experiment exp(&sq, j * 10);
			if (Routing1(&exp, &sq))
			{
				count[0][j]++;
			}
			if (Routing2(&exp, &sq))
			{
				count[1][j]++;
			}
			int* param1 = GetParameter1(exp.Faults, &sq);
			if (Routing3(&exp, param1, &sq))
			{
				count[2][j]++;
			}
			int* param2 = GetParameter2(exp.Faults, param1, &sq);
			if (Routing3(&exp, param2, &sq))
			{
				count[3][j]++;
			}
			delete[] param1;
			delete[] param2;
		}
	}

	ofstream of("kekka2.csv");
	for (size_t i = 0; i < 4; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of << count[i][j] << ',';
		}
		of << endl;
	}
	of.close();

	cout << endl << "end" << endl;
	
	getchar();
}

