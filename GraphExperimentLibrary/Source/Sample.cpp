#include <iostream>
#include <fstream>
#include <random>

#include "..\Header\Sample.h"
#include "..\Header\Routing.h"

using namespace std;

/*
namespace Sample {
	// CalcDistance�����������ǂ����`�F�b�N
	void check_CalcDistance(const int minDim, const int MaxDim)
	{
		/// n��minDim����maxDim�ɂ����āACalcDistance�̌v�Z���ʂ�S�T���̂��̂Ɣ�r
		/// SQ�p�Ȃ̂ŁA�e�^�C�v1���_���`�F�b�N

		SpinedCube sq;
		for (size_t dim = minDim; dim <= MaxDim; dim++)
		{
			cout << "n = " << dim << " ���v�Z��..." << endl;
			sq.SetDimension(static_cast<int>(dim));
			for (size_t i = 0; i < 4; i++)
			{
				int* ary = sq.CalcAllDistanceBFS(static_cast<ulong>(i));
				for (size_t d = 0; d < sq.GetNodeNum(); d++)
				{
					int dist = static_cast<int>(SPR::GetMinimalExpansion(	static_cast<ulong>(i), 
																			static_cast<ulong>(d), 
																			static_cast<int>(dim)
																		).GetCount());
					if (ary[d] != dist)
					{
						printf_s("d(%d, %d) CalcDist=%d, WPS=%d\n", static_cast<int>(i), static_cast<int>(d), dist, ary[d]);
					}
				}
				delete[] ary;
			}
			cout << endl;

		}
	}

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
					parameter[node] += param[neighbor];
				}
			}
		}
		return parameter;
	}

	void RoutingExperiment(SGraph *g, int times, char* filename)
	{
		int count[6][10];		// ���[�e�B���O���������i�[����z��
		double length[6][10];	// �o�H���̘a���i�[����z��
		double fstep[10];		// ���s���̃X�e�b�v�����i�[����z��
		for (size_t i = 0; i < 10; i++)
		{
			fstep[i] = 0;
			for (size_t j = 0; j < 6; j++)
			{
				length[j][i] = 0;
				count[j][i] = 0;
			}
		}
		char contentName[][4] = { "SR", "NR1", "NR2", "ER1", "ER2", "ER3" };
		for (size_t i = 1; i <= times; i++)
		{
			printf_s("%6d / %d\r", i, times);
			for (int j = 0; j < 10; j++)
			{
				Experiment exp(g, j * 10);	// �p�����^�̏���
				int len;
				// SR
				if ((len = Routing::SR(&exp, g)) > 0)
				{
					count[0][j]++;
					length[0][j] += len;
				}

				// NR1
				if ((len = Routing::NR1(&exp, g)) > 0)
				{
					count[1][j]++;
					length[1][j] += len;
				}

				// NR2
				if ((len = Routing::NR2(&exp, g)) > 0)
				{
					count[2][j]++;
					length[2][j] += len;
				}
				else
				{
					fstep[j] += len;
				}

				// ER1
				int* param1 = GetParameter1(exp.Faults, g);
				if ((len = Routing::ER(&exp, param1, g)) > 0)
				{
					count[3][j]++;
					length[3][j] += len;
				}

				// ER2
				int* param2 = GetParameter2(exp.Faults, param1, g);
				if ((len = Routing::ER(&exp, param2, g)) > 0)
				{
					count[4][j]++;
					length[4][j] += len;
				}

				// ER3
				int* param3 = GetParameter2(exp.Faults, param2, g);
				if ((len = Routing::ER(&exp, param2, g)) > 0)
				{
					count[5][j]++;
					length[5][j] += len;
				}

				delete[] param1;
				delete[] param2;
				delete[] param3;
			}
		}

		ofstream of(filename);

		of << "���B��\n";
		for (size_t i = 0; i < 6; i++)
		{
			of << contentName[i] << ',';
			for (size_t j = 0; j < 10; j++)
			{
				of << ((double)count[i][j] / times) << ',';
			}
			of << endl;
		}
		of << endl;

		of << "���όo�H��\n";
		for (size_t i = 0; i < 6; i++)
		{
			of << contentName[i] << ',';
			for (size_t j = 0; j < 10; j++)
			{
				of << (length[i][j] / count[i][j]) << ',';
			}
			of << endl;
		}
		of << endl;

		of << "���ώ��s�X�e�b�v��\nNR2,";
		for (size_t j = 0; j < 10; j++)
		{
			of << (fstep[j] / (count[2][j] - times)) << ',';
		}
		of.close();
	}

}
*/