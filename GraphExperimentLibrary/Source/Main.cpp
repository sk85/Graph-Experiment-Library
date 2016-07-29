#include <iostream>
#include <fstream>

#include "..\Header\Common.h"
#include <Graph\SpinedCube.h>
#include <Routing.h>

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
	// �����̏�����ݒ�
	SpinedCube sq;				// �Ώۂ̃O���t��錾
	sq.SetDimension(10);		// ���������Z�b�g
	const int trials = 1000;	// ���s��
	const int routingNum = 1;	// ���[�e�B���O�̎�ނ̐�

	// ���ʂ�ێ�����̈�̊m�ۂƏ�����
	int success[routingNum][10];	// ���[�e�B���O������
	int step[routingNum][10];		// �X�e�b�v���̘a
	int fstep[routingNum][10];		// ���s���X�e�b�v���̘a
	for (size_t i = 0; i < routingNum; i++)	// ������
	{
		for (size_t j = 0; j < 10; j++)
		{
			success[i][j] = 0;
			step[i][j] = 0;
			fstep[i][j] = 0;
		}
	}

	// �����{��
	for (uint32_t i = 1; i <= trials; i++)
	{
		for (int j = 0; j < 10; j++)
		{
			sq.GenerateFaults(j * 10);	// �̏�𔭐�������

			uint32_t node1, node2;		// �o���m�[�h�ƖړI�m�[�h
			do
			{
				node1 = sq.GetNodeRandom();
				node2 = sq.GetConnectedNodeRandom(node1);
			} while (node2 == node1);	// �A���Ȍ�₪������܂Ń��[�v

			// �p�����^�𐶐�
			int *param = Routing::CreateZeroParameter(&sq);

			int result;
			// ���[�e�B���O
			result = Routing::ExtraRouting(&sq, node1, node2, param);
			if (result > 0)
			{
				success[0][j]++;
				step[0][j] += result;
			}
			else
			{
				fstep[0][j] += -result;
			}

			// �S�~�|��
			delete[] param;
		}
		printf_s("%5d / %d\r", i, trials);	// �i���̕\��
	}

	// ���ʂ̏o��
	ofstream of("result.csv");
	of << "������" << endl;
	for (size_t i = 0; i < routingNum; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)success[i][j] / trials) << ',';
		}
		of << endl;
	}
	of << "���όo�H��" << endl;
	for (size_t i = 0; i < routingNum; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)step[i][j] / success[i][j]) << ',';
		}
		of << endl;
	}
	of << "���s�܂ł̕��σX�e�b�v��" << endl;
	for (size_t i = 0; i < routingNum; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)fstep[i][j] / (trials - success[i][j])) << ',';
		}
		of << endl;
	}
	of.close();



	std::cout << "end1";
	getchar();
	return 0;

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

	ofstream of2("kekka2.csv");
	for (size_t i = 0; i < 4; i++)
	{
		for (size_t j = 0; j < 10; j++)
		{
			of2 << count[i][j] << ',';
		}
		of2 << endl;
	}
	of2.close();

	cout << endl << "end" << endl;
	
	getchar();
}

