#pragma once

#include <iostream>

#include "SNode.h"

/// <summary>
/// キューブ系のノードを表すクラス
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

	// SNodeから継承
	virtual uint32_t GetIndex() override;
	virtual void SetAddr(uint32_t addr) override;
};