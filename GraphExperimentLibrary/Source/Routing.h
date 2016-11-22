#pragma once

#include "Graph\SGraph.h"

namespace Routing
{
	/// <summary>
	/// �p�����^�𐶐�
	/// �S��0(NormalRouting�p)
	/// </summary>
	/// <param name="g">�ΏۃO���t</param>
	/// <returns>
	/// �p�����^
	/// </returns>
	int* CreateZeroParameter(SGraph *g);

	/// <summary>
	/// �p�����^�𐶐�
	/// ��̏�ȗאڒ��_��
	/// </summary>
	/// <param name="g">�ΏۃO���t</param>
	/// <returns>
	/// �p�����^
	/// </returns>
	int* CreateParameter(SGraph *g);

	/// <summary>
	/// �p�����^�𐶐�
	/// ��̏�ȗאڒ��_�̃p�����^�̘a
	/// </summary>
	/// <param name="g">�ΏۃO���t</param>
	/// <param name="param">�O�̃p�����^</param>
	/// <returns>
	/// �p�����^
	/// </returns>
	int* CreateParameter(SGraph *g, int *param);

	/// <summary>
	/// Simple Routing
	/// �P�D�O���אڒ��_��I��
	/// �Q�D������Ȃ���Ύ��s
	/// </summary>
	/// <param name="g">�ΏۃO���t</param>
	/// <param name="node1">�o���m�[�h</param>
	/// <param name="node2">�ړI�m�[�h</param>
	/// <returns>
	/// �߂�l�̐�Βl���I�����̃X�e�b�v��
	/// �������͐��̐��A���s����0�����̐�
	/// </returns>
	int SimpleRouting(SGraph *g, const uint32_t node1, const uint32_t node2);

	/// <summary>
	/// Normal Routing 1
	/// �P�D�O���אڒ��_��I��
	/// �Q�D�����אڒ��_��I��
	/// �R�D������Ȃ���Ύ��s
	/// �����ɒʂ����m�[�h�͑I�΂Ȃ�
	/// </summary>
	/// <param name="g">�ΏۃO���t</param>
	/// <param name="node1">�o���m�[�h</param>
	/// <param name="node2">�ړI�m�[�h</param>
	/// <returns>
	/// �߂�l�̐�Βl���I�����̃X�e�b�v��
	/// �������͐��̐��A���s����0�����̐�
	/// </returns>
	int NormalRouting1(SGraph *g, uint32_t node1, uint32_t node2);

	/// <summary>
	/// Normal Routing 2
	/// �P�D�O���אڒ��_��I��
	/// �Q�D�����אڒ��_��I��
	/// �R�D������Ȃ���Ύ��s
	/// �����O�̃m�[�h�͑I�΂Ȃ�
	/// </summary>
	/// <param name="g">�ΏۃO���t</param>
	/// <param name="node1">�o���m�[�h</param>
	/// <param name="node2">�ړI�m�[�h</param>
	/// <returns>
	/// �߂�l�̐�Βl���I�����̃X�e�b�v��
	/// �������͐��̐��A���s����0�����̐�
	/// </returns>
	int NormalRouting2(SGraph *g, uint32_t node1, uint32_t node2);

	/// <summary>
	/// Extre Routing
	/// �P�D�O���אڒ��_�̂����p�����^���ő�̂��̂�I��
	/// �Q�D�����אڒ��_�̂����p�����^���ő�̂��̂�I��
	/// �R�D������Ȃ���Ύ��s
	/// �����O�̃m�[�h�͑I�΂Ȃ�
	/// </summary>
	/// <param name="g">�ΏۃO���t</param>
	/// <param name="node1">�o���m�[�h</param>
	/// <param name="node2">�ړI�m�[�h</param>
	/// <param name="param">�p�����^</param>
	/// <returns>
	/// �߂�l�̐�Βl���I�����̃X�e�b�v��
	/// �������͐��̐��A���s����0�����̐�
	/// </returns>
	int ExtraRouting(SGraph *g, uint32_t node1, uint32_t node2, int* param);
}