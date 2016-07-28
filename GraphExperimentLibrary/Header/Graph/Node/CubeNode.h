#pragma once

#include <iostream>

#include "SNode.h"

/// <summary>
/// �L���[�u�n�̃m�[�h��\���N���X
/// </summary>
class CubeNode : public SNode
{
private:
	uint32_t Addr;
public:
	CubeNode() {};
	CubeNode(const CubeNode& obj) {};
	CubeNode(uint32_t addr) { SetAddr(addr); };
	virtual ~CubeNode() {};

	// SNode����p��
	virtual uint32_t GetIndex() override;
	virtual void SetAddr(uint32_t addr) override;
};