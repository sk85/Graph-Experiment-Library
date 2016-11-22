#pragma once

#include "Graph\SGraph.h"

namespace Routing
{
	/// <summary>
	/// パラメタを生成
	/// 全部0(NormalRouting用)
	/// </summary>
	/// <param name="g">対象グラフ</param>
	/// <returns>
	/// パラメタ
	/// </returns>
	int* CreateZeroParameter(SGraph *g);

	/// <summary>
	/// パラメタを生成
	/// 非故障な隣接頂点数
	/// </summary>
	/// <param name="g">対象グラフ</param>
	/// <returns>
	/// パラメタ
	/// </returns>
	int* CreateParameter(SGraph *g);

	/// <summary>
	/// パラメタを生成
	/// 非故障な隣接頂点のパラメタの和
	/// </summary>
	/// <param name="g">対象グラフ</param>
	/// <param name="param">前のパラメタ</param>
	/// <returns>
	/// パラメタ
	/// </returns>
	int* CreateParameter(SGraph *g, int *param);

	/// <summary>
	/// Simple Routing
	/// １．前方隣接頂点を選択
	/// ２．見つからなければ失敗
	/// </summary>
	/// <param name="g">対象グラフ</param>
	/// <param name="node1">出発ノード</param>
	/// <param name="node2">目的ノード</param>
	/// <returns>
	/// 戻り値の絶対値が終了時のステップ数
	/// 成功時は正の数、失敗時は0か負の数
	/// </returns>
	int SimpleRouting(SGraph *g, const uint32_t node1, const uint32_t node2);

	/// <summary>
	/// Normal Routing 1
	/// １．前方隣接頂点を選択
	/// ２．横方隣接頂点を選択
	/// ３．見つからなければ失敗
	/// ※既に通ったノードは選ばない
	/// </summary>
	/// <param name="g">対象グラフ</param>
	/// <param name="node1">出発ノード</param>
	/// <param name="node2">目的ノード</param>
	/// <returns>
	/// 戻り値の絶対値が終了時のステップ数
	/// 成功時は正の数、失敗時は0か負の数
	/// </returns>
	int NormalRouting1(SGraph *g, uint32_t node1, uint32_t node2);

	/// <summary>
	/// Normal Routing 2
	/// １．前方隣接頂点を選択
	/// ２．横方隣接頂点を選択
	/// ３．見つからなければ失敗
	/// ※一個手前のノードは選ばない
	/// </summary>
	/// <param name="g">対象グラフ</param>
	/// <param name="node1">出発ノード</param>
	/// <param name="node2">目的ノード</param>
	/// <returns>
	/// 戻り値の絶対値が終了時のステップ数
	/// 成功時は正の数、失敗時は0か負の数
	/// </returns>
	int NormalRouting2(SGraph *g, uint32_t node1, uint32_t node2);

	/// <summary>
	/// Extre Routing
	/// １．前方隣接頂点のうちパラメタが最大のものを選択
	/// ２．横方隣接頂点のうちパラメタが最大のものを選択
	/// ３．見つからなければ失敗
	/// ※一個手前のノードは選ばない
	/// </summary>
	/// <param name="g">対象グラフ</param>
	/// <param name="node1">出発ノード</param>
	/// <param name="node2">目的ノード</param>
	/// <param name="param">パラメタ</param>
	/// <returns>
	/// 戻り値の絶対値が終了時のステップ数
	/// 成功時は正の数、失敗時は0か負の数
	/// </returns>
	int ExtraRouting(SGraph *g, uint32_t node1, uint32_t node2, int* param);
}