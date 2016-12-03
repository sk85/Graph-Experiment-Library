#pragma once

#include "..\Score.h"
#include "SGraph.h"

#define DEBUG


class LTQ : public SGraph
{
public:
#ifdef DEBUG
	void Debug_CalcInnerForward()
	{
		for (int dim = 2; dim <= 11; dim++)
		{
			printf_s("dim = %d\n", dim);
			SetDimension(dim);
			for (uint32_t node1 = 0; node1 < GetNodeNum(); node1++)
			{
				for (uint32_t node2 = 0; node2 < GetNodeNum(); node2++)
				{
					uint32_t forward = CalcInnerForward(node1, node2);
					int distance = CalcDistance(node1, node2);
					for (int i = 0; i < GetDegree(node1); i++)
					{
						if (forward & (1 << i))
						{
							uint32_t neighbor = GetNeighbor(node1, i);
							int distance_n = CalcDistance(neighbor, node2);
							if (distance_n != distance - 1)
							{
								printf_s("d(%u, %u) = %d, d(%u, %u) = %d", node1, node2, distance, neighbor, node2, distance_n);
								getchar();
							}
						}
					}
				}
			}
		}
	}
#endif
	// SGraph���p��
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
	virtual uint32_t GetForwardNeighbor(uint32_t s, uint32_t d);

	/// <summary>
	///		Capability���v�Z
	///		<para>�T�u�O���t���ƂɌv�Z(���삭��̑��_)</para>
	/// </summary>
	/// <returns>Capability��Score�I�u�W�F�N�g</returns>
	Score* CalcCapability();

	/// <summary>
	///		Capability���v�Z
	///		<para>����</para>
	/// </summary>
	/// <returns>Capability��Score�I�u�W�F�N�g</returns>
	Score* CalcCapability2();

	/// <summary>���a���v�Z����</summary>
	/// <returns>���a</returns>
	int GetDiameter();

	// ��r�p�̎G�ȃ��[�e�B���O
	int LTQ::Routing_Stupid(uint32_t node1, uint32_t node2);

	/// <summary>
	///		Simple Routing whth Capability
	///		<para>SR��Score�̍������̂�D�悵�đI��</para>
	///		<para>���삭��̑��_����</para>
	/// </summary>
	/// <param name="node1">�o�����_�̃A�h���X</param>
	/// <param name="node2">�ړI���_�̃A�h���X</param>
	/// <param name="score">���[�e�B���O�ɗp����w�W</param>
	/// <returns>
	///		���[�e�B���O�̃X�e�b�v���B
	///		���s�Ȃ��0�܂��͕��̐�
	/// </returns>
	int Routing_SimpleCapability(uint32_t node1, uint32_t node2, Score* score);

	// ���삭��̑��_��
	int Routing_SimpleCapability2(uint32_t node1, uint32_t node2, Score* score);

	// �����T�u�O���t���̑O���אڒ��_��Ԃ��B
	// �ŒZ�o�H�I���̓r���Ō����expansion�Ȃ̂ŁA�S�ĂƂ͌���Ȃ�
	uint32_t CalcInnerForward(uint32_t node1, uint32_t node2);
};