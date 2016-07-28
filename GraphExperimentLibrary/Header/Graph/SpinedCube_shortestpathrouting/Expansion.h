#pragma once

#include <iostream>

#include "..\..\Common.h"
#include "Node.h"

class Expansion {
private:
	Node Array[32];
	int Count;
public:
	// �f�t�H���g�R���X�g���N�^
	Expansion() {	// �f�t�H���g
		Count = 0;
	}

	// �R�s�[�R���X�g���N�^
	Expansion(const Expansion &obj) {
		Count = obj.Count;
		for (int i = 0; i < Count; ++i)
			Array[i] = obj.Array[i];
	}

	// Node�ɂ��R���X�g���N�^
	Expansion(const Node &bin) {
		Count = 1;
		Array[0] = bin;
	}

	// Expansion���Ȃ��Ƃ��p�̃R���X�g���N�^
	Expansion(int msg) {
		if (msg == 0) {
			Count = 1000000;
		}
		else {
			throw "NULL�ȊO��int�l�����R���X�g���N�^�͑��݂��܂���";
		}
	}

	// ���Z�q�̃I�[�o�[���[�h
	Expansion& operator=(const Expansion &obj);			// =

	Expansion& operator+=(const Expansion &obj);		// += (Expansion + Expansion�̏ꍇ)

	Expansion& operator+=(const Node &bin);				// += (Expansion + Node�̏ꍇ)

	Expansion& operator+=(const ulong &bin);				// += (Expansion + ulong�̏ꍇ)

	Expansion operator+(const Expansion &obj) const;	// + (Expansion + Expansion�̏ꍇ)

	Expansion operator+(const Node &bin) const;			// + (Expansion + Node�̏ꍇ)

	bool operator>(const Expansion &bin) const;			// + (Expansion + Node�̏ꍇ)


	void Add(Node bin) {
		Array[Count++] = bin;
	}

	void Show() {
		if (Count == 1000000) {
			std::cout << "Expansion�͂���܂���" << std::endl;
			return;
		}
		for (int i = 0; i < Count; ++i) {
			std::cout << Array[i] << std::endl;
		}
	}

	bool Check(Node s, Node d) {
		ulong tmp = 0;
		for (int i = 0; i < Count; ++i) {
			tmp ^= Array[i].GetAddr();
		}
		return tmp != (s ^ d).GetAddr();
	}

	int GetCount() {
		return Count;
	}


};


// ������Z�q�̃I�[�o�[���[�h
inline Expansion& Expansion::operator=(const Expansion &obj) {
	Count = obj.Count;
	for (int i = 0; i < 32; ++i)
		Array[i] = obj.Array[i];
	return *this;
}

// += �̃I�[�o�[���[�h(Expansion + Expansion�̏ꍇ)
inline Expansion& Expansion::operator+=(const Expansion &obj) {
	for (int i = 0; i < obj.Count; ++i)
		Array[Count + i] = obj.Array[i];
	Count += obj.Count;
	return *this;
}

// += �̃I�[�o�[���[�h(Expansion + Node�̏ꍇ)
inline Expansion& Expansion::operator+=(const Node &bin) {
	Array[Count++] = bin;
	return *this;
}

// += �̃I�[�o�[���[�h(Expansion + ulong�̏ꍇ)
inline Expansion& Expansion::operator+=(const ulong &bin) {
	Array[Count++] = Node(bin);
	return *this;
}

// + �̃I�[�o�[���[�h(Expansion + Expansion�̏ꍇ)
inline Expansion Expansion::operator+(const Expansion &obj) const {
	Expansion exp = *this;
	for (int i = 0; i < obj.Count; ++i)
		exp.Array[Count + i] = obj.Array[i];
	exp.Count += obj.Count;
	return exp;
}

// + �̃I�[�o�[���[�h(Expansion + Node�̏ꍇ)
inline Expansion Expansion::operator+(const Node &bin) const {
	Expansion exp = *this;
	exp.Array[Count] = bin;
	exp.Count++;
	return exp;
}

// > �̃I�[�o�[���[�h
inline bool Expansion::operator>(const Expansion &bin) const {
	return Count > bin.Count;
}