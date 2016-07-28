#pragma once

#include <iostream>
#include <queue>
#include <random>

#include "Common.h"


class SGraph {
protected:
	/// <summary>次元数</summary>
	int Dimension;

	/// <summary>ノード数</summary>
	uint32_t NodeNum;

	/// <summary>32bitMT</summary>
	std::mt19937 mt;
public:

	void SetDimension(int _dim);
	int GetDimension();
	uint32_t GetNodeNum();

	/// <summary>乱数のシードをセット</summary>
	/// <param name="seed">シード値</param>
	void SetRandSeed(int seed);

	/// <summary>直径を計算</summary>
	/// <returns>直径</returns>
	virtual int CalcDiameter() = 0;



	// 隣接頂点を返す
	/*
		0 <= index < Dimension を満たすindexに対し、それぞれ別の隣接頂点を返す
		param	node : ノード
				index : 隣接頂点の番号
		return	隣接頂点
			必ず定義すること
	*/
	virtual ulong GetNeighbor(ulong node, int index) = 0;



	// 2頂点間の距離を計算(故障なし)
	/*
		param	node1, node2 : ノード
		return	距離
			デフォルトでは幅優先探索。
			必要に応じてオーバーライドすべし。
	*/
	virtual int CalcDistance(uint32_t node1, uint32_t node2);

	// 2頂点間の距離を計算(故障あり)
	/*
	param	node1, node2 : ノード
			faults : 故障頂点集合
	return	距離
	デフォルトでは幅優先探索。
	必要に応じてオーバーライドすべし。
	*/
	int CalcDistance(const bool *faults, uint32_t node1, uint32_t node2);

	// ある頂点から任意の頂点への距離を幅優先探索(故障なし)
	/*
		param	node : ノード
		return	nodeからの距離の配列
			返り値は使い終わったら解放すること
	*/
	int* CalcAllDistanceBFS(uint32_t node);

	// ある頂点から任意の頂点への距離を幅優先探索(故障あり)
	/*
		param	faults : 故障情報
				node : ノード
		return	nodeからの距離の配列(故障なら-1)
				返り値は使い終わったら解放すること
	*/
	int* CalcAllDistanceBFS(const bool *faults, uint32_t node);



	/// <summary>故障要素集合を作る</summary>
	/// <param name="faultRatio">故障率</param>
	/// <param name="randSeed">32bitMTのシード値</param>
	/// <returns>第i要素が故障かを示すbool[i]</returns>
	bool* CreateFaults(int faultRatio);

	/// <summary>非故障な頂点からランダムに1つ選ぶ</summary>
	/// <param name="faults">故障頂点集合</param>
	/// <param name="randSeed">32bitMTのシード値</param>
	/// <returns>ノード</returns>
	uint32_t GetNodeRandom(bool *faults, uint32_t faultRatio);

	/// <summary>ある頂点と連結な頂点をランダムに選ぶ</summary>
	/// <param name="faults">故障頂点集合</param>
	/// <param name="node">ノード1</param>
	/// <param name="randSeed">32bitMTのシード値</param>
	/// <returns>
	///		成功時 : node1と連結なノード
	///		失敗時 : 連結なノードがないならば-1
	/// </returns>
	uint32_t GetConnectedNode(bool *faults, uint32_t node);

	/// <summary>実験の要素をまとめて取得</summary>
	/// <param name="faultRatio">故障率</param>
	/// <returns>
	///		Experiment
	/// </returns>
	//Experiment* CreateExperiment2(int faultRatio);
};


/// <summary>実験に共通して使われる要素一式をまとめるクラス</summary>
class Experiment
{
public:
	/// <summary>故障頂点集合</summary>
	bool* Faults;

	/// <summary>非故障かつ連結な2頂点</summary>
	uint32_t node1, node2;

	Experiment(SGraph *g, int faultRatio)
	{
		// 故障を作る
		this->Faults = g->CreateFaults(faultRatio);

		// ランダムに連結な2頂点を選ぶ
		this->node2 = -1;
		while (this->node2 == -1)
		{
			this->node1 = g->GetNodeRandom(Faults, faultRatio);
			this->node2 = g->GetConnectedNode(Faults, node1);
		}
	}

	~Experiment()
	{
		if (Faults != nullptr){
			delete[] Faults;
		}
	}
};