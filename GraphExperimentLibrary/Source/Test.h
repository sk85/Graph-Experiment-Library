#pragma once

#include "Graph\SGraph.h"

namespace Test
{
	// LTQ��GetPreferredNeighbor�����������e�X�g����
	void e161024(int minDim, int maxDim);

	// SGraph��CalcDistance�����������e�X�g����
	void e160930(SGraph *g, int maxDim);

	// 2016/07/20�ɂ��������
	// 2016/09/30�C�ӂ�SGraph�ɑ΂��ēK���ł���悤�ɍX�V
	// SGraph�ɑ΂��郋�[�e�B���O���낢��
	void e160720(SGraph *g, char *filename, int trials);
	
	// 2016/07/31�ɂ��������
	// �p���P�[�L�O���t�̒��a�����߂�
	void e160731();
}