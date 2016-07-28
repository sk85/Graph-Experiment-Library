#include <iostream>
#include <random>

#include "..\Header\Sample.h"

using namespace std;


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


}