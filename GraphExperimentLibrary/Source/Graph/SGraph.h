#pragma once

#include <iostream>
#include <queue>
#include <random>

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

	// 前方隣接頂点ビット列などから次のIndexを返す
	// fromから下に探索して見つからなければ-1
	int GetNextBitIndex(uint32_t bits, int from)
	{
		for (int i = from; i >= 0; --i)
		{
			if (bits & (1 << i)) return i;
		}
		return -1;
	}

public:
	//SGraph();
	virtual ~SGraph();

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

	/// <summary>
	///		頂点の次数を返す
	/// </summary>
	/// <returns>次数</returns>
	virtual int GetDegree(uint32_t node) = 0;

	/// <summary>
	///		index(0≦index＜Dimension)に対して一意な隣接頂点を返す
	///		<para>位相に応じて必ずオーバーライドすること</para>
	/// </summary>
	/// <param name="node">ノード</param>
	/// <param name="index">何番目の隣接頂点か</param>
	/// <returns>隣接頂点のアドレス</returns>
	virtual uint32_t GetNeighbor(uint32_t node, int index) = 0;

	/// <summary>
	///		ノードsからdへの前方隣接頂点を返す
	///		<para>変更がなければすべての隣接頂点の距離を計算して求めるためO(n^2)</para>
	///		<para>必要に応じてオーバーライドすること</para>
	/// </summary>
	/// <param name="s">出発頂点のアドレス</param>
	/// <param name="d">目的頂点のアドレス</param>
	/// <returns>第i隣接頂点が前方であるときに第iビットが1であるようなビット列</returns>
	virtual uint32_t GetForwardNeighbor(uint32_t s, uint32_t d);

	/// <summary>
	///		ノードsからdへの横方隣接頂点を返す
	///		<para>変更がなければすべての隣接頂点の距離を計算して求めるためO(n^2)</para>
	///		<para>必要に応じてオーバーライドすること</para>
	/// </summary>
	/// <param name="s">出発頂点のアドレス</param>
	/// <param name="d">目的頂点のアドレス</param>
	/// <returns>第i隣接頂点が横方であるときに第iビットが1であるようなビット列</returns>
	virtual uint32_t GetSideNeighbor(uint32_t s, uint32_t d);

	/// <summary>
	///		ノードsからdへの後方隣接頂点を返す
	///		<para>変更がなければすべての隣接頂点の距離を計算して求めるためO(n^2)</para>
	///		<para>必要に応じてオーバーライドすること</para>
	/// </summary>
	/// <param name="s">出発頂点のアドレス</param>
	/// <param name="d">目的頂点のアドレス</param>
	/// <returns>第i隣接頂点が後方であるときに第iビットが1であるようなビット列</returns>
	virtual uint32_t GetBackwardNeighbor(uint32_t s, uint32_t d);

	/// <summary>
	///		頂点が故障かどうかを返す
	/// </summary>
	/// <param name="node">ノード</param>
	/// <returns>故障かどうか</returns>
	bool IsFault(uint32_t node);

	/// <summary>
	///		2頂点が連結かどうかを返す(ちょっと重い)
	/// </summary>
	/// <param name="node1">ノード</param>
	/// <param name="node2">ノード</param>
	/// <returns>連結かどうか</returns>
	bool IsConnected(uint32_t node1, uint32_t node2);




	/// <summary>
	/// 2頂点間の距離を計算(故障を考慮しない)
	/// デフォルトでは内部的には単に幅優先探索を呼ぶ
	/// 必要に応じてオーバーライドする
	/// </summary>
	/// <param name="node1">始点</param>
	/// <param name="node2">終点</param>
	/// <returns>距離</returns>
	virtual int CalcDistance(uint32_t node1, uint32_t node2);

	/// <summary>
	/// 2頂点間の距離を計算(故障を考慮する)
	/// </summary>
	/// <param name="node1">始点</param>
	/// <param name="node2">終点</param>
	/// <returns>距離</returns>
	int CalcDistanceF(uint32_t node1, uint32_t node2);

	/// <summary>ある頂点から任意の頂点への距離を幅優先探索(故障を考慮しない)</summary>
	/// <param name="node">ノード</param>
	/// <returns>
	/// nodeからの距離の配列
	/// 使い終わったら解放すること
	/// </returns>
	int* CalcAllDistanceBFS(uint32_t node);

	/// <summary>ある頂点から任意の頂点への距離を幅優先探索(故障を考慮する)</summary>
	/// <param name="node">ノード</param>
	/// <returns>
	/// nodeからの距離の配列(故障なら-1)
	/// 使い終わったら解放すること
	/// </returns>
	int* CalcAllDistanceBFSF(uint32_t node);



	/// <summary>故障要素集合を作る</summary>
	/// <param name="faultRatio">故障率(%)</param>
	void GenerateFaults(int faultRatio);

	/// <summary>非故障な頂点からランダムに1つ選ぶ</summary>
	/// <returns>ノード</returns>
	uint32_t GetNodeRandom();

	/// <summary>ある頂点と連結な頂点をランダムに選ぶ</summary>
	/// <param name="node">ノード</param>
	/// <returns>
	///		連結なノードが存在しないとき失敗
	///		失敗時はreturnが引数と一致
	/// </returns>
	uint32_t GetConnectedNodeRandom(uint32_t node);


	// 
	int Routing_Simple(const uint32_t node1, const uint32_t node2);
};
