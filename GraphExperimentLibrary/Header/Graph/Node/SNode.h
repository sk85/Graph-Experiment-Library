#pragma once

/// <summary>
/// ノードを表す基本クラス
/// 各ノードがノードの総数以下の数字に一意に割り当ててあれば実装は自由で
/// </summary>
class SNode
{
public:

	/// <summary>
	/// ノードの通し番号を取得
	/// 0 ～ NodeNumにノードを一意に割り当て
	/// </summary>
	/// <returns>通し番号</returns>
	virtual uint32_t GetIndex() = 0;

	/// <summary>
	/// ノードアドレスをセット
	/// </summary>
	virtual void SetAddr(uint32_t addr) = 0;
};
