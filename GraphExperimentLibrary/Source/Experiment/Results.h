#pragma once
#include <string>
#include <fstream>
#include <iostream>
#define MAX_ROUTING_NUM 10

using namespace std;

class Results
{
private:
	string Names[MAX_ROUTING_NUM];
	int SuccessCount[MAX_ROUTING_NUM][10];
	int TotalSteps[MAX_ROUTING_NUM][10];
	int Count = 0;

public:

	/// <summary>結果を記録したいルーティングを追加</summary>
	/// <param name="name">name</param>
	void Add(char* name);

	/// <summary>指定したルーティングの結果を追加</summary>
	/// <param name="id">ルーティングのid</param>
	/// <param name="faultRatio">故障率</param>
	/// <param name="step">ルーティングステップ数</param>
	void Update(int id, int faultRatio, int step);

	/// <summary>CSV形式で出力</summary>
	/// <param name="path">出力先のパス</param>
	/// <param name="trials">試行回数</param>
	void Save(char* path, int trials);
};