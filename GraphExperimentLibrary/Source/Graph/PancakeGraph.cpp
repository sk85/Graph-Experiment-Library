#include "PancakeGraph.h"


unsigned PancakeGraph::InitPerm(int nn, int rr) {
	int i, j;
	n = nn; r = rr;
	if (r <= 0 || n <= 0 || r>n || r>MAX_R || n>MAX_N) return 0;
	for (i = r - 1; i >= 0; i--) {
		if (i == r - 1) a[i][0] = 1; else a[i][0] = a[i + 1][n - i - 2];
		for (j = 1; j<n - i; j++) a[i][j] = a[i][0] + a[i][j - 1];
	}
	for (i = 0; i<r; i++) for (j = 1; j<n - i; j++) if (a[i][j] <= a[i][j - 1])
		return 0;
	return a[0][n - 1];
}

unsigned PancakeGraph::PermToNum(int*vec) {
	int i, j, k; unsigned u = 0;
	for (i = 0; i<r; i++) {
		for (k = vec[i], j = 0; j<i; j++) if (vec[j]<vec[i]) k--;
		if (k) u += a[i][k - 1];
	}
	return u;
}

void PancakeGraph::NumToPerm(int*vec, unsigned num) {
	int i, j, k; char f[MAX_N];
	for (j = 0; j<n; j++) f[j] = 0;
	for (i = 0; i<r; i++) {
		for (k = 0; a[i][k] <= num; k++);
		if (k) num -= a[i][k - 1];
		for (j = 0; k >= 0; k--, j++) while (f[j]) j++;
		f[--j] = 1; vec[i] = j;
	}
}


uint32_t PancakeGraph::CalcNodeNum()
{
	uint32_t tmp = 1;
	for (uint32_t i = 2; i <= (uint32_t)this->Dimension; i++)
	{
		tmp *= i;
	}
	return tmp;
}

int PancakeGraph::GetDegree(uint32_t node)
{
	return this->Dimension - 1;
}

uint32_t PancakeGraph::GetNeighbor(uint32_t node, int index)
{
	// 頂点番号から順列のアドレスを取得
	int *perm = new int[this->Dimension];
	this->InitPerm(this->Dimension, this->Dimension);
	this->NumToPerm(perm, node);

	// index + 1番目で前置反転置換
	int front = 0, back = index + 1;
	while (front < back)
	{
		int tmp = perm[front];
		perm[front] = perm[back];
		perm[back] = tmp;
		front++;
		back--;
	}

	// 変換後の順列を数値に変換
	uint32_t num = this->PermToNum(perm);

	delete[] perm;

	return num;
}