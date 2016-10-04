#pragma once

#include "SGraph.h"

class MobiusCube : public SGraph {
private:
	// 0-mobiusか1-mobiusかを表すタイプ
	int type;

public:
	// タイプをセット
	void SetType(int _type);

	// SGraphより継承
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	// virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
};