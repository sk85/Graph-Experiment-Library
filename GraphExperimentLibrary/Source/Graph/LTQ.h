#pragma once

#include "..\Score.h"
#include "SGraph.h"

#define DEBUG


class LTQ : public SGraph
{
public:
#ifdef DEBUG
	void Debug_CalcInnerForward()
	{
		for (size_t dim = 2; dim <= 11; dim++)
		{
			printf_s("dim = %d\n", dim);
			SetDimension(dim);
			for (size_t node1 = 0; node1 < GetNodeNum(); node1++)
			{
				for (size_t node2 = 0; node2 < GetNodeNum(); node2++)
				{
					uint32_t forward = CalcInnerForward(node1, node2);
					int distance = CalcDistance(node1, node2);
					for (int i = 0; i < GetDegree(node1); i++)
					{
						if (forward & (1 << i))
						{
							uint32_t neighbor = GetNeighbor(node1, i);
							int distance_n = CalcDistance(neighbor, node2);
							if (distance_n != distance - 1)
							{
								printf_s("d(%d, %d) = %d, d(%d, %d) = %d", node1, node2, distance, neighbor, node2, distance_n);
								getchar();
							}
						}
					}
				}
			}
		}
	}
#endif
	// SGraphより継承
	virtual uint32_t GetNeighbor(uint32_t s, int index) override;
	virtual int CalcDistance(uint32_t s, uint32_t d) override;
	virtual int GetDegree(uint32_t node) override;
	virtual uint32_t CalcNodeNum() override;
	virtual uint32_t GetForwardNeighbor(uint32_t s, uint32_t d);

	/// <summary>
	///		Capabilityを計算
	///		<para>サブグラフごとに計算(高野くんの卒論)</para>
	/// </summary>
	/// <returns>CapabilityのScoreオブジェクト</returns>
	Score* CalcCapability1();

	/// <summary>
	///		Capabilityを計算
	///		<para>改二</para>
	/// </summary>
	/// <returns>CapabilityのScoreオブジェクト</returns>
	Score* CalcCapability2();

	/// <summary>直径を計算する</summary>
	/// <returns>直径</returns>
	int GetDiameter();

	// 比較用の雑なルーティング
	int LTQ::Routing_Stupid(uint32_t node1, uint32_t node2);

	/// <summary>
	///		ルーティング
	///		<para>高野くんの卒論方式</para>
	/// </summary>
	/// <param name="node1">出発頂点のアドレス</param>
	/// <param name="node2">目的頂点のアドレス</param>
	/// <param name="score">ルーティングに用いる指標</param>
	/// <returns>
	///		ルーティングのステップ数。
	///		失敗ならば0または負の数
	/// </returns>
	int Routing_TakanoSotsuron(uint32_t node1, uint32_t node2, Score* score);

	// 高野くんの卒論改
	int Routing_TakanoSotsuronKai(uint32_t node1, uint32_t node2, Score* score);

	// 同じサブグラフ内の前方隣接頂点を返す。
	// 最短経路選択の途中で現れるexpansionなので、全てとは限らない
	uint32_t CalcInnerForward(uint32_t node1, uint32_t node2);
};