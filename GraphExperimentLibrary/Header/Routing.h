#pragma once
#include "Graph\SGraph.h"

namespace Routing
{
	/// <summary>Simple Routing
	/// 前方隣接頂点しか選ばないルーティング
	/// </summary>
	/// <param name="exp">Experiment</param>
	/// <param name="g">グラフインスタンス</param>
	/// <returns>
	/// 正の数なら成功：距離
	/// 負の数なら失敗：失敗までのステップ数(正負反転)
	/// </returns>
	int SR(SGraph *g, SNode *node1, SNode *node2);

	/// <summary>Normal Routing(全部記憶)
	/// 前方隣接頂点と横方向を選ぶルーティング
	/// </summary>
	/// <param name="exp">Experiment</param>
	/// <param name="g">グラフインスタンス</param>
	/// <returns>
	/// 正の数なら成功：距離
	/// 負の数なら失敗：失敗までのステップ数(正負反転)
	/// </returns>
	int NR1(SGraph *g);

	/// <summary>Normal Routing(1つ前だけ記憶)
	/// 前方隣接頂点と横方向を選ぶルーティング
	/// </summary>
	/// <param name="exp">Experiment</param>
	/// <param name="g">グラフインスタンス</param>
	/// <returns>
	/// 正の数なら成功：距離
	/// 負の数なら失敗：失敗までのステップ数(正負反転)
	/// </returns>
	int NR2(SGraph *g);

	/// <summary>Extra Routing
	/// NR + パラメタが高いノードを優先
	/// </summary>
	/// <param name="exp">Experiment</param>
	/// <param name="param">ルーティング指標</param>
	/// <param name="g">グラフインスタンス</param>
	/// <returns>
	/// 正の数なら成功：距離
	/// 負の数なら失敗：失敗までのステップ数(正負反転)
	/// </returns>
	int ER(int* param, SGraph *g);
}