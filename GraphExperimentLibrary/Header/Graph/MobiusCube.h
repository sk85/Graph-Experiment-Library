#pragma once

#include "SGraph.h"

class MobiusCube : public SGraph {
private:
	// 0-mobius��1-mobius����\���^�C�v
	int type;

public:
	// �^�C�v���Z�b�g
	void SetType(int _type);

	// SGraph���p��
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	// virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
};