#pragma once

#include "..\..\Common.h"
#include <iostream>

class Node {
private:
	ulong Addr;
public:
	// �R���X�g���N�^
	Node();	// �f�t�H���g

	Node(const Node &obj);	// �R�s�[

	Node(ulong nodeAddr);	// addr�������Ɏ��ꍇ

							// ���Z�q�̃I�[�o�[���[�h
	Node &operator=(const Node &obj);	// =

	int operator[](int i);	// []

	Node operator^(Node &bin);	// ^ 

								// ^= ���Z�q�̃I�[�o�[���[�h
								//// �E�̃I�y�����h��ulong�̏ꍇ
	Node &operator^=(Node obj);

	// == ���Z�q�̃I�[�o�[���[�h
	//// �E�̃I�y�����h��ulong�̏ꍇ
	bool operator==(Node obj);

	// != ���Z�q�̃I�[�o�[���[�h
	//// �E�̃I�y�����h��ulong�̏ꍇ
	bool operator!=(Node obj);

	// cout�p<<���Z�q�̃I�[�o�[���[�h��friend�錾
	friend std::ostream& operator<<(std::ostream& os, const Node& obj);

	ulong Subsequence(int index, int length);

	// c�̍X�V
	void Pop(ulong bin, int index);
	void Pop(Node &bin);


	// addr���擾
	ulong GetAddr() {
		return Addr;
	}

	// �^�C�v���擾
	ulong GetType() {
		return Addr & 0b11;
	}

	// �^�C�v�ȊO���擾
	ulong GetHead() {
		return Addr & (~0b11);
	}

	// �אڒ��_�W�����擾
	Node* getNeighbor(size_t dim);

	Node* test(size_t dim);
};


inline Node Node::operator^(Node &bin) {
	return Addr ^ bin.Addr;
}

// ��������擾
inline ulong Node::Subsequence(int index, int length) {
	return (Addr >> (index - length + 1)) & ((1 << length) - 1);
}

// c�̍X�V
inline void Node::Pop(ulong bin, int index) {
	ulong tmp = bin;
	int count = 0;
	while (tmp > 0) {
		tmp >>= 0b1;
		++count;
	}
	Addr ^= (bin << (index - count + 1));
}
inline void Node::Pop(Node &bin) {
	Addr ^= bin.Addr;
}