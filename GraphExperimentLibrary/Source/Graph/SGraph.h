#pragma once

#include <iostream>
#include <queue>
#include <random>

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

	// �O���אڒ��_�r�b�g��Ȃǂ��玟��Index��Ԃ�
	// from���牺�ɒT�����Č�����Ȃ����-1
	int GetNextBitIndex(uint32_t bits, int from)
	{
		for (int i = from; i >= 0; --i)
		{
			if (bits & (1 << i)) return i;
		}
		return -1;
	}

public:
	//SGraph();
	virtual ~SGraph();

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

	/// <summary>
	///		���_�̎�����Ԃ�
	/// </summary>
	/// <returns>����</returns>
	virtual int GetDegree(uint32_t node) = 0;

	/// <summary>
	///		index(0��index��Dimension)�ɑ΂��Ĉ�ӂȗאڒ��_��Ԃ�
	///		<para>�ʑ��ɉ����ĕK���I�[�o�[���C�h���邱��</para>
	/// </summary>
	/// <param name="node">�m�[�h</param>
	/// <param name="index">���Ԗڂ̗אڒ��_��</param>
	/// <returns>�אڒ��_�̃A�h���X</returns>
	virtual uint32_t GetNeighbor(uint32_t node, int index) = 0;

	/// <summary>
	///		�m�[�hs����d�ւ̑O���אڒ��_��Ԃ�
	///		<para>�ύX���Ȃ���΂��ׂĂ̗אڒ��_�̋������v�Z���ċ��߂邽��O(n^2)</para>
	///		<para>�K�v�ɉ����ăI�[�o�[���C�h���邱��</para>
	/// </summary>
	/// <param name="s">�o�����_�̃A�h���X</param>
	/// <param name="d">�ړI���_�̃A�h���X</param>
	/// <returns>��i�אڒ��_���O���ł���Ƃ��ɑ�i�r�b�g��1�ł���悤�ȃr�b�g��</returns>
	virtual uint32_t GetForwardNeighbor(uint32_t s, uint32_t d);

	/// <summary>
	///		�m�[�hs����d�ւ̉����אڒ��_��Ԃ�
	///		<para>�ύX���Ȃ���΂��ׂĂ̗אڒ��_�̋������v�Z���ċ��߂邽��O(n^2)</para>
	///		<para>�K�v�ɉ����ăI�[�o�[���C�h���邱��</para>
	/// </summary>
	/// <param name="s">�o�����_�̃A�h���X</param>
	/// <param name="d">�ړI���_�̃A�h���X</param>
	/// <returns>��i�אڒ��_�������ł���Ƃ��ɑ�i�r�b�g��1�ł���悤�ȃr�b�g��</returns>
	virtual uint32_t GetSideNeighbor(uint32_t s, uint32_t d);

	/// <summary>
	///		�m�[�hs����d�ւ̌���אڒ��_��Ԃ�
	///		<para>�ύX���Ȃ���΂��ׂĂ̗אڒ��_�̋������v�Z���ċ��߂邽��O(n^2)</para>
	///		<para>�K�v�ɉ����ăI�[�o�[���C�h���邱��</para>
	/// </summary>
	/// <param name="s">�o�����_�̃A�h���X</param>
	/// <param name="d">�ړI���_�̃A�h���X</param>
	/// <returns>��i�אڒ��_������ł���Ƃ��ɑ�i�r�b�g��1�ł���悤�ȃr�b�g��</returns>
	virtual uint32_t GetBackwardNeighbor(uint32_t s, uint32_t d);

	/// <summary>
	///		���_���̏Ⴉ�ǂ�����Ԃ�
	/// </summary>
	/// <param name="node">�m�[�h</param>
	/// <returns>�̏Ⴉ�ǂ���</returns>
	bool IsFault(uint32_t node);

	/// <summary>
	///		2���_���A�����ǂ�����Ԃ�(������Əd��)
	/// </summary>
	/// <param name="node1">�m�[�h</param>
	/// <param name="node2">�m�[�h</param>
	/// <returns>�A�����ǂ���</returns>
	bool IsConnected(uint32_t node1, uint32_t node2);




	/// <summary>
	/// 2���_�Ԃ̋������v�Z(�̏���l�����Ȃ�)
	/// �f�t�H���g�ł͓����I�ɂ͒P�ɕ��D��T�����Ă�
	/// �K�v�ɉ����ăI�[�o�[���C�h����
	/// </summary>
	/// <param name="node1">�n�_</param>
	/// <param name="node2">�I�_</param>
	/// <returns>����</returns>
	virtual int CalcDistance(uint32_t node1, uint32_t node2);

	/// <summary>
	/// 2���_�Ԃ̋������v�Z(�̏���l������)
	/// </summary>
	/// <param name="node1">�n�_</param>
	/// <param name="node2">�I�_</param>
	/// <returns>����</returns>
	int CalcDistanceF(uint32_t node1, uint32_t node2);

	/// <summary>���钸�_����C�ӂ̒��_�ւ̋����𕝗D��T��(�̏���l�����Ȃ�)</summary>
	/// <param name="node">�m�[�h</param>
	/// <returns>
	/// node����̋����̔z��
	/// �g���I������������邱��
	/// </returns>
	int* CalcAllDistanceBFS(uint32_t node);

	/// <summary>���钸�_����C�ӂ̒��_�ւ̋����𕝗D��T��(�̏���l������)</summary>
	/// <param name="node">�m�[�h</param>
	/// <returns>
	/// node����̋����̔z��(�̏�Ȃ�-1)
	/// �g���I������������邱��
	/// </returns>
	int* CalcAllDistanceBFSF(uint32_t node);



	/// <summary>�̏�v�f�W�������</summary>
	/// <param name="faultRatio">�̏ᗦ(%)</param>
	void GenerateFaults(int faultRatio);

	/// <summary>��̏�Ȓ��_���烉���_����1�I��</summary>
	/// <returns>�m�[�h</returns>
	uint32_t GetNodeRandom();

	/// <summary>���钸�_�ƘA���Ȓ��_�������_���ɑI��</summary>
	/// <param name="node">�m�[�h</param>
	/// <returns>
	///		�A���ȃm�[�h�����݂��Ȃ��Ƃ����s
	///		���s����return�������ƈ�v
	/// </returns>
	uint32_t GetConnectedNodeRandom(uint32_t node);


	// 
	int Routing_Simple(const uint32_t node1, const uint32_t node2);
};
