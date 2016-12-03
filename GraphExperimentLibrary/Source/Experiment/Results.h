#pragma once
#include <string>
#include <fstream>
#include <iostream>
#define MAX_ROUTING_NUM 10

using namespace std;

class Results
{
private:
	string Names[MAX_ROUTING_NUM];
	int SuccessCount[MAX_ROUTING_NUM][10];
	int TotalSteps[MAX_ROUTING_NUM][10];
	int Count = 0;

public:

	/// <summary>���ʂ��L�^���������[�e�B���O��ǉ�</summary>
	/// <param name="name">name</param>
	void Add(char* name);

	/// <summary>�w�肵�����[�e�B���O�̌��ʂ�ǉ�</summary>
	/// <param name="id">���[�e�B���O��id</param>
	/// <param name="faultRatio">�̏ᗦ</param>
	/// <param name="step">���[�e�B���O�X�e�b�v��</param>
	void Update(int id, int faultRatio, int step);

	/// <summary>CSV�`���ŏo��</summary>
	/// <param name="path">�o�͐�̃p�X</param>
	/// <param name="trials">���s��</param>
	void Save(char* path, int trials);
};