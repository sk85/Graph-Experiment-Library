#pragma once

#include "..\Graph\SGraph.h"

namespace Test
{
	// LTQのCapability関連の実験
	void e161122(int dim, int trials, char* path);

	// LTQのGetPreferredNeighborが正しいかテストする
	void e161024(int minDim, int maxDim);

	// SGraphのCalcDistanceが正しいかテストする
	void e160930(SGraph *g, int maxDim);

	// 2016/07/20にやった実験
	// 2016/09/30任意のSGraphに対して適応できるように更新
	// SGraphに対するルーティングいろいろ
	void e160720(SGraph *g, char *filename, int trials);
	
	// 2016/07/31にやった実験
	// パンケーキグラフの直径を求める
	void e160731();
}