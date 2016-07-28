#pragma once

#include <iostream>
#include <queue>
#include <random>

#include "Node\SNode.h"


class SGraph {
protected:
	/// <summary>������</summary>
	int Dimension;

	/// <summary>�m�[�h��</summary>
	uint32_t NodeNum;

	/// <summary>32bitMT</summary>
	std::mt19937 MT;

	/// <summary>�̏ᗦ(%)</summary>
	int FaultRatio;

	/// <summary>�̏ᐔ</summary>
	uint32_t FaultNum;

	/// <summary>�e�m�[�h���̏Ⴕ�Ă��邩�������z��</summary>
	bool* Faults;

	/// <summary>�m�[�h�����v�Z����</summary>
	/// <returns>�m�[�h��</returns>
	virtual uint32_t CalcNodeNum() = 0;

public:
	SGraph();
	~SGraph();

	/// <summary>���������Z�b�g</summary>
	/// <param name="_dim">������</param>
	void SetDimension(int _dim);

	/// <summary>���������擾</summary>
	/// <param name="seed">������</param>
	int GetDimension();

	/// <summary>�m�[�h�����擾</summary>
	/// <param name="seed">�m�[�h��</param>
	uint32_t GetNodeNum();

	/// <summary>�����̃V�[�h���Z�b�g</summary>
	/// <param name="seed">�V�[�h�l</param>
	void SetRandSeed(int seed);

	/// <summary>���_�̎�����Ԃ�</summary>
	/// <returns>����</returns>
	virtual int GetDegree(SNode* node) = 0;

	/// <summary>
	/// index(0 <= index < Dimension)�ɑ΂��Ĉ�ӂȗאڒ��_��Ԃ�
	/// �K���I�[�o�[���C�h����
	/// </summary>
	/// <param name="node">�m�[�h</param>
	/// <param name="index">�אڒ��_�̔ԍ�</param>
	/// <returns>�אڒ��_</returns>
	virtual SNode* GetNeighbor(SNode* node, int index) = 0;

	/// <summary>
	/// ���_���̏Ⴉ�ǂ�����Ԃ�
	/// </summary>
	/// <param name="node">�m�[�h</param>
	/// <returns>�̏Ⴉ�ǂ���</returns>
	bool IsFault(SNode* node);

	/// <summary>
	/// 2���_���A�����ǂ�����Ԃ�(������Əd��)
	/// </summary>
	/// <param name="node1">�m�[�h</param>
	/// <param name="node2">�m�[�h</param>
	/// <returns>�A�����ǂ���</returns>
	bool IsConnected(SNode* node1, SNode* node2);



	/// <summary>
	/// 2���_�Ԃ̋������v�Z
	/// �f�t�H���g�ł͓����I�ɂ͒P�ɕ��D��T�����Ă�
	/// �K�v�ɉ����ăI�[�o�[���C�h����
	/// </summary>
	/// <param name="node1">�n�_</param>
	/// <param name="node2">�I�_</param>
	/// <returns>����</returns>
	virtual int CalcDistance(SNode* node1, SNode* node2);

	/// <summary>���钸�_����C�ӂ̒��_�ւ̋����𕝗D��T��</summary>
	/// <param name="faults">�̏ᒸ�_�W��</param>
	/// <param name="node">�m�[�h</param>
	/// <returns>
	/// node����̋����̔z��(�̏�Ȃ�-1)
	/// �g���I������������邱��
	/// </returns>
	int* CalcAllDistanceBFS(SNode* node);



	/// <summary>�̏�v�f�W�������</summary>
	/// <param name="faultRatio">�̏ᗦ(%)</param>
	void GenerateFaults(int faultRatio);

	/// <summary>��̏�Ȓ��_���烉���_����1�I��</summary>
	/// <param name="node">�I�񂾃m�[�h������|�C���^</param>
	/// <returns>�m�[�h</returns>
	void GetNodeRandom(SNode* node);

	/// <summary>���钸�_�ƘA���Ȓ��_�������_���ɑI��</summary>
	/// <param name="node1">���ƂȂ�m�[�h�m�[�h</param>
	/// <param name="node2">�������m�[�h</param>
	/// <returns>
	///		�A���ȃm�[�h�����݂��Ȃ��Ȃ�Ύ��s
	/// </returns>
	bool GetConnectedNode(SNode* node1, SNode* node2);
};





/// <summary>�����ɋ��ʂ��Ďg����v�f�ꎮ���܂Ƃ߂�N���X</summary>
class Experiment
{
public:
	/// <summary>�̏ᒸ�_�W��</summary>
	bool* Faults;

	/// <summary>��̏Ⴉ�A����2���_</summary>
	SNode *node1, *node2;

	Experiment(SGraph *g, int faultRatio)
	{
		// �̏�����
		g->GenerateFaults(faultRatio);

		// �����_���ɘA����2���_��I��
		this->node2 = nullptr;
		while (this->node2 != nullptr)
		{
			/*
			this->node1 = g->GetNodeRandom(faultRatio);
			this->node2 = g->GetConnectedNode(Faults, nullptr);*/
		}
	}

	~Experiment()
	{
		if (Faults != nullptr){
			delete[] Faults;
		}
	}
};

