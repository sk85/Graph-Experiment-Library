#pragma once

#include <iostream>
#include <queue>
#include <random>

#include "Common.h"


class SGraph {
protected:
	/// <summary>������</summary>
	int Dimension;

	/// <summary>�m�[�h��</summary>
	uint32_t NodeNum;

	/// <summary>32bitMT</summary>
	std::mt19937 mt;
public:

	void SetDimension(int _dim);
	int GetDimension();
	uint32_t GetNodeNum();

	/// <summary>�����̃V�[�h���Z�b�g</summary>
	/// <param name="seed">�V�[�h�l</param>
	void SetRandSeed(int seed);

	/// <summary>���a���v�Z</summary>
	/// <returns>���a</returns>
	virtual int CalcDiameter() = 0;



	// �אڒ��_��Ԃ�
	/*
		0 <= index < Dimension �𖞂���index�ɑ΂��A���ꂼ��ʂ̗אڒ��_��Ԃ�
		param	node : �m�[�h
				index : �אڒ��_�̔ԍ�
		return	�אڒ��_
			�K����`���邱��
	*/
	virtual ulong GetNeighbor(ulong node, int index) = 0;



	// 2���_�Ԃ̋������v�Z(�̏�Ȃ�)
	/*
		param	node1, node2 : �m�[�h
		return	����
			�f�t�H���g�ł͕��D��T���B
			�K�v�ɉ����ăI�[�o�[���C�h���ׂ��B
	*/
	virtual int CalcDistance(uint32_t node1, uint32_t node2);

	// 2���_�Ԃ̋������v�Z(�̏Ⴀ��)
	/*
	param	node1, node2 : �m�[�h
			faults : �̏ᒸ�_�W��
	return	����
	�f�t�H���g�ł͕��D��T���B
	�K�v�ɉ����ăI�[�o�[���C�h���ׂ��B
	*/
	int CalcDistance(const bool *faults, uint32_t node1, uint32_t node2);

	// ���钸�_����C�ӂ̒��_�ւ̋����𕝗D��T��(�̏�Ȃ�)
	/*
		param	node : �m�[�h
		return	node����̋����̔z��
			�Ԃ�l�͎g���I������������邱��
	*/
	int* CalcAllDistanceBFS(uint32_t node);

	// ���钸�_����C�ӂ̒��_�ւ̋����𕝗D��T��(�̏Ⴀ��)
	/*
		param	faults : �̏���
				node : �m�[�h
		return	node����̋����̔z��(�̏�Ȃ�-1)
				�Ԃ�l�͎g���I������������邱��
	*/
	int* CalcAllDistanceBFS(const bool *faults, uint32_t node);



	/// <summary>�̏�v�f�W�������</summary>
	/// <param name="faultRatio">�̏ᗦ</param>
	/// <param name="randSeed">32bitMT�̃V�[�h�l</param>
	/// <returns>��i�v�f���̏Ⴉ������bool[i]</returns>
	bool* CreateFaults(int faultRatio);

	/// <summary>��̏�Ȓ��_���烉���_����1�I��</summary>
	/// <param name="faults">�̏ᒸ�_�W��</param>
	/// <param name="randSeed">32bitMT�̃V�[�h�l</param>
	/// <returns>�m�[�h</returns>
	uint32_t GetNodeRandom(bool *faults, uint32_t faultRatio);

	/// <summary>���钸�_�ƘA���Ȓ��_�������_���ɑI��</summary>
	/// <param name="faults">�̏ᒸ�_�W��</param>
	/// <param name="node">�m�[�h1</param>
	/// <param name="randSeed">32bitMT�̃V�[�h�l</param>
	/// <returns>
	///		������ : node1�ƘA���ȃm�[�h
	///		���s�� : �A���ȃm�[�h���Ȃ��Ȃ��-1
	/// </returns>
	uint32_t GetConnectedNode(bool *faults, uint32_t node);

	/// <summary>�����̗v�f���܂Ƃ߂Ď擾</summary>
	/// <param name="faultRatio">�̏ᗦ</param>
	/// <returns>
	///		Experiment
	/// </returns>
	//Experiment* CreateExperiment2(int faultRatio);
};


/// <summary>�����ɋ��ʂ��Ďg����v�f�ꎮ���܂Ƃ߂�N���X</summary>
class Experiment
{
public:
	/// <summary>�̏ᒸ�_�W��</summary>
	bool* Faults;

	/// <summary>��̏Ⴉ�A����2���_</summary>
	uint32_t node1, node2;

	Experiment(SGraph *g, int faultRatio)
	{
		// �̏�����
		this->Faults = g->CreateFaults(faultRatio);

		// �����_���ɘA����2���_��I��
		this->node2 = -1;
		while (this->node2 == -1)
		{
			this->node1 = g->GetNodeRandom(Faults, faultRatio);
			this->node2 = g->GetConnectedNode(Faults, node1);
		}
	}

	~Experiment()
	{
		if (Faults != nullptr){
			delete[] Faults;
		}
	}
};