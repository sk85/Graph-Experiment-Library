#pragma once

#include <Graph\SpinedCube.h>

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
	/// Extre Routing
	/// �P�D�O���אڒ��_�̂����p�����^���ő�̂��̂�I��
	/// �Q�D�����אڒ��_�̂����p�����^���ő�̂��̂�I��
	/// �R�D������Ȃ���Ύ��s
	/// </summary>
	/// <param name="g">�ΏۃO���t</param>
	/// <param name="node1">�o���m�[�h</param>
	/// <param name="node2">�ړI�m�[�h</param>
	/// <param name="index">�אڒ��_�̔ԍ�</param>
	/// <returns>
	/// �߂�l�̐�Βl���I�����̃X�e�b�v��
	/// �������͐��̐��A���s����0�����̐�
	/// </returns>
	int ExtraRouting(SGraph *g, uint32_t node1, uint32_t node2, int* param);
}