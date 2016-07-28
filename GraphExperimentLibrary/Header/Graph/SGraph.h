#pragma once

#include <iostream>
#include <queue>
#include <random>

#include "Node\SNode.h"


class SGraph {
protected:
	/// <summary>次元数</summary>
	int Dimension;

	/// <summary>ノード数</summary>
	uint32_t NodeNum;

	/// <summary>32bitMT</summary>
	std::mt19937 MT;

	/// <summary>故障率(%)</summary>
	int FaultRatio;

	/// <summary>故障数</summary>
	uint32_t FaultNum;

	/// <summary>各ノードが故障しているかを示す配列</summary>
	bool* Faults;

	/// <summary>ノード数を計算する</summary>
	/// <returns>ノード数</returns>
	virtual uint32_t CalcNodeNum() = 0;

public:
	SGraph();
	~SGraph();

	/// <summary>次元数をセット</summary>
	/// <param name="_dim">次元数</param>
	void SetDimension(int _dim);

	/// <summary>次元数を取得</summary>
	/// <param name="seed">次元数</param>
	int GetDimension();

	/// <summary>ノード数を取得</summary>
	/// <param name="seed">ノード数</param>
	uint32_t GetNodeNum();

	/// <summary>乱数のシードをセット</summary>
	/// <param name="seed">シード値</param>
	void SetRandSeed(int seed);

	/// <summary>頂点の次数を返す</summary>
	/// <returns>次数</returns>
	virtual int GetDegree(SNode* node) = 0;

	/// <summary>
	/// index(0 <= index < Dimension)に対して一意な隣接頂点を返す
	/// 必ずオーバーライドする
	/// </summary>
	/// <param name="node">ノード</param>
	/// <param name="index">隣接頂点の番号</param>
	/// <returns>隣接頂点</returns>
	virtual SNode* GetNeighbor(SNode* node, int index) = 0;

	/// <summary>
	/// 頂点が故障かどうかを返す
	/// </summary>
	/// <param name="node">ノード</param>
	/// <returns>故障かどうか</returns>
	bool IsFault(SNode* node);

	/// <summary>
	/// 2頂点が連結かどうかを返す(ちょっと重い)
	/// </summary>
	/// <param name="node1">ノード</param>
	/// <param name="node2">ノード</param>
	/// <returns>連結かどうか</returns>
	bool IsConnected(SNode* node1, SNode* node2);



	/// <summary>
	/// 2頂点間の距離を計算
	/// デフォルトでは内部的には単に幅優先探索を呼ぶ
	/// 必要に応じてオーバーライドする
	/// </summary>
	/// <param name="node1">始点</param>
	/// <param name="node2">終点</param>
	/// <returns>距離</returns>
	virtual int CalcDistance(SNode* node1, SNode* node2);

	/// <summary>ある頂点から任意の頂点への距離を幅優先探索</summary>
	/// <param name="faults">故障頂点集合</param>
	/// <param name="node">ノード</param>
	/// <returns>
	/// nodeからの距離の配列(故障なら-1)
	/// 使い終わったら解放すること
	/// </returns>
	int* CalcAllDistanceBFS(SNode* node);



	/// <summary>故障要素集合を作る</summary>
	/// <param name="faultRatio">故障率(%)</param>
	void GenerateFaults(int faultRatio);

	/// <summary>非故障な頂点からランダムに1つ選ぶ</summary>
	/// <param name="node">選んだノードを入れるポインタ</param>
	/// <returns>ノード</returns>
	void GetNodeRandom(SNode* node);

	/// <summary>ある頂点と連結な頂点をランダムに選ぶ</summary>
	/// <param name="node1">元となるノードノード</param>
	/// <param name="node2">見つけたノード</param>
	/// <returns>
	///		連結なノードが存在しないならば失敗
	/// </returns>
	bool GetConnectedNodeRandom(SNode* node1, SNode* node2);

	/// <summary>連結で非故障な2頂点をランダムに選ぶ</summary>
	/// <param name="node1">ノード</param>
	/// <param name="node2">ノード</param>
	/// <returns>
	///		存在しなければ失敗
	///		厳密には判定していないけど、100回やってダメなら失敗。
	///		失敗したらGenerateFaultsし直すか、条件を見直すべき。
	/// </returns>
	bool GetConnectedNodesRandom(SNode* node1, SNode* node2);
};
