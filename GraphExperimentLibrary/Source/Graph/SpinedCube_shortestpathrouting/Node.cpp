#include "Node.h"


// �R���X�g���N�^����
Node::Node() {};

Node::Node(const Node &obj) {
	Addr = obj.Addr;
}

Node::Node(ulong nodeAddr) {
	Addr = nodeAddr;
}

// ���Z�q�̃I�[�o�[���[�h
Node& Node::operator=(const Node &obj) {
	Addr = obj.Addr;
	return *this;
}

int Node::operator[](int i) {
	if (i < 0 || i > 32) {
		return -1;
	}
	else {
		return (Addr & (1 << i)) >> i;
	}
}


Node &Node::operator^=(Node obj) {
	Addr ^= obj.Addr;
	return *this;
}

bool Node::operator==(Node obj) {
	return Addr == obj.Addr;
}

bool Node::operator!=(Node obj) {
	return Addr != obj.Addr;
}

// cout�p��<<���Z�q�̃I�[�o�[���[�h
std::ostream &operator<<(std::ostream& os, const Node &obj)
{
	for (int i = 32 - 1; i >= 0; --i) {
		if (i < 32 - 1 && i % 8 == 7)
			os << " ";
		os << ((obj.Addr & (1 << i)) >> i);
	}
	return os;
};


Node* Node::getNeighbor(size_t dim) {
	ulong type_bin = 0b100 + GetType();
	Node *neibor_set = new Node[dim];
	for (size_t index = dim - 1; index > 3; --index)
	{
		neibor_set[index] = Addr ^ (type_bin << (index - 2));
	}
	if (dim > 3)
		neibor_set[3] = Addr ^ 0b1000 ^ ((Addr & 0b01) << 2);
	if (dim > 2)
		neibor_set[2] = Addr ^ 0b0100 ^ ((Addr & 0b01) << 1);
	if (dim > 1)
		neibor_set[1] = Addr ^ 0b0010;
	neibor_set[0] = Addr ^ 0b0001;
	return neibor_set;
}