#include <fstream>
#include <iostream>
#include <chrono>

#include <Graph\SpinedCube.h>
#include <Graph\PancakeGraph.h>
#include <Test.h>
#include <Routing.h>
#include "..\Header\Common.h"

using namespace std;

namespace Test
{
	void e161024(int minDim, int maxDim)
	{
		printf_s("LTQ::GetPreferredNeighbor(uint32_t s, uint32_t d)�̓���m�F���J�n���܂�\n");

		LTQ ltq;

		for (size_t dim = minDim; dim < maxDim; dim++)
		{
			ltq.SetDimension(dim);

			auto start = std::chrono::system_clock::now();      // �v���X�^�[�g������ۑ�
			printf_s("n = %d�J�n\n", dim);

			for (uint32_t s = 0; s < ltq.GetNodeNum(); s++)
			{
				for (uint32_t d = 0; d < ltq.GetNodeNum(); d++)
				{
					// �����̌v�Z
					uint32_t correctAnswer = 0;
					int distance = ltq.CalcDistance(s, d);
					for (int i = 0; i < ltq.GetDegree(s); i++)
					{
						if (ltq.CalcDistance(ltq.GetNeighbor(s, i), d) < distance)
						{
							correctAnswer |= 1 << i;
						}
					}

					// �v�Z
					int testAnswer = ltq.GetPreferredNeighbor(s, d);

					// �\��
					if (correctAnswer != testAnswer)
					{
						printf_s("d(%d, %d) = %d  ", s, d, distance);
						//printf_s("count = %d, testCount = %d\n", count, countTest);
						printf_s("d = %d  ", d);
						showBinary(d);
						getchar();
					}
				}
			}
			auto end = std::chrono::system_clock::now();       // �v���I��������ۑ�
			auto dur = end - start;        // �v�������Ԃ��v�Z
			auto msec = std::chrono::duration_cast<std::chrono::milliseconds>(dur).count();
			printf_s("...ok. ���v���� : %dmsec.\n", msec);
		}
	}

	void e160930(SGraph *g, int maxDim)
	{
		for (size_t dim = 2; dim <= maxDim; dim++)
		{
			printf_s("Dim = %d\n", dim);
			g->SetDimension(dim);

			for (size_t s = 0; s < g->GetNodeNum(); s++)
			{
				int *distAry = g->CalcAllDistanceBFS(s);
				for (size_t d = 0; d < g->GetNodeNum(); d++)
				{
					int dist = g->CalcDistance(s, d);
					if (distAry[d] != dist)
					{
						printf_s("(%d, %d) BFS:%d Calc:%d\n", s, d, distAry[d], dist);
						showBinary(s);
						showBinary(d);
						getchar();
					}
				}
				delete[] distAry;
			}
		}
	}

	void e160720(SGraph *g, char *filename, int trials)
	{
		// �����̏�����ݒ�
		g->SetDimension(10);		// ���������Z�b�g
		const int routingNum = 6;	// ���[�e�B���O�̎�ނ̐�
		char routingName[][4] = { "SR", "NR1", "NR2", "ER1", "ER2", "ER3" };

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
				g->GenerateFaults(j * 10);	// �̏�𔭐�������

				uint32_t node1, node2;		// �o���m�[�h�ƖړI�m�[�h
				do
				{
					node1 = g->GetNodeRandom();
					node2 = g->GetConnectedNodeRandom(node1);
				} while (node2 == node1);	// �A���Ȍ�₪������܂Ń��[�v

											// �p�����^�𐶐�
				int *param1 = Routing::CreateParameter(g);
				int *param2 = Routing::CreateParameter(g, param1);
				int *param3 = Routing::CreateParameter(g, param2);

				int result;
				// Simple
				result = Routing::SimpleRouting(g, node1, node2);
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
				result = Routing::NormalRouting1(g, node1, node2);
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
				result = Routing::NormalRouting2(g, node1, node2);
				if (result > 0)
				{
					success[2][j]++;
					step[2][j] += result;
				}
				else
				{
					fstep[2][j] += -result;
				}

				// Extra 1
				result = Routing::ExtraRouting(g, node1, node2, param1);
				if (result > 0)
				{
					success[3][j]++;
					step[3][j] += result;
				}
				else
				{
					fstep[3][j] += -result;
				}

				// Extra 2
				result = Routing::ExtraRouting(g, node1, node2, param2);
				if (result > 0)
				{
					success[4][j]++;
					step[4][j] += result;
				}
				else
				{
					fstep[4][j] += -result;
				}

				// Extra 3
				result = Routing::ExtraRouting(g, node1, node2, param3);
				if (result > 0)
				{
					success[5][j]++;
					step[5][j] += result;
				}
				else
				{
					fstep[5][j] += -result;
				}

				// �S�~�|��
				delete[] param1;
				delete[] param2;
				delete[] param3;
			}
			printf_s("%5d / %d\r", i, trials);	// �i���̕\��
		}
		std::cout << endl;

		// ���ʂ̏o��
		ofstream of(filename);
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
	}

	void e160731()
	{
		// PancakeGraph�N���X�̃C���X�^���Xp��錾
		PancakeGraph p;

		for (size_t i = 2; i <= 10; i++)
		{
			// ���������Z�b�g
			p.SetDimension(i);

			// �����̕\���擾
			int* lengthList = p.CalcAllDistanceBFS(0);

			// �ő�l(���a)��T��
			int diameter = 0;
			for (size_t j = 0; j < p.GetNodeNum(); j++)
			{
				if (lengthList[j] > diameter) diameter = lengthList[j];
			}

			// �\��
			printf_s("n=%2u : NodeNum=%7u , diameter=%d\n", i, p.GetNodeNum(), diameter);

			// ���������
			delete[] lengthList;
		}
	}
}