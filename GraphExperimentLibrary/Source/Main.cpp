#include <iostream>
#include <fstream>

#include "..\Header\Common.h"
#include <Graph\SpinedCube.h>
#include <Routing.h>

#include <chrono>

using namespace std;

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
	const int routingNum = 4;	// ���[�e�B���O�̎�ނ̐�
	char routingName[][4] = {"SR", "NR1", "NR2", "ER"};

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
			// Simple
			result = Routing::SimpleRouting(&sq, node1, node2);
			if (result > 0)
			{
				success[0][j]++;
				step[0][j] += result;
			}
			else
			{
				fstep[0][j] += -result;
			}

			// Normal 1
			result = Routing::NormalRouting1(&sq, node1, node2);
			if (result > 0)
			{
				success[1][j]++;
				step[1][j] += result;
			}
			else
			{
				fstep[1][j] += -result;
			}

			// Normal 2
			result = Routing::NormalRouting2(&sq, node1, node2);
			if (result > 0)
			{
				success[2][j]++;
				step[2][j] += result;
			}
			else
			{
				fstep[2][j] += -result;
			}

			// Extra
			result = Routing::ExtraRouting(&sq, node1, node2, param);
			if (result > 0)
			{
				success[3][j]++;
				step[3][j] += result;
			}
			else
			{
				fstep[3][j] += -result;
			}

			// �S�~�|��
			delete[] param;
		}
		printf_s("%5d / %d\r", i, trials);	// �i���̕\��
	}
	std::cout << endl;

	// ���ʂ̏o��
	ofstream of("result.csv");
	of << "������,0%,10%,20%,30%,40%,50%,60%,70%,80%,90%,\n";
	for (size_t i = 0; i < routingNum; i++)
	{
		of << routingName[i] << ',';
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)success[i][j] / trials) << ',';
		}
		of << endl;
	}
	of << "\n���όo�H��,0%,10%,20%,30%,40%,50%,60%,70%,80%,90%,\n";
	for (size_t i = 0; i < routingNum; i++)
	{
		of << routingName[i] << ',';
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)step[i][j] / success[i][j]) << ',';
		}
		of << endl;
	}
	of << "\n���s�܂ł̕��σX�e�b�v��,0%,10%,20%,30%,40%,50%,60%,70%,80%,90%,\n";
	for (size_t i = 0; i < routingNum; i++)
	{
		of << routingName[i] << ',';
		for (size_t j = 0; j < 10; j++)
		{
			of << ((double)fstep[i][j] / (trials - success[i][j])) << ',';
		}
		of << endl;
	}
	of.close();

	std::cout << "FInish.\nPress enter.";
	getchar();
	return 0;
}

