#pragma once
#include "Graph\SGraph.h"

namespace Routing
{
	/// <summary>Simple Routing
	/// �O���אڒ��_�����I�΂Ȃ����[�e�B���O
	/// </summary>
	/// <param name="exp">Experiment</param>
	/// <param name="g">�O���t�C���X�^���X</param>
	/// <returns>
	/// ���̐��Ȃ琬���F����
	/// ���̐��Ȃ玸�s�F���s�܂ł̃X�e�b�v��(�������])
	/// </returns>
	int SR(SGraph *g, SNode *node1, SNode *node2);

	/// <summary>Normal Routing(�S���L��)
	/// �O���אڒ��_�Ɖ�������I�ԃ��[�e�B���O
	/// </summary>
	/// <param name="exp">Experiment</param>
	/// <param name="g">�O���t�C���X�^���X</param>
	/// <returns>
	/// ���̐��Ȃ琬���F����
	/// ���̐��Ȃ玸�s�F���s�܂ł̃X�e�b�v��(�������])
	/// </returns>
	int NR1(SGraph *g);

	/// <summary>Normal Routing(1�O�����L��)
	/// �O���אڒ��_�Ɖ�������I�ԃ��[�e�B���O
	/// </summary>
	/// <param name="exp">Experiment</param>
	/// <param name="g">�O���t�C���X�^���X</param>
	/// <returns>
	/// ���̐��Ȃ琬���F����
	/// ���̐��Ȃ玸�s�F���s�܂ł̃X�e�b�v��(�������])
	/// </returns>
	int NR2(SGraph *g);

	/// <summary>Extra Routing
	/// NR + �p�����^�������m�[�h��D��
	/// </summary>
	/// <param name="exp">Experiment</param>
	/// <param name="param">���[�e�B���O�w�W</param>
	/// <param name="g">�O���t�C���X�^���X</param>
	/// <returns>
	/// ���̐��Ȃ琬���F����
	/// ���̐��Ȃ玸�s�F���s�܂ł̃X�e�b�v��(�������])
	/// </returns>
	int ER(int* param, SGraph *g);
}